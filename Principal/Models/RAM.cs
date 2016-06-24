using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PrincipalAPI.Models
{
    public class RAM
    {
        [Key]
        public int RAMID { get; set; }
        public int Value { get; set; }
        public DateTime Date { get; set; }

        [Required]
        public int MetricID { get; set; }
        public virtual Metrics Metrics { get; set; }
    }
}