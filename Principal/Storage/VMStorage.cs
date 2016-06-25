using PrincipalAPI.Controllers;
using PrincipalAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipalAPI.Storage
{
    class VMStorage
    {
        string currentControllerIP = "6.6.6.7";
        public void StoreMasterVM()
        {
            // Configure and add current controller to db
            ControllersController c = new ControllersController();
            if (!c.ControllerExists(currentControllerIP))
            {
                Controller currentController = new Controller();
                currentController.ControllerIP = currentControllerIP;
                // c.GetController(currentControllerIP);
                c.Post(currentController);
            }
        }


    }
}
