using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 Acceleration;
    Vector3 CameraAcceleration;

    public float Sensitivity = 5f;
    public float Velocity = 5f;
    CharacterController modelController;

    public Camera Camera;
    public GameObject Model;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Data.Network.InitSocket( "quikers.xyz", 23000 );
        modelController = GetComponent<CharacterController>();
    }

    public void OnApplicationQuit() { Data.Network.Socket.Close(); }

    void Update()
    {
        ModelMovement();

        CameraRotation();
    }

    void ModelMovement()
    {
        Acceleration = Vector3.zero;

        Acceleration.x = Input.GetAxis( "Horizontal" ) * Velocity;
        Acceleration.z = Input.GetAxis( "Vertical" ) * Velocity;

        Acceleration = transform.rotation * Acceleration;

        modelController.Move( Acceleration * Time.deltaTime );
    }

    void CameraRotation()
    {
        CameraAcceleration = Vector3.zero;

        CameraAcceleration.x = Input.GetAxis( "Camera Horizontal" );
        CameraAcceleration.y = Input.GetAxis( "Camera Vertical" );

        Camera.transform.Translate( CameraAcceleration * Sensitivity * Time.deltaTime );

        //TProtationX += ( Input.GetAxis( "Horizontal" ) * cameraSensitivity < -0.15f ? Input.GetAxis( "Horizontal" ) * cameraSensitivity : Input.GetAxis( "Horizontal" ) * cameraSensitivity > 0.15f ? Input.GetAxis( "Horizontal" ) * cameraSensitivity : 0f );
        //transform.localEulerAngles = new Vector3( 0f, TProtationX );
        //Camera.main.transform.Translate( CameraPosTpY * cameraSensitivity * Time.deltaTime );


        //Debug.Log( CameraAcceleration );
    }
}