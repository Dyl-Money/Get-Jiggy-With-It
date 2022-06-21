//-----------------------------------------------------------------------------------------------------	
// Script controls whole gameplay, UI and all sounds
//-----------------------------------------------------------------------------------------------------	
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


[AddComponentMenu("Scripts/Jigsaw Puzzle/Game Controller")]
public class GameController : MonoBehaviour 
{

	public Camera gameCamera;
	public PuzzleController puzzle;

    public bool findByTag = false;

    // Background (assembled puzzle preview)
    public Renderer background;
	public bool adjustBackground = true;
	public float backgroundTransparency = 0.1f;

	// Game UI
	public GameObject pauseUI;
	public GameObject winUI;
	public GameObject loseUI;
	public Text hintCounterUI;
	public Text timerCounterUI;
	public Toggle musicToggleUI;
	public Toggle soundToggleUI;

	// Music-related
	public AudioSource musicPlayer; 
	public AudioClip musicMain;
	public AudioClip musicWin;
	public AudioClip musicLose;

	// Sound-related
	public AudioSource soundPlayer; 
	public AudioClip soundGrab;
	public AudioClip soundDrop;
	public AudioClip soundAssemble;

	// Game rules
	public float timer;					// Time limit for level
	public int hintLimit = -1;			// Hints limit for level
	public bool invertRules = false;	// Allows to invert basic rules - i.e. player should decompose  the images



	// Important internal variables - please don't change them blindly
	CameraController cameraScript;
	float timerTime = 20.0f;
	float remainingTime;
	bool gameFinished = false;
    int remainingHints;
	Color backgroundColor;
	static Vector3 oldPointerPosition;



    //=====================================================================================================
    // Initialize
    void OnEnable () 
	{
         // Prepare Camera
        if (!gameCamera) 
			gameCamera = Camera.main;
		
		gameCamera.orthographic = true;
		cameraScript = gameCamera.GetComponent<CameraController>();


		// Prepare AudioSources for soundPlayer and musicPlayer
		if (!soundPlayer   &&   (soundGrab  ||  soundDrop  ||  soundAssemble))
			soundPlayer = gameObject.AddComponent<AudioSource>();
		
		if (!musicPlayer   &&   (musicMain  ||  musicWin  ||  musicLose)) 
			musicPlayer = gameObject.AddComponent<AudioSource>();

        // Try to automatically find/assign puzzle and background by tags
        if (findByTag)
        {
            GameObject foundObject = GameObject.FindGameObjectWithTag("Puzzle_Main");
            if (foundObject)
                puzzle = foundObject.GetComponent<PuzzleController>();

            foundObject = GameObject.FindGameObjectWithTag("Puzzle_Background");
            if (foundObject)
                background = foundObject.GetComponent<Renderer>();
        }
        
		// Initiate puzzle and prepare background
		if (StartPuzzle (puzzle))
		{
			puzzle.SetPiecesActive(true); 
			PrepareBackground (background);
		}


		// Load saved data
		Load ();
		LoadAudioActivity();   

		PlayMusic (musicMain, true); 


		// Prepare UI (disable all redudant at start)   
		if (winUI) 
			winUI.SetActive(false);
		
		if (loseUI)
			loseUI.SetActive(false);
		
		if (pauseUI) 
			pauseUI.SetActive(false);  
		
		if (timerCounterUI) 
			timerCounterUI.gameObject.SetActive(timer > 0);
		
		if (hintCounterUI) 
		{
            if (hintCounterUI.transform.parent)
                hintCounterUI.transform.parent.gameObject.SetActive(remainingHints != 0);
           
            hintCounterUI.transform.gameObject.SetActive(remainingHints > 0);

            hintCounterUI.text = remainingHints.ToString();
        }


		// Init timer
		timerTime = Time.time + remainingTime;
		Time.timeScale = 1.0f;

        Cursor.lockState = CursorLockMode.Confined;


        if (!puzzle)
			this.enabled = false;
	}

	//-----------------------------------------------------------------------------------------------------	
	// Main game cycle
	void Update () 
	{
		if (Input.GetKeyUp(KeyCode.Escape)) 
			Pause ();


		if (puzzle  &&  Time.timeScale > 0  &&  !gameFinished)
		{
			// Process puzzle and react on it state
            switch (puzzle.ProcessPuzzle (
                                            GetPointerPosition(gameCamera),
                                            Input.GetMouseButton(0)  &&  (!cameraScript || !cameraScript.IsCameraMoved())  &&  ((puzzle.GetCurrentPiece()==null && !EventSystem.current.IsPointerOverGameObject())  ||  puzzle.GetCurrentPiece() != null),
                                            GetRotationDirection()
                                          ) )
			{
				case PuzzleState.None:
					;
					break;

				case PuzzleState.DragPiece:
					PlaySound(soundGrab);
					break;

				case PuzzleState.ReturnPiece:
					PlaySound(soundAssemble);
					break;

				case PuzzleState.DropPiece:
					PlaySound(soundDrop);
					break;

				// Hide all pieces and finish game - if whole puzzle Assembled 	
				case PuzzleState.PuzzleAssembled:
					if (background && !invertRules) 
						puzzle.SetPiecesActive(false); 
					
					if (winUI) 
						winUI.SetActive(true);
					
					PlayMusic(musicWin, false);
					gameFinished = true;
					break;	
			}


			ProcessTimer ();
		}
		else  // Show background (assembled puzzle) if gameFinished
			if (gameFinished  &&  (!loseUI  ||  (loseUI && !loseUI.activeSelf)) ) 
				if(!invertRules) 
					ShowBackground();


        // Control Camera   
        if (cameraScript && puzzle)
           // if (puzzle.GetCurrentPiece() == null)  cameraScript.ManualUpdate();
            cameraScript.enabled = (puzzle.GetCurrentPiece() == null);

    }

	//-----------------------------------------------------------------------------------------------------	 
	// Get current pointer(mouse or single touch) position  
	static Vector3 GetPointerPosition (Camera _camera) 
	{
		Vector3 pointerPosition = oldPointerPosition;

        // For mobile/desktop
        if (Input.touchCount > 0)  
           pointerPosition = oldPointerPosition = _camera.ScreenToWorldPoint(Input.GetTouch(0).position);
          else
           pointerPosition = oldPointerPosition = _camera.ScreenToWorldPoint(Input.mousePosition);


        return pointerPosition;
	}

    //-----------------------------------------------------------------------------------------------------	 
    // Get current rotation basing on mouse or touches
    float GetRotationDirection () 
	{
        float rotation = 0;

         // For Desktop - just set rotation to "clockwise" (don't change the permanent speed)
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL
			if (Input.GetMouseButton(1))
                rotation = 1;
        #else // For mobile - calculate angle changing between touches and use it.
            if (Input.touchCount > 1)  
            {
					// If there are two touches on the device... Store both touches.
					Touch touchZero = Input.GetTouch (0);
					Touch touchOne 	= Input.GetTouch (1);

					// Find the angle between positions.
					float currentAngle = Vector2.SignedAngle(touchZero.position, touchOne.position); 
					float previousAngle = Vector2.SignedAngle(touchZero.position - touchZero.deltaPosition, touchOne.position - touchOne.deltaPosition);

					rotation = currentAngle - previousAngle;
			}
                 //Alternative (sign/direction based):  // rotation = (int)Mathf.Sign(Vector2.SignedAngle(Vector2.up, Input.GetTouch(1).position-Input.GetTouch(0).position));
        #endif

        return rotation;
	}

	//-----------------------------------------------------------------------------------------------------	 
	// Switch puzzle and background to another
	public void SwitchPuzzle (PuzzleController _puzzle, Renderer _background = null)
	{
		if (_puzzle  &&  _puzzle != puzzle) 
			StartPuzzle (_puzzle);
		
		if (_background  &&  _background != background) 
			PrepareBackground (_background);
	}

	//-----------------------------------------------------------------------------------------------------	 
	// Prepare puzzle and Decompose it if needed
	public bool StartPuzzle (PuzzleController _puzzle)
	{
		if (!_puzzle) 
			_puzzle = gameObject.GetComponent<PuzzleController>();
		
		if (!_puzzle) 
		{
			Debug.LogWarning("<b>PuzzleKit::GameController:</b> <i>PuzzleController</i> should be assigned to <i>puzzle</i> property - please check " + gameObject.name, gameObject);  
			return false;
		}


        if (puzzle.pieces == null || puzzle.pieces.Length == 0)
            puzzle.Prepare();


        if (puzzle  &&  puzzle != _puzzle) 
			puzzle.gameObject.SetActive(false);

        puzzle = _puzzle;
		puzzle.gameObject.SetActive(true); 


		if (!PlayerPrefs.HasKey (puzzle.name + "_Positions")  ||  !puzzle.enablePositionSaving)
			if (!invertRules) 
				puzzle.DecomposePuzzle (); 
			else
				puzzle.NonrandomPuzzle ();


		puzzle.invertedRules = invertRules;

		gameFinished = false;

		return true;
	}

	//-----------------------------------------------------------------------------------------------------	 
	// Show background (assembled puzzle)
	void ShowBackground () 
	{
		if (background  &&  backgroundColor.a < 1) 
		{
			backgroundColor.a = Mathf.Lerp (backgroundColor.a, 1.0f, Time.deltaTime); 
			background.material.color = backgroundColor;
		}

	}

	//-----------------------------------------------------------------------------------------------------	 
	// Prepare background (assembled puzzle)
	void PrepareBackground (Renderer _background) 
	{
        if (_background)
        {
           if (background)
                background.gameObject.SetActive(false);

            background = _background;
            background.gameObject.SetActive(true);

            backgroundColor = background.material.color;

            if (backgroundTransparency < 1.0f)
            {
                backgroundColor.a = backgroundTransparency;
                background.material.color = backgroundColor;
            }

            if (adjustBackground)
                AdjustBackground();  

        }
		else 
			background = null;

	}

    //-----------------------------------------------------------------------------------------------------	
    // Automatically align camera with background
    public void AlignCameraWithBackground(float _sizeTuner = 0)
    {
        if (!cameraScript)   return;


        cameraScript.enabled = false;

        if (gameCamera.aspect > 1)
            gameCamera.orthographicSize = background.bounds.size.x / (gameCamera.aspect * 2) + _sizeTuner;
        else
            gameCamera.orthographicSize = background.bounds.size.x - gameCamera.aspect + _sizeTuner;

        cameraScript.ReturnCamera();
        cameraScript.SetInitialZoom(gameCamera.orthographicSize);
        cameraScript.enabled = true;
    }

    //-----------------------------------------------------------------------------------------------------	
    // Adjust background to puzzle
    public void AdjustBackground () 
	{
		if (background  &&  background.transform.parent != puzzle.transform)  
		{
			background.transform.parent = puzzle.transform;
            background.transform.localPosition = new Vector3(0, 0, 0.2f);


            // Try to adjust background size according to puzzle bounds
            if (background as SpriteRenderer)
			{
				// Temporarily reset Puzzle rotation 
				Quaternion tmpRotation = puzzle.transform.rotation;
				puzzle.transform.localRotation = Quaternion.identity;

				// Reset background transform
				background.transform.localRotation = Quaternion.identity;	
				background.transform.localScale = Vector3.one;


                // Calculate background scale  to make it the same size as puzzle
                background.transform.localScale = new Vector3(puzzle.puzzleBounds.size.x/background.bounds.size.x, puzzle.puzzleBounds.size.y/background.bounds.size.y, background.transform.localScale.z);	
				
                // Aligned background position
				//background.transform.position = new Vector3(puzzle.puzzleBounds.min.x, puzzle.puzzleBounds.max.y, background.transform.position.z);
              /*  // TODO: Shift background if it's origin not in LeftTop corner 		 			 	
                if (Mathf.Abs(background.bounds.min.x - puzzle.puzzleBounds.min.x) > 1  ||  Mathf.Abs(background.bounds.max.y - puzzle.puzzleBounds.max.y) > 1)
					background.transform.localPosition = new Vector3(background.transform.localPosition.x + background.bounds.extents.x,  background.transform.localPosition.y - background.bounds.extents.y,  background.transform.localPosition.z);
               */
                
                // Return proprer puzzle rotation
                puzzle.transform.localRotation = tmpRotation;
			}

		}

	}

    //-----------------------------------------------------------------------------------------------------	 
    // Show Hint and update remainingHints
    public void ShowHint () 
	{
        if (gameFinished  ||  remainingHints == 0)  return;
         else
            puzzle.ReturnPiece (-1);

        remainingHints--;

        if (remainingHints == 0)
           if (hintCounterUI)
                hintCounterUI.transform.parent.gameObject.SetActive(false);
           else
                hintCounterUI.transform.gameObject.SetActive(false);


        if (hintCounterUI) 
			hintCounterUI.text = remainingHints.ToString();
		
		if (soundPlayer  &&  soundPlayer.enabled) 
			soundPlayer.PlayOneShot(soundAssemble);
		
	}

	//-----------------------------------------------------------------------------------------------------	 
	// Process Timer
	void ProcessTimer () 
	{
        if (timer > 0  &&  gameFinished == false)
            if (timerTime < Time.time) 
			{ // Lose game if time is out
				PlayMusic(musicLose, false);

				if (loseUI) 
					loseUI.SetActive(true);
			
				gameFinished = true;
			}
			else
				if (timerCounterUI)
                    {
                        float minutes_tmp = (int)(Mathf.Abs(Time.time - timerTime) / 60);
                        float seconds_tmp = (int)(Mathf.Abs(Time.time - timerTime) % 60);

                        seconds_tmp = (seconds_tmp == 60)  ?  0 : seconds_tmp;

                        timerCounterUI.text = minutes_tmp.ToString() + ":" + seconds_tmp.ToString("00");

                    }

    }

	//-----------------------------------------------------------------------------------------------------	 
	// Pause game and show pauseUI
	public void Pause () 
	{
        if (Time.timeScale > 0 )
		{
			Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            if (pauseUI) 
				pauseUI.SetActive(true);
		}
		else  
		{
			Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Confined;
            if (pauseUI) 
				pauseUI.SetActive(false);
		}

	}


    //-----------------------------------------------------------------------------------------------------	 
    // Reset current puzzle
    public void ResetPuzzle()
    {
        if (puzzle == null)
            return;

        Time.timeScale = 0;

        puzzle.ResetProgress(puzzle.name);

        remainingHints = hintLimit;
        timerTime = Time.time + timer;

        PlayerPrefs.SetInt(puzzle.name + "_hints", hintLimit);
        PlayerPrefs.SetFloat(puzzle.name + "_timer", timer);

        if (hintCounterUI)
        {
            hintCounterUI.gameObject.SetActive(remainingHints > 0);
            hintCounterUI.text = remainingHints.ToString();
        }

        puzzle.DecomposePuzzle();      

        Time.timeScale = 1.0f;
    }

    //-----------------------------------------------------------------------------------------------------	 
    // Restart current level
    public void Restart () 
	{
		Time.timeScale = 1.0f;

		if (puzzle != null) 
		{
			PlayerPrefs.SetString (puzzle.name, "");
			PlayerPrefs.DeleteKey (puzzle.name + "_Positions");
			PlayerPrefs.SetInt (puzzle.name + "_hints", hintLimit);
			PlayerPrefs.SetFloat (puzzle.name + "_timer", timer);
		}

		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);

	}

	//-----------------------------------------------------------------------------------------------------	 
	// Load custom level
	public void LoadLevel (int _levelId) 
	{
		Time.timeScale = 1.0f;
		SceneManager.LoadScene (_levelId);

	}

	//-----------------------------------------------------------------------------------------------------	
	// Load MusicPlayer and SoundPlayer Activity 
	void LoadAudioActivity () 
	{
		if (PlayerPrefs.HasKey("MusicPlayer")  &&  musicPlayer)  
		{
			musicPlayer.enabled = PlayerPrefs.GetInt("MusicPlayer") > 0 ? true : false;
			if (musicToggleUI) 
				musicToggleUI.isOn = musicPlayer.enabled;
		}


		if (PlayerPrefs.HasKey("SoundPlayer")  &&  soundPlayer)  
		{
			soundPlayer.enabled = PlayerPrefs.GetInt("SoundPlayer") > 0 ? true : false;
			if (soundToggleUI) 
				soundToggleUI.isOn = soundPlayer.enabled;
		}

	}

	//-----------------------------------------------------------------------------------------------------	
	// Enable/disable music 
	public void SetMusicActive (bool _enabled) 
	{
		if (musicPlayer) 
		{
			musicPlayer.enabled = _enabled;
			if (musicToggleUI)  
				musicToggleUI.isOn = _enabled;
			
			PlayerPrefs.SetInt("MusicPlayer", _enabled ? 1 : 0);
			PlayMusic (musicMain, true);
		}

	}

	//-----------------------------------------------------------------------------------------------------	 
	// Enable/disable sounds 
	public void SetSoundActive (bool _enabled) 
	{
		if (soundPlayer) 
		{
			soundPlayer.enabled = _enabled;
			if (soundToggleUI)
				soundToggleUI.isOn = _enabled;
			
			PlayerPrefs.SetInt("SoundPlayer", _enabled ? 1 : 0);
		}

	}

	//-----------------------------------------------------------------------------------------------------	 
	// Change and Play music clip
	public void PlayMusic (AudioClip _music, bool _loop) 
	{
		if (musicPlayer  &&  musicPlayer.enabled  &&  _music)
		{
			musicPlayer.loop = _loop;
			musicPlayer.clip = _music;
			musicPlayer.Play();
		}

	}

	public void PlayMusic (AudioClip _music) 
	{
		if (musicPlayer  &&  musicPlayer.enabled  &&  _music)
		{
			musicPlayer.clip = _music;
			musicPlayer.Play();
		}

	}

	//-----------------------------------------------------------------------------------------------------	 
	// Play sound clip once
	public void PlaySound (AudioClip _sound) 
	{
		if (soundPlayer  &&  soundPlayer.enabled  &&  _sound) 
			soundPlayer.PlayOneShot(_sound);

	}	  

	//-----------------------------------------------------------------------------------------------------	
	// Save progress (Assembled pieces)
	public void Save ()
	{
		if (puzzle != null) 
		{
			puzzle.SaveProgress (puzzle.name);
			PlayerPrefs.SetInt (puzzle.name + "_hints", remainingHints);
			PlayerPrefs.SetFloat (puzzle.name + "_timer", timerTime - Time.time);
		}

	}

	//-----------------------------------------------------------------------------------------------------	
	// Load puzzle (Assembled pieces)
	public void Load ()
	{
		if (!puzzle)
			return;
		else
			puzzle.LoadProgress(puzzle.name); 


		if (PlayerPrefs.HasKey (puzzle.name + "_hints"))
		{
			remainingHints = PlayerPrefs.GetInt (puzzle.name + "_hints");
			if (hintCounterUI)
				hintCounterUI.text = remainingHints.ToString ();
		} 
		else
			{
				Debug.Log ("No saved data found for: " + puzzle.name + "_hints", gameObject);
				remainingHints = hintLimit;  
			}
        

		if (PlayerPrefs.HasKey (puzzle.name + "_timer"))
			remainingTime = PlayerPrefs.GetFloat (puzzle.name + "_timer");
		else 
			{
				Debug.Log ("No saved data found for: " + puzzle.name + "_timer", gameObject);  
				remainingTime = timer;
			}

	}  

	//-----------------------------------------------------------------------------------------------------	
	// Save progress if player closes the application
	public void OnApplicationQuit() 
	{
		Save ();
		PlayerPrefs.Save();
	}

	//-----------------------------------------------------------------------------------------------------	
}