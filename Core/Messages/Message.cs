using System;

namespace CoreAMQP.Messages
{
    [Serializable]
    public abstract class Message
    {
        public readonly object Payload;
        public readonly Type Type;

        public Message(Type Type, object Payload)
        {
            this.Type = Type;
            this.Payload = Payload;
        }

    }
}