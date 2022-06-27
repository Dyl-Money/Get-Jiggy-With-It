//-----------------------------------------------------------------------------------------------------	
// Script allows to pan and zoom(pinch) camera to have access to small details in scene 
//-----------------------------------------------------------------------------------------------------	
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[AddComponentMenu("Scripts/Jigsaw Puzzle/Camera Controller")]
public class CameraController : MonoBehaviour 
{
    Camera controlledCamera;

	[Header("Zoom")]

	[Range (0f, 5f)]
	public float zoomSpeed = 1;                     // Zoom changing speed. Disable Zooming functionality if 0
    public Vector2 zoomLimits = new Vector2(3, -3); // Camera orthographicSize changing limits
	public bool doubleClickZooming = true;			// Enable/Disable Zooming by double-click/tap
    public bool disableZooming;                     // Disable Zooming functionality

    [Header("Movement")]
    public float panSpeed = 1;
    public Vector2 panLimits = new Vector2(10, 8);  // Camera x,y  position changing limits		
    public bool disablePanning;                     // Disable Panning functionality


    // Important internal variables - please don't change them blindly
    Transform cameraTransform;
    float initialZoom;
    Vector3 initialPosition;
    Vector3 deltaPosition;
    Vector3 oldPointerPosition;
    readonly float doubleClickMaxDelay = 0.3f;
	float doubleClickDelay;           
    bool inMovement;
 
 
    //=======================================================================================================================================================================
    // Get initial data
    void Start () 
	{
        if (!controlledCamera)
            controlledCamera = gameObject.GetComponent<Camera>();

        if (!controlledCamera)
        {
          controlledCamera = Camera.main;
          Debug.LogWarning("There's no <i>camera</i> attached/assigned for <b>CameraController<b> component of " + gameObject.name + ". We'll use first found Main Camera.", gameObject);
        }

        if (!controlledCamera)
           Debug.LogError("We can't find any camera in scene - please add it", gameObject);

        cameraTransform = controlledCamera.transform;

        initialZoom = controlledCamera.orthographicSize;      
		initialPosition = cameraTransform.position;
  
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Process Camera Zoom and Pan
    void LateUpdate ()
	{
        // Panning camera      
        if (!disablePanning  &&  Input.touchCount <= 1)
        {
            inMovement = false;

            if (Input.GetMouseButtonDown(0)  ||  oldPointerPosition == Vector3.forward)
                oldPointerPosition = controlledCamera.ScreenToWorldPoint(Input.mousePosition);


            if (Input.GetMouseButton(0))
            {
                inMovement = true;
                deltaPosition = oldPointerPosition - controlledCamera.ScreenToWorldPoint(Input.mousePosition);

                if (Mathf.Abs(deltaPosition.x) > 0.001f || Mathf.Abs(deltaPosition.y) > 0.001f)
                {
                    cameraTransform.position = new Vector3(
                                                            Mathf.Clamp(cameraTransform.position.x + deltaPosition.x, initialPosition.x - panLimits.x, initialPosition.x + panLimits.x),
                                                            Mathf.Clamp(cameraTransform.position.y + deltaPosition.y, initialPosition.y - panLimits.y, initialPosition.y + panLimits.y),
                                                            cameraTransform.position.z
                                                          );

                    oldPointerPosition = controlledCamera.ScreenToWorldPoint(Input.mousePosition);
                }
            }
        }


        

        // Zooming
        if (!disableZooming) 
		{ 
			#if UNITY_EDITOR  ||  UNITY_STANDALONE  ||  UNITY_WEBPLAYER 
			// Mouse scroll zoom
				controlledCamera.orthographicSize -= zoomSpeed * Input.GetAxis ("Mouse ScrollWheel");
           
            #else  // For touch-devices:  Pinch-zoom
				if (Input.touchCount > 1) 
				{
                    oldPointerPosition = Vector3.forward;
					// If there are two touches on the device... Store both touches.
					Touch touchZero = Input.GetTouch (0);
					Touch touchOne 	= Input.GetTouch (1);

					// Find the position in the previous frame of each touch.
					Vector3 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
					Vector3 touchOnePrevPos  = touchOne.position - touchOne.deltaPosition;

					// Find the magnitude (the distance) between the touches in each frame, then calculate the difference between them.
					float deltaMagnitudeDiff = (touchZeroPrevPos - touchOnePrevPos).magnitude - (touchZero.position - touchOne.position).magnitude;

					// Change the orthographic size based on the change in distance between the touches.
					controlledCamera.orthographicSize += deltaMagnitudeDiff * zoomSpeed / 100;
				}
            #endif



            //Double-click(tap) zoom
            if (doubleClickZooming  &&  Input.GetMouseButtonUp (0)  &&  Time.timeScale > 0)
				if (doubleClickDelay > Time.time) 
				{
                    if (controlledCamera.orthographicSize < initialZoom)
						controlledCamera.orthographicSize = initialZoom;
					else
                        {
                             cameraTransform.position = controlledCamera.ScreenToWorldPoint(Input.mousePosition);
                             cameraTransform.position = new Vector3(
                                                                        Mathf.Clamp(cameraTransform.position.x, initialPosition.x - panLimits.x, initialPosition.x + panLimits.x),
                                                                        Mathf.Clamp(cameraTransform.position.y, initialPosition.y - panLimits.y, initialPosition.y + panLimits.y),
                                                                        cameraTransform.position.z
                                                                   );
                            controlledCamera.orthographicSize = initialZoom + zoomLimits.y;
                        }
						
                    doubleClickDelay = 0;
				} 
				else
					doubleClickDelay = Time.time + doubleClickMaxDelay;


            // Check if Camera orthographicSize(zoom) is still within zoomLimits
            controlledCamera.orthographicSize = Mathf.Clamp(controlledCamera.orthographicSize, initialZoom + zoomLimits.y, initialZoom + zoomLimits.x);

		}


    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Return true if camera have been moved during last frame
    public bool IsCameraMoved()
    {
        return inMovement;
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Return camera to initial position (and reset orthographicSize if needed)
    public void ReturnCamera(bool resetZoom = false)
    {
        cameraTransform.position = initialPosition;

        if (resetZoom)
            controlledCamera.orthographicSize = initialZoom;
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // Set Initials
    public void SetInitialZoom(float _initialZoom)
    {
        initialZoom = _initialZoom;
    }

    public void SetInitialPosition(Vector3 _initialPosition)
    {
        initialPosition = _initialPosition;
    }

    //===================================================================================================== 
    // Utility visualization function
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube( Application.isPlaying ? initialPosition : transform.position, new Vector3(panLimits.x*2, panLimits.y*2, 0));        
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
}