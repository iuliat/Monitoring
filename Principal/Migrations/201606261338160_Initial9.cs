namespace PrincipalAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial9 : DbMigration
    {
        public override void Up()
        {
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
                "dbo.Metrics",
                c => new
                    {
                        MetricID = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.MetricID);
            
            CreateTable(
                "dbo.Hosts",
                c => new
                    {
                        HostID = c.Int(nullable: false, identity: true),
                        HostIP = c.String(unicode: false),
                        Hostname = c.String(unicode: false),
                        MasterVMID = c.Int(nullable: false),
                        MetricID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.HostID)
                .ForeignKey("dbo.MasterVMs", t => t.MasterVMID, cascadeDelete: true)
                .ForeignKey("dbo.Metrics", t => t.MetricID, cascadeDelete: true)
                .Index(t => t.MasterVMID)
                .Index(t => t.MetricID);
            
            CreateTable(
                "dbo.MasterVMs",
                c => new
                    {
                        MasterVMID = c.Int(nullable: false, identity: true),
                        MasterVMIP = c.String(unicode: false),
                        Hostname = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.MasterVMID);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        NotificationID = c.Int(nullable: false, identity: true),
                        Content = c.String(unicode: false),
                        HostID = c.String(nullable: false, unicode: false),
                        Host_HostID = c.Int(),
                    })
                .PrimaryKey(t => t.NotificationID)
                .ForeignKey("dbo.Hosts", t => t.Host_HostID)
                .Index(t => t.Host_HostID);
            
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
            DropForeignKey("dbo.Notifications", "Host_HostID", "dbo.Hosts");
            DropForeignKey("dbo.Hosts", "MetricID", "dbo.Metrics");
            DropForeignKey("dbo.Hosts", "MasterVMID", "dbo.MasterVMs");
            DropForeignKey("dbo.CPUs", "MetricID", "dbo.Metrics");
            DropIndex("dbo.RAMs", new[] { "MetricID" });
            DropIndex("dbo.Notifications", new[] { "Host_HostID" });
            DropIndex("dbo.Hosts", new[] { "MetricID" });
            DropIndex("dbo.Hosts", new[] { "MasterVMID" });
            DropIndex("dbo.CPUs", new[] { "MetricID" });
            DropTable("dbo.RAMs");
            DropTable("dbo.Notifications");
            DropTable("dbo.MasterVMs");
            DropTable("dbo.Hosts");
            DropTable("dbo.Metrics");
            DropTable("dbo.CPUs");
        }
    }
}
