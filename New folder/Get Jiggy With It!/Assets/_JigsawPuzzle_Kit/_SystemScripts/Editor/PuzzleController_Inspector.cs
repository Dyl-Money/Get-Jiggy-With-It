//----------------------------------------------------------------------------------------------------------------------------------------------------------
// Provides with custom and more convenient Inspector GUI for PuzzleController
//----------------------------------------------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;



[CustomEditor (typeof(PuzzleController))]
public class PuzzleController_Inspector: Editor 
{

	// Important internal variables
	PuzzleController puzzle;
	bool showDecomposition;
	bool showPieces;
	Color defaultGUIColor;


	//========================================================================================================================================================== 
	public void OnEnable ()
	{
		puzzle = target as PuzzleController;
		if (!Application.isPlaying  &&  puzzle.pieces == null) 
			puzzle.Prepare ();
	}
		

	//----------------------------------------------------------------------------------------------------------------------------------------------------------
	// Draw whole custom inspector GUI
	public override void OnInspectorGUI()
	{   
		puzzle = target as PuzzleController;
		defaultGUIColor = GUI.color;

        puzzle.pieceMaterial_assembled = EditorGUILayout.ObjectField(new GUIContent("Assembled material:", "This Material will be used for assembled puzzle-elements"), puzzle.pieceMaterial_assembled, typeof(Material), false) as Material;


        // Show foldout with UI for DECOMPOSITION SETTINGS
        if (!Application.isPlaying)
        {
            EditorGUILayout.Space();
            if (!puzzle.changeOnlyRotation)
            {
                puzzle.enablePositionSaving = EditorGUILayout.Toggle(new GUIContent("Save positions", "Position and rotation of all pieces (in incomplete puzzles) will be saved/restored"), puzzle.enablePositionSaving);
                puzzle.enablePiecesGroups = EditorGUILayout.Toggle(new GUIContent("Enable groups", "Enables unassemdbled pieces grouping (For now - please don't use for imported and 3D puzzles)"), puzzle.enablePiecesGroups);
                puzzle.fullyIn3D = EditorGUILayout.Toggle(new GUIContent("Process fully in 3D", "Allow shifting(including during decomposition) pieces in all 3 dimensions.\n Require pieces to be strictly in their 3D place for assembling puzzle."), puzzle.fullyIn3D);
                EditorGUILayout.Space();
            }
            else
                {
                    puzzle.enablePositionSaving = false;
                    puzzle.enablePiecesGroups = false;
                    puzzle.fullyIn3D = false;
                }


        showDecomposition = EditorGUILayout.Foldout(showDecomposition, new GUIContent("DECOMPOSITION SETTINGS", "Contains settings related to decomposition areas"));
			if(showDecomposition)
			{
               GUILayout.BeginVertical();
                if (!puzzle.changeOnlyRotation)
                { 				
				    EditorGUILayout.LabelField(new GUIContent("Areas locations:", "Location of areas around puzzle where pieces should be randomly moved during decomposition"), EditorStyles.boldLabel);

                    GUILayout.BeginVertical("HORIZONTAL", "Box");
                        GUILayout.BeginHorizontal("Button");
					        puzzle.decomposeToLeft = EditorGUILayout.ToggleLeft("Left", puzzle.decomposeToLeft);
                            puzzle.decomposeToRight = EditorGUILayout.ToggleLeft("Right", puzzle.decomposeToRight);	
					    GUILayout.EndHorizontal();

                        if (puzzle.decomposeToLeft  || puzzle.decomposeToRight)
                        { 
                            if (puzzle.fullyIn3D)
                                puzzle.horizontalAreasSize = EditorGUILayout.Vector3Field(new GUIContent("Size", "Decomposition areas size"), puzzle.horizontalAreasSize);
                            else
                                puzzle.horizontalAreasSize = EditorGUILayout.Vector2Field(new GUIContent("Size", "Decomposition areas size"), puzzle.horizontalAreasSize);

                            puzzle.autoHorizontalAreaOffset = EditorGUILayout.Toggle(new GUIContent("Calculate offset", "Automaticaly calculate decomposition areas offset from puzzle"), puzzle.autoHorizontalAreaOffset);

                            if (!puzzle.autoHorizontalAreaOffset)
                                if (puzzle.fullyIn3D)
                                    puzzle.horizontalAreaOffset = EditorGUILayout.Vector3Field("Offset", puzzle.horizontalAreaOffset);
                                else
                                    puzzle.horizontalAreaOffset = EditorGUILayout.Vector2Field("Offset", puzzle.horizontalAreaOffset);
                        }
                    GUILayout.EndVertical();


                    GUILayout.BeginVertical("VERTICAL", "Box");
                        GUILayout.BeginHorizontal("Button");
					        puzzle.decomposeToTop = EditorGUILayout.ToggleLeft("Top", puzzle.decomposeToTop);
						    puzzle.decomposeToBottom = EditorGUILayout.ToggleLeft("Bottom", puzzle.decomposeToBottom); 	
					    GUILayout.EndHorizontal();

                        if (puzzle.decomposeToTop || puzzle.decomposeToBottom)
                        {
                            if (puzzle.fullyIn3D)
                                puzzle.verticalAreasSize = EditorGUILayout.Vector3Field(new GUIContent("Size", "Decomposition areas size"), puzzle.verticalAreasSize);
                            else
                                puzzle.verticalAreasSize = EditorGUILayout.Vector2Field(new GUIContent("Size", "Decomposition areas size"), puzzle.verticalAreasSize);

                            puzzle.autoVerticalAreaOffset = EditorGUILayout.Toggle(new GUIContent("Calculate offset", "Automaticaly calculate decomposition areas offset from puzzle"), puzzle.autoVerticalAreaOffset);

                            if (!puzzle.autoVerticalAreaOffset)
                                if (puzzle.fullyIn3D)
                                    puzzle.verticalAreaOffset = EditorGUILayout.Vector3Field("Offset", puzzle.verticalAreaOffset);
                                else
                                    puzzle.verticalAreaOffset = EditorGUILayout.Vector2Field("Offset", puzzle.verticalAreaOffset);
                        }
                    GUILayout.EndVertical();
               
                    EditorGUILayout.Space(); 
                }


					//EditorGUILayout.Space(); 					
					//EditorGUILayout.LabelField(new GUIContent("Pieces properties:", "Some important pieces properties applying during puzzledecomposition"), EditorStyles.boldLabel);
					//puzzle.finalTransparency  = EditorGUILayout.FloatField(new GUIContent("Final Transparency", "Set piece transparency when it assembled to puzzle" ), puzzle.finalTransparency);
					puzzle.randomizeRotation = EditorGUILayout.Toggle(new GUIContent("Randomize rotation", "Sets should pieces be rotated during decomposition" ), puzzle.randomizeRotation); 
                    if (puzzle.randomizeRotation)
                        puzzle.changeOnlyRotation = EditorGUILayout.Toggle(new GUIContent("Change only rotation", "Pieces willn't be moved during decomposition (only rotated)"), puzzle.changeOnlyRotation);
                     else
                        puzzle.changeOnlyRotation = false;


                    EditorGUILayout.Space();

			    GUILayout.EndVertical();
			}


			// Draw PREPARE PUZZLE button
			GUI.color = Color.yellow;
				
			if (GUILayout.Button(new GUIContent("RECALCULATE", "Recalculates whole puzzle and prepares it to be used"))) 
				puzzle.Prepare ();
			
			GUI.color = defaultGUIColor;

		}


		// Show foldout with UI for PIECES MOVEMENT SETTING
		EditorGUILayout.Space();
		showPieces = EditorGUILayout.Foldout(showPieces, new GUIContent("PIECES MOVEMENT SETTINGS", "Pieces movement/rotation properties"));

		if(showPieces)
		{
			GUILayout.BeginVertical("box");

				GUILayout.BeginVertical("Button");
					puzzle.allowedDistance  = EditorGUILayout.FloatField(new GUIContent("Magnet Distance", "Allowed position offset to consider piece placed to it origin" ), puzzle.allowedDistance);
					puzzle.allowedRotation  = EditorGUILayout.FloatField(new GUIContent("Magnet Rotation", "Allowed rotation offset to consider piece placed to it origin" ), puzzle.allowedRotation);
				GUILayout.EndVertical();				
				EditorGUILayout.Space();

				puzzle.movementTime = EditorGUILayout.FloatField(new GUIContent("Movement Time", "Piece needs this amount of time to reach destination during automatic movement" ), puzzle.movementTime);
				EditorGUILayout.Space();

				if (puzzle.randomizeRotation) 
					puzzle.rotationSpeed = EditorGUILayout.FloatField(new GUIContent("Rotation Speed", "How fast piece can be rotated by player" ), puzzle.rotationSpeed);
				EditorGUILayout.Space();

				#if !UNITY_STANDALONE
					puzzle.mobileDragOffsetY = EditorGUILayout.FloatField(new GUIContent("Drag Y-offset", "Piece offset(in % of piece size) during dragging by player" ), puzzle.mobileDragOffsetY);
				#endif

				puzzle.dragOffsetZ = EditorGUILayout.FloatField(new GUIContent("Drag Z-offset", "Piece offset during dragging by player" ), puzzle.dragOffsetZ);
				puzzle.dragTiltSpeed = EditorGUILayout.FloatField(new GUIContent("Drag Tilt Speed", "Piece tilt-speed during dragging by player" ), puzzle.dragTiltSpeed);
				EditorGUILayout.Space();

			GUILayout.EndVertical();
		}


		// SetDirty if changed and update SceneView
		if (GUI.changed) 
		{
			EditorUtility.SetDirty(this);  
			SceneView.RepaintAll(); 
			puzzle.enabled = false;
			puzzle.enabled = true; 
			if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(puzzle.gameObject.scene);     
		}

	}

	//----------------------------------------------------------------------------------------------------------------------------------------------------------
}
#endif