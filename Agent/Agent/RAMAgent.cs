using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent
{
    public class RAMAgent: AgentModel
    {
        public override Dictionary<string, string> getResources()
        {
            String availableRAM = this.getAvailableRAM();

            Dictionary<string, string> message = new Dictionary<string, string>();

            message.Add("availableCPU", availableRAM);

            return message;
        }

        public String getAvailableRAM()
        {
            PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            return ramCounter.NextValue() + "MB";
        }
    }
}
