using System;
using System.IO;
using System.Threading;
using Networking;

namespace RPG_GameServer {
    public partial class Program {

        public static void InitClient( TcpSocket socket ) {
            RPGConsole.WriteLine( "New connection requested." );
            Packet packet = socket.ReceiveOnce();
            if ( packet.Type.Name.ToLower() != "user" ) {
                RPGConsole.WriteLine( $"Unexpected type \"{packet.Type.Name}\", expected \"{typeof( Player ).Name}\" instead.", ConsoleColor.Red );
                return;
            }
            if ( !packet.TryDeserializePacket( out Player user ) ) {
                RPGConsole.WriteLine( $"Could not deserialize packet into \"{typeof( Player ).Name}\". Are you using outdated software?", ConsoleColor.Red );
                return;
            }

            if ( !Data.Players.Exists( user.Username ) ) {
                socket.ConnectionLost += ClientDisconnected;
                socket.DataReceived += DataReceived;
                socket.DataSent += DataSent;
                socket.Receive();

                user.Socket = socket;

                Data.Players.Add( user );
                RPGConsole.WriteLine( $"{user.Username} has connected.", ConsoleColor.Green );
            } else {
                Player oldUser = Data.Players[ user.Username ];

                if ( oldUser.Socket.RemoteEndPoint.ToString().Split( ':' )[ 0 ] != socket.RemoteEndPoint.ToString().Split( ':' )[ 0 ] )
                    RPGConsole.WriteLine( $"Duplicate username in login attempt: \"{user.Username}\", connection rejected.", ConsoleColor.Red );

                socket.Send( Command.UsernameTaken );

                Thread.Sleep( 10 ); // Make sure that all messages have been sent before disconnecting
                socket.Close();
            }

            Broadcast( Data.Players );
        }

        public static void ClientDisconnected( TcpSocket socket, Exception ex ) {
            socket.ConnectionLost -= ClientDisconnected;

            string error = "";
            if ( ex != null && ex.GetType() != typeof( IOException ) && ex.GetType() != typeof( NullReferenceException ) && ex.GetType() != typeof( ObjectDisposedException ) )
                error = "\n" + ex;
            if ( !Data.Players.Exists( socket ) ) {
                RPGConsole.WriteLine( $"A user lost connection.{error}", ConsoleColor.Red );

                Data.Players.ClearDisconnectedUsers();
                Broadcast( Data.Players );
                return;
            }
            Player user = Data.Players[ socket ];

            RPGConsole.WriteLine( $"{user.Username}({socket.LocalEndPoint}) Lost connection.{error}", ConsoleColor.Red );

            socket.Close();
            Data.Players.Remove( socket );

            Broadcast( Data.Players );

            socket.ConnectionLost += ClientDisconnected;
        }

        public static void DataReceived( TcpSocket socket, Packet packet ) {
            switch ( packet.Type.Name.ToLower() ) {
                default:
                    RPGConsole.WriteLine( $"Received a packet of unknown type \"{packet.Type.Name}\".", ConsoleColor.Red );
                    break;
                case "string":
                    if ( packet.TryDeserializePacket( out string message ) )
                        RPGConsole.WriteLine(
                            $"Received \"{packet.Type.Name}\" from {Data.Players[ socket ].Username}({socket.LocalEndPoint})\n{message}",
                            ConsoleColor.DarkMagenta );
                    break;
            }
        }

        public static void DataSent( TcpSocket socket, Packet packet ) {
            /* Ignored for now... */
        }
    }
}