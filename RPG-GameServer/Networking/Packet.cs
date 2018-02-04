using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Networking {

    [Serializable]
    public class Packet {

        public object Content;
        public ExtEndPoint RemoteEP = new ExtEndPoint( IPAddress.None, 0, 0 );

        public Type Type => Content?.GetType() ?? typeof( Packet );
        public string TypeName => this.Type?.Name ?? "PacketDummy";
        public string TypeNameLower => this.TypeName.ToLower();

        public Packet Clone() => this.Clone( RemoteEP );
        public Packet Clone( ExtEndPoint ep ) => new Packet( Content, ep );
        public byte[] AsByteArray => Packet.ToByteArray( this );
        public static Packet Dummy => new Packet();

        public bool IsDummy => Equals( Dummy ) && Content == null;

        public Packet() { }
        public Packet( object value ) => Content = value;
        public Packet( ExtEndPoint remoteEP ) => RemoteEP = remoteEP;
        public Packet( object value, ExtEndPoint remoteEP ) {
            Content = value;
            RemoteEP = remoteEP;
        }

        public Packet SetOrigin( IPAddress address ) => SetOrigin( new ExtEndPoint( address, RemoteEP.RPort, RemoteEP.SPort ) );
        public Packet SetOrigin( int receiverPort, int senderPort ) => SetOrigin( new ExtEndPoint( RemoteEP.Address, receiverPort, senderPort ) );
        public Packet SetOrigin( ExtEndPoint remoteEP ) {
            RemoteEP = remoteEP;
            return this;
        }

        public static Packet FromByteArray( IEnumerable<byte> buffer ) {
            BinaryFormatter bf = new BinaryFormatter();
            using ( MemoryStream ms = new MemoryStream( buffer.ToArray() ) ) {
                return (Packet)bf.Deserialize( ms );
            }
        }

        public static byte[] ToByteArray( Packet packet ) {
            BinaryFormatter bf = new BinaryFormatter();
            using ( MemoryStream ms = new MemoryStream() ) {
                bf.Serialize( ms, packet );
                return ms.ToArray();
            }
        }

    }

}
