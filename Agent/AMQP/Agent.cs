using CoreAMQP;
using CoreAMQP.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AgentAPI.AMQP
{
    public abstract class Agent : AMQPNode
    {
        #region Constructors
        public Agent() : this("user", "password", "127.0.0.1")
        {
        }

        public Agent(String customUser, String customPassword) : this(customUser, customPassword, "127.0.0.1")
        {
        }

        public Agent(String customBrokerAddress) : this("user", "password", customBrokerAddress)
        {
        }

        public Agent(String customUser, String customPassword, String customBrokerAddress) : base(customUser, customPassword, customBrokerAddress)
        {
        }
        #endregion

        protected override void ReceiveDispatch(object Sender, BasicDeliverEventArgs Event)
        {
            Byte[] ReceivedBody = Event.Body;

            IBasicProperties ReceiveProperties = Event.BasicProperties;

            IBasicProperties SendProperties = Channel.CreateBasicProperties();
            SendProperties.CorrelationId = ReceiveProperties.CorrelationId;
            SendProperties.ReplyTo = ReceiveProperties.ReplyTo;

            Message ReceivedMessage = DeserializeToType<Message>(ReceivedBody);

            Type ReceivedMessageType = ReceivedMessage.Type;
            dynamic Propper = Convert.ChangeType(ReceivedMessage, ReceivedMessageType);
            Message SendMessage = this.Receive(Propper);

            // If received message needs a reply
            if (SendMessage != null)
            {
                // Send needed reply
                this.Send(SendProperties, SendMessage);
            }
            Channel.BasicAck(deliveryTag: Event.DeliveryTag, multiple: false);

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Received "
                + "( Message Type: "
                + ReceivedMessage.GetType()
                + ", Source: "
                + ReceiveProperties.ReplyTo
                + " )");
        }

        public void Send(Message newMessage)
        {
            IBasicProperties SendProperties = Channel.CreateBasicProperties();
            SendProperties.ReplyTo = AMQPNode.DefaultQueueName;
            SendProperties.CorrelationId = Guid.NewGuid().ToString();

            this.Send(SendProperties, newMessage);
        }

        public void Send(IBasicProperties SendProperties, Message newMessage)
        {
            Byte[] SendMessage = SerializeFromType<Message>(newMessage);

            base.Send(SendProperties, SendMessage);
        }

        public abstract Dictionary<string, string> getResources();

        public Message Receive(MessageFibonacci newMessage)
        {
            return new MessageFibonacci(MessageFibonacci.Calculate(Convert.ToUInt64(newMessage.Payload)));
        }

        public Message Receive(MessageIPV4 newMessage)
        {
            return new MessageIPV4(MessageIPV4.GetLocalIPAddress());
        }

        public Message Receive(EstablishedCommunicationMessage newMessage)
        {
            return new EstablishedCommunicationMessage(EstablishedCommunicationMessage.IsValidCommunication());
        }
    }
}
