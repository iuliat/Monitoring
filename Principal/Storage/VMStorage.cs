using PrincipalAPI.Controllers;
using PrincipalAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipalAPI.Storage
{
    public class VMStorage
    {
  
        string currentControllerIP = "6.6.6.89";
        public void StoreMasterVM()
        {
            // Configure and add current controller to db
            MasterVMsController c = new MasterVMsController();
            if (!c.MasterVMExistsIP(currentControllerIP))
            {
                MasterVM currentController = new MasterVM();
                currentController.MasterVMIP = currentControllerIP;
                // c.GetController(currentControllerIP);
                c.Post(currentController);
            }
        }

        public static List<MasterVM> getAllMasterVMs() {
            MasterVMsController c = new MasterVMsController();
            return c.GetMasterVMs().ToList();
        }

        public static List<Host> getAllVMs()
        {
            HostsController c = new HostsController();
            return c.GetHosts().ToList();
        }

        public List<Host> GetAllVMs()
        {
            // Configure and add current controller to db

            HostsController h = new HostsController();
            List<Host> vms = h.GetHosts().ToList();
            return vms;

        }

        public void AddNewVM(Host newHost)
        {
            HostsController h = new HostsController();

            newHost.MetricID = this.AddNewMetric().MetricID;
            var createdHost = h.Post(newHost);
        }

        public Metrics AddNewMetric()
        {
            MetricsController m = new MetricsController();
            Metrics newMetric = new Metrics();
            m.Post(newMetric);
            return newMetric;
        }

        public Host GetHostByHostname(string hostname)
        {
            HostsController h = new HostsController();

            return h.GetHostByHostname(hostname).Queryable.First();
        }

        public void AddRAM(int HostID, RAM newRAM)
        {
            RAMsController r = new RAMsController();

            newRAM.MetricID =  new HostsController().GetHost(HostID).Queryable.First().MetricID;
            r.Post(newRAM);
        }

        public void AddCPU(int HostID, CPU newCPU)
        {
            CPUsController r = new CPUsController();

            newCPU.MetricID = new HostsController().GetHost(HostID).Queryable.First().MetricID;
            r.Post(newCPU);
        }
    }
}
