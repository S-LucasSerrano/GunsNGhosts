using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLerper : MonoBehaviour
{
    [Space] [SerializeField] Camera lerpingCamera = null;

    /// <summary>  Target that this camera is focusing on. </summary>
    [Space][SerializeField] Transform target = null;

    [SerializeField][Range(0,1)] float lerp = 0.1f;

    [SerializeField] [Range(0, 1)] float aimOffset = 0.5f;

    /// If the camera should not move in one of the axles.
    [Space] [SerializeField] bool xConstrain = false;
    [SerializeField] bool yConstrain = false;

    /// Point that mark the limits of what the camera can see.
    [SerializeField] Transform topLimit, bottonLimit, leftLimit, rightLimit;
    /// Size of the viewport in world space.
    float cameraWidth, cameraHeigh = 0;


    // -----------------------------------------------------------

	private void Reset()
	{
        lerpingCamera = Camera.main;
	}

	private void Start()
	{
        // Calculate the viewport size in world space.
        Vector3 camLeftBottom = lerpingCamera.ViewportToWorldPoint(Vector3.zero);
        Vector3 camRighTop = lerpingCamera.ViewportToWorldPoint(Vector3.one);
        cameraWidth = (camRighTop.x - camLeftBottom.x) / 2;
        cameraHeigh = (camRighTop.y - camLeftBottom.y) / 2;
	}


    // -----------------------------------------------------------

    void Update()
    {
        Lerp();
		LimitPosition();
	}

    void Lerp()
	{
        Vector3 targetPosition = Input.mousePosition;
        targetPosition = Camera.main.ScreenToWorldPoint(targetPosition);
        targetPosition = Vector3.Lerp(target.position, targetPosition, aimOffset);

        // Constrains.
        if (xConstrain)
            targetPosition.x = transform.position.x;
        if (yConstrain)
            targetPosition.y = transform.position.y;

        transform.position = Vector3.Lerp(transform.position, targetPosition, lerp);
    }

    void LimitPosition()
	{
        // Top.
        if (transform.position.y + cameraHeigh > topLimit.position.y)
		{
            transform.position = new Vector3(
                transform.position.x,
                topLimit.position.y - cameraHeigh,
                transform.position.z);
        }
        // Bottom.
        if (transform.position.y - cameraHeigh < bottonLimit.position.y)
		{
            transform.position = new Vector3(
                transform.position.x,
                bottonLimit.position.y + cameraHeigh,
                transform.position.z);
        }
        // Left.
        if (transform.position.x - cameraWidth < leftLimit.position.x)
		{
            transform.position = new Vector3(
                leftLimit.position.x + cameraWidth,
                transform.position.y,
                transform.position.z);
        }
        // Right.
        if (transform.position.x + cameraWidth > rightLimit.position.x)
        {
            transform.position = new Vector3(
                rightLimit.position.x - cameraWidth,
                transform.position.y,
                transform.position.z );
        }
	}
}
