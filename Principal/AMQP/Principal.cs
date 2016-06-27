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

        private Object SendQueueLock = new Object();

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

        public Message Receive(MessageRAMInit newMessage)
        {
            Dictionary<String, Object> ReceivedPayload = newMessage.Payload as Dictionary<String, Object>;
            String HostName = Convert.ToString(ReceivedPayload["HostName"]);
            String AgentReceiveQueueName = Convert.ToString(ReceivedPayload["ReplyTo"]);
            //Lookup HostId, upperLimit, LowerLimit for HostName
            Int32 HostId = 1;
            Int32 upperLimit = 30000;
            Int32 lowerLimit = 0;

            Dictionary<String, Object> Payload = new Dictionary<String, Object>();
            Payload.Add("HostId", HostId);
            Payload.Add("upperLimit", upperLimit);
            Payload.Add("lowerLimit", lowerLimit);

            MessageRAMInit RAMInit = new MessageRAMInit(Payload);

            lock (SendQueueLock)
            {
                this.ChangeSendQueueName(AgentReceiveQueueName);
                this.OpenSender();
                this.Send(RAMInit);
                this.CloseSender();
            }

            return null;
        }

        public Message Receive(MessageCPUInit newMessage)
        {
            Dictionary<String, Object> ReceivedPayload = newMessage.Payload as Dictionary<String, Object>;
            String HostName = Convert.ToString(ReceivedPayload["HostName"]);
            String AgentReceiveQueueName = Convert.ToString(ReceivedPayload["ReplyTo"]);
            //Lookup HostId, upperLimit, LowerLimit for HostName
            Int32 HostId = 1;
            Int32 upperLimit = 100;
            Int32 lowerLimit = 0;

            Dictionary<String, Object> Payload = new Dictionary<String, Object>();
            Payload.Add("HostId", HostId);
            Payload.Add("upperLimit", upperLimit);
            Payload.Add("lowerLimit", lowerLimit);

            MessageCPUInit CPUInit = new MessageCPUInit(Payload);

            lock (SendQueueLock)
            {
                this.ChangeSendQueueName(AgentReceiveQueueName);
                this.OpenSender();
                this.Send(CPUInit);
                this.CloseSender();
            }

            return null;
        }

        public Message Receive(MessageRAM newMessage)
        {
            List<Dictionary<String, Object>> CollectedCPU = newMessage.Payload as List<Dictionary<String, Object>>;
            StringBuilder Value = new StringBuilder();
            Value.Append("{ ");
            foreach (Dictionary<String, Object> InstanceCPU in CollectedCPU)
            {
                Value.Append("{ ");
                Value.Append("Value: ");
                Value.Append((Int32)Convert.ToSingle(InstanceCPU["Value"]));
                Value.Append(", Date: ");
                Value.Append(Convert.ToDateTime(InstanceCPU["Date"]));
                Value.Append(", Category: ");
                Value.Append(Convert.ToString(InstanceCPU["Category"]));
                Value.Append(", upperLimit: ");
                Value.Append(Convert.ToInt32(InstanceCPU["upperLimit"]));
                Value.Append(", lowerLimit: ");
                Value.Append(Convert.ToInt32(InstanceCPU["lowerLimit"]));
                Value.Append(" }");
            }
            Value.Append(" }");

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Received RAM Message "
                + "( Message Type: "
                + newMessage.GetType()
                + ", Value: "
                + Value.ToString()
                + " )");

            return null;
        }

        public Message Receive(MessageCPU newMessage)
        {
            List<Dictionary<String, Object>> CollectedCPU = newMessage.Payload as List<Dictionary<String, Object>>;
            StringBuilder Value = new StringBuilder();
            Value.Append("{ ");
            foreach(Dictionary<String, Object> InstanceCPU in CollectedCPU)
            {
                Value.Append("{ ");
                Value.Append("Value: ");
                Value.Append((Int32)Convert.ToSingle(InstanceCPU["Value"]));
                Value.Append(", Date: ");
                Value.Append(Convert.ToDateTime(InstanceCPU["Date"]));
                Value.Append(", Category: ");
                Value.Append(Convert.ToString(InstanceCPU["Category"]));
                Value.Append(", upperLimit: ");
                Value.Append(Convert.ToInt32(InstanceCPU["upperLimit"]));
                Value.Append(", lowerLimit: ");
                Value.Append(Convert.ToInt32(InstanceCPU["lowerLimit"]));
                Value.Append(" }");
            }
            Value.Append(" }");

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Received CPU Message "
                + "( Message Type: "
                + newMessage.GetType()
                + ", Value: "
                + Value.ToString()
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
