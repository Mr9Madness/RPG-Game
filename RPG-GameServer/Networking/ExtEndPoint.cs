using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Networking {

    [Serializable]
    public class ExtEndPoint {

        public int RPort;
        public int SPort;
        public IPAddress Address;

        public IPEndPoint REndPoint => new IPEndPoint( Address, RPort );
        public IPEndPoint SEndPoint => new IPEndPoint( Address, SPort );

        public ExtEndPoint( IPAddress address, int receiverPort, int senderPort ) {
            Address = address;
            RPort = receiverPort;
            SPort = senderPort;
        }

        public override string ToString() => ToString( true, true );
        public string ToString( bool includeSenderPort ) => ToString( true, includeSenderPort );
        public string ToString( bool includeReceiverPort, bool includeSenderPort ) {
            string str = Address.ToString();

            if ( includeReceiverPort )
                str += $":{RPort}";
            if ( includeSenderPort )
                str += $"::{SPort}";

            return str;
        }

    }

}
