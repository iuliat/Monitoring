using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AgentAPI.AMQP;
using CoreAMQP.Messages;

namespace AgentAPI.Agents
{
    public class AgentRAM : Agent
    {
        private Boolean Initialized = false;
        private PerformanceCounter CounterRAM;

        private Object CollectedRAMLock = new Object();
        private List<Dictionary<String, Object>> CollectedRAM = new List<Dictionary<String, Object>>();

        public readonly String Category = "Memory | Available MBytes";
        public Int32 HostId { get; private set; }
        public Int32 upperLimit { get; private set; }
        public Int32 lowerLimit { get; private set; }

        public AgentRAM(String customUser, String customPassword, String customBrokerAddress) : base(customUser, customPassword, customBrokerAddress)
        {
            this.HostId = -1;
            this.upperLimit = 30000;
            this.lowerLimit = 0;

            this.CounterRAM = new PerformanceCounter("Memory", "Available MBytes");
        }

        public override void OpenSender()
        {
            base.OpenSender();

            Dictionary<String, Object> Payload = new Dictionary<String, Object>();
            Payload.Add("HostName", "127.0.0.1");
            Payload.Add("ReplyTo", this.ReceiveQueueName);

            MessageRAMInit RAMInit = new MessageRAMInit(Payload);

            this.Send(RAMInit);
        }

        public override Dictionary<String, String> getResources()
        {
            Dictionary<String, String> message = new Dictionary<String, String>();
            String availableRAM = this.getAvailableRAM() + "MB";

            message.Add("availableRAM", availableRAM);

            return message;
        }

        public Single getAvailableRAM()
        {
            return CounterRAM.NextValue();
        }

        protected override void DataCollect(Object sender, ElapsedEventArgs e)
        {
            if (this.Initialized == true)
            {
                Dictionary<String, Object> InstanceRAM = new Dictionary<String, Object>();
                InstanceRAM.Add("Value", this.getAvailableRAM());
                InstanceRAM.Add("Date", DateTime.Now);
                InstanceRAM.Add("Category", this.Category);
                InstanceRAM.Add("HostId", this.HostId);
                InstanceRAM.Add("upperLimit", this.upperLimit);
                InstanceRAM.Add("lowerLimit", this.lowerLimit);

                lock (CollectedRAMLock)
                {
                    CollectedRAM.Add(InstanceRAM);
                }
            }
        }

        protected override void DataTransmit(Object sender, ElapsedEventArgs e)
        {
            if (this.Initialized == true)
            {
                MessageRAM newMessage;

                lock (CollectedRAMLock)
                {
                    newMessage = new MessageRAM(new List<Dictionary<String, Object>>(CollectedRAM));
                    CollectedRAM.Clear();
                }

                this.Send(newMessage);
            }
        }

        public override Message Receive(MessageRAMInit newMessage)
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
