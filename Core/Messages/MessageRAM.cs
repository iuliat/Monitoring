using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace CoreAMQP.Messages
{
    [Serializable]
    public class MessageRAM : Message
    {
        public MessageRAM(List<Single> Payload) : base(typeof(MessageRAM), Payload)
        {
        }
    }
}
