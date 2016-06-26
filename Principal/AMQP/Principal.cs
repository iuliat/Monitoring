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
    public class Principal : VMMonitorNode
    {
        public new const String DefaultReceiveQueueName = "Principal Receive Queue";
        public new const String DefaultSendQueueName = "Principal Send Queue";

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

        #region Principal Custom Message Handlers
        public override Message Receive(Message newMessage)
        {
            Type newMessageType = newMessage.Type;
            dynamic Propper = Convert.ChangeType(newMessage, newMessageType);

            return this.Receive(Propper);
        }

        public Message Receive(MessageFibonacci newMessage)
        {
            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Received Fibonacci Result "
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
                + " ] Received IPV4 Result "
                + "( Message Type: "
                + newMessage.GetType()
                + ", Value: "
                + (newMessage.Payload as List<String>).ToArray<String>()
                + " )");

            return null;
        }

        public Message Receive(EstablishedCommunicationMessage newMessage)
        {
            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Received Established Communication State Response "
                + "( Message Type: "
                + newMessage.GetType()
                + ", Value: "
                + Convert.ToBoolean(newMessage.Payload)
                + " )");

            return null;
        }

        public Message Receive(MessageRAM newMessage)
        {
            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Received RAM Message "
                + "( Message Type: "
                + newMessage.GetType()
                + ", Value: "
                + "{ "
                + String.Join<Single>(" ", (newMessage.Payload as List<Single>))
                + " }"
                + " )");

            return null;
        }

        public Message Receive(MessageCPU newMessage)
        {
            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Received CPU Message "
                + "( Message Type: "
                + newMessage.GetType()
                + ", Value: "
                + "{ "
                + String.Join<Single>(" ", (newMessage.Payload as List<Single>))
                + " }"
                + " )");

            return null;
        }
        #endregion

        #region Principal Behaviour
        // Debug hook method
        protected override void ReceiveDispatch(object Sender, BasicDeliverEventArgs Event)
        {
            base.ReceiveDispatch(Sender, Event);
        }

        public override Boolean Filter(BasicDeliverEventArgs Event)
        {
            if (Event.BasicProperties.AppId == "PrincipalAPI.AMQP.Principal")
                return true;

            return false;
        }
        #endregion
    }
}
