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
        private PerformanceCounter CounterRAM;

        private Object CollectedRAMLock = new Object();
        private List<Single> CollectedRAM = new List<Single>();

        public AgentRAM(String customUser, String customPassword, String customBrokerAddress) : base(customUser, customPassword, customBrokerAddress)
        {
            this.CounterRAM = new PerformanceCounter("Memory", "Available MBytes");
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
            lock (CollectedRAMLock)
            {
                CollectedRAM.Add(this.getAvailableRAM());
            }
        }

        protected override void DataTransmit(Object sender, ElapsedEventArgs e)
        {
            MessageRAM newMessage;

            lock (CollectedRAMLock)
            {
                newMessage = new MessageRAM(new List<Single>(CollectedRAM));
                CollectedRAM.Clear();
            }

            this.Send(newMessage);
        }
    }
}
