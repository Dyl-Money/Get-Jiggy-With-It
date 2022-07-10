//-----------------------------------------------------------------------------------------------------	
// Script allows to choose puzzle  from the list and assign it to GameController
//-----------------------------------------------------------------------------------------------------	
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("Scripts/Jigsaw Puzzle/Choose Puzzle From List")]
public class ChoosePuzzleFromList : MonoBehaviour 
{
    [System.Serializable]
    public class Puzzles
	{
		public PuzzleController puzzle;
		public Renderer background;
	}



	public GameController gameController;
	public int activePuzzle;
	public Vector3 puzzlePosition;
	public Puzzles[] puzzles;


	//=====================================================================================================
	void OnEnable () 
	{
		if (!gameController) 
		{
			Debug.LogWarning("There's no <i>GameController</i> assigned to <b>ChoosePuzzle</b> component", gameObject);
			gameController = gameObject.GetComponent<GameController>();
		}

        SwitchPuzzle(activePuzzle);

        this.enabled = false;
	}

    //----------------------------------------------------------------------------------------------------
    // Switch
    public void SwitchPuzzle(int _id)
    {
        if (_id < puzzles.Length  &&  _id >= 0)
        {
            gameController.enabled = false;

            if (gameController.background)
                gameController.background.gameObject.SetActive(false);

            if (gameController.puzzle)
                gameController.puzzle.gameObject.SetActive(false);

            activePuzzle = _id;

            gameController.puzzle = Instantiate(puzzles[activePuzzle].puzzle, puzzlePosition, Quaternion.identity);
            if (puzzles[activePuzzle].background)
                gameController.background = Instantiate(puzzles[activePuzzle].background, Vector3.zero, Quaternion.identity).GetComponent<Renderer>();
        }
        else
            Debug.LogWarning("Saved puzzle number is out of <i>Puzzles list</i> in <b>ChoosePuzzle</b> component", gameObject);


        gameController.SwitchPuzzle(puzzles[activePuzzle].puzzle, puzzles[activePuzzle].background);
        gameController.enabled = true;

    }

    //----------------------------------------------------------------------------------------------------
}