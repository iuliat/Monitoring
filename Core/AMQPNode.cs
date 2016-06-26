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
        public static readonly String DefaultQueueName = "VMMonitorQueue6";

        public String BrokerAddress { get; private set; }
        public String User { get; private set; }
        protected string Password { get; private set; }

        protected IConnection BrokerConnection;
        protected IModel Channel;
        protected EventingBasicConsumer Consumer;
        protected string replyQueueName;

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

        public AMQPNode(String customUser, String customPassword, String customBrokerAddress)
        {
            IPAddress[] BrokerIPAddress;

            BrokerIPAddress = Dns.GetHostAddresses(customBrokerAddress);

            if (BrokerIPAddress == null || BrokerIPAddress.Length < 1)
                throw new InvalidDataException("The provided address (" + customBrokerAddress + ") is invalid");

            this.BrokerAddress = customBrokerAddress;
            this.User = customUser ?? "";
            this.Password = customPassword ?? "";

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

        public virtual void Open()
        {
            IConnectionFactory AMQP = new ConnectionFactory()
            {
                HostName = this.BrokerAddress,
                UserName = this.User,
                Password = this.Password
            };

            this.BrokerConnection = AMQP.CreateConnection();
            this.Channel = BrokerConnection.CreateModel();

            // Create queue if it does not exist
            this.Channel.QueueDeclare(
                queue: AMQPNode.DefaultQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            this.Channel.BasicQos(0, 1, false);
            Consumer = new EventingBasicConsumer(Channel);
            Consumer.Received += ReceiveDispatch;

            // Create a consumer of the queue
            Channel.BasicConsume(queue: AMQPNode.DefaultQueueName, noAck: false, consumer: Consumer);

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Opened "
                + "( Address: "
                + this.BrokerAddress
                + ", User: "
                + this.User
                + " )");
        }

        public void Close()
        {
            this.BrokerConnection.Close();

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Closed "
                + "( Address: "
                + this.BrokerAddress
                + ", User: "
                + this.User
                + " )");
        }

        protected abstract void ReceiveDispatch(object Sender, BasicDeliverEventArgs Event);

        public void Send(Byte[] newMessage)
        {
            IBasicProperties SendProperties = Channel.CreateBasicProperties();
            SendProperties.ReplyTo = AMQPNode.DefaultQueueName;
            SendProperties.CorrelationId = Guid.NewGuid().ToString();

            this.Send(SendProperties, newMessage);
        }

        public void Send(IBasicProperties SendProperties, Byte[] newMessage)
        {
            Byte[] SendMessage = newMessage;
            String Destination = SendProperties.ReplyTo ?? AMQPNode.DefaultQueueName;

            Channel.BasicPublish(exchange: "", routingKey: Destination, basicProperties: SendProperties, body: SendMessage);

            System.Diagnostics.Debug.WriteLine("[ "
                + this.GetType()
                + " ] Sent "
                + "( Message Type: "
                + SendMessage.GetType()
                + ", Destination: "
                + Destination
                + " )");
        }
    }
}