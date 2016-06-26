using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrincipalAPI.Models
{
    public class Notifications
    {

        [Key]
        public int NotificationID { get; set; }


        public string Content { get; set; }


        [Required]
        public string HostID { get; set; }
        public virtual Host Host { get; set; }

    }
}