using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public void Start() { Network.InitSocket( "quikers.xyz", 23000 ); }

    public void Update()
    {

    }
}