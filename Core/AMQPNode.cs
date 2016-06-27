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
    public abstract class AMQPNode
    {
        public const String DefaultReceiveQueueName = "AMQP Receive Queue";
        public const String DefaultSendQueueName = "AMQP Send Queue";

        public String BrokerAddress { get; private set; }
        public String User { get; private set; }
        protected String Password { get; private set; }
        public String ReceiveQueueName { get; private set; }
        public String SendQueueName { get; private set; }

        protected IConnection BrokerConnection = null;
        protected IModel ChannelSend = null;
        protected IModel ChannelReceive = null;
        protected EventingBasicConsumer ConsumerReceive = null;
        protected String TagReceive = null;

        #region Helpers
        public static byte[] SerializeFromType<T>(T Object)
        {
            MemoryStream ByteBuffer = new MemoryStream();
            if (Object != null)
            {
                BinaryFormatter Binary = new BinaryFormatter();

                Binary.Serialize(ByteBuffer, Object);
            }
            return ByteBuffer.ToArray();
        }

        public static T DeserializeToType<T>(byte[] ByteArray)
        {
            MemoryStream ByteBuffer = new MemoryStream(ByteArray);
            if ((ByteArray == null) || (ByteArray.Length < 1))
            {
                return default(T);
            }

            BinaryFormatter Binary = new BinaryFormatter();

            T DeserializedObject = (T)Binary.Deserialize(ByteBuffer);

            return DeserializedObject;
        }
        #endregion

        #region Constructors
        public AMQPNode() : this("user", "password", "127.0.0.1")
        {
        }

        public AMQPNode(String customUser, String customPassword) : this(customUser, customPassword, "127.0.0.1")
        {
        }

        public AMQPNode(String customBrokerAddress) : this("user", "password", customBrokerAddress)
        {
        }

        public AMQPNode(String customUser, String customPassword, String customBrokerAddress, String ReceiveQueueName = DefaultReceiveQueueName, String SendQueueName = DefaultSendQueueName)
        {
            IPAddress[] BrokerIPAddress;

            BrokerIPAddress = Dns.GetHostAddresses(customBrokerAddress);

            if (BrokerIPAddress == null || BrokerIPAddress.Length < 1)
                throw new InvalidDataException("The provided address (" + customBrokerAddress + ") is invalid");

            this.BrokerAddress = customBrokerAddress;
            this.User = customUser ?? "";
            this.Password = customPassword ?? "";
            this.ReceiveQueueName = ReceiveQueueName;
            this.SendQueueName = SendQueueName;

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Created "
                + "( Address: "
                + this.BrokerAddress
                + ", User: "
                + this.User
                + " )");
        }
        #endregion

        #region Destructors
        ~AMQPNode()
        {
            this.Close();

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Destroyed "
                + "( "
                + " )");
        }
        #endregion

        public virtual void Open()
        {
            IConnectionFactory AMQP = new ConnectionFactory()
            {
                HostName = this.BrokerAddress,
                UserName = this.User,
                Password = this.Password
            };

            this.BrokerConnection = AMQP.CreateConnection();
            this.OpenReceiver();
            this.OpenSender();
        }

        public virtual void OpenReceiver()
        {
            // Create receive queue if it does not exist
            this.ChannelReceive = BrokerConnection.CreateModel();
            this.ChannelReceive.QueueDeclare(
                queue: this.ReceiveQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            this.ChannelReceive.BasicQos(0, 1, false);
            ConsumerReceive = new EventingBasicConsumer(ChannelReceive);
            ConsumerReceive.Received += ReceiveDispatch;

            // Create a consumer of the queue
            this.TagReceive = ChannelReceive.BasicConsume(queue: this.ReceiveQueueName, noAck: false, consumer: this.ConsumerReceive);

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Opened Receiver "
                + "( Address: "
                + this.BrokerAddress
                + ", User: "
                + this.User
                + ", Receive Queue: "
                + this.ReceiveQueueName
                + " )");
        }

        public virtual void OpenSender()
        {
            // Create send queue if it does not exist
            this.ChannelSend = BrokerConnection.CreateModel();
            this.ChannelSend.QueueDeclare(
                queue: this.SendQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Opened Sender "
                + "( Address: "
                + this.BrokerAddress
                + ", User: "
                + this.User
                + ", Send Queue: "
                + this.SendQueueName
                + " )");
        }

        public void Close()
        {
            try
            {
                this.CloseReceiver();
                this.CloseSender();
                this.BrokerConnection.Close();

                BrokerConnection = null;

                System.Diagnostics.Debug.WriteLine("[ "
                    + this.GetType()
                    + " ] Closed "
                    + "( Address: "
                    + this.BrokerAddress
                    + ", User: "
                    + this.User
                    + " )");
            }
            catch (Exception /*Ignored*/)
            {

            }
        }

        public void CloseReceiver()
        {
            try
            {
                ChannelReceive.BasicCancel(TagReceive);
                TagReceive = null;
                ConsumerReceive = null;
                ChannelReceive.Close();
                ChannelReceive = null;

                System.Diagnostics.Debug.WriteLine("[ "
                    + this.GetType()
                    + " ] Closed Receiver "
                    + "( Address: "
                    + this.BrokerAddress
                    + ", User: "
                    + this.User
                    + ", Receive Queue: "
                    + this.ReceiveQueueName
                    + " )");
            }
            catch (Exception /*Ignored*/)
            {

            }
        }

        public void CloseSender()
        {
            try
            {
                ChannelSend.Close();
                ChannelSend = null;

                System.Diagnostics.Debug.WriteLine("[ "
                    + this.GetType()
                    + " ] Closed Sender "
                    + "( Address: "
                    + this.BrokerAddress
                    + ", User: "
                    + this.User
                    + ", Send Queue: "
                    + this.SendQueueName
                    + " )");
            }
            catch (Exception /*Ignored*/)
            {

            }
        }

        public void ChangeReceiveQueueName(String newName)
        {
            if (this.ChannelReceive != null)
            {
                this.CloseReceiver();
                this.ReceiveQueueName = newName;
                this.OpenReceiver();
            }
            else
            {
                this.ReceiveQueueName = newName;
            }
        }

        public void ChangeSendQueueName(String newName)
        {
            if (this.ChannelSend != null)
            {
                this.CloseSender();
                this.SendQueueName = newName;
                this.OpenSender();
            }
            else
            {
                this.SendQueueName = newName;
            }
        }

        protected abstract void ReceiveDispatch(object Sender, BasicDeliverEventArgs Event);

        public void Send(Byte[] newMessage)
        {
            IBasicProperties SendProperties = ChannelSend.CreateBasicProperties();
            SendProperties.AppId = this.GetType().ToString();
            SendProperties.MessageId = Guid.NewGuid().ToString();
            SendProperties.ReplyTo = this.ReceiveQueueName;
            SendProperties.CorrelationId = this.SendQueueName;

            this.Send(SendProperties, newMessage);
        }

        public void Send(IBasicProperties SendProperties, Byte[] newMessage)
        {
            Byte[] SendMessage = newMessage;
            String Destination = SendProperties.CorrelationId;

            ChannelSend.BasicPublish(exchange: "", routingKey: Destination, basicProperties: SendProperties, body: SendMessage);

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Sent "
                + "( Message Type: "
                + SendMessage.GetType()
                + ", Destination: "
                + Destination
                + ", Id: "
                + SendProperties.MessageId
                + " )");
        }
    }
}