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

namespace ControllerAPI.AMQP
{
    public class AgentConsumer
    {

        private IConnection connection;
        private IModel channel;
        private string replyQueueName;
        private QueueingBasicConsumer consumer;

        public AgentConsumer()

        { 
            var server = "127.0.0.1";
            var factory = new ConnectionFactory()
            {
                HostName = server,
                UserName = "user",
                Password = "Passw0rd",
                VirtualHost = "/"
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(queue: replyQueueName, noAck: true, consumer: consumer);

        }

        public Dictionary<string,string> Call(string message)
        {
            var corrId = Guid.NewGuid().ToString();
            var props = channel.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            props.CorrelationId = corrId;

            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: "rpc_queue", basicProperties: props, body: messageBytes);

            while (true)
            {
                var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                if (ea.BasicProperties.CorrelationId == corrId)
                {
                    var receivedMessage = ByteArrayToObject<Dictionary<string, string>>(ea.Body);
                    return receivedMessage;

                }
            }

        }

        public T ByteArrayToObject<T>(byte[] arrBytes)
        {
            if (arrBytes == null || arrBytes.Length < 1)
                return default(T);

            BinaryFormatter binForm = new BinaryFormatter();

            T obj = (T)binForm.Deserialize(new MemoryStream(arrBytes));

            return obj;
        }

        
        public void Close()
        {
            connection.Close();
        }

    }
}
