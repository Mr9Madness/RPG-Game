using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class PlayerController : MonoBehaviour {
        public void Start() { Data.Network.InitSocket( "quikers.xyz", 23000 ); }

        public void OnApplicationQuit() { Data.Network.Socket.Close(); }

        public void Update() { }

    }
}