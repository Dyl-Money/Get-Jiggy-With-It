using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void GotoGameScene()
    {
        SceneManager.LoadScene("GameScreen2");
    }

    public void GotoMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}