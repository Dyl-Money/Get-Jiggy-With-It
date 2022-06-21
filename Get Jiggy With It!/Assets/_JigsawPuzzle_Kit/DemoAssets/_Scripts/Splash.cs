//-----------------------------------------------------------------------------------------
// Simple script to show Splash image before game
//-----------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class Splash : MonoBehaviour 
{
	public Image splashTexture;	        // Splash image texture
	public float animationTime = 1.0f;	// How fast should it appear/disappear
	public float stayTime = 2.0f;		// How long should splash be wisible before disappearing


	// Important internal variables - please don't change them blindly
	float stateTime;
	int state = 0;
	Color color;


	//=======================================================================================================	
	// Initialize internal variables
	void Start () 
	{
		Time.timeScale = 1.0f;

		if (!splashTexture)
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
		else 
			{				
				color = new Color(splashTexture.color.r, splashTexture.color.g, splashTexture.color.b, 0.0f);
				stateTime = Time.time + animationTime;
			}
			
	}

	//----------------------------------------------------------------------------------
	// Process Splash through all stages
	void Update ()
	{
		if (Time.time > stateTime)
			switch (state) 
			{
				case 0:	// Process appearance animation
					state = 1;
					stateTime = Time.time + stayTime;
					break;

				case 1: // Stay on screen
					state = 2;
					stateTime = Time.time + animationTime;
					break;

				case 2: // Process disappearance animation
					state = 3;
					break;

				case 3: // Load next level   
					SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
					break;
			}
		else 
			{
				color.a += Time.deltaTime * (1 - state);
				splashTexture.color = color;
			}

	}

	//----------------------------------------------------------------------------------
}