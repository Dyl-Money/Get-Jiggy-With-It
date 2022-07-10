using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]

public class AudioPause : MonoBehaviour
{
    public AudioClip song1;
    public AudioClip song2;

    private AudioSource audioSource;
    private bool paused1;
    private bool paused2;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.GetInt("Muted", 0) == 0)
        {
            audioSource.clip = song1;
            audioSource.Play(0);
        }
    }

    public void ToggleSound()
    {
        if (PlayerPrefs.GetInt("Muted", 0) == 0)
        {
            PlayerPrefs.SetInt("Muted", 1);
            audioSource.Pause();
        }
        else
        {
            PlayerPrefs.SetInt("Muted", 0);
            audioSource.clip = song1;
            audioSource.Play(0);
        }
    }
}
