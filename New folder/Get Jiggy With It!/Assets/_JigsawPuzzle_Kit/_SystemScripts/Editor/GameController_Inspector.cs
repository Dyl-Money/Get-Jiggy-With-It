//----------------------------------------------------------------------------------------------------------------------------------------------------------
// Provides with custom and more convenient Inspector GUI for GameController
//----------------------------------------------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;



[CustomEditor(typeof(GameController))]
public class GameController_Inspector : Editor 
{
	
	// Important internal variables
	GameController game;
	bool showRules;
	bool showMusic;
	bool showSound;
	bool showGameUI;
	bool showBackground;


	//========================================================================================================================================================== 
	// Draw whole custom inspector GUI
	public override void OnInspectorGUI()
	{   
		game = (GameController)target;

		// General
		game.gameCamera = EditorGUILayout.ObjectField(new GUIContent("Game Camera", "Link to Camera to be used for puzzle (leave empty to try to use Camera.main)"), game.gameCamera , typeof(Camera), true) as Camera;

        game.findByTag = EditorGUILayout.Toggle(new GUIContent("Find puzzle by Tag", "Will try to automatically find/assign puzzle and background by tags"), game.findByTag);

        if (!game.findByTag)
            game.puzzle = EditorGUILayout.ObjectField(new GUIContent("Puzzle", "Link to PuzzleController to be processed (leave empty to try to use PuzzleManager attached to this object)"), game.puzzle, typeof(PuzzleController), true) as PuzzleController; 


		// Game rules
		showRules = EditorGUILayout.Foldout(showRules, new GUIContent("GAME  RULES", "Contains settings game-rules settings"));
		if(showRules)
		{
			GUILayout.BeginVertical("box");
			game.timer  = EditorGUILayout.FloatField(new GUIContent("Time limit", "Set time limit for level (-1 to disable)" ), game.timer);
			game.hintLimit  = EditorGUILayout.IntField(new GUIContent("Hints limit", "Set hints limit for level (-1 ro disable)" ), game.hintLimit);
			game.invertRules = EditorGUILayout.Toggle(new GUIContent("Invert rules", "Invert basic rules - i.e. player should decompose  the images"), game.invertRules);
			GUILayout.EndVertical();
		}


		// Music-related
		showMusic = EditorGUILayout.Foldout(showMusic, new GUIContent("MUSIC  SETTINGS", "Contains settings related to game music"));
		if(showMusic)
		{
			GUILayout.BeginVertical("box");
			game.musicPlayer = EditorGUILayout.ObjectField(new GUIContent("Music Player", "Link to AudioSource component to be used for playing music (leave empty to create new)"), game.musicPlayer, typeof(AudioSource), true) as AudioSource;
			game.musicMain = EditorGUILayout.ObjectField(new GUIContent("   Main", "Sound clip to be used as gameplay music"), game.musicMain, typeof(AudioClip), true) as AudioClip;
			game.musicWin = EditorGUILayout.ObjectField(new GUIContent("   Win", "Sound clip to be used as music if player won"), game.musicWin, typeof(AudioClip), true) as AudioClip;
			game.musicLose = EditorGUILayout.ObjectField(new GUIContent("   Lose", "Sound clip to be used as music if player lost"), game.musicLose, typeof(AudioClip), true) as AudioClip;
			GUILayout.EndVertical();
		}

		// Sound-related
		showSound = EditorGUILayout.Foldout(showSound, new GUIContent("SOUND  SETTINGS", "Contains settings related to game sound effects"));
		if(showSound)
		{
			GUILayout.BeginVertical("box");
			game.soundPlayer = EditorGUILayout.ObjectField(new GUIContent("Sound Player", "Link to AudioSource component to be used for playing sound effects (leave empty to create new)"), game.soundPlayer, typeof(AudioSource), true) as AudioSource;
			game.soundGrab = EditorGUILayout.ObjectField(new GUIContent("   Grab", "Sound clip will be played when player grabs puzzle-piece"), game.soundGrab, typeof(AudioClip), true) as AudioClip;
			game.soundDrop = EditorGUILayout.ObjectField(new GUIContent("   Drop", "Sound clip will be played when player drops puzzle-piece"), game.soundDrop, typeof(AudioClip), true) as AudioClip;
			game.soundAssemble = EditorGUILayout.ObjectField(new GUIContent("   Assemble", "Sound clip will be played when player assemble puzzle-piece to puzzle"), game.soundAssemble, typeof(AudioClip), true) as AudioClip;
			GUILayout.EndVertical();
		}   

		// Game UI related
		showGameUI = EditorGUILayout.Foldout(showGameUI, new GUIContent("GUI  SETTINGS", "Contains settings related to game interface"));
		if(showGameUI)
		{
			GUILayout.BeginVertical("box");
			game.pauseUI = EditorGUILayout.ObjectField(new GUIContent("Pause", "Link to object to be shown when game paused"), game.pauseUI, typeof(GameObject), true) as GameObject;
			game.winUI = EditorGUILayout.ObjectField(new GUIContent("Win", "Link to object to be shown when player won the game"), game.winUI, typeof(GameObject), true) as GameObject;
			//if (game.timer >= 0)
                game.loseUI = EditorGUILayout.ObjectField(new GUIContent("Lose", "Link to object to be shown when player lost game (if timer enabled)"), game.loseUI, typeof(GameObject), true) as GameObject;
			if (game.hintLimit != 0) game.hintCounterUI = EditorGUILayout.ObjectField(new GUIContent("Hints counter", "Link to UI text to visualize remaining hints amount (if hints enabled)"), game.hintCounterUI, typeof(Text), true) as Text;
			if (game.timer >= 0) game.timerCounterUI = EditorGUILayout.ObjectField(new GUIContent("Time counter", "Link to UI text to visualize remaining time (if timer enabled)"), game.timerCounterUI, typeof(Text), true) as Text;
			game.musicToggleUI = EditorGUILayout.ObjectField(new GUIContent("Music toggle", "Link to UI toggle to handle/visualize music enabling/disabling"), game.musicToggleUI, typeof(Toggle), true) as Toggle;
			game.soundToggleUI = EditorGUILayout.ObjectField(new GUIContent("Sound toggle", "Link to UI toggle to handle/visualize sound enabling/disabling"), game.soundToggleUI, typeof(Toggle), true) as Toggle;
			GUILayout.EndVertical();
		} 

		// Background-related (assembled puzzle preview)
		showBackground = EditorGUILayout.Foldout(showBackground, new GUIContent("BACKGROUND  SETTINGS", "Contains settings related to Background (preview of assembled puzzle)"));
		if(showBackground)
		{
			GUILayout.BeginVertical("box");
			game.background = EditorGUILayout.ObjectField(new GUIContent("Renderer", "Link to background(preview of assembled puzzle) object renderer"), game.background, typeof(Renderer), true) as Renderer;
			if (game.background) 
			{
				game.backgroundTransparency = EditorGUILayout.FloatField(new GUIContent("Transparency", "Set background transparency" ), game.backgroundTransparency);
				game.adjustBackground = EditorGUILayout.Toggle(new GUIContent("Auto-adjusting", "Try to automatically adjust background transform to the puzzle (can fails for complex cases)") , game.adjustBackground);       
			}
			GUILayout.EndVertical();
		} 


		// SetDirty if changed and update SceneView
		if (!Application.isPlaying  &&  GUI.changed) 
			EditorSceneManager.MarkSceneDirty(game.gameObject.scene);
		
	}

	//----------------------------------------------------------------------------------------------------------------------------------------------------------
}