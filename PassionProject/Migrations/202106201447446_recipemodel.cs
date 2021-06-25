namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recipemodel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Recipes", "IngredientID");
            DropColumn("dbo.Recipes", "IngredientName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipes", "IngredientName", c => c.String());
            AddColumn("dbo.Recipes", "IngredientID", c => c.Int(nullable: false));
        }
    }
}
