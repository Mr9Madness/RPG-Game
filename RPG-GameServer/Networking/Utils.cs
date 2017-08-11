using UnityEngine;

namespace Networking {

    public static class ServerData {
        public static Players Players = new Players();
    }

    public class EntityTransform {
        public bool Active = false;
        public Vector3 Position = new Vector3();
        public Vector3 Rotation = new Vector3();
        public Vector3 Scale = new Vector3();
    }

}

