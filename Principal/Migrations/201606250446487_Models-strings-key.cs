namespace PrincipalAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modelsstringskey : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Controllers",
                c => new
                    {
                        ControllerIP = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        ControllerCorrID = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.ControllerIP);
            
            CreateTable(
                "dbo.Hosts",
                c => new
                    {
                        HostIP = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        HostCorrID = c.String(unicode: false),
                        ControllerIP = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        MetricID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.HostIP)
                .ForeignKey("dbo.Controllers", t => t.ControllerIP, cascadeDelete: true)
                .ForeignKey("dbo.Metrics", t => t.MetricID, cascadeDelete: true)
                .Index(t => t.ControllerIP)
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
            DropForeignKey("dbo.Hosts", "ControllerIP", "dbo.Controllers");
            DropIndex("dbo.RAMs", new[] { "MetricID" });
            DropIndex("dbo.CPUs", new[] { "MetricID" });
            DropIndex("dbo.Hosts", new[] { "MetricID" });
            DropIndex("dbo.Hosts", new[] { "ControllerIP" });
            DropTable("dbo.RAMs");
            DropTable("dbo.CPUs");
            DropTable("dbo.Metrics");
            DropTable("dbo.Hosts");
            DropTable("dbo.Controllers");
        }
    }
}
