namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedpic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "RecipeHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Recipes", "PicExtension", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "PicExtension");
            DropColumn("dbo.Recipes", "RecipeHasPic");
        }
    }
}
