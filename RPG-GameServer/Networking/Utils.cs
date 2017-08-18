using System;
using UnityEngine;

namespace Networking {

    public static class UnityExtensionMethods {
        public static float[] ToArray( this Vector3 v ) => new[] { v.x, v.y, v.z };
        public static Vector3 ToVector3( this float[] f ) => new Vector3( f[ 0 ], f[ 1 ], f[ 2 ] );
    }

    public static class ServerData {
        public static Players Players = new Players();
    }

    [Serializable]
    public class PlayerEvent {
        public Players PlayerList = ServerData.Players;
        public Player PlayerInQuestion;

        public bool IsPlayerAdded;

        public PlayerEvent( Player playerInQuestion, bool isPlayerAdded ) {
            PlayerInQuestion = playerInQuestion;
            IsPlayerAdded = isPlayerAdded;
        }
    }

    [Serializable]
    public class EntityTransform {
        public bool Active = false;
        private float[] _position = new float[ 3 ];
        private float[] _rotation = new float[ 3 ];
        private float[] _scale = new float[ 3 ];

        public Vector3 Position { get => _position.ToVector3(); set => _position = value.ToArray(); }
        public Vector3 Rotation { get => _rotation.ToVector3(); set => _rotation = value.ToArray(); }
        public Vector3 Scale { get => _scale.ToVector3(); set => _scale = value.ToArray(); }
    }

}

