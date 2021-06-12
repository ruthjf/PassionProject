namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recipesappuser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "AppUserID", c => c.Int(nullable: false));
            CreateIndex("dbo.Recipes", "AppUserID");
            AddForeignKey("dbo.Recipes", "AppUserID", "dbo.AppUsers", "AppUserID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Recipes", "AppUserID", "dbo.AppUsers");
            DropIndex("dbo.Recipes", new[] { "AppUserID" });
            DropColumn("dbo.Recipes", "AppUserID");
        }
    }
}
