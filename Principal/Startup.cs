using CoreAMQP.Messages;
using Microsoft.Owin;
using Owin;
using PrincipalAPI.Controllers;
using PrincipalAPI.Models;
using PrincipalAPI.Storage;
using System;
using System.Collections.Generic;

[assembly: OwinStartup(typeof(PrincipalAPI.Startup))]

namespace PrincipalAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            VMStorage storageOpps = new VMStorage();
            storageOpps.StoreMasterVM();

            try
            {
                var rpcClient = new PrincipalAPI.AMQP.Principal("user", "Passw0rd", "192.168.1.206");
                // var rpcClient = new PrincipalAPI.AMQP.Principal("user", "Passw0rd", "192.168.71.50");
                rpcClient.Open();

                Console.WriteLine(" [x] Requesting Fibonacci)");

                for (UInt64 Value = 0; Value < 1; Value++)
                {
                    rpcClient.Send(new MessageFibonacci(Value));
                   // Message Response = rpcClient.Messages.First.Value;
                }
                rpcClient.Send(new MessageIPV4(new List<String>()));
                rpcClient.Send(new EstablishedCommunicationMessage(false));
                // rpcClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex.Message);
            }

        }

    }
}
