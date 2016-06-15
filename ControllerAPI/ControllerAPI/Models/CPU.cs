using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControllerAPI.Models
{
    public class CPU
    {
        [Key]
        public int CPUID { get; set; }
        public int Value { get; set; }
        public DateTime Date { get; set; }

        [Required]
        public int MetricID { get; set; }
        public virtual Metrics Metrics { get; set; }

    }
}