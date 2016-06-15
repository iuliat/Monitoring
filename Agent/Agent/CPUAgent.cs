using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent
{
    public class CPUAgent : AgentModel
    {
        public override Dictionary<string, string> getResources()
        {
            string availableCPU = this.getAvailableCPU();
            Dictionary<string, string> message = new Dictionary<string, string>();

            message.Add("availableCPU", availableCPU);


            return message;
        }

        public string getAvailableCPU()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter();

            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";

            return cpuCounter.NextValue() + "%";
        }
    }
}
