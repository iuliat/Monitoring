﻿using System;

namespace CoreAMQP.Messages
{
    [Serializable]
    public class MessageFibonacci : Message
    {
        public MessageFibonacci(UInt64 Payload) : base(typeof(MessageFibonacci), Payload)
        {
        }

        public static UInt64 Calculate(UInt64 Value)
        {
            if ((Value == 0) || (Value == 1))
            {
                return Value;
            }

            return MessageFibonacci.Calculate(Value - 1)
                + MessageFibonacci.Calculate(Value - 2);
        }

    }
}
