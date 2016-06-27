using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace CoreAMQP.Messages
{
    [Serializable]
    public class MessageCPUInit : Message
    {
        public MessageCPUInit(Dictionary<String, Object> Payload) : base(typeof(MessageCPUInit), Payload)
        {
        }
    }
}
