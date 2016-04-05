using UnityEngine;
using System.Collections;

/// <summary>
/// Taken from https://www.reddit.com/r/Unity3D/comments/3xhnt4/xpost_from_gamedev_unity_scripts_and_tips_for/
/// </summary>
public class PauseFreeCamera : MonoBehaviour {
    
    float turnSpeed = 2.0f;		// Speed of camera turning when mouse moves in along an axis
    float panSpeed = 1.0f;		// Speed of the camera when being panned
    float zoomSpeed = 1.0f;		// Speed of the camera going back and forth

    private Vector3 mouseOrigin;	// Position of cursor when mouse dragging starts
    private bool isPanning;		// Is the camera being panned?
    private bool isRotating;	// Is the camera being rotated?
    private bool isZooming;		// Is the camera zooming?

    [HideInInspector]
    public bool usingPauseFreeCamera = false;
    [HideInInspector]
    public bool usingSlowMotion = false;

    public MonoBehaviour[] cameraScriptToDisable;

    float originalTimeScale;

    Vector3 originalCameraPos;
    Quaternion originalCameraRotation;

    public KeyCode toggleScreenshotModeKey = KeyCode.F10;
    public KeyCode nextFrameKey = KeyCode.F11;
    public KeyCode toggleSlowMotionModeKey = KeyCode.F12;

    Transform originalParent;

    private void switchCameraMode()
    {
        if (usingPauseFreeCamera)
        {
            usingPauseFreeCamera = false;
            Time.timeScale = 1;

            foreach (var script in cameraScriptToDisable)
            {
                script.enabled = true;
            }
            transform.localPosition = originalCameraPos;
            transform.localRotation = originalCameraRotation;
            transform.parent = originalParent;
        }
        else
        {
            foreach (var script in cameraScriptToDisable)
            {
                script.enabled = false;
            }
            originalParent = transform.parent;
            transform.parent = null;
            originalCameraRotation = transform.localRotation;
            originalCameraPos = transform.localPosition;
            Time.timeScale = 0;
            usingPauseFreeCamera = true;
        }
    }

    IEnumerator AdvanceOneFrame()
    {
        Time.timeScale = 0.9f;
        yield return null;
        Time.timeScale = 0;
    }

    void Update()
        {
            if (Input.GetKeyDown(nextFrameKey))
            {
                StartCoroutine(AdvanceOneFrame());
            }

            if(Input.GetKeyDown(toggleScreenshotModeKey)) {
                switchCameraMode();
            }

            if (Input.GetKeyDown(KeyCode.F12))
            {
                usingSlowMotion = !usingSlowMotion;
                if (usingSlowMotion == false)
                    Time.timeScale = 1;
            }

            if (usingSlowMotion)
            {
                if (Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(1))
                {
                    usingSlowMotion = false;
                    if (!usingPauseFreeCamera)
                        switchCameraMode();
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0.1f)
                {
                    Time.timeScale = Mathf.Clamp(Time.timeScale + 0.35f, 0.1f, 1);
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < -0.1f)
                {
                    Time.timeScale = Mathf.Clamp(Time.timeScale - 0.35f, 0.1f, 1);
                }
            }

            if (!usingPauseFreeCamera)
                return;

        if (Input.GetAxisRaw("Vertical") != 0)
        {
                transform.localPosition = transform.localPosition + (transform.forward * Input.GetAxisRaw("Vertical") * 0.035f);
        }
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
                transform.localPosition = transform.localPosition + (transform.right * Input.GetAxisRaw("Horizontal") * 0.035f);
        }
	
		if(Input.GetMouseButtonDown(0))
		{
			mouseOrigin = Input.mousePosition;
			isRotating = true;
		}
		
		if(Input.GetMouseButtonDown(1))
		{
			mouseOrigin = Input.mousePosition;
			isPanning = true;
		}
		
		if(Input.GetMouseButtonDown(2))
		{
			mouseOrigin = Input.mousePosition;
			isZooming = true;
		}
		
		if (!Input.GetMouseButton(0)) isRotating=false;
		if (!Input.GetMouseButton(1)) isPanning=false;
		if (!Input.GetMouseButton(2)) isZooming=false;
		
		if (isRotating)
		{
	        	Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
                transform.RotateAround(transform.localPosition, transform.right, -pos.y * turnSpeed);
                transform.RotateAround(transform.localPosition, Vector3.up, pos.x * turnSpeed);
		}

		if (isPanning)
		{
	        	Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
	        	Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
	        	transform.Translate(move, Space.Self);
		}

		if (isZooming)
		{
	        	Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
 
	        	Vector3 move = pos.y * zoomSpeed * transform.forward; 
	        	transform.Translate(move, Space.World);
		}
    }
}