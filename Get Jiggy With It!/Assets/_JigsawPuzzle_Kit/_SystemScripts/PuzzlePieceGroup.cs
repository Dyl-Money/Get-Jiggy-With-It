//---------------------------------------------------------------------------------------------------------------------------------------------------
// Main class for puzzle process grouping
//---------------------------------------------------------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PuzzlePieceGroup : MonoBehaviour 
{
	public List<PuzzlePiece> puzzlePieces; // List of pieces in the group
	public Transform thisTransform;


	//---------------------------------------------------------------------------------------------------------------------------------------------------
	// Init
	private void Awake() 
	{
		thisTransform = transform;
		puzzlePieces = new List<PuzzlePiece>();
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------
	// Main function to process grouping pieces and clamping groups
	public static bool MergeGroupsOrPieces(PuzzlePiece _puzzlePiece1, PuzzlePiece _puzzlePiece2, PuzzleController _puzzleController) 
	{
		GameObject object1 = _puzzlePiece1.transform.gameObject;
		GameObject object2 = _puzzlePiece2.transform.gameObject;


		//Convert to group if it's not a single piece
		object1 = GetGroupObjectIfPossible(object1);
		object2 = GetGroupObjectIfPossible(object2);

		if (object1 == object2) 
			return false;

		// Get groups
		PuzzlePieceGroup pieceGroup1 = object1.GetComponent<PuzzlePieceGroup>();
		PuzzlePieceGroup pieceGroup2 = object2.GetComponent<PuzzlePieceGroup>();


		//// If both objects non-grouped
		if (pieceGroup1 == null  &&  pieceGroup2 == null) 
		{
			bool clamped = TryClampPieces(_puzzlePiece1, _puzzlePiece2, _puzzleController);

			// Create new group
			if (clamped) 
			{
				GameObject pieceGroupObject = new GameObject("_PieceGroup".ToString());
				pieceGroupObject.transform.parent = _puzzleController.transform;
				PuzzlePieceGroup pieceGroup = pieceGroupObject.AddComponent<PuzzlePieceGroup>();

				// Include pieces in the group
				pieceGroup.AddNewPiece(_puzzlePiece1);
				pieceGroup.AddNewPiece(_puzzlePiece2);

				pieceGroup.RecalculateCenterAndTransformGroup();
			}

			return clamped;
		} 
		else  //// If both objects are groups
			if (pieceGroup1 != null  &&  pieceGroup2 != null) 
			{
				foreach (PuzzlePiece puzzlePiece1 in pieceGroup1.puzzlePieces) 
					foreach (PuzzlePiece puzzlePiece2 in pieceGroup2.puzzlePieces) 
						if (TryClampPieces(puzzlePiece1, puzzlePiece2, _puzzleController)) 
						{
							// Move pieces to one group
							foreach (PuzzlePiece puzzlePiece in pieceGroup2.puzzlePieces) 
								pieceGroup1.AddNewPiece(puzzlePiece);
							
							pieceGroup1.RecalculateCenterAndTransformGroup();
							Destroy(pieceGroup2.gameObject);

							return true;
						}
				
			} 
			else  //// One object is a group and another is non-grouped piece
			{
				PuzzlePieceGroup pieceGroup;
				PuzzlePiece puzzlePiece;

				// Figure out which object is group/single piece
				if (pieceGroup1 == null) 
				{
					pieceGroup = pieceGroup2;
					puzzlePiece = _puzzlePiece1;
				} 
				else 
					{
						pieceGroup = pieceGroup1;
					    puzzlePiece = _puzzlePiece2;
					}

				// Try to clamp each piece in group with the single piece
				for (int i = 0; i < pieceGroup.puzzlePieces.Count; ++i)
					if (TryClampPieces(puzzlePiece, pieceGroup.puzzlePieces[i], _puzzleController)) 
					{
						pieceGroup.AddNewPiece(puzzlePiece);
						pieceGroup.RecalculateCenterAndTransformGroup();

						return true;
					}
				
			}
	

		return false;
	}
		

	//---------------------------------------------------------------------------------------------------------------------------------------------------
	// Try to group 2 pieces
	private static bool TryClampPieces(PuzzlePiece _puzzlePiece1, PuzzlePiece _puzzlePiece2, PuzzleController _puzzleController) 
	{
		// Get difference in positions etc
		float eulerAnglesZ = (_puzzlePiece1.transform.rotation * Quaternion.Inverse(_puzzlePiece2.transform.rotation)).eulerAngles.z;
		Vector2 startDiff = _puzzlePiece1.startPosition - _puzzlePiece2.startPosition;
		Vector2 diff = _puzzlePiece1.renderer.bounds.center - _puzzlePiece2.renderer.bounds.center;
		Vector2 averageSize = (_puzzlePiece1.size + _puzzlePiece2.size) / 2;


		// Check is difference in allowed limits
		if (Mathf.Abs(Mathf.DeltaAngle(eulerAnglesZ, 0)) > _puzzleController.allowedRotation) 
			return false;
		
		if (Mathf.Abs(startDiff.x) > _puzzleController.allowedDistance  &&  Mathf.Abs(startDiff.y) > _puzzleController.allowedDistance) 
			return false;

		if (Mathf.Abs(startDiff.x) > averageSize.x*1.1f  ||  Mathf.Abs(startDiff.y) > averageSize.y*1.1f) 
			return false;
		

		// Get transforms (group transforms if possible) and update localRotation
		Transform pieceOrGroup1Transform = GetGroupObjectIfPossible(_puzzlePiece1.transform.gameObject).transform;
		Transform pieceOrGroup2Transform = GetGroupObjectIfPossible(_puzzlePiece2.transform.gameObject).transform;
		Quaternion originalRotation = pieceOrGroup1Transform.localRotation;
		pieceOrGroup1Transform.localRotation = pieceOrGroup2Transform.localRotation;
	
		// Create temp-object to get proper/not-Affected rotationDiff 
		GameObject tempObject = new GameObject("_TempObject");
		Transform tempObjectTransform = tempObject.transform;
		 tempObjectTransform.position = new Vector3(startDiff.x, startDiff.y, 0);
		 tempObjectTransform.RotateAround(Vector3.zero, Vector3.forward, _puzzlePiece1.transform.rotation.eulerAngles.z);
		 Vector2 rotatedStartDiff = new Vector2(tempObjectTransform.position.x, tempObjectTransform.position.y);
		 tempObjectTransform.parent = null;
		Destroy(tempObject);

		// Check is rotation in allowed limits
		if ((diff - rotatedStartDiff).magnitude > _puzzleController.allowedDistance) 
		{
			pieceOrGroup1Transform.localRotation = originalRotation;
			return false;
		}

		// Update localPosition
		rotatedStartDiff -= (Vector2)(_puzzlePiece1.transform.position - _puzzlePiece2.transform.position);
		pieceOrGroup1Transform.localPosition += new Vector3(rotatedStartDiff.x, rotatedStartDiff.y, 0);

		return true;
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------
	// Return Group object if object or its paren is group
	public static GameObject GetGroupObjectIfPossible(GameObject _pieceOrGroupObject) 
	{
		// Is it a Group
		PuzzlePieceGroup pieceGroup = _pieceOrGroupObject.GetComponent<PuzzlePieceGroup>();

		// Is it part of a Group
		if (pieceGroup == null) 
			if (_pieceOrGroupObject.transform.parent.GetComponent<PuzzlePieceGroup>() != null) 
				_pieceOrGroupObject = _pieceOrGroupObject.transform.parent.gameObject; 

		return _pieceOrGroupObject;
	}
		
	//---------------------------------------------------------------------------------------------------------------------------------------------------
	// Add piece and update it transform
	private void AddNewPiece(PuzzlePiece puzzlePiece) 
	{
		if (puzzlePieces.Count == 0) 
			thisTransform.localRotation = puzzlePiece.transform.localRotation;
		
		puzzlePieces.Add(puzzlePiece);

		float pieceZ = puzzlePiece.transform.localPosition.z;
		if (pieceZ != thisTransform.localPosition.z) 
			thisTransform.localPosition = new Vector3(transform.localPosition.x, thisTransform.localPosition.y, pieceZ);
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------
	// Recalculate group center and pieces transform inside
	private void RecalculateCenterAndTransformGroup() 
	{
		Vector3 center = Vector3.zero;
		foreach (PuzzlePiece puzzlePiece in puzzlePieces) 
		{
			center += puzzlePiece.renderer.bounds.center;
			puzzlePiece.transform.parent = null;
		}			
		thisTransform.position = center / puzzlePieces.Count;

		//
		foreach (PuzzlePiece puzzlePiece in puzzlePieces) 
		{
			puzzlePiece.transform.parent = thisTransform;
			puzzlePiece.transform.localRotation = Quaternion.identity;
			puzzlePiece.transform.localPosition = new Vector3(puzzlePiece.transform.localPosition.x, puzzlePiece.transform.localPosition.y, -0.0001f);
		}
			
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------
}