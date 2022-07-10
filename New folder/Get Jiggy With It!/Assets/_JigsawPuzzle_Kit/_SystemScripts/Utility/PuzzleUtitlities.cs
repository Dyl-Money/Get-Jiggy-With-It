//----------------------------------------------------------------------------------------------------------------------------------------------------------
// Useful utility functions
//----------------------------------------------------------------------------------------------------------------------------------------------------------
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public static class PuzzleUtitlities
{
    //----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Add anew tag to Unity tags list
    public static void AddPuzzleTags(string _puzzleTag = "Puzzle_Main", string _puzzleBackgroundTag = "Puzzle_Background")
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if ((asset != null) && (asset.Length > 0))
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");


            bool puzzleTagExists = false, backgroundTagExists = false;

            // Check  if tags already exists
            for (int i = 0; i < tags.arraySize; ++i)
            {
                if (tags.GetArrayElementAtIndex(i).stringValue == _puzzleTag)
                    puzzleTagExists = true;

                if (tags.GetArrayElementAtIndex(i).stringValue == _puzzleBackgroundTag)
                    backgroundTagExists = true;
            }

            // Add tags to tags list if needed
            if (!puzzleTagExists)
            { 
                tags.InsertArrayElementAtIndex(tags.arraySize);
                tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = _puzzleTag;
            }

            if (!backgroundTagExists)
            {
                tags.InsertArrayElementAtIndex(tags.arraySize);
                tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = _puzzleBackgroundTag;
            }

            // Apply changes
            so.ApplyModifiedProperties();
            so.Update();
        }
        else
            Debug.LogWarning("Can't automatically add tags related to PuzzleKit");

    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Add anew tag to Unity tags list
    public static void AddTag(string _tag)
    {
        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");

        if ((asset != null) && (asset.Length > 0))
        {
            SerializedObject so = new SerializedObject(asset[0]);
            SerializedProperty tags = so.FindProperty("tags");

            // Check and exit if tag already exists
            for (int i = 0; i < tags.arraySize; ++i)
                if (tags.GetArrayElementAtIndex(i).stringValue == _tag)
                    return;

            // Add the tag in the end of tags list
            tags.InsertArrayElementAtIndex(tags.arraySize);
            tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = _tag;

            so.ApplyModifiedProperties();
            so.Update();

        }
        else
            Debug.LogWarning("Can't add a new tag: " + _tag);
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------
    /*
    // If there's no Material for pieces assembled state - Generate it
    if (!pieceMaterial_assembled)
    {
        pieceMaterial_assembled = new Material(thisTransform.GetChild(0).GetComponent<Renderer>().sharedMaterial);
        pieceMaterial_assembled.color = new Color(pieceMaterial_assembled.color.r, pieceMaterial_assembled.color.g, pieceMaterial_assembled.color.b, finalTransparency);
        if (pieceMaterial_assembled.HasProperty("_BevelIntens"))
            pieceMaterial_assembled.SetFloat("_BevelIntens", finalTransparency / 2);
        pieceMaterial_assembled.name = "Temp_pieceMaterial_assembled";
    }
    */
}
#endif