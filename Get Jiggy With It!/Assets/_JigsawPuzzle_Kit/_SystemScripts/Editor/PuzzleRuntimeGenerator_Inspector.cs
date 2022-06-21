//----------------------------------------------------------------------------------------------------------------------------------------------------------
// Provides with custom and more convenient Inspector GUI for PuzzleGenerator_Runtime
//----------------------------------------------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(PuzzleGenerator_Runtime))]
public class PuzzleRuntimeGenerator_Inspector : Editor
{
    // Important internal variables
    PuzzleGenerator_Runtime puzzle;
    static bool initialized;


    //========================================================================================================================================================== 
    public void OnEnable()
    {
       // puzzle = target as PuzzleGenerator_Runtime;
        if (!initialized)
        {
            PuzzleUtitlities.AddPuzzleTags();            
            initialized = true;
        }
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------
}