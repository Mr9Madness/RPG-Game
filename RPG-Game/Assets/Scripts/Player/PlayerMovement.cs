using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtMethods {

    public static int ToInt( this float num ) { return Mathf.RoundToInt( num ); }

}

public class PlayerMovement : MonoBehaviour {

    public bool CursorEnabled {
        get { return Cursor.visible; }
        set {
            Debug.Log(value);
            Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    private bool isGrounded { get { return Physics.Raycast( transform.position + Vector3.up * 0.1f, Vector3.down, MaxDistance ); } }
    private bool isSprinting;

    [ Range( 1f, 100f ) ]   public float Strength           = 50f;
    [ Range( 0.1f, 1f ) ]   public float MaxDistance        = 0.3f;
    [ Range( 1.0f, 10f ) ]  public float speedMultiplier    = 4f;
    [ Range( 0.1f, 1f ) ]   public float WalkMultiplier     = 0.5f;
    [ Range( 360f, 720f ) ] public float rotationSpeed      = 450f;

    private float massCalculatedForce { get { return r.mass * Strength; } }

    private Transform cam;
    private Vector3 rawMoveDirection;
    private Rigidbody r;

    // Use this for initialization
    void Start() {
        r = GetComponent<Rigidbody>();
        cam = Camera.main.transform;

        CursorEnabled = false;
    }

    // Update is called once per frame
    void FixedUpdate() {
        // If escape is pressed
        if ( Input.GetKeyDown( KeyCode.Escape ) )
            // Toggle the cursor for debugging
            CursorEnabled = !CursorEnabled;

        // If space is pressed
        if ( Input.GetKeyDown( KeyCode.Space ) && isGrounded )
            // Jump with a velocity of 6 units
            r.AddForce( new Vector3(0, massCalculatedForce * speedMultiplier * Time.fixedDeltaTime, 0), ForceMode.Force );

        // Get the horizontal and vertical input from the user
        float hInput = Input.GetAxisRaw( "Horizontal" ) * speedMultiplier;
        float vInput = Input.GetAxisRaw( "Vertical" ) * speedMultiplier;

        // ======== Player Movement Mechanics ========

        // -------- Move --------

        // Create 2 directional vector3's
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;

        // Prepare the vector3's made above
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // If the player is grounded
        if ( isGrounded ) {
            isSprinting = Input.GetKey( KeyCode.LeftShift );
            // Update the rawMoveDirection with user input and the directions together
            rawMoveDirection = forward * vInput + right * hInput;
        }

        // Update the position to the new position
        r.AddForce( rawMoveDirection.normalized * massCalculatedForce * speedMultiplier * 2f * ( !isSprinting ? WalkMultiplier : 1 ) * Time.fixedDeltaTime, ForceMode.Impulse );
        //transform.position += rawMoveDirection.normalized * speedMultiplier * ( !isSprinting ? WalkMultiplier : 1 ) * Time.fixedDeltaTime;

        // -------- Rotate --------

        // Make sure that nothing happens after this unless the user moved.
        if ( vInput.ToInt() == 0 && hInput.ToInt() == 0 )
            return;

        // Convert the calculated move direction to a Quaternion
        Quaternion rotateTo = Quaternion.LookRotation( rawMoveDirection );
        // Gradually rotate towards the new rotation so the movement is smooth
        r.MoveRotation( Quaternion.RotateTowards( transform.rotation, rotateTo, rotationSpeed * Time.fixedDeltaTime ) );
    }
}
