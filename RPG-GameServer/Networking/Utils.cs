

namespace Networking {
    public class Data {
        public static Players Players { get; set; } = new Players();
    }

    public class EntityTransform {
        public float[] Position = new float[ 3 ];
        public float[] Rotation = new float[ 3 ];
        public float[] Scale = new float[ 3 ];
    }
}
