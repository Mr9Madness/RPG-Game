using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UdpNetworking;

namespace RPG_GameServer {

    public class Program {

        private static void OnPacketReceived( UdpSocket s, Packet p ) {
            Console.WriteLine( $"RECV ({p.TypeName})\"{p.Content}\"({p.AsByteArray.Length}) FROM {p.RemoteEP?.ToString() ?? "NULL"} ON {s.LocalEP?.ToString() ?? "NULL"}" );
        }

        private static void OnPacketSent( UdpSocket s, Packet p ) {
            Console.WriteLine( $"SENT ({p.TypeName})\"{p.Content}\"({p.AsByteArray.Length}) TO {s.RemoteEP?.ToString() ?? "NULL"} ON {s.LocalEP?.ToString() ?? "NULL"}" );
        }

        private static void OnSocketAccepted( UdpServer serv, UdpSocket s ) {
            if ( Clients.Any( c => c.RemoteEP.ToString() == s.RemoteEP.ToString() ) )
                return;

            Clients.Add( s );
            Console.WriteLine( $"{s.RemoteEP} has connected." );
        }

        private static UdpServer _listener;
        public static List<UdpSocket> Clients = new List<UdpSocket>();

        public static void Main( string[] args ) {
            UdpServer.OnSocketAccepted += OnSocketAccepted;
            UdpSocket.OnPacketReceived += OnPacketReceived;
            UdpSocket.OnPacketSent += OnPacketSent;

            _listener = new UdpServer( 8080 );

            CancellationTokenSource cts = new CancellationTokenSource();
            Task.Run( () => {
                while ( true ) {
                    UdpSocket s = _listener.AcceptSocket();

                    if ( Clients.Any( c => c.RemoteEP.ToString() == s.RemoteEP.ToString() ) )
                        return;

                    // ReSharper disable once MethodSupportsCancellation
                    s.StartReceiving();
                    Clients.Add( s );
                }
            }, cts.Token );

            while ( true ) {
                string input = Console.ReadLine();
                if ( string.IsNullOrWhiteSpace( input ) )
                    continue;

                if ( input.ToLower() == "/nc".ToLower() ) {
                    Process.Start( "Client.exe" );
                    continue;
                }
                if ( input.ToLower() == "/startlistening".ToLower() ) {
                    Task.Run( () => {
                        while ( true ) {
                            UdpSocket s = _listener.AcceptSocket();

                            if ( Clients.All( c => c.RemoteEP.ToString() != s.RemoteEP.ToString() ) )
                                Clients.Add( s );
                        }
                    }, cts.Token );
                }
                if ( input.ToLower() == "/stoplistening".ToLower() ) {
                    cts.Cancel();
                    continue;
                }
                
                Clients.ForEach( c => c.SendAsync( input ) );
            }
        }

    }

}
