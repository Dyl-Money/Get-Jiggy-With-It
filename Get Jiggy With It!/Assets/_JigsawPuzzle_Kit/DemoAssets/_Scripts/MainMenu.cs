//-----------------------------------------------------------------------------------------------------	
// Simple script to handle main menu (mainly audio-related aspects)
//-----------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class MainMenu : MonoBehaviour 
{

	public AudioSource musicPlayer; 
	public AudioSource soundPlayer; 

	public AudioClip musicMain;
	public AudioClip soundClick;
	public AudioClip soundDialog;
	public Toggle musicToggleUI;
	public Toggle soundToggleUI;



	//=====================================================================================================
	void Start () 
	{
		// Prepare AudioSources for soundPlayer and musicPlayer
		if (!soundPlayer  &&  (soundClick  ||  soundDialog)) soundPlayer = gameObject.AddComponent<AudioSource>();
		if (!musicPlayer  &&  musicMain) 
		{
			musicPlayer = gameObject.AddComponent<AudioSource>();
			musicPlayer.loop = true;
			PlayMusic (musicMain); 
		} 


		LoadAudioActivity(); 
	}


	//-----------------------------------------------------------------------------------------------------	 
	// Load custom level
	void LoadLevel (int _levelId) 
	{
		Time.timeScale = 1;
		SceneManager.LoadScene(_levelId);

	}

	//-----------------------------------------------------------------------------------------------------	
	// Load MusicPlayer and SoundPlayer Activity 
	void LoadAudioActivity () 
	{
		if (PlayerPrefs.HasKey("MusicPlayer")  &&  musicPlayer)  
		{
			musicPlayer.enabled = PlayerPrefs.GetInt("MusicPlayer") > 0 ? true : false;
			if (musicToggleUI) 
				musicToggleUI.isOn = musicPlayer.enabled;
		}

		if (PlayerPrefs.HasKey("SoundPlayer")  &&  soundPlayer)  
		{
			soundPlayer.enabled = PlayerPrefs.GetInt("SoundPlayer") > 0 ? true : false;
			if (soundToggleUI) 
				soundToggleUI.isOn = soundPlayer.enabled;
		}

	}

	//-----------------------------------------------------------------------------------------------------	
	// Enable/disable music 
	void SetMusicActive (bool _enabled) 
	{
		if (musicPlayer) 
		{
			musicPlayer.enabled = _enabled;

			if (musicToggleUI) 
				musicToggleUI.isOn = _enabled;
			
			PlayerPrefs.SetInt("MusicPlayer", _enabled ? 1 : 0);

			if (musicPlayer.enabled) 
				musicPlayer.Play();
		}

	}

	//-----------------------------------------------------------------------------------------------------	 
	// Enable/disable sounds 
	void SetSoundActive (bool _enabled) 
	{
		if (soundPlayer) 
		{
			soundPlayer.enabled = _enabled;
			if (soundToggleUI) soundToggleUI.isOn = _enabled;
			PlayerPrefs.SetInt("SoundPlayer", _enabled ? 1 : 0);
		}

	}
	//-----------------------------------------------------------------------------------------------------	 
	// Change and Play music clip
	void PlayMusic (AudioClip _music) 
	{
		if (musicPlayer  &&  musicPlayer.enabled  &&  _music)
		{
			musicPlayer.clip = _music;
			musicPlayer.Play();
		}

	}

	//-----------------------------------------------------------------------------------------------------	 
	// Play sound clip once
	void PlaySound (AudioClip _sound) 
	{
		if (soundPlayer  &&  soundPlayer.enabled  &&  _sound) 
			soundPlayer.PlayOneShot(_sound);

	}	  


}