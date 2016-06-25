using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipalAPI.Models
{
    public class Host
    {

        //[Key]
        //public int HostID { get; set; }

        [Key]
        public string HostIP { get; set; }

        public string HostCorrID { get; set; }
        //[ForeignKey("MetricsRefID")]
        //public int MetricsRefID { get; set; }

        [Required]
        public string ControllerIP { get; set; }
        public virtual Controller Controller { get; set; }

        [Required]
        public int MetricID { get; set; }
        public virtual Metrics Metrics { get; set; }
    }
}