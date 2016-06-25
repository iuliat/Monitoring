using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CoreAMQP.Messages
{
    [Serializable]
    public class EstablishedCommunicationMessage : Message
    {
        public EstablishedCommunicationMessage(Boolean Payload) : base(typeof(EstablishedCommunicationMessage), Payload)
        {
        }

        public static Boolean IsValidCommunication()
        {
            return true;
        }

    }
}
