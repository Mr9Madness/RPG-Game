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
            if ( !packet.TryDeserializePacket( out Player player ) ) {
                RPGConsole.WriteLine( $"Could not deserialize packet into \"{typeof( Player ).Name}\". Are you using outdated software?", ConsoleColor.Red );
                return;
            }

            if ( !ServerData.Players.Exists( player.Username ) ) {
                socket.ConnectionLost += ClientDisconnected;
                socket.DataReceived += DataReceived;
                socket.DataSent += DataSent;
                socket.Receive();

                player.Socket = socket;

                ServerData.Players.Add( player );
                RPGConsole.WriteLine( $"{player.Username} has connected.", ConsoleColor.Green );

                Broadcast( new PlayerEvent( player, true ) );
            } else {
                Player oldUser = ServerData.Players[ player.Username ];

                if ( oldUser.Socket.RemoteEndPoint.ToString().Split( ':' )[ 0 ] != socket.RemoteEndPoint.ToString().Split( ':' )[ 0 ] )
                    RPGConsole.WriteLine( $"Duplicate username in login attempt: \"{player.Username}\", connection rejected.", ConsoleColor.Red );

                socket.Send( new Command(CommandType.UsernameTaken) );

                Thread.Sleep( 10 ); // Make sure that all messages have been sent before disconnecting
                socket.Close();

                Broadcast( ServerData.Players );
            }
        }

        public static void ClientDisconnected( TcpSocket socket, Exception ex ) {
            socket.ConnectionLost -= ClientDisconnected;

            string error = "";
            if ( ex != null && ex.GetType() != typeof( IOException ) && ex.GetType() != typeof( NullReferenceException ) && ex.GetType() != typeof( ObjectDisposedException ) )
                error = "\n" + ex;
            if ( !ServerData.Players.Exists( socket ) ) {
                RPGConsole.WriteLine( $"A player lost connection.{error}", ConsoleColor.Red );

                ServerData.Players.ClearDisconnectedPlayers();
                Broadcast( ServerData.Players );
                return;
            }
            Player player = ServerData.Players[ socket ];

            RPGConsole.WriteLine( $"{player.Username}({socket.LocalEndPoint}) Lost connection.{error}", ConsoleColor.Red );
            Broadcast( new PlayerEvent( player, false ) );

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