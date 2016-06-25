using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace CoreAMQP.Messages
{
    [Serializable]
    public class MessageIPV4 : Message
    {
        public MessageIPV4(List<String> Payload) : base(typeof(MessageIPV4), Payload)
        {
        }

        public static List<String> GetLocalIPAddress()
        {
            List<String> IPv4s = new List<String>();

            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                throw new Exception("No network connection is available.");
            }

            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPv4s.Add(ip.ToString());
                }
            }

            return IPv4s;
            //throw new Exception("Local IP Address Not Found!");
        }

    }
}
