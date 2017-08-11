using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 Accelaration;
    Vector3 CameraAccelaration;

    public float velocity = 5;
    CharacterController modelController;

    public Camera Camera;
    public GameObject Model;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Network.InitSocket( "quikers.xyz", 23000 );
        modelController = GetComponent<CharacterController>();
    }

    public void OnApplicationQuit() { Network.Socket.Close(); }

    void Update()
    {
        ModelMovement();

        CameraRotation();
    }

    void ModelMovement()
    {
        Accelaration = Vector3.zero;

        Accelaration.x = Input.GetAxis( "Horizontal" ) * velocity;
        Accelaration.z = Input.GetAxis( "Vertical" ) * velocity;

        Accelaration = transform.rotation * Accelaration;

        modelController.Move( Accelaration * Time.deltaTime );
    }

    void CameraRotation()
    {
        CameraAccelaration = Vector3.zero;

        CameraAccelaration.x = Input.GetAxis( "Camera Horizontal" );
        CameraAccelaration.y = Input.GetAxis( "Camera Vertical" );



        //Debug.Log( CameraAccelaration );
    }
}