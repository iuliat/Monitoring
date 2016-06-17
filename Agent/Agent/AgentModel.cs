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

namespace Agent
{
    public abstract class AgentModel
    {
        public void start()
        {

            var server = "127.0.0.1";

            var factory = new ConnectionFactory()
            {
                HostName = server,
                UserName = "user",
                Password = "Passw0rd"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queue: "rpc_queue", noAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");

                while (true)
                {
                    string response = null;
                    Dictionary<string, string> response2 = null;
                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;
                    replyProps.ReplyTo = props.ReplyTo;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body);

                        Console.WriteLine(" [.] ram: {0}", message);
                        response2 = this.getResources();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.ToString());
                        response = "";
                    }
                    finally
                    {
                        var responseBytes = ObjectToByteArray(response2);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                }
            }
        }

        public byte[] ObjectToByteArray<T>(T obj)
        {
            MemoryStream m = new MemoryStream();
            if (obj != null)
            {
                BinaryFormatter b = new BinaryFormatter();

                b.Serialize(m, obj);
            }
            return m.ToArray();
        }

        public abstract Dictionary<string, string> getResources();

        /// <summary>
        /// Assumes only valid positive integer input.
        /// Don't expect this one to work for big numbers, and it's probably the slowest recursive implementation possible.
        /// </summary>
        private static int fib(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }

            return fib(n - 1) + fib(n - 2);
        }

        public static List<String> GetLocalIPAddress()
        {
            List<String> IPv4s = new List<String>();

            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                throw new Exception("No network connection is available.");
            }

            var host = Dns.GetHostEntry(Dns.GetHostName());
            // host.Hostname


            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPv4s.Add(ip.ToString());
                }
            }

            throw new Exception("Local IP Address Not Found!");
        }

    }
}
