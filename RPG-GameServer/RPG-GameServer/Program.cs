using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking;

namespace RPG_GameServer {
    class Program {

        public static TcpServer Server = new TcpServer( 23000 );

        static void Main( string[] args ) {
            Server.ClientConnectionRequested += InitClient;
            Server.Start();

            List< string > test = new List< string >( new[] { "0:test", "1:test" } );

            string[] strArr = new string[ test.Count ];
            for ( int i = 0; i < test.Count; i++ )
                strArr[i] = test[i];
        }

        public static void InitClient( TcpSocket socket ) {
            Packet packet = socket.ReceiveOnce();
            if ( packet.Type.Name.ToLower() != "player" ) {
                Console.WriteLine( "Unexpected type \"{0}\". Expected type \"{1}\".", packet.Type.Name,
                                   typeof( Player ).Name );
                return;
            }
            if ( !packet.TryDeserializePacket( out Player player ) ) {
                Console.WriteLine( "Could not deserialize packet into \"{0}\". Are you using outdated software?",
                                   typeof( Player ).Name );
                return;
            }

            socket.ConnectionLost += ClientDisconnected;
            socket.DataReceived += DataReceived;
            socket.DataSent += DataSent;
            socket.Receive();

            player.Socket = socket;
        }

        public static void Broadcast( object data ) { Broadcast( new Packet( data ) ); }

        public static void Broadcast( Packet packet ) {
            foreach ( Player player in Data.Players )
                player.Socket.Send( packet );
        }

        public static void ClientDisconnected( TcpSocket socket, Exception ex ) { }

        public static void DataReceived( TcpSocket socket, Packet packet ) { }

        public static void DataSent( TcpSocket socket, Packet packet ) {
            /* Ignored for now... */
        }

    }
}