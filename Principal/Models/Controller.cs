using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipalAPI.Models
{
    public class Controller
    {
        //[Key]
        //public int ControllerID { get; set; }

        [Key]
        public string ControllerIP { get; set; }
        public string ControllerCorrID { get; set; }
        public virtual ICollection<Host> hosts { get; set; }
    }
}
