using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public void Start() { Network.InitSocket( "quikers.xyz", 23000 ); }

    public void OnApplicationQuit() { Network.Socket.Close(); }

    void Update()
    {
        if ( Input.GetKey( KeyCode.A ) )
            Accelaration.x = 1;

        if (Input.GetKey( KeyCode.D ))
            Accelaration.x = -1;

        if (Input.GetKey( KeyCode.W ))
            Accelaration.z = 1;

        if (Input.GetKey( KeyCode.S ))
            Accelaration.x = -1;

    }

}