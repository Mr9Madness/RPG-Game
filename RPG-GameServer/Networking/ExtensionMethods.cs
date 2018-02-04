using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Networking {

    public static class ExtMethods {

        public static int GetPort( this EndPoint ep ) {
            string str = ep.ToString();
            return int.Parse( str.Substring( str.IndexOf( ':' ) + 1 ) );
        }

        public static IPAddress GetAddress( this EndPoint ep ) {
            string str = ep.ToString();
            return IPAddress.Parse( str.Substring( 0, str.IndexOf( ':' ) ) );
        }

        public static bool IsLocal( this IPAddress ip ) {
            byte[] bytes = ip.GetAddressBytes();

            IPHostEntry host = Dns.GetHostEntry( Dns.GetHostName() );
            IPAddress localIP = host.AddressList.FirstOrDefault( localip => localip.AddressFamily == AddressFamily.InterNetwork );
            byte[] localIPBytes = localIP?.GetAddressBytes() ?? throw new Exception( "Could not find any IPv4 network interfaces" );

            // Return if the address is 127.0.0.1 or local
            return bytes[ 0 ] == 127 && bytes[ 1 ] == 0 && bytes[ 2 ] == 0 && bytes[ 3 ] == 1 || bytes[ 0 ] == localIPBytes[ 0 ] && bytes[ 1 ] == localIPBytes[ 1 ];
        }

        public static bool IsAny( this IPAddress ip ) => ip.Equals( IPAddress.Any );
        public static bool IsNone( this IPAddress ip ) => ip.Equals( IPAddress.None );
        public static bool IsAnyOrNone( this IPAddress ip ) => ip.IsAny() || ip.IsNone();

    }

}
