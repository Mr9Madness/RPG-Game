using System;
using System.Threading;
using Networking;

using Debug = UnityEngine.Debug;

namespace Data {
    public static class Network {

        public static Players players;

        public static TcpSocket Socket;

        public static void InitSocket( string hostname, int port ) { InitSocket( hostname, port, DefaultSuccessful, DefaultFailed, DefaultLost, DefaultReceived, DefaultSent ); }

        public static void InitSocket( string hostname, int port, TcpSocketEventHandler successfulCallback, TcpSocketErrorEventHandler failedCallback, TcpSocketErrorEventHandler lostCallback, TcpDataEventHandler receivedCallback, TcpDataEventHandler sentCallback ) {
            Socket = new TcpSocket();

            Socket.ConnectionSuccessful += successfulCallback;
            Socket.ConnectionFailed += failedCallback;
            Socket.ConnectionLost += lostCallback;
            Socket.DataReceived += receivedCallback;
            Socket.DataSent += sentCallback;

            Socket.Connect( hostname, port );
        }

        public static void Send( object obj ) { Send( new Packet( obj ) ); }

        public static void Send( Packet packet ) {
            ThreadHandler.Create( () => {
                Socket.Send( packet );
            } );
        }

        // DEFAULT EVENTS ~~ feel free to edit and remove these ya cunt.

        private static void DefaultSuccessful( TcpSocket socket ) {
            Send( new Player( "Test Player" ) );
            Debug.Log( $"Connected to the server at {socket.RemoteEndPoint}." );
        }

        private static void DefaultFailed( TcpSocket socket, Exception ex ) { Debug.Log( "Failed to connect to the server." ); }
        private static void DefaultLost( TcpSocket socket, Exception ex ) { Debug.Log( "Lost connection to the server." ); }

        private static void DefaultReceived( TcpSocket socket, Packet packet ) {
            Debug.Log( $"Packet received with type \"{packet.Type.Name}\"." );

            switch ( packet.Type.Name.ToLower() ) {
                default:
                    Debug.Log( $"Received packet with unknown type \"{packet.Type.Name}\"" );
                    break;
                case "players":
                    Players playerList;
                    if ( packet.TryDeserializePacket( out playerList ) )
                        players = playerList;
                    break;
                case "command":
                    Command command;
                    if ( packet.TryDeserializePacket( out command ) )
                        HandleCommand( command );
                    break;
                case "entitytransform":
                    break;
            }
        }

        private static void DefaultSent( TcpSocket socket, Packet packet ) { Debug.Log( $"Packet sent with type \"{packet.Type.Name}\"." ); }

        private static void HandleCommand( Command command ) {
            switch ( command.Type ) {
                case CommandType.Kick:
                    break;
                case CommandType.UsernameTaken:
                    Debug.Log( $"Username \"{User.Username}\" is already taken." );
                    break;
                default:
                    Debug.Log( $"Received unknown command \"{command}\"" );
                    break;
                case CommandType.Disconnect:
                    break;
            }
        }
    }
}