namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recipedtoupdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Recipes", "AppUserID", "dbo.AppUsers");
            DropIndex("dbo.Recipes", new[] { "AppUserID" });
            AlterColumn("dbo.Recipes", "AppUserID", c => c.Int());
            CreateIndex("dbo.Recipes", "AppUserID");
            AddForeignKey("dbo.Recipes", "AppUserID", "dbo.AppUsers", "AppUserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Recipes", "AppUserID", "dbo.AppUsers");
            DropIndex("dbo.Recipes", new[] { "AppUserID" });
            AlterColumn("dbo.Recipes", "AppUserID", c => c.Int(nullable: false));
            CreateIndex("dbo.Recipes", "AppUserID");
            AddForeignKey("dbo.Recipes", "AppUserID", "dbo.AppUsers", "AppUserID", cascadeDelete: true);
        }
    }
}
