using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipalAPI.Models
{
    public class MasterVM
    {
        [Key]
        public int MasterVMID { get; set; }

        public string MasterVMIP { get; set; }
        public string Hostname { get; set; }
        public virtual ICollection<Host> hosts { get; set; }
    }
}
