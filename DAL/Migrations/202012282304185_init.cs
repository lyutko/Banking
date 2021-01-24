namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.String(nullable: false, maxLength: 20),
                        Description = c.String(),
                        Amount = c.Decimal(nullable: false, storeType: "money"),
                        IsActive = c.Boolean(nullable: false),
                        IsBlocked = c.Boolean(nullable: false),
                        ClientId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .Index(t => t.Number, unique: true)
                .Index(t => t.ClientId);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IPN = c.String(nullable: false, maxLength: 10),
                        FirstName = c.String(nullable: false),
                        SecondName = c.String(nullable: false),
                        LastName = c.String(),
                        Birthday = c.DateTime(nullable: false, storeType: "date"),
                        Phone = c.String(nullable: false, maxLength: 15),
                        Email = c.String(),
                        Address = c.String(),
                        Image = c.String(maxLength: 250),
                        RegisteredDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.IPN, unique: true);
            
            CreateTable(
                "dbo.Operations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Amount = c.Decimal(nullable: false, storeType: "money"),
                        DateTime = c.DateTime(nullable: false),
                        ResultIsSuccess = c.Boolean(nullable: false),
                        FromAccountNumber = c.String(nullable: false),
                        ToAccountNumber = c.String(nullable: false),
                        AccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.AccountId, cascadeDelete: true)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Login = c.String(nullable: false, maxLength: 250, unicode: false),
                        IsActive = c.Boolean(nullable: false),
                        LastLoginDate = c.DateTime(nullable: false),
                        PasswordSalt = c.String(nullable: false, maxLength: 250),
                        PasswordHash = c.String(nullable: false, maxLength: 250),
                        Name = c.String(),
                        ClientId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Clients", t => t.ClientId, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.Login, unique: true)
                .Index(t => t.ClientId, unique: true)
                .Index(t => t.RoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Users", "ClientId", "dbo.Clients");
            DropForeignKey("dbo.Operations", "AccountId", "dbo.Accounts");
            DropForeignKey("dbo.Accounts", "ClientId", "dbo.Clients");
            DropIndex("dbo.Users", new[] { "RoleId" });
            DropIndex("dbo.Users", new[] { "ClientId" });
            DropIndex("dbo.Users", new[] { "Login" });
            DropIndex("dbo.Operations", new[] { "AccountId" });
            DropIndex("dbo.Clients", new[] { "IPN" });
            DropIndex("dbo.Accounts", new[] { "ClientId" });
            DropIndex("dbo.Accounts", new[] { "Number" });
            DropTable("dbo.Users");
            DropTable("dbo.Roles");
            DropTable("dbo.Operations");
            DropTable("dbo.Clients");
            DropTable("dbo.Accounts");
        }
    }
}
