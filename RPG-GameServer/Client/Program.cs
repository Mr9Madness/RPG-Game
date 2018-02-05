using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UdpNetworking;

namespace Client {

    public class Program {

        private static UdpSocket socket;

        private static void OnPacketReceived( UdpSocket s, Packet p ) {
            Console.WriteLine( $"RECV ({p.TypeName})\"{p.Content}\"({p.AsByteArray.Length}) FROM {p.RemoteEP?.ToString() ?? "NULL"} ON {s.LocalEP?.ToString() ?? "NULL"}" );
        }

        private static void OnPacketSent( UdpSocket s, Packet p ) {
            Console.WriteLine( $"SENT ({p.TypeName})\"{p.Content}\"({p.AsByteArray.Length}) TO {s.RemoteEP?.ToString() ?? "NULL"} ON {s.LocalEP?.ToString() ?? "NULL"}" );
        }

        public static void Main( string[] args ) {
            UdpSocket.OnPacketReceived += OnPacketReceived;
            UdpSocket.OnPacketSent += OnPacketSent;

            //socket = new UdpSocket( "127.0.0.1", 8080 );
            socket = new UdpSocket( "213.46.57.198", 8080 );
            
            socket.StartReceiving();
            
            while ( true ) {
                string input = Console.ReadLine();
                if ( string.IsNullOrWhiteSpace( input ) )
                    continue;

                if ( input == "/spam" )
                    Task.Run( () => {
                        for ( int i = 0; i < 50; i++ )
                            socket.SendAsync( "SPAM!" );
                    } );

                socket.SendAsync( input );
            }
        }

    }
    
}
