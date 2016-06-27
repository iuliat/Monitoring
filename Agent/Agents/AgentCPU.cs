using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgentAPI.AMQP;
using System.Timers;
using CoreAMQP.Messages;

namespace AgentAPI.Agents
{
    public class AgentCPU : Agent
    {
        private Boolean Initialized = false;
        private PerformanceCounter CounterCPU;

        private Object CollectedCPULock = new Object();
        private List<Dictionary<String, Object>> CollectedCPU = new List<Dictionary<String, Object>>();

        public readonly String Category = "Processor | % Processor Time | _Total";
        public Int32 HostId { get; private set; }
        public Int32 upperLimit { get; private set; }
        public Int32 lowerLimit { get; private set; }

        public AgentCPU(String customUser, String customPassword, String customBrokerAddress) : base(customUser, customPassword, customBrokerAddress)
        {
            this.HostId = -1;
            this.upperLimit = 100;
            this.lowerLimit = 0;

            this.CounterCPU = new PerformanceCounter();

            this.CounterCPU.CategoryName = "Processor";
            this.CounterCPU.CounterName = "% Processor Time";
            this.CounterCPU.InstanceName = "_Total";
        }

        public override void OpenSender()
        {
            base.OpenSender();

            Dictionary<String, Object> Payload = new Dictionary<String, Object>();
            Payload.Add("HostName", "127.0.0.1");
            Payload.Add("ReplyTo", this.ReceiveQueueName);

            MessageCPUInit CPUInit = new MessageCPUInit(Payload);

            this.Send(CPUInit);
        }

        public override Dictionary<String, String> getResources()
        {
            Dictionary<String, String> message = new Dictionary<String, String>();
            String availableCPU = this.getAvailableCPU() + "%";

            message.Add("availableCPU", availableCPU);

            return message;
        }

        public Single getAvailableCPU()
        {
            return CounterCPU.NextValue();
        }

        protected override void DataCollect(Object sender, ElapsedEventArgs e)
        {
            if (this.Initialized == true)
            {

                Dictionary<String, Object> InstanceCPU = new Dictionary<String, Object>();
                InstanceCPU.Add("Value", this.getAvailableCPU());
                InstanceCPU.Add("Date", DateTime.Now);
                InstanceCPU.Add("Category", this.Category);
                InstanceCPU.Add("HostId", this.HostId);
                InstanceCPU.Add("upperLimit", this.upperLimit);
                InstanceCPU.Add("lowerLimit", this.lowerLimit);

                lock (CollectedCPULock)
                {
                    CollectedCPU.Add(InstanceCPU);
                }
            }
        }

        protected override void DataTransmit(Object sender, ElapsedEventArgs e)
        {
            if (this.Initialized == true)
            {
                MessageCPU newMessage;

                lock (CollectedCPULock)
                {
                    newMessage = new MessageCPU(new List<Dictionary<String, Object>>(CollectedCPU));
                    CollectedCPU.Clear();
                }

                this.Send(newMessage);
            }
        }

        public override Message Receive(MessageCPUInit newMessage)
        {
            Dictionary<String, Object> Payload = newMessage.Payload as Dictionary<String, Object>;
            Int32 newHostId = Convert.ToInt32(Payload["HostId"]);
            this.upperLimit = Convert.ToInt32(Payload["upperLimit"]);
            this.lowerLimit = Convert.ToInt32(Payload["lowerLimit"]);

            if (newHostId != this.HostId)
            {
                this.HostId = newHostId;
                this.ChangeReceiveQueueName(this.GetType() + "|" + this.HostId);
                this.Initialized = true;
            }

            return null;
        }
    }
}
