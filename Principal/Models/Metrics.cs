using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipalAPI.Models
{
    public class Metrics
    {
        [Key]
        public int MetricID { get; set; }

        public virtual ICollection<CPU> CPUs { get; set; }
        public virtual ICollection<RAM> RAMs { get; set; }

        public virtual ICollection<Host> hosts { get; set; }


    }
}