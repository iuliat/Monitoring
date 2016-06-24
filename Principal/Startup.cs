﻿using CoreAMQP.Messages;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartup(typeof(PrincipalAPI.Startup))]

namespace PrincipalAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            try
            {
                var rpcClient = new PrincipalAPI.AMQP.Principal("user", "Passw0rd", "192.168.1.206");
                // var rpcClient = new PrincipalAPI.AMQP.Principal("user", "Passw0rd", "192.168.71.50");
                rpcClient.Open();

                Console.WriteLine(" [x] Requesting Fibonacci)");

                for (UInt64 Value = 0; Value < 1; Value++)
                {
                    rpcClient.Send(new MessageFibonacci(Value));
                    Message Response = rpcClient.Messages.First.Value;
                }

                // rpcClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("error:" + ex.Message);
            }

        }

    }
}