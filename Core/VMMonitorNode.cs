using CoreAMQP.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace CoreAMQP
{
    public abstract class VMMonitorNode : AMQPNode
    {
        public new const String DefaultReceiveQueueName = "VMMonitor Receive Queue";
        public new const String DefaultSendQueueName = "VMMonitor Send Queue";

        #region Constructors
        public VMMonitorNode() : this("user", "password", "127.0.0.1")
        {
        }

        public VMMonitorNode(String customUser, String customPassword) : this(customUser, customPassword, "127.0.0.1")
        {
        }

        public VMMonitorNode(String customBrokerAddress) : this("user", "password", customBrokerAddress)
        {
        }

        public VMMonitorNode(String customUser, String customPassword, String customBrokerAddress) : base(customUser, customPassword, customBrokerAddress)
        {
        }
        #endregion

        protected override void ReceiveDispatch(object Sender, BasicDeliverEventArgs Event)
        {
            Byte[] ReceivedBody = Event.Body;

            IBasicProperties ReceiveProperties = Event.BasicProperties;

            QueueDeclareOk QueueState = ChannelReceive.QueueDeclarePassive(this.ReceiveQueueName);
            UInt32 MessageCount = QueueState != null ? QueueState.MessageCount : 0;
            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] State "
                + "( Message Count: "
                + MessageCount
                + " )");

            // If received message should be filtered
            if (Filter(Event))
            {
                // Don't process it
                return;
            }

            Message ReceivedMessage = DeserializeToType<Message>(ReceivedBody);
            Message SendMessage = null;
            try
            {
                SendMessage = this.Receive(ReceivedMessage);
                System.Diagnostics.Debug.WriteLine("[ "
                    + this.GetType()
                    + " ] Handled "
                    + "( Message Type: "
                    + ReceivedMessage.Type
                    + ", Source: "
                    + ReceiveProperties.AppId
                    + ", Id: "
                    + ReceiveProperties.MessageId
                    + " )");
            }
            catch (Exception ReceptionHandling)
            {
                System.Diagnostics.Debug.WriteLine(ReceptionHandling.Message);
                System.Diagnostics.Debug.WriteLine("[ "
                    + this.GetType()
                    + " ] Failed to Handle "
                    + "( Message Type: "
                    + ReceivedMessage.GetType()
                    + ", Source: "
                    + ReceiveProperties.AppId
                    + ", Id: "
                    + ReceiveProperties.MessageId
                    + " )");
            }

            // If received message needs a reply
            if (SendMessage != null)
            {
                IBasicProperties SendProperties = ChannelSend.CreateBasicProperties();
                SendProperties.AppId = this.GetType().ToString();
                SendProperties.ReplyTo = ReceiveProperties.ReplyTo;
                SendProperties.MessageId = ReceiveProperties.MessageId;

                // Send needed reply
                this.Send(SendProperties, SendMessage);
            }
            ChannelReceive.BasicAck(deliveryTag: Event.DeliveryTag, multiple: false);

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Received "
                + "( Message Type: "
                + ReceivedMessage.GetType()
                + ", Source: "
                + ReceiveProperties.AppId
                + ", Id: "
                + ReceiveProperties.MessageId
                + " )");
        }

        public virtual Boolean Filter(BasicDeliverEventArgs Event)
        {
            return false;
        }

        public void Send(Message newMessage)
        {
            IBasicProperties SendProperties = ChannelSend.CreateBasicProperties();
            SendProperties.AppId = this.GetType().ToString();
            SendProperties.MessageId = Guid.NewGuid().ToString();
            SendProperties.ReplyTo = this.ReceiveQueueName;
            SendProperties.CorrelationId = this.SendQueueName;

            this.Send(SendProperties, newMessage);
        }

        public void Send(IBasicProperties SendProperties, Message newMessage)
        {
            Byte[] SendMessage = SerializeFromType<Message>(newMessage);

            base.Send(SendProperties, SendMessage);
        }

        public abstract Message Receive(Message newMessage);
    }
}