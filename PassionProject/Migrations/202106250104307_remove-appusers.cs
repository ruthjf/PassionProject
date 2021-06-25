namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeappusers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Recipes", "AppUserID", "dbo.AppUsers");
            DropIndex("dbo.Recipes", new[] { "AppUserID" });
            DropColumn("dbo.Recipes", "AppUserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipes", "AppUserID", c => c.Int());
            CreateIndex("dbo.Recipes", "AppUserID");
            AddForeignKey("dbo.Recipes", "AppUserID", "dbo.AppUsers", "AppUserID");
        }
    }
}
