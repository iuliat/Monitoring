using PrincipalAPI.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PrincipalAPI.Models
{
    public class Host
    {

        [Key]
        public int HostID { get; set; }


        public string HostIP { get; set; }

        public string Hostname { get; set; }

        public virtual ICollection<Notifications> notifications { get; set; }

        [Required]
        public int MasterVMID { get; set; }

        public virtual MasterVM MasterVMs { get; set; }

        //    public List<string>  AllVMSids{
        //        get
        //        {
        //            return VMStorage.getAllMasterVMs().Select(vm => vm.MasterVMIP).ToList();

        //        }
        //}


        public static SelectList allMasters
        {
            get
            {
                return new SelectList(VMStorage.getAllMasterVMs().Select(i => new KeyValuePair<string, int>(i.MasterVMIP, i.MasterVMID)), "Value", "Key");

            }
            //new SelectList(
            //            Enum.GetValues(typeof(RequestStatus))
            //            .OfType<RequestStatus>()
            //            .Select(i => new KeyValuePair<string, RequestStatus>(i.ToString(), i)),
            //            "Value", "Key", RequestStatus.Pending)
        }

        public static SelectList allHosts
        {
            get
            {
                return new SelectList(VMStorage.getAllVMs().Select(i => new KeyValuePair<string, int>(i.HostIP, i.HostID)), "Value", "Key");

            }
            //new SelectList(
            //            Enum.GetValues(typeof(RequestStatus))
            //            .OfType<RequestStatus>()
            //            .Select(i => new KeyValuePair<string, RequestStatus>(i.ToString(), i)),
            //            "Value", "Key", RequestStatus.Pending)
        }

        public int MetricID { get; set; }
        public virtual Metrics Metrics { get; set; }

 
    }
}