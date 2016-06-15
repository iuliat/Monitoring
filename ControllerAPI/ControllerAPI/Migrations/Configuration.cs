namespace ControllerAPI.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ControllerAPI.Models.ControllerAPIContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

            // register mysql code generator
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
        }

        protected override void Seed(ControllerAPI.Models.ControllerAPIContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            ////
            context.Hosts.AddOrUpdate(new Host[] {
                new Host() {  IP = "192.168.199.10" , ControllerID = 1, MetricID=1},
                new Host() {  IP = "192.168.199.22" , ControllerID = 1, MetricID=2},

            });

            context.Controllers.AddOrUpdate(new Controller[] {
                new Controller() {ControllerID = 1, IP = "127.0.0.1"  },
                new Controller() { ControllerID = 2, IP = "127.0.0.1"  },

            });

            context.Metrics.AddOrUpdate(new Metrics[] {
                new Metrics() {  MetricID=1},
                new Metrics() {  MetricID=2},

            });

            //context.Metrics.AddOrUpdate(new Metrics[] {
            //    new Metrics() {CPURefId = '1', RAMRefId='1'  },
            //});

            //context.CPUs.AddOrUpdate(new CPU[] {
            //    new CPU() { Value=123  },
            //});

            //context.RAMs.AddOrUpdate(new RAM[] {
            //    new RAM() { Value=13  },
            //});



        }
    }
}
