using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Unload : MonoBehaviour
{
    public int scene;

    bool unloaded;

    public void OnTriggered()
    {
        if (!unloaded)
        {
            unloaded = true;

            AnyManager.anyManager.UnloadScene(scene);
        }
    }
}
