﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerAPI.Models
{
    public class Controller
    {
        [Key]
        public int ControllerID { get; set; }

        [Required]
        public string IP { get; set; }

        public virtual ICollection<Host> hosts { get; set; }
    }
}
