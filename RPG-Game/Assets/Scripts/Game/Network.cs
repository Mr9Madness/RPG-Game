using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour {

    public GameObject PlayerPrefab;

    // Use this for initialization
    void Start() {
        Data.Network.InitSocket( "127.0.0.1", 23000 );//"quikers.xyz", 23000 );
        Data.Network.PlayerPrefab = PlayerPrefab;
    }

    // Update is called once per frame
    void Update() {

    }
}
