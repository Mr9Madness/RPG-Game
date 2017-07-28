using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Networking {

    [Serializable]
    public class TcpSocket {

        public TcpClient Socket;
        private NetworkStream Stream { get { try { return Socket.GetStream(); } catch ( Exception ) { return null; } } }
        public EndPoint LocalEndPoint { get { try { return Socket.Client.LocalEndPoint; } catch ( Exception ) { return null; } } }
        public EndPoint RemoteEndPoint { get { try { return Socket.Client.RemoteEndPoint; } catch ( Exception ) { return null; } } }
        public bool IsClientNull { get { try { return Socket == null; } catch ( Exception ) { return true; } } }
        public bool Connected { get { try { return Socket.Connected; } catch ( Exception ) { return false; } } }

        public event TcpClientEventHandler ConnectionSuccessful;
        public event TcpClientErrorEventHandler ConnectionFailed;
        public event TcpClientErrorEventHandler ConnectionLost;
        public event TcpDataEventHandler DataReceived;
        public event TcpDataEventHandler DataSent;

        public TcpSocket() { Socket = new TcpClient(); }
        public TcpSocket( TcpClient socket ) { Socket = socket; }

        public void Connect( string hostname, int port ) {
            IPAddress ip;
            if ( !IPAddress.TryParse( hostname, out ip ) )
                ip = Dns.GetHostAddresses( hostname )[ 0 ];
            if ( ip == null )
                throw new Exception( "Could not resolve hostname \"" + hostname + "\"" );

            Socket.BeginConnect( ip.ToString(), port,
                ar => {
                    try {
                        TcpClient client = ( TcpClient )ar.AsyncState;
                        client.EndConnect( ar );

                        ConnectionSuccessful?.Invoke( this );
                    } catch ( Exception ex ) {
                        ConnectionFailed?.Invoke( this, ex );
                    }
                }, Socket );
        }

        public void Close() { Socket.Close(); }

        public void Send( object data ) { Send( new Packet( data ) ); }
        public void Send( Packet data ) {
            if ( !Connected )
                return;

            ThreadHandler.Create( // Initiate the sender thread
                () => {
                    try {
                        byte[] buffer = data.SerializePacket();
                        Stream.Write( buffer, 0, buffer.Length );

                        DataSent?.Invoke( this, new Packet( buffer ) );
                    } catch ( Exception ex ) {
                        ConnectionLost?.Invoke( this, ex );
                    }
                }
            );
        }

        public void Receive( int bufferSize = 128 ) {
            if ( !Connected )
                return;

            ThreadHandler.Create( // Initiate the receiver thread
                () => {
                    bool error = false;
                    do {
                        try {
                            Packet packet = ReceiveOnce();
                            packet.ParseFailed += PacketParseFailed;
                            DataReceived?.Invoke( this, packet );
                        } catch ( Exception ex ) {
                            error = true;
                            ConnectionLost?.Invoke( this, ex );
                        }
                    } while ( Connected && !error );
                }
            );
        }

        public Packet ReceiveOnce( int bufferSize = 512 ) {
            byte[] temp = new byte[ bufferSize ];
            int length = Stream.Read( temp, 0, temp.Length );
            List<byte> buffer = new List<byte>( temp ).GetRange( 0, length );

            return new Packet( buffer.ToArray() );
        }

        private void PacketParseFailed( Packet packet ) { Console.WriteLine( "Failed to convert packet with type \"" + packet.Type + "\" to type \"string\"" ); }

    }

}
