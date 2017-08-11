using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game {
    public class PlayerController : MonoBehaviour {

        Vector3 Accelaration;

        public void Start() { Network.InitSocket( "quikers.xyz", 23000 ); }

        public void OnApplicationQuit() { Data.Network.Socket.Close(); }

        public void Update() { }
    }
}