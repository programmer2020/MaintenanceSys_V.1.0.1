namespace maintenanceApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _intiate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MaintenanceCustomerCityModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerCity = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MaintenanceModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Customer_Name = c.String(nullable: false, maxLength: 250),
                        Customer_Phone_1 = c.String(nullable: false, maxLength: 250),
                        Customer_Phone_2 = c.String(nullable: false, maxLength: 250),
                        Customer_Adress = c.String(nullable: false, maxLength: 250),
                        Device_SerialNumber = c.String(nullable: false, maxLength: 250),
                        Device_Model = c.String(nullable: false, maxLength: 250),
                        Accrssories = c.String(nullable: false, maxLength: 250),
                        CreationDate = c.DateTime(nullable: false),
                        ClientRemarks = c.String(nullable: false, maxLength: 500),
                        Recommendations = c.String(nullable: false, maxLength: 500),
                        TechnicalReport = c.String(nullable: false, maxLength: 500),
                        CheckCompleted_Date = c.DateTime(),
                        Actual_Repair_Date = c.DateTime(),
                        Deliver_Date = c.DateTime(),
                        Approved_Date = c.DateTime(),
                        StartCheckingDate = c.DateTime(),
                        StartReparingDate = c.DateTime(),
                        QualityRejectDate = c.DateTime(),
                        QualityApprovedDate = c.DateTime(),
                        price = c.Double(nullable: false),
                        isAccessoriesAvailable = c.Boolean(nullable: false),
                        MaintenanceCustomerCityModelId = c.Int(nullable: false),
                        userName = c.String(nullable: false),
                        MaintenancePriorityModelId = c.Int(),
                        MaintenanceStatusModelId = c.Int(nullable: false),
                        deliverReason = c.String(),
                        isDeleted = c.Boolean(nullable: false),
                        isRepeated = c.Boolean(nullable: false),
                        user_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MaintenanceCustomerCityModels", t => t.MaintenanceCustomerCityModelId, cascadeDelete: true)
                .ForeignKey("dbo.MaintenancePriorityModels", t => t.MaintenancePriorityModelId)
                .ForeignKey("dbo.MaintenanceStatusModels", t => t.MaintenanceStatusModelId, cascadeDelete: true)
                .ForeignKey("dbo.IdentityUsers", t => t.user_Id)
                .Index(t => t.MaintenanceCustomerCityModelId)
                .Index(t => t.MaintenancePriorityModelId)
                .Index(t => t.MaintenanceStatusModelId)
                .Index(t => t.user_Id);
            
            CreateTable(
                "dbo.MaintenancePriorityModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PriorityName = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MaintenanceStatusModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StatusName = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.IdentityUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IdentityUsers", t => t.IdentityUser_Id)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.IdentityUsers", t => t.IdentityUser_Id)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                        IdentityUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.IdentityUsers", t => t.IdentityUser_Id)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.IdentityUser_Id);
            
            CreateTable(
                "dbo.MaintenanceCommentsModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        comment = c.String(),
                        comment_DateTme = c.DateTime(nullable: false),
                        userImagePath = c.String(),
                        user_Id = c.String(),
                        MaintenanceModelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MaintenanceModels", t => t.MaintenanceModelId, cascadeDelete: true)
                .Index(t => t.MaintenanceModelId);
            
            CreateTable(
                "dbo.notificationLinksModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        notificationLink = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.notificationsModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        notificationCount = c.Int(nullable: false),
                        MaintenanceModelId = c.Int(nullable: false),
                        notificationLinksId = c.Int(nullable: false),
                        user_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MaintenanceModels", t => t.MaintenanceModelId, cascadeDelete: true)
                .ForeignKey("dbo.notificationLinksModels", t => t.notificationLinksId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.user_Id)
                .Index(t => t.MaintenanceModelId)
                .Index(t => t.notificationLinksId)
                .Index(t => t.user_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.TechnicalReportModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TechnicalReportReport = c.String(),
                        PlannedRepairDate = c.DateTime(nullable: false),
                        ActualRepairDate = c.DateTime(nullable: false),
                        MaintenanceModelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MaintenanceModels", t => t.MaintenanceModelId, cascadeDelete: true)
                .Index(t => t.MaintenanceModelId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.IdentityUsers", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "Id", "dbo.IdentityUsers");
            DropForeignKey("dbo.TechnicalReportModels", "MaintenanceModelId", "dbo.MaintenanceModels");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.notificationsModels", "user_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.notificationsModels", "notificationLinksId", "dbo.notificationLinksModels");
            DropForeignKey("dbo.notificationsModels", "MaintenanceModelId", "dbo.MaintenanceModels");
            DropForeignKey("dbo.MaintenanceCommentsModels", "MaintenanceModelId", "dbo.MaintenanceModels");
            DropForeignKey("dbo.MaintenanceModels", "user_Id", "dbo.IdentityUsers");
            DropForeignKey("dbo.AspNetUserRoles", "IdentityUser_Id", "dbo.IdentityUsers");
            DropForeignKey("dbo.AspNetUserLogins", "IdentityUser_Id", "dbo.IdentityUsers");
            DropForeignKey("dbo.AspNetUserClaims", "IdentityUser_Id", "dbo.IdentityUsers");
            DropForeignKey("dbo.MaintenanceModels", "MaintenanceStatusModelId", "dbo.MaintenanceStatusModels");
            DropForeignKey("dbo.MaintenanceModels", "MaintenancePriorityModelId", "dbo.MaintenancePriorityModels");
            DropForeignKey("dbo.MaintenanceModels", "MaintenanceCustomerCityModelId", "dbo.MaintenanceCustomerCityModels");
            DropIndex("dbo.AspNetUsers", new[] { "Id" });
            DropIndex("dbo.TechnicalReportModels", new[] { "MaintenanceModelId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.notificationsModels", new[] { "user_Id" });
            DropIndex("dbo.notificationsModels", new[] { "notificationLinksId" });
            DropIndex("dbo.notificationsModels", new[] { "MaintenanceModelId" });
            DropIndex("dbo.MaintenanceCommentsModels", new[] { "MaintenanceModelId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "IdentityUser_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "IdentityUser_Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "IdentityUser_Id" });
            DropIndex("dbo.IdentityUsers", "UserNameIndex");
            DropIndex("dbo.MaintenanceModels", new[] { "user_Id" });
            DropIndex("dbo.MaintenanceModels", new[] { "MaintenanceStatusModelId" });
            DropIndex("dbo.MaintenanceModels", new[] { "MaintenancePriorityModelId" });
            DropIndex("dbo.MaintenanceModels", new[] { "MaintenanceCustomerCityModelId" });
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.TechnicalReportModels");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.notificationsModels");
            DropTable("dbo.notificationLinksModels");
            DropTable("dbo.MaintenanceCommentsModels");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.IdentityUsers");
            DropTable("dbo.MaintenanceStatusModels");
            DropTable("dbo.MaintenancePriorityModels");
            DropTable("dbo.MaintenanceModels");
            DropTable("dbo.MaintenanceCustomerCityModels");
        }
    }
}
