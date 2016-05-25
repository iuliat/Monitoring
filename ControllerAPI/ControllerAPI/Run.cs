using ControllerAPI.AMQP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerAPI
{
    public class Run
    {
        public void RunProgram()
        {
            var rpcClient = new AMQPConnection();

            Console.WriteLine(" [x] Requesting fib(30)");
            var response = rpcClient.Call("30");
            Console.WriteLine(" [.] Got '{0}'", response);

            //rpcClient.Close();
        }
    }
}
