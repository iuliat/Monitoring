using CoreAMQP.Messages;
using Microsoft.Owin;
using Owin;
using PrincipalAPI.AMQP;
using PrincipalAPI.Storage;
using System;
using System.Collections.Generic;
using System.Web.Hosting;

[assembly: OwinStartup(typeof(PrincipalAPI.Startup))]

namespace PrincipalAPI
{
    public partial class Startup
    {
        //private static VMStorage Database = null;

        private static Object PrincipalLock = new Object();
        private static Principal Principal = null;

        private static void CreatePrincipal()
        {
            lock (PrincipalLock)
            {
                DestroyPrincipal();

                Principal = new Principal("user", "Passw0rd", "192.168.1.206");
                Principal.ChangeReceiveQueueName(typeof(Principal).ToString());
            }
        }

        private static void DestroyPrincipal()
        {
            lock (PrincipalLock)
            {
                if (Principal != null)
                {
                    StopPrincipal();

                    Principal = null;
                }
            }
        }

        private static void StartPrincipal()
        {
            lock (PrincipalLock)
            {
                if (Principal != null)
                {
                    Principal.Open();
                    Principal.CloseSender();
                }
            }
        }

        private static void StopPrincipal()
        {
            lock (PrincipalLock)
            {
                if (Principal != null)
                {
                    Principal.Close();
                }
            }
        }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            //VMStorage storageOpps = new VMStorage();
            //storageOpps.StoreMasterVM();

            try
            {
                Startup.CreatePrincipal();
                Startup.StartPrincipal();

                AppDomain.CurrentDomain.SetData("Principal", Principal);

                /*
                Console.WriteLine(" [x] Requesting Communication Established Information");
                Principal.Send(new EstablishedCommunicationMessage(false));

                Console.WriteLine(" [x] Requesting IPs (V4)");
                Principal.Send(new MessageIPV4(new List<String>()));

                for (UInt64 Value = 0; Value < 10; Value++)
                {
                    Console.WriteLine(" [x] Requesting Fibonacci ({0})", Value);
                    Principal.Send(new MessageFibonacci(Value));
                }
                */
            }
            catch (Exception ex)
            {
                Startup.DestroyPrincipal();
                Console.WriteLine("error:" + ex.Message);
            }
        }

        public Startup()
        {
            System.Diagnostics.Debug.WriteLine("PrincipalAPI Init Start");
        }

        ~Startup()
        {
            System.Diagnostics.Debug.WriteLine("PrincipalAPI Init Stop");
        }
    }
}
