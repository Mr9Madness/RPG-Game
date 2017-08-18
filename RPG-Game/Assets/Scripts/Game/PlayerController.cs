using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    Vector3 Acceleration;
    Vector3 CameraAcceleration;

    public float Sensitivity = 5f;
    public float Velocity = 5f;
    CharacterController modelController;

    public Camera Camera;
    public GameObject Model;

    public void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        modelController = GetComponent<CharacterController>();
    }

    public void OnApplicationQuit() { Data.Network.Socket.Close(); }

    void Update() {
        ModelMovement();

        CameraRotation();
    }

    void ModelMovement() {
        Acceleration.x = Input.GetAxis( "Horizontal" ) * Velocity;
        Acceleration.z = Input.GetAxis( "Vertical" ) * Velocity;

        Acceleration = transform.rotation * Acceleration;

        modelController.Move( Acceleration * Time.deltaTime );

        Debug.Log( "Accel: " + Acceleration );

    }

    void CameraRotation() {
        CameraAcceleration.x = Input.GetAxis( "Camera Horizontal" );
        CameraAcceleration.y = Input.GetAxis( "Camera Vertical" );

        Camera.transform.Translate( CameraAcceleration * Sensitivity * Time.deltaTime );

        Debug.Log( "Camera Accel: " + CameraAcceleration );
    }
}