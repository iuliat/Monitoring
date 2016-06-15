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

        //public object ByteArrayToObject(byte[] _ByteArray)
        //{
        //    try
        //    {
        //        // convert byte array to memory stream
        //        System.IO.MemoryStream _MemoryStream = new System.IO.MemoryStream(_ByteArray);

        //        // create new BinaryFormatter
        //        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _BinaryFormatter
        //                    = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        //        // set memory stream position to starting point
        //        _MemoryStream.Position = 0;

        //        // Deserializes a stream into an object graph and return as a object.
        //        return _BinaryFormatter.Deserialize(_MemoryStream);
        //    }
        //    catch (Exception _Exception)
        //    {
        //        // Error
        //        Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
        //    }

        //    // Error occured, return null
        //    return null;
        //}

        public T ByteArrayToObject<T>(byte[] arrBytes)
        {
            if (arrBytes == null || arrBytes.Length < 1)
                return default(T);

            BinaryFormatter binForm = new BinaryFormatter();

            T obj = (T)binForm.Deserialize(new MemoryStream(arrBytes));

            return obj;
        }

        //private Object ByteArrayToObject(byte[] arrBytes)

        //{

        //    MemoryStream memStream = new MemoryStream();

        //    BinaryFormatter binForm = new BinaryFormatter();

        //    memStream.Write(arrBytes, 0, arrBytes.Length);

        //    memStream.Seek(0, SeekOrigin.Begin);


        //    Object obj = (Object)binForm.Deserialize(memStream);

        //    return obj;

        //}
        //public  byte[] ObjectToByteArray<T>(T obj)
        //{
        //    MemoryStream m = new MemoryStream();
        //    if (obj != null)
        //    {
        //        BinaryFormatter b = new BinaryFormatter();

        //        b.Serialize(m, obj);
        //    }
        //    return m.ToArray();
        //}
        //public T ByteArrayToObject<T>(byte[] arrBytes)
        //{
        //    if (arrBytes == null || arrBytes.Length < 1)
        //        return default(T);

        //    BinaryFormatter binForm = new BinaryFormatter();

        //    T obj = (T) binForm.Deserialize(new MemoryStream(arrBytes));

        //    return obj;
        //}

        //public Dictionary<string, string> MyMethod(object obj)
        //{
        //    Dictionary<string, string> newDict = null;
        //    if (typeof(IDictionary).IsAssignableFrom(obj.GetType()))
        //    {
        //        IDictionary idict = (IDictionary)obj;

        //        newDict = new Dictionary<string, string>();
        //        foreach (object key in idict.Keys)
        //        {
        //            newDict.Add(key.ToString(), idict[key].ToString());
        //        }
        //    }

        //    return newDict;
        //}
        public void Close()
        {
            connection.Close();
        }

    }
}
