using System;
using Networking;

public static class Network {

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

    public static void ReceiveOnce() {
        ThreadHandler.Create( () => {
            Packet packet = Socket.ReceiveOnce();
            if ( packet != null )
                Socket.InvokeDataReceived( Socket, packet );
        } );
    }

    // DEFAULT EVENTS ~~ feel free to edit and remove these ya cunt.

    private static void DefaultSuccessful( TcpSocket socket ) {
        Send( new Player( "Test Player" ) );
        UnityEngine.Debug.Log( $"Connected to the server at {socket.RemoteEndPoint}." );
    }
    private static void DefaultFailed( TcpSocket socket, Exception ex ) { UnityEngine.Debug.Log( "Failed to connect to the server." ); }
    private static void DefaultLost( TcpSocket socket, Exception ex ) { UnityEngine.Debug.Log( "Lost connection to the server." ); }
    private static void DefaultReceived( TcpSocket socket, Packet packet ) { UnityEngine.Debug.Log( $"Packet received with type \"{packet.Type.Name}\"." ); }
    private static void DefaultSent( TcpSocket socket, Packet packet ) { UnityEngine.Debug.Log( $"Packet sent with type \"{packet.Type.Name}\"." ); }
}