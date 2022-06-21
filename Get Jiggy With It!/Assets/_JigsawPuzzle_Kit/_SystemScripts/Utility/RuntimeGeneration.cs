//-----------------------------------------------------------------------------------------------------	
//  Simple demo script to help in puzzle-generation demonstration
//-----------------------------------------------------------------------------------------------------	
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using SimpleFileBrowser;

public class RuntimeGeneration : MonoBehaviour 
{
    public Texture2D image;                         // Will be used as main puzzle image
    public bool generateBackground = true;          // Automatically generate puzzle background from the source image
    public bool alignWithCamera = true;             // Automatically align puzzle/background with camera center
    public bool clearOldSaves = true;               // Clear existing Save data data during generation
    [TextArea]
    public string pathToImage;                      // pathToImage should starts from "http://"(for online image)  or  from "file://" (for local) 

    public PuzzleGenerator_Runtime puzzleGenerator;
	public GameController gameController;
	public Text rows;
	public Text cols;

    public SimpleFileBrowser.FileBrowser scriptA;
    GameObject gm;
    FileBrowser gms;

    string filePath;
    string path;

    //============================================================================================================================================================
    void Start()
    {
        // Name of game object that sccript is on
        gm = GameObject.Find("SimpleFileBrowserCanvas");
        // Now actuallly finding the script
        gms = gm.GetComponent<FileBrowser>();

    }
    public void GeneratePuzzle ()
	{
		if (puzzleGenerator == null || gameController == null) 
		{
			Debug.LogWarning ("Please assign <i>puzzleGenerator</i> and <i>gameController</i> to " + gameObject.name + " <b>RuntimeGenerator</b>", gameObject);
			return;
		}

        gameController.enabled = false;

        //Delete previously generated puzzle
        if (gameController.puzzle != null)
            Destroy(gameController.puzzle.gameObject);
        if (gameController.background != null)
            Destroy(gameController.background.gameObject);


        //pathToImage = "File://" + gms.path;

        if (!image)
        {
            filePath = PlayerPrefs.GetString("filePath", path);
            Debug.Log(filePath);
            puzzleGenerator.CreateFromExternalImage(filePath);

            //puzzleGenerator.CreateFromExternalImage(pathToImage);
        }
        else
            gameController.puzzle = puzzleGenerator.CreatePuzzleFromImage(image);


        StartCoroutine(StartPuzzleWhenReady());
    }

    //-----------------------------------------------------------------------------------------------------
    IEnumerator StartPuzzleWhenReady()
    {
        while (puzzleGenerator.puzzle == null)
        {
            yield return null;
        }

        if (clearOldSaves)
        { 
           PlayerPrefs.DeleteKey(puzzleGenerator.puzzle.name);
           PlayerPrefs.DeleteKey(puzzleGenerator.puzzle.name + "_Positions");
        }

        gameController.puzzle = puzzleGenerator.puzzle;


        // Align with Camera if needed
        if (alignWithCamera)
            puzzleGenerator.puzzle.AlignWithCameraCenter(gameController.gameCamera);

        // Generate backround if needed
        if (generateBackground)
            gameController.background = puzzleGenerator.puzzle.GenerateBackground(puzzleGenerator.GetSourceImage());

 
        gameController.enabled = true;
    }

    //-----------------------------------------------------------------------------------------------------	
    public void SetRows (float _amount) 
	{
		if (puzzleGenerator != null)
			puzzleGenerator.rows = (int)_amount;

		if (rows != null)
			rows.text = ((int)_amount).ToString();		
	}

	//-----------------------------------------------------------------------------------------------------	
	public void SetCols (float _amount) 
	{
		if (puzzleGenerator != null)
			puzzleGenerator.cols = (int)_amount;

		if (cols != null)
			cols.text = ((int)_amount).ToString();
	}

	//-----------------------------------------------------------------------------------------------------	
}