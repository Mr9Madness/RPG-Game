using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Networking;
using System.Diagnostics;
using System.Threading;

namespace Client {

    public class Program {

        private static UdpSocket s = new UdpSocket();

        public static void Main( string[] args ) {
            s.Run();
        }
    }

    public class UdpSocket {

        private Socket _receiver;
        private Socket _sender;

        private int _serverPort = 8080;
        private IPAddress _serverIP = IPAddress.Parse( "127.0.0.1" );
        //private static IPAddress _serverIP = IPAddress.Parse( "213.46.57.198" );

        private IPEndPoint _serverEP => new IPEndPoint( _serverIP, _serverPort );

        private void InitializeSockets() {
            _receiver = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
            _sender = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
        }

        Packet lastPacketSent = null;
        string currentID = null;
        int sentPackets = 0;
        int returnedPackets = 0;
        int packetLoss => sentPackets - returnedPackets;

        public void Run() {

            InitializeSockets();

            _sender.Connect( _serverEP );
            Packet dummy = Packet.Dummy.SetOrigin( 0, _sender.LocalEndPoint.GetPort() );
            _receiver.SendTo( dummy.AsByteArray, _serverEP );
            sentPackets++;
            Console.WriteLine( $"SENT ({dummy.TypeName})\"{dummy.Content}\" TO {_serverEP} WITH RECVSOC" );

            Task.Run( () => {
                while ( true ) {
                    ReceivePacket( out Packet p, out ExtEndPoint fromEP );

                    if ( lastPacketSent != null && currentID != null && lastPacketSent.Content.ToString() == currentID ) {
                        Console.WriteLine( $"RTRN ({p.TypeName})\"{p.Content}\" FROM {fromEP} WITH RECVSOC" );
                    } else
                        Console.WriteLine( $"RECV ({p.TypeName})\"{p.Content}\" FROM {fromEP} WITH RECVSOC" );
                }
            } );

            Task.Run( () => {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                Stopwatch sw = Stopwatch.StartNew();
                while ( true ) {
                    Console.WriteLine( $"PACKETS SENT: {sentPackets} RETURNED: {returnedPackets} LOST: {packetLoss}" );
                    if ( packetLoss > 10 )
                        break;
                    currentID = Guid.NewGuid().ToString( "n" ).Substring( 0, 10 );
                    lastPacketSent = new Packet( currentID );
                    SendPacketTo( lastPacketSent, _serverEP );
                    Console.WriteLine( $"SENT ({lastPacketSent.TypeName})\"{lastPacketSent.Content}\" TO {_serverEP} WITH SENDSOC" );

                    int ms = (int)sw.ElapsedMilliseconds;
                    const int sleepMax = 1;
                    Thread.Sleep( sleepMax - ms >= 0 ? sleepMax - ms : 0 );
                    sw.Restart();
                }
            } );

            while ( true ) {
                string input = Console.ReadLine();
                if ( string.IsNullOrWhiteSpace( input ) )
                    continue;

                Packet p = new Packet( input );
                SendPacketTo( p, _serverEP );
                Console.WriteLine( $"SENT ({p.TypeName})\"{p.Content}\" TO {_serverEP} WITH SENDSOC" );
            }
        }

        public void SendPacketTo( object value, IPEndPoint remoteEP ) => SendPacketTo( new Packet( value ), remoteEP );
        public void SendPacketTo( Packet packet, IPEndPoint remoteEP ) {
            _sender.SendTo( packet.SetOrigin( _receiver.LocalEndPoint.GetPort(), _sender.LocalEndPoint.GetPort() ).AsByteArray, remoteEP );
            sentPackets++;
        }

        public void ReceivePacket( out Packet packet, out ExtEndPoint remoteEP ) => remoteEP = ReceivePacket( out packet, 4096 );
        public void ReceivePacket( out Packet packet, out ExtEndPoint remoteEP, int maxBufferSize ) => remoteEP = ReceivePacket( out packet, maxBufferSize );
        public ExtEndPoint ReceivePacket( out Packet packet ) => ReceivePacket( out packet, 4096 );
        public ExtEndPoint ReceivePacket( out Packet packet, int maxBufferSize ) {
            byte[] buffer = new byte[ maxBufferSize ];
            EndPoint fromEP = new IPEndPoint( IPAddress.Any, 0 );
            int len = _receiver.ReceiveFrom( buffer, ref fromEP );
            returnedPackets++;
            packet = Packet.FromByteArray( buffer.ToList().GetRange( 0, len ) ).SetOrigin( fromEP.GetAddress() );

            if ( packet.RemoteEP.RPort == 0 )
                packet.RemoteEP.RPort = fromEP.GetPort();

            return packet.RemoteEP;
        }

    }

}
