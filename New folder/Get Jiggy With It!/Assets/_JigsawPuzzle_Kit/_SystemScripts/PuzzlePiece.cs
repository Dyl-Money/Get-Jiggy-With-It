//----------------------------------------------------------------------------------------------------------------------------------------------------------
// Script contatins all necessary info about particular puzzle-piece
//----------------------------------------------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PuzzlePiece 
{	
	public Transform transform;			// Link to transform
	public Vector3 startPosition;		// Initial position	
	public Quaternion startRotation;	// Initial rotation
	public Vector3 targetPosition;		// Target position for movement
	public Renderer renderer; 			// Link to renderer
	public Vector3 size;				// Size of piece
    
	// Important internal variables - please don't change them blindly
//	Vector3 velocity = Vector3.zero;
    public Material materialAssembled;  // Material when piece assembled in puzzle


	//===================================================================================================== 
	// Constructor
	public PuzzlePiece (Transform _transform, Material _materialAssembled)
	{
		transform = _transform;
		startPosition = _transform.localPosition;
		startRotation = _transform.localRotation;
	
		renderer = _transform.GetComponent<Renderer> ();
		materialAssembled = _materialAssembled;

		size = renderer.bounds.size;
	}

	//----------------------------------------------------------------------------------------------------
	// Calculate  piece rendedrer center offset from the piece pivot
	public Vector2 GetPieceCenterOffset () 
	{
		Vector2 pieceCenterOffset = new Vector2 (renderer.bounds.center.x - transform.position.x, renderer.bounds.center.y - transform.position.y);
		return pieceCenterOffset;
	}

    //---------------------------------------------------------------------------------------------------- 
    // Process piece movement  
    public IEnumerator Move (Vector3 _targetPosition, bool _inLocalSpace, float _movementTime) 
	{
        // Initialize
        targetPosition = _targetPosition;
        float movementTime = _movementTime;
        bool useLocalSpace = _inLocalSpace;
        Vector3 velocity = Vector3.zero;

        // Use proper positions data according to used movement space (Local or World)  and  Smoothly move piece until it reaches targetPosition
        //  while (Vector3.Distance (useLocalSpace ? transform.localPosition : transform.position,  targetPosition) > 0.1f)
        while ( ((useLocalSpace ? transform.localPosition : transform.position) - targetPosition).sqrMagnitude > 0.01f)
        {            
            if (useLocalSpace)
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref velocity, movementTime);
            else
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, movementTime);

            yield return null;
        }

        // Set final position and Assemble if needed
        if (useLocalSpace)
            transform.localPosition = targetPosition;
        else
            transform.position = targetPosition;

        if (targetPosition == startPosition)
            Assemble();
    }
    
    //----------------------------------------------------------------------------------------------------   
    // Set to assembled state
    public void Assemble ()
	{
		if (transform.childCount > 0) 
			transform.GetChild(0).gameObject.SetActive(false);

		renderer.material = materialAssembled;
	}

	//----------------------------------------------------------------------------------------------------	
}