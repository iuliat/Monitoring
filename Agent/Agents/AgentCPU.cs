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
        private PerformanceCounter CounterCPU;

        private Object CollectedCPULock = new Object();
        private List<Single> CollectedCPU = new List<Single>();

        public AgentCPU(String customUser, String customPassword, String customBrokerAddress) : base(customUser, customPassword, customBrokerAddress)
        {
            this.CounterCPU = new PerformanceCounter();

            this.CounterCPU.CategoryName = "Processor";
            this.CounterCPU.CounterName = "% Processor Time";
            this.CounterCPU.InstanceName = "_Total";
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
            lock (CollectedCPULock)
            {
                CollectedCPU.Add(this.getAvailableCPU());
            }
        }

        protected override void DataTransmit(Object sender, ElapsedEventArgs e)
        {
            MessageCPU newMessage;

            lock (CollectedCPULock)
            {
                newMessage = new MessageCPU(new List<Single>(CollectedCPU));
                CollectedCPU.Clear();
            }

            this.Send(newMessage);
        }
    }
}
