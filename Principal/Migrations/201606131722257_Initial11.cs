namespace PrincipalAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Controllers",
                c => new
                    {
                        ControllerID = c.Int(nullable: false, identity: true),
                        IP = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.ControllerID);
            
            CreateTable(
                "dbo.Hosts",
                c => new
                    {
                        HostID = c.Int(nullable: false, identity: true),
                        IP = c.String(nullable: false, unicode: false),
                        ControllerID = c.Int(nullable: false),
                        MetricID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.HostID)
                .ForeignKey("dbo.Controllers", t => t.ControllerID, cascadeDelete: true)
                .ForeignKey("dbo.Metrics", t => t.MetricID, cascadeDelete: true)
                .Index(t => t.ControllerID)
                .Index(t => t.MetricID);
            
            CreateTable(
                "dbo.Metrics",
                c => new
                    {
                        MetricID = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.MetricID);
            
            CreateTable(
                "dbo.CPUs",
                c => new
                    {
                        CPUID = c.Int(nullable: false, identity: true),
                        Value = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        MetricID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CPUID)
                .ForeignKey("dbo.Metrics", t => t.MetricID, cascadeDelete: true)
                .Index(t => t.MetricID);
            
            CreateTable(
                "dbo.RAMs",
                c => new
                    {
                        RAMID = c.Int(nullable: false, identity: true),
                        Value = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        MetricID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RAMID)
                .ForeignKey("dbo.Metrics", t => t.MetricID, cascadeDelete: true)
                .Index(t => t.MetricID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RAMs", "MetricID", "dbo.Metrics");
            DropForeignKey("dbo.Hosts", "MetricID", "dbo.Metrics");
            DropForeignKey("dbo.CPUs", "MetricID", "dbo.Metrics");
            DropForeignKey("dbo.Hosts", "ControllerID", "dbo.Controllers");
            DropIndex("dbo.RAMs", new[] { "MetricID" });
            DropIndex("dbo.CPUs", new[] { "MetricID" });
            DropIndex("dbo.Hosts", new[] { "MetricID" });
            DropIndex("dbo.Hosts", new[] { "ControllerID" });
            DropTable("dbo.RAMs");
            DropTable("dbo.CPUs");
            DropTable("dbo.Metrics");
            DropTable("dbo.Hosts");
            DropTable("dbo.Controllers");
        }
    }
}
