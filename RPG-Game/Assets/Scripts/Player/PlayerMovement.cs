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
    [ Range( 1, 3 ) ]   public float MaxDistance        = 1.1f;
    [ Range( 0.1f, 20 ) ]   public float speedMultiplier    = 2;
    [ Range( 0.1f, 10 ) ]   public float WalkMultiplier     = 1;
    [ Range( 360f, 720f ) ] public float rotationSpeed      = 450f;

    private float massCalculatedForce { get { return r.mass * Strength; } }

    private Transform cam;
    private Rigidbody r;

    // Use this for initialization
    void Start() {
        r = GetComponent<Rigidbody>();
        cam = Camera.main.transform;

        CursorEnabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // If escape is pressed
        if ( Input.GetKeyDown( KeyCode.Escape ) )
            // Toggle the cursor for debugging
            CursorEnabled = !CursorEnabled;

        // If space is pressed
        if ( Input.GetKeyDown( KeyCode.Space ) && isGrounded )
        {
            r.AddForce( new Vector3( 0, massCalculatedForce  * ( !isSprinting ? WalkMultiplier : speedMultiplier )* Time.deltaTime, 0 ), ForceMode.Force );
        }
        // Jump with a velocity of 6 units

        // Get the horizontal and vertical input from the user
        float hInput = Input.GetAxisRaw( "Horizontal" ) * ( !isSprinting ? WalkMultiplier : speedMultiplier ) * Time.deltaTime;
        float vInput = Input.GetAxisRaw( "Vertical" ) * ( !isSprinting ? WalkMultiplier : speedMultiplier ) * Time.deltaTime;

        // ======== Player Movement Mechanics ========

        // -------- Move --------

        // If the player is grounded
        if ( isGrounded )
        {
            isSprinting = Input.GetKey( KeyCode.LeftShift );
            // Update the rawMoveDirection with user input and the directions together
        }

        // Update the position to the new position
        r.AddForce( new Vector3( 0, 0, vInput ) * massCalculatedForce * ( !isSprinting ? WalkMultiplier : speedMultiplier ) * Time.fixedDeltaTime, ForceMode.Impulse );

        transform.rotation = Quaternion.Euler( transform.rotation.x, transform.rotation.y + hInput, transform.rotation.z );
        // -------- Rotate --------

        // Make sure that nothing happens after this unless the user moved.
        if ( vInput.ToInt() == 0 && hInput.ToInt() == 0 )
            return;

        // Convert the calculated move direction to a Quaternion
        Quaternion rotateTo = Quaternion.LookRotation( new Vector3( 0, 0, vInput ) );
        // Gradually rotate towards the new rotation so the movement is smooth
        r.MoveRotation( Quaternion.RotateTowards( transform.rotation, rotateTo, rotationSpeed * Time.fixedDeltaTime ) );
    }
}