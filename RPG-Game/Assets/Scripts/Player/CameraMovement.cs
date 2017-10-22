using System;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float mouseXSpeedMod = 5f;
    public float mouseYSpeedMod = 3f;

    public float maxRange = 5f;
    public float minRange = 2f;
    public int zoomRate = 30;
    public float lerpRate = 1f;

    private float desiredDistance;

    private float x, y;

    private int cameraRepositionCount = 0;

    private Camera cam;

    // Use this for initialization
    void Start()
    {
        cam = Camera.main;

        x = cam.transform.eulerAngles.x;
        y = cam.transform.eulerAngles.y;

        cam.transform.LookAt( transform );
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Get the mouse and joystick input and store them (yes this is the wrong way to do it but it was fast so stfu, bitch)
        float inputX = Input.GetAxis( "Camera Horizontal" );
        float inputY = Input.GetAxis( "Camera Vertical" );

        // Add the input to the current position
        x += inputX * mouseXSpeedMod;
        y -= inputY * mouseYSpeedMod;

        // If the character is moving horizontally but the camera is not being moved at all
        if (inputX.ToInt() == 0 && inputY.ToInt() == 0 && Input.GetAxis( "Horizontal" ).ToInt() != 0)
        {
            // If the camera reposition timer is done
            if (cameraRepositionCount <= 0)
            {
                float rotationAngle = transform.eulerAngles.y;
                float camRotationAngle = cam.transform.eulerAngles.y;

                // Lerp the camera's rotation to the new rotation with input
                x = Mathf.LerpAngle( camRotationAngle, rotationAngle, lerpRate * Time.deltaTime );
            }
            else
                // Count down the camera reposition timer
                cameraRepositionCount--;

        }
        else if (inputX.ToInt() != 0 || inputY.ToInt() != 0) // If the camera was being moved
            // Reset the camera reposition timer
            cameraRepositionCount = Mathf.RoundToInt( 2000 * Time.deltaTime );

        // Make sure that the camera can't rotate further
        y = ClampAngle( y, -60, 80 );

        // Save the current new rotation in a Quaternion
        Quaternion rot = Quaternion.Euler( y, x, 0 );

        // If the scrollwheel was used, zoom in or out from the character
        desiredDistance -= Input.GetAxis( "Mouse ScrollWheel" ) * Time.deltaTime * zoomRate * Mathf.Abs( desiredDistance );
        // Make sure that the distance does not exceed the min or max range
        desiredDistance = Mathf.Clamp( desiredDistance, minRange, maxRange );

        // Save the current new position in a Vector3
        Vector3 pos = transform.position - rot * Vector3.forward * desiredDistance;

        // Update the old rotation and position
        cam.transform.rotation = rot;
        cam.transform.position = new Vector3( pos.x, pos.y + 1, pos.z );
    }

    /// <summary>
    /// Clamps an angle between the given min and max values and also makes sure that the angle does not exceed -360 or +360
    /// </summary>
    /// <param name="angle">The angle to clamp</param>
    /// <param name="min">The minimum value</param>
    /// <param name="max">The maximum value</param>
    /// <returns></returns>
    private static float ClampAngle( float angle, float min, float max )
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp( angle, min, max );
    }
}