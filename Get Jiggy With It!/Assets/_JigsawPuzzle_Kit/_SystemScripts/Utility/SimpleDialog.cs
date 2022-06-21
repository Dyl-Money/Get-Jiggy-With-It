//-----------------------------------------------------------------------------------------------------	
// Script processes simple dialogs functionality
//-----------------------------------------------------------------------------------------------------	
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class SimpleDialog : MonoBehaviour 
{
	
	// Message structure
	[System.Serializable]
	public class DialogMessage
	{
		[TextArea(1,10)]
		public string text;				// Message text
		public float delay;	    		// How long should message stay on screen
		public int characterId;			// Index of character sprite in characters list
		public bool inverted;			// Use inverted position/animation for  dialog window or not
		public UnityEvent startAction;	// Call the event at message
	}



	public GameObject messageWindow;			// Link to Dialog visualization object
	public Image messageCharacter;				// Link to spritededicated for character rendering
	public Text messageText;					// Link to message text mesh
	public GameObject backgroundPanel;			// [optional] Link to gameObject that will serve as  a background for the dialog
	public Button nextButton;					// [optional] Link to UI.Button that will immediately switch to next message in the dialog
	public AnimationClip defaultAnimation;		// Link to animation to show object 
	public AnimationClip invertedAnimation;		// Link to animation to show/make inverted visualization object position
	public int lineLengthLimit;					// Number of symbols in line (before World-wrap)
	public bool pauseGame = false; 				// Pause game or not  during the dialog

	public DialogMessage[] messages; 			// List of all messages
	public Sprite[] characters;					// List of characters sprites


	// Important internal variables - please don't change them blindly
	int currentMessage = -1;
	float timeToNextMessage;
	Animation anim;


	//================================================================================================
	// Process world-wrapping by "<br>" symbol and according to lineLength
	void Start () 
	{
		lineLengthLimit++;
		for (int i = 0; i < messages.Length; i++ )
		{
			messages[i].text = messages[i].text.Replace("<br>","\n");
			if (lineLengthLimit > 1) 
				for (int j = 1; j <= messages[i].text.Length/lineLengthLimit; j++)
					messages[i].text = messages[i].text.Insert(j*lineLengthLimit-1, "\n");
		}   


		anim = messageWindow.GetComponent<Animation> ();
		if (anim)
			if (defaultAnimation)
				anim.clip = defaultAnimation;
			else 
				if (anim.clip)
					defaultAnimation = anim.clip;


		messageWindow.SetActive(true);
		if (nextButton) nextButton.gameObject.SetActive(true); 
		if (backgroundPanel) backgroundPanel.SetActive(true);

        if (pauseGame)
            Time.timeScale = 0;


        NextMessage();
	}

	//-----------------------------------------------------------------------------------------------------	
	// Go to next message when current message delay expired or  player click/touch screen
	void Update () 
	{
		if (currentMessage < messages.Length)
			if ( (!nextButton  &&  Input.GetMouseButtonUp(0))   ||   (!pauseGame  &&  messages[currentMessage].delay > -1  &&  timeToNextMessage < Time.time) ) 
				NextMessage();		
	}

	//-----------------------------------------------------------------------------------------------------	
	// Go to message with specified  index. So  dialog will be processed startingfrom it 
	public void GoToMessage (int id) 
	{
		currentMessage = id-1;
		NextMessage();
	}

	//-----------------------------------------------------------------------------------------------------	
	// Skip the whole dialog
	public void SkipDialog () 
	{
		currentMessage = messages.Length;

		messageWindow.SetActive(false);

        if (nextButton)
            nextButton.gameObject.SetActive(false);
        if (backgroundPanel) 
			backgroundPanel.SetActive(false); 
		
		Time.timeScale = 1;
	}

	//-----------------------------------------------------------------------------------------------------	
	// Go to next message if it exists. Function processes all visualization changing (text, character sprite, animation etc)
	public void NextMessage () 
	{		
		currentMessage++;
		messageWindow.SetActive(false);


        // Update visualization accordingly to next message (keep hided if all messages have been shown)
        if (currentMessage < messages.Length)
        {
            if (anim && invertedAnimation)
                if (messages[currentMessage].inverted)
                    anim.clip = invertedAnimation;
                else
                    if (currentMessage > 0 && messages[currentMessage - 1].inverted && defaultAnimation)
                    anim.clip = defaultAnimation;

            if (characters != null && (characters.Length > messages[currentMessage].characterId))
                messageCharacter.sprite = characters[messages[currentMessage].characterId];

            messageText.text = messages[currentMessage].text;
            timeToNextMessage = Time.time + messages[currentMessage].delay;

            messageWindow.SetActive(true);
            messages[currentMessage].startAction.Invoke();


            if (pauseGame)
            { 
                anim[anim.clip.name].time = 1;
                if (messages[currentMessage].delay == 0)
                    NextMessage();
            }

        }
        else
            SkipDialog();

	}

	//-----------------------------------------------------------------------------------------------------	
}