using System;
using System.IO;
using System.Threading;
using Networking;

namespace RPG_GameServer {
    public partial class Program {

        public static void InitClient( TcpSocket socket ) {
            RPGConsole.WriteLine( "New connection requested." );
            Packet packet = socket.ReceiveOnce();
            if ( packet.Type?.Name.ToLower() != "user" ) {
                RPGConsole.WriteLine( $"Unexpected type \"{packet.Type?.Name}\", expected \"{typeof( User ).Name}\" instead.", ConsoleColor.Red );
                return;
            }
            if ( !packet.TryDeserializePacket( out User user ) ) {
                RPGConsole.WriteLine( $"Could not deserialize packet into \"{typeof( User ).Name}\". Are you using outdated software?", ConsoleColor.Red );
                return;
            }

            if ( !Data.UserList.Exists( user.Username ) ) {
                socket.ConnectionLost += ClientDisconnected;
                socket.DataReceived += DataReceived;
                socket.DataSent += DataSent;
                socket.Receive();

                user.ConnectionInfo = socket;

                Data.UserList.AddUser( user );
                RPGConsole.WriteLine( $"{user.Username} has connected.", ConsoleColor.Green );

                Broadcast( new PlayerEvent{ User = user, Status = "connected" } );
            } else {
                User oldUser = Data.UserList[ user.Username ];

                if ( oldUser.ConnectionInfo.RemoteEndPoint.ToString().Split( ':' )[ 0 ] != socket.RemoteEndPoint.ToString().Split( ':' )[ 0 ] )
                    RPGConsole.WriteLine( $"Duplicate username in login attempt: \"{user.Username}\", connection rejected.", ConsoleColor.Red );

                socket.Send( new Command{ Type = CommandType.UsernameTaken, User = user } );

                Thread.Sleep( 10 ); // Make sure that all messages have been sent before disconnecting
                socket.Close();

                Broadcast( Data.UserList );
            }
        }

        public static void ClientDisconnected( TcpSocket socket, Exception ex ) {
            socket.ConnectionLost -= ClientDisconnected;

            string error = "";
            if ( ex != null && ex.GetType() != typeof( IOException ) && ex.GetType() != typeof( NullReferenceException ) && ex.GetType() != typeof( ObjectDisposedException ) )
                error = "\n" + ex;
            if ( !Data.UserList.Exists( socket ) ) {
                RPGConsole.WriteLine( $"A user lost connection.{error}", ConsoleColor.Red );

                Data.UserList.ClearDisconnectedPlayers();
                Broadcast( Data.UserList );
                return;
            }
            User user = Data.UserList[ socket ];

            RPGConsole.WriteLine( $"{user.Username}({socket.LocalEndPoint}) Lost connection.{error}", ConsoleColor.Red );
            Broadcast( new PlayerEvent { User = user, Status = "disconnected" } );

            socket.Close();
            Data.UserList.RemoveUser( socket );

            Broadcast( Data.UserList );

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

                    RPGConsole.WriteLine( $"Received \"{packet.Type.Name}\" from {Data.UserList[ socket ].Username}({socket.LocalEndPoint})\n{message}", ConsoleColor.DarkMagenta );
                    Broadcast( packet );
                    break;
            }
        }

        public static void DataSent( TcpSocket socket, Packet packet ) {
            /* Ignored for now... */
        }
    }
}