using CoreAMQP;
using CoreAMQP.Messages;
using PrincipalAPI.Models;
using PrincipalAPI.Storage;
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
            VMStorage Database = new VMStorage();

            Dictionary<String, Object> ReceivedPayload = newMessage.Payload as Dictionary<String, Object>;
            String HostName = Convert.ToString(ReceivedPayload["HostName"]);
            //String IP = Convert.ToString(ReceivedPayload["IP"]);
            String AgentReceiveQueueName = Convert.ToString(ReceivedPayload["ReplyTo"]);

            Host AgentHost = Database.GetHostByHostname(HostName);
            if(AgentHost == null)
            {
                AgentHost = new Host();
                AgentHost.Hostname = HostName;
                //AgentHost.HostIP = IP;
                Database.AddNewVM(AgentHost);
            }
            Int32 HostId = Database.GetHostByHostname(HostName).HostID;

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
            VMStorage Database = new VMStorage();

            Dictionary<String, Object> ReceivedPayload = newMessage.Payload as Dictionary<String, Object>;
            String HostName = Convert.ToString(ReceivedPayload["HostName"]);
            //String IP = Convert.ToString(ReceivedPayload["IP"]);
            String AgentReceiveQueueName = Convert.ToString(ReceivedPayload["ReplyTo"]);

            Host AgentHost = Database.GetHostByHostname(HostName);
            if (AgentHost == null)
            {
                AgentHost = new Host();
                AgentHost.Hostname = HostName;
                //AgentHost.HostIP = IP;
                Database.AddNewVM(AgentHost);
            }
            Int32 HostId = Database.GetHostByHostname(HostName).HostID;
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
            VMStorage Database = new VMStorage();

            List<Dictionary<String, Object>> CollectedRAM = newMessage.Payload as List<Dictionary<String, Object>>;
            StringBuilder ValueString = new StringBuilder();
            ValueString.Append("{ ");
            foreach (Dictionary<String, Object> InstanceRAM in CollectedRAM)
            {
                Int32 HostId = Convert.ToInt32(InstanceRAM["HostId"]);
                RAM newRAM = new RAM();
                Int32 Value = (Int32)Convert.ToSingle(InstanceRAM["Value"]);
                newRAM.Value = Value;
                DateTime Date = Convert.ToDateTime(InstanceRAM["Date"]);
                newRAM.Date = Date;
                String Category = Convert.ToString(InstanceRAM["Category"]);
                newRAM.Category = Category;
                Int32 upperLimit = Convert.ToInt32(InstanceRAM["upperLimit"]);
                newRAM.upperLimit = upperLimit;
                Int32 lowerLimit = Convert.ToInt32(InstanceRAM["lowerLimit"]);
                newRAM.lowerLimit = lowerLimit;
                Database.AddRAM(HostId, newRAM);

                ValueString.Append("{ ");
                ValueString.Append("Value: ");
                ValueString.Append(Value + "MB");
                ValueString.Append(", Date: ");
                ValueString.Append(Date);
                ValueString.Append(", Category: ");
                ValueString.Append(Category);
                ValueString.Append(", upperLimit: ");
                ValueString.Append(upperLimit + "MB");
                ValueString.Append(", lowerLimit: ");
                ValueString.Append(lowerLimit + "MB");
                ValueString.Append(" }");
            }
            ValueString.Append(" }");

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Received RAM Message "
                + "( Message Type: "
                + newMessage.GetType()
                + ", Value: "
                + ValueString.ToString()
                + " )");

            return null;
        }

        public Message Receive(MessageCPU newMessage)
        {
            VMStorage Database = new VMStorage();

            List<Dictionary<String, Object>> CollectedCPU = newMessage.Payload as List<Dictionary<String, Object>>;
            StringBuilder ValueString = new StringBuilder();
            ValueString.Append("{ ");
            foreach(Dictionary<String, Object> InstanceCPU in CollectedCPU)
            {
                Int32 HostId = Convert.ToInt32(InstanceCPU["HostId"]);
                CPU newCPU = new CPU();
                Int32 Value = (Int32)Convert.ToSingle(InstanceCPU["Value"]);
                newCPU.Value = Value;
                DateTime Date = Convert.ToDateTime(InstanceCPU["Date"]);
                newCPU.Date = Date;
                String Category = Convert.ToString(InstanceCPU["Category"]);
                newCPU.Category = Category;
                Int32 upperLimit = Convert.ToInt32(InstanceCPU["upperLimit"]);
                newCPU.upperLimit = upperLimit;
                Int32 lowerLimit = Convert.ToInt32(InstanceCPU["lowerLimit"]);
                newCPU.lowerLimit = lowerLimit;
                Database.AddCPU(HostId, newCPU);

                ValueString.Append("{ ");
                ValueString.Append("Value: ");
                ValueString.Append(Value + "%");
                ValueString.Append(", Date: ");
                ValueString.Append(Date);
                ValueString.Append(", Category: ");
                ValueString.Append(Category);
                ValueString.Append(", upperLimit: ");
                ValueString.Append(upperLimit + "%");
                ValueString.Append(", lowerLimit: ");
                ValueString.Append(lowerLimit + "%");
                ValueString.Append(" }");
            }
            ValueString.Append(" }");

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Received CPU Message "
                + "( Message Type: "
                + newMessage.GetType()
                + ", Value: "
                + ValueString.ToString()
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
            // if (Event.BasicProperties.AppId == "PrincipalAPI.AMQP.Principal")
            //     return true;

            return false;
        }
        #endregion
    }
}
