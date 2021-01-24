namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shangeAccount : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Accounts", new[] { "Number" });
            AlterColumn("dbo.Accounts", "Number", c => c.String(nullable: false, maxLength: 34));
            CreateIndex("dbo.Accounts", "Number", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Accounts", new[] { "Number" });
            AlterColumn("dbo.Accounts", "Number", c => c.String(nullable: false, maxLength: 20));
            CreateIndex("dbo.Accounts", "Number", unique: true);
        }
    }
}
