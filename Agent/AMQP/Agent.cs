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
using System.Timers;

namespace AgentAPI.AMQP
{
    public abstract class Agent : VMMonitorNode
    {
        public static readonly Double DefaultIntervalCollect = 1000D;
        public static readonly Double DefaultIntervalTransmit = 5000D;

        private Timer TriggerCollect;
        private Timer TriggerTransmit;

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
            this.TriggerCollect = new Timer();
            this.TriggerCollect.Enabled = false;
            this.TriggerCollect.AutoReset = true;
            this.TriggerCollect.Interval = DefaultIntervalCollect;
            this.TriggerCollect.Elapsed += DataCollect;

            this.TriggerTransmit = new Timer();
            this.TriggerTransmit.Enabled = false;
            this.TriggerTransmit.AutoReset = true;
            this.TriggerTransmit.Interval = DefaultIntervalTransmit;
            this.TriggerTransmit.Elapsed += DataTransmit;
        }
        #endregion

        #region Agent Custom Message Handlers
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
                + " ] Received Fibonacci Request "
                + "( Message Type: "
                + newMessage.GetType()
                + ", Value: "
                + Convert.ToUInt64(newMessage.Payload)
                + " )");

            return new MessageFibonacci(MessageFibonacci.Calculate(Convert.ToUInt64(newMessage.Payload)));
        }

        public Message Receive(MessageIPV4 newMessage)
        {
            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Received IPV4 Request "
                + "( Message Type: "
                + newMessage.GetType()
                + " )");

            return new MessageIPV4(MessageIPV4.GetLocalIPAddress());
        }

        public Message Receive(EstablishedCommunicationMessage newMessage)
        {
            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Received Established Communication State Request "
                + "( Message Type: "
                + newMessage.GetType()
                + ", Value: "
                + Convert.ToBoolean(newMessage.Payload)
                + " )");

            return new EstablishedCommunicationMessage(EstablishedCommunicationMessage.IsValidCommunication());
        }

        public virtual Message Receive(MessageRAMInit newMessage)
        {
            throw new NotImplementedException();
        }

        public Message Receive(MessageRAM newMessage)
        {
            return null;
        }

        public virtual Message Receive(MessageCPUInit newMessage)
        {
            throw new NotImplementedException();
        }

        public Message Receive(MessageCPU newMessage)
        {
            return null;
        }
        #endregion

        #region Agent Behaviour
        public override void Open()
        {
            try
            {
                base.Open();

                this.TriggerCollect.Start();
                this.TriggerTransmit.Start();
            }
            catch (Exception /*E*/)
            {
                try
                {
                    this.TriggerCollect.Stop();
                    this.TriggerTransmit.Stop();

                    base.Close();
                }
                catch (Exception /*E*/)
                {

                }
            }
        }

        // Debug hook method
        protected override void ReceiveDispatch(object Sender, BasicDeliverEventArgs Event)
        {
            base.ReceiveDispatch(Sender, Event);
        }

        public override Boolean Filter(BasicDeliverEventArgs Event)
        {
            // if (Event.BasicProperties.AppId != "PrincipalAPI.AMQP.Principal")
            //     return true;

            return false;
        }

        public abstract Dictionary<string, string> getResources();

        protected abstract void DataCollect(object sender, ElapsedEventArgs e);

        protected abstract void DataTransmit(object sender, ElapsedEventArgs e);
        #endregion
    }
}
