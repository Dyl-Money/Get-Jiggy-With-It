using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour
{
    private AudioPause music;
    public Button musicToggleButton;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    void Start()
    {
        music = GameObject.FindObjectOfType<AudioPause>();
        UpdateIcon();
    }

    public void PauseMusic()
    {
        music.ToggleSound();
        UpdateIcon();
    }

    void UpdateIcon()
    {
        if(PlayerPrefs.GetInt("Muted", 0) == 0)
        {
            musicToggleButton.GetComponent<Image>().sprite = musicOnSprite;
        }
        else
        {
            musicToggleButton.GetComponent<Image>().sprite = musicOffSprite;
        }
    }
}
