using System;
using System.IO;
using System.Threading;
using Networking;

namespace RPG_GameServer {
    public partial class Program {

        public static void InitClient( TcpSocket socket ) {
            RPGConsole.WriteLine( "New connection requested." );
            Packet packet = socket.ReceiveOnce();
            if ( packet.Type?.Name.ToLower() != "player" ) {
                RPGConsole.WriteLine( $"Unexpected type \"{packet.Type?.Name}\", expected \"{typeof( Player ).Name}\" instead.", ConsoleColor.Red );
                return;
            }
            if ( !packet.TryDeserializePacket( out Player user ) ) {
                RPGConsole.WriteLine( $"Could not deserialize packet into \"{typeof( Player ).Name}\". Are you using outdated software?", ConsoleColor.Red );
                return;
            }

            if ( !ServerData.Players.Exists( user.Username ) ) {
                socket.ConnectionLost += ClientDisconnected;
                socket.DataReceived += DataReceived;
                socket.DataSent += DataSent;
                socket.Receive();

                user.Socket = socket;

                ServerData.Players.Add( user );
                RPGConsole.WriteLine( $"{user.Username} has connected.", ConsoleColor.Green );
            } else {
                Player oldUser = ServerData.Players[ user.Username ];

                if ( oldUser.Socket.RemoteEndPoint.ToString().Split( ':' )[ 0 ] != socket.RemoteEndPoint.ToString().Split( ':' )[ 0 ] )
                    RPGConsole.WriteLine( $"Duplicate username in login attempt: \"{user.Username}\", connection rejected.", ConsoleColor.Red );

                socket.Send( new Command(CommandType.UsernameTaken) );

                Thread.Sleep( 10 ); // Make sure that all messages have been sent before disconnecting
                socket.Close();
            }

            Broadcast( ServerData.Players );
        }

        public static void ClientDisconnected( TcpSocket socket, Exception ex ) {
            socket.ConnectionLost -= ClientDisconnected;

            string error = "";
            if ( ex != null && ex.GetType() != typeof( IOException ) && ex.GetType() != typeof( NullReferenceException ) && ex.GetType() != typeof( ObjectDisposedException ) )
                error = "\n" + ex;
            if ( !ServerData.Players.Exists( socket ) ) {
                RPGConsole.WriteLine( $"A user lost connection.{error}", ConsoleColor.Red );

                ServerData.Players.ClearDisconnectedPlayers();
                Broadcast( ServerData.Players );
                return;
            }
            Player user = ServerData.Players[ socket ];

            RPGConsole.WriteLine( $"{user.Username}({socket.LocalEndPoint}) Lost connection.{error}", ConsoleColor.Red );

            socket.Close();
            ServerData.Players.Remove( socket );

            Broadcast( ServerData.Players );

            socket.ConnectionLost += ClientDisconnected;
        }

        public static void DataReceived( TcpSocket socket, Packet packet ) {
            switch ( packet.Type.Name.ToLower() ) {
                default:
                    //RPGConsole.WriteLine( $"Received a packet of unknown type \"{packet.Type.Name}\".\r\n", ConsoleColor.Red );
                    Broadcast( packet );
                    break;
                case "string":
                    if ( !packet.TryDeserializePacket( out string message ) ) {
                        RPGConsole.WriteLine( $"Could not convert packet into a \"{packet.Type.Name}\"", ConsoleColor.Red );
                        break;
                    }

                    RPGConsole.WriteLine( $"Received \"{packet.Type.Name}\" from {ServerData.Players[ socket ].Username}({socket.LocalEndPoint})\n{message}", ConsoleColor.DarkMagenta );
                    Broadcast( packet );
                    break;
            }
        }

        public static void DataSent( TcpSocket socket, Packet packet ) {
            /* Ignored for now... */
        }
    }
}