using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace CoreAMQP.Messages
{
    [Serializable]
    public class MessageCPU : Message
    {
        public MessageCPU(List<Single> Payload) : base(typeof(MessageCPU), Payload)
        {
        }
    }
}
