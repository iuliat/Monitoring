using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using ControllerAPI.AMQP;

[assembly: OwinStartup(typeof(ControllerAPI.Startup))]

namespace ControllerAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            Run application = new Run();
            application.RunProgram();
            try
            {
                var rpcClient = new AgentConsumer();

                Console.WriteLine(" [x] Requesting ram)");
                var asd = rpcClient.Call("ram");
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex.Message);
            }

            //      Console.WriteLine(" [.] Got '{0}'", response);
        }

    }
}
