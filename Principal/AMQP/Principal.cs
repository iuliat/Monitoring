using CoreAMQP;
using CoreAMQP.Messages;
using PrincipalAPI.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PrincipalAPI.AMQP
{
    public class Principal : AMQPNode
    {
        public readonly LinkedList<Message> Messages = new LinkedList<Message>();

        #region Constructors
        public Principal() : this("user", "password", "127.0.0.1")
        {
        }

        public Principal(String customUser, String customPassword) : this(customUser, customPassword, "127.0.0.1")
        {
        }

        public Principal(String customBrokerAddress) : this("user", "password", customBrokerAddress)
        {
        }

        public Principal(String customUser, String customPassword, String customBrokerAddress) : base(customUser, customPassword, customBrokerAddress)
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

        public Message Receive(MessageFibonacci newMessage)
        {
            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] ReceivedValue "
                + "( Message Type: "
                + newMessage.GetType()
                + ", Value: "
                + Convert.ToUInt64(newMessage.Payload)
            + " )");

            return null;
        }

        public Message Receive(MessageIPV4 newMessage)
        {
            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] ReceivedValue "
                + "( Message Type: "
                + newMessage.GetType()
                + ", Value: "
                + "tampenie"
            + " )");

            return null;
        }

        public Message Receive(EstablishedCommunicationMessage newMessage)
        {
            //using (var context = new PrincipalAPIContext())
            //{
            //    var currentController = new Controller();

            //    currentController.ControllerID = 77;
            //    currentController.IP = "23.23.24";
            //    currentController.hosts = null;

            //    context.Controllers.Add(currentController);
            //    context.SaveChanges();
            //}

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] ReceivedValue "
                + "( Message Type: "
                + newMessage.GetType()
                + ", Value: "
                + Convert.ToBoolean(newMessage.Payload)
            + " )");

            return null;
        }
    }
}
