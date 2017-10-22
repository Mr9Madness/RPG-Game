using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Networking {

    public class TcpServer : TcpListener {

        #region Events

        public event SocketEventHandler ClientConnectionRequested;

        #endregion

        #region Local Variables

        public EndPoint LocalEndPoint => LocalEndpoint;

        #endregion

        #region Constructors

        public TcpServer( int port, SocketEventHandler callback ) : base( IPAddress.Any, port ) {
            ClientConnectionRequested += callback;
            BeginAccepting();
        }
        public TcpServer( IPAddress ip, int port, SocketEventHandler callback ) : base( ip, port ) {
            ClientConnectionRequested += callback;
            BeginAccepting();
        }

        #endregion

        #region Methods

        private void BeginAccepting() {
            Start();
            BeginAcceptTcpClient( ar => {
                try {
                    TcpSocket socket = new TcpSocket( ( ( TcpListener )ar.AsyncState ).EndAcceptTcpClient( ar ) );
                    ClientConnectionRequested?.Invoke( socket );
                } catch ( ObjectDisposedException ) { /* Is caused when exitting the server while still listening for new clients */ }

                BeginAccepting();
            }, this );
        }

        #endregion

    }

    public class UdpServer {

        #region Events

        public event SocketEventHandler ClientConnectionRequested;

        #endregion

        #region Local Variables

        private Socket _socket;
        public int Port { get { try { return int.Parse( _socket.LocalEndPoint.ToString().Split( ':' )[ 1 ] ); } catch ( Exception ) { return 0; } } }
        public IPAddress IP { get { try { return IPAddress.Parse( _socket.LocalEndPoint.ToString().Split( ':' )[ 0 ] ); } catch ( Exception ) { return null; } } }
        public EndPoint LocalEndPoint => _socket.LocalEndPoint;
        public EndPoint RemoteEndPoint => _socket.RemoteEndPoint;

        #endregion

        #region Constructors

        public UdpServer( int port, SocketEventHandler callback ) {
            _socket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IPv4 );
            _socket.Bind( new IPEndPoint( IPAddress.Parse( "0.0.0.0" ), port ) );

            ClientConnectionRequested += callback;
            BeginReceiving();
        }

        /// <summary>
        /// Initializes the UDP server and automatically starts waiting for connections.
        /// </summary>
        /// <param name="ip">The local IP to listen to</param>
        /// <param name="port">The local port to listen to</param>
        /// <param name="callback">The method to call if a client has been found</param>
        public UdpServer( string ip, int port, SocketEventHandler callback ) {
            _socket = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.IPv4 );
            if ( !IPAddress.TryParse( ip, out IPAddress tmp ) )
                throw new InvalidCastException( $"Could not parse \"{ip}\" to a valid IP address" );

            _socket.Bind( new IPEndPoint( tmp, port ) );

            ClientConnectionRequested += callback;
            BeginReceiving();
        }

        #endregion

        #region Methods

        private void BeginReceiving() {
            while ( ServerActive ) {
                byte[] bytes = new byte[ 4096 ];
                int length = _socket.Receive( bytes );

                Packet p = new Packet( new List<byte>( bytes ).GetRange( 0, length ) );
                if ( !p.Type.Name.ToLower() == "login" )

            }
        }

        #endregion

    }

}
