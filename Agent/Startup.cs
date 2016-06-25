using AgentAPI.Agents;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartup(typeof(AgentAPI.Startup))]

namespace AgentAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            try
            {
                var rpcClient = new AgentRAM("user", "Passw0rd", "192.168.71.50");
                // var rpcClient = new AgentRAM("user", "Passw0rd", "192.168.71.50");
                rpcClient.Open();

                // rpcClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex.Message);
            }
        }

    }
}
