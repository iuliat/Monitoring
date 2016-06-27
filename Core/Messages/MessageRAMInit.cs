using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace CoreAMQP.Messages
{
    [Serializable]
    public class MessageRAMInit : Message
    {
        public MessageRAMInit(Dictionary<String, Object> Payload) : base(typeof(MessageRAMInit), Payload)
        {
        }
    }
}
