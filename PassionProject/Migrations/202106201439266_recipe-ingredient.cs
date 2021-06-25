namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recipeingredient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "IngredientID", c => c.Int(nullable: false));
            AddColumn("dbo.Recipes", "IngredientName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "IngredientName");
            DropColumn("dbo.Recipes", "IngredientID");
        }
    }
}
