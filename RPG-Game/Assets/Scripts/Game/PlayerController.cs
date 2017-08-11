using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 Acceleration;
    Vector3 CameraAcceleration;

    public float velocity = 5;
    CharacterController modelController;

    public Camera Camera;
    public GameObject Model;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Data.Network.InitSocket( "127.0.0.1", 23000 );//"quikers.xyz", 23000 );
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

        Acceleration.x = Input.GetAxis( "Horizontal" ) * velocity;
        Acceleration.z = Input.GetAxis( "Vertical" ) * velocity;

        Acceleration = transform.rotation * Acceleration;

        modelController.Move( Acceleration * Time.deltaTime );
    }

    void CameraRotation()
    {
        CameraAcceleration = Vector3.zero;

        CameraAcceleration.x = Input.GetAxis( "Camera Horizontal" );
        CameraAcceleration.y = Input.GetAxis( "Camera Vertical" );



        //Debug.Log( CameraAcceleration );
    }
}