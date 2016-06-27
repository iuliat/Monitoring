using AgentAPI.Agents;
using AgentAPI.AMQP;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartup(typeof(AgentAPI.Startup))]

namespace AgentAPI
{
    public partial class Startup
    {
        private static Object AgentsLock = new Object();
        private static Agent[] Agents = null;

        private static void CreateAgents()
        {
            lock (AgentsLock)
            {
                DestroyAgents();

                Agents = new Agent[2];
                Agents[0] = new AgentRAM("user", "Passw0rd", "192.168.1.206");
                Agents[1] = new AgentCPU("user", "Passw0rd", "192.168.1.206");
                //Agents[0] = new AgentRAM("user", "Passw0rd", "192.168.71.50");
                //Agents[1] = new AgentCPU("user", "Passw0rd", "192.168.71.50");

                Agents[0].ChangeReceiveQueueName(typeof(AgentRAM).ToString() + "|" + Guid.NewGuid());
                Agents[0].ChangeSendQueueName("PrincipalAPI.AMQP.Principal");

                Agents[1].ChangeReceiveQueueName(typeof(AgentCPU).ToString() + "|" + Guid.NewGuid());
                Agents[1].ChangeSendQueueName("PrincipalAPI.AMQP.Principal");
            }
        }

        private static void DestroyAgents()
        {
            lock (AgentsLock)
            {
                if (Agents != null)
                {
                    StopAgents();

                    Agents = null;
                }
            }
        }

        private static void StartAgents()
        {
            lock (AgentsLock)
            {
                if (Agents != null)
                {
                    foreach (Agent thisAgent in Agents)
                    {
                        thisAgent.Open();
                    }
                }
            }
        }

        private static void StopAgents()
        {
            lock (AgentsLock)
            {
                if (Agents != null)
                {
                    foreach (Agent thisAgent in Agents)
                    {
                        thisAgent.Close();
                    }
                }
            }
        }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            try
            {
                Startup.CreateAgents();
                Startup.StartAgents();

                AppDomain.CurrentDomain.SetData("Agents", Agents);
            }
            catch (Exception ex)
            {
                Startup.DestroyAgents();
                Console.WriteLine("error:" + ex.Message);
            }
        }

        public Startup()
        {
            System.Diagnostics.Debug.WriteLine("AgentAPI Init Start");
        }

        ~Startup()
        {
            System.Diagnostics.Debug.WriteLine("AgentAPI Init Stop");
        }
    }
}
