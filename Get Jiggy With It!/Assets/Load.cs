using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour
{
    public int scene;

    bool loaded;

    public void OnTriggered()
    {
        if (!loaded)
        {
            SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

            loaded = true;
        }
    }
}
