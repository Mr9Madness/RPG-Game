using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Networking;

namespace RPG_GameServer {

    public class UdpServer {

        private static Socket _receiver;

        private static List<ExtEndPoint> _clientList = new List<ExtEndPoint>();

        public static int Port = 8080;

        private static void InitializeSockets() {
            _receiver = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
        }

        private static ExtEndPoint GetClient( ExtEndPoint ep ) => _clientList.FirstOrDefault( c => c.Address.Equals( ep.Address ) && c.RPort == ep.RPort && c.SPort == ep.SPort );
        private static ExtEndPoint GetClient( IPEndPoint ep ) => _clientList.FirstOrDefault( c => c.Address.Equals( ep.Address ) && ( c.RPort == ep.Port || c.SPort == ep.Port ) );

        public static void Main( string[] args ) {
            InitializeSockets();

            _receiver.Bind( new IPEndPoint( IPAddress.Any, Port ) );

            Task.Run( () => {
                while ( true ) {
                    ReceivePacket( out Packet p, out ExtEndPoint fromEP );
                    Console.WriteLine( $"RECV ({p.TypeName})\"{p.Content}\" FROM {fromEP} WITH RECVSOC" );

                    if ( GetClient( fromEP.SEndPoint ) == null )
                        _clientList.Add( fromEP );

                    SendPacketTo( p, fromEP.REndPoint );
                    Console.WriteLine( $"SENT ({p.TypeName})\"{p.Content}\" TO {fromEP.REndPoint} WITH SENDSOC" );
                }
            } );

            while ( true ) {
                string input = Console.ReadLine();
                if ( string.IsNullOrWhiteSpace( input ) )
                    continue;

                if ( input == "/nc" ) {
                    Process.Start( "Client.exe" );
                    continue;
                }

                Packet p = new Packet( input );
                _clientList.ForEach( c => {
                    SendPacketTo( p, c.REndPoint );
                    Console.WriteLine( $"SENT ({p.TypeName})\"{p.Content}\" TO {c.REndPoint} WITH SENDSOC" );
                } );
            }
        }

        public static void SendPacketTo( object value, IPEndPoint remoteEP ) => SendPacketTo( new Packet( value ), remoteEP );
        public static void SendPacketTo( Packet packet, IPEndPoint remoteEP ) {
            Socket _sender = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
            _sender.Bind( new IPEndPoint( IPAddress.Any, remoteEP.Address.IsLocal() ? 0 : Port ) );
            _sender.Connect( remoteEP );
            _sender.SendTo( packet.SetOrigin( _receiver.LocalEndPoint.GetPort(), _sender.LocalEndPoint.GetPort() ).AsByteArray, remoteEP );
            _sender.Close();
        }

        public static void ReceivePacket( out Packet packet, out ExtEndPoint remoteEP ) => remoteEP = ReceivePacket( out packet, 4096 );
        public static void ReceivePacket( out Packet packet, out ExtEndPoint remoteEP, int maxBufferSize ) => remoteEP = ReceivePacket( out packet, maxBufferSize );
        public static ExtEndPoint ReceivePacket( out Packet packet ) => ReceivePacket( out packet, 4096 );
        public static ExtEndPoint ReceivePacket( out Packet packet, int maxBufferSize ) {
            byte[] buffer = new byte[ maxBufferSize ];
            EndPoint fromEP = new IPEndPoint( IPAddress.Any, 0 );
            int len = _receiver.ReceiveFrom( buffer, ref fromEP );
            packet = Packet.FromByteArray( buffer.ToList().GetRange( 0, len ) ).SetOrigin( fromEP.GetAddress() );

            if ( packet.RemoteEP.RPort == 0 )
                packet.RemoteEP.RPort = fromEP.GetPort();

            return packet.RemoteEP;
        }

    }

}
