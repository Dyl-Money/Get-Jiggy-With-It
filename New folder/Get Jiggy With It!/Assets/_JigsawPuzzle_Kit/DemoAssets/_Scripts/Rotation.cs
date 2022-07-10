//-----------------------------------------------------------------------------------------------------	
// Simple script to rotate object left/right along Z-axis (used in 3D-puzzle example-scene)
//-----------------------------------------------------------------------------------------------------	
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Rotation : MonoBehaviour 
{

	public float rotationSpeed = 10.0f;
	public Vector3 rotationAxis = Vector3.forward;
	Transform cTransform;

	//=====================================================================================================
	void Start () 
	{
		cTransform = transform;

	}

	//-----------------------------------------------------------------------------------------------------	
	public void RotateLeft () 
	{
		cTransform.Rotate(rotationAxis * rotationSpeed);

	}

	//-----------------------------------------------------------------------------------------------------	
	public void RotateRight () 
	{
		cTransform.Rotate(rotationAxis * -rotationSpeed);

	}

	//-----------------------------------------------------------------------------------------------------	
}
