using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public Camera mainCamera; // Reference to the main camera
    private Vector3 mousePosition;

    void Start()
    {
        // Hide the system mouse cursor
        Cursor.visible = false;

        // If the camera is not assigned in the Inspector, automatically get the main camera
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // Update the crosshair's position to follow the mouse
        FollowMouse();
    }

    void FollowMouse()
    {
        // Get the mouse position in screen space
        mousePosition = Input.mousePosition;

        // Convert the mouse position from screen space to world space
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // Set the crosshair's position to the mouse position (set z to 0 since it's 2D)
        transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);
    }
}
