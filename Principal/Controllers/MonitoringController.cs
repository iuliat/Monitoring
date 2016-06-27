using PrincipalAPI.Controllers;
using PrincipalAPI.Models;
using PrincipalAPI.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AdminLteMvc.Controllers
{
    /// <summary>
    /// This is an example controller using the AdminLTE NuGet package's CSHTML templates, CSS, and JavaScript
    /// You can delete these, or use them as handy references when building your own applications
    /// </summary>
    public class MonitoringController : System.Web.Mvc.Controller
    {
        /// <summary>
        /// The home page of the AdminLTE demo dashboard, recreated in this new system
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Notifications()
        {
            return View();
        }

        public ActionResult Overview()
        {
            return View();
        }

        public ActionResult VMs()
        {
            VMStorage storageOpps = new VMStorage();
            
            return View(storageOpps.GetAllVMs());
        }

        public ActionResult NewVM()
        {
            return View(new Host());
        }

        public ActionResult Post(Host newHost)
        {
            VMStorage storageOpps = new VMStorage();
            storageOpps.AddNewVM(newHost);

            return RedirectToAction("VMs");
            //this.Redirect("Monitoring/VMs");

        }

        /// <summary>
        /// The color page of the AdminLTE demo, demonstrating the AdminLte.Color enum and supporting methods
        /// </summary>
        /// <returns></returns>
        public ActionResult Colors()
        {
            return View();
        }

        public ActionResult Graphs()
        {
            //VMStorage storageOpps = new VMStorage();

            //return View(storageOpps.GetAllVMs());
            return View(new Host());
        }

        public ActionResult GraphsCPUs()
        {
            return View(new Host());
        }

        public ActionResult GraphsRAMs()
        {
            return View(new Host());
        }

        public JsonResult GetCPUsJSON(int hostid)
        {
            hostid = 2;
            PrincipalAPIContext db = new PrincipalAPIContext();
            var searchedHost = db.Hosts.Where(i => i.HostID == hostid).ToArray().First();
            return Json(db.CPUs.Where(i => i.MetricID == searchedHost.MetricID).Select(i => i.Value).ToList(), JsonRequestBehavior.AllowGet);
        }


        //public JsonResult GetCPUsJSON()
        //{
        //    PrincipalAPIContext db = new PrincipalAPIContext();
        //    return Json(db.CPUs.Where(i => i.MetricID == 1).Select(i => i.Value).ToList(), JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetRAMsJSON()
        {
            PrincipalAPIContext db = new PrincipalAPIContext();
            return Json(db.RAMs.Select(i => i.Value).ToList(), JsonRequestBehavior.AllowGet);
        }


    }
}