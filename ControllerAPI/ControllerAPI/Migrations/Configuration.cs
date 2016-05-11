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
            //
            context.Hosts.AddOrUpdate(new Host[] {
                new Host() { ID = 1, Name = "Hat" },
                new Host() { ID = 2, Name = "Socks" },
                new Host() { ID = 3, Name = "Scarf" },
                new Host() { ID = 4, Name = "Yo-yo" },
                new Host() { ID = 5, Name = "Puzzle" },
            });
        }
    }
}
