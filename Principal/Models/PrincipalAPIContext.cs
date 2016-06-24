using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PrincipalAPI.Models
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class PrincipalAPIContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx


        public PrincipalAPIContext() : base("name=PrincipalAPIContext")
        {
        }

        public System.Data.Entity.DbSet<PrincipalAPI.Models.Host> Hosts { get; set; }
        public System.Data.Entity.DbSet<PrincipalAPI.Models.Controller> Controllers { get; set; }
        public System.Data.Entity.DbSet<PrincipalAPI.Models.CPU> CPUs { get; set; }
        public System.Data.Entity.DbSet<PrincipalAPI.Models.RAM> RAMs { get; set; }
        public System.Data.Entity.DbSet<PrincipalAPI.Models.Metrics> Metrics { get; set; }
    }
}
