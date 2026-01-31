using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        // Find the main camera in the scene and store a reference to it.
        // Ensure your main camera is tagged as "MainCamera" in the Inspector.
        mainCamera = Camera.main; 
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Please tag a camera as 'MainCamera'.");
        }
    }

    private void LateUpdate()
    {
        // LateUpdate is used to ensure the camera has already updated its position
        // and rotation for the current frame, which prevents jittering.

        if (mainCamera != null)
        {
            // Make the object look at the camera's position.
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }
    }
}