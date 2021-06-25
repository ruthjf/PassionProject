namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ingredientrecipe : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RecipeIngredients", "Recipe_RecipeID", "dbo.Recipes");
            DropForeignKey("dbo.RecipeIngredients", "Ingredient_IngredientID", "dbo.Ingredients");
            DropIndex("dbo.RecipeIngredients", new[] { "Recipe_RecipeID" });
            DropIndex("dbo.RecipeIngredients", new[] { "Ingredient_IngredientID" });
            AddColumn("dbo.Ingredients", "Recipe_RecipeID", c => c.Int());
            AddColumn("dbo.Ingredients", "Recipe_RecipeID1", c => c.Int());
            AddColumn("dbo.Recipes", "Ingredient_IngredientID", c => c.Int());
            CreateIndex("dbo.Ingredients", "Recipe_RecipeID");
            CreateIndex("dbo.Ingredients", "Recipe_RecipeID1");
            CreateIndex("dbo.Recipes", "Ingredient_IngredientID");
            AddForeignKey("dbo.Ingredients", "Recipe_RecipeID", "dbo.Recipes", "RecipeID");
            AddForeignKey("dbo.Ingredients", "Recipe_RecipeID1", "dbo.Recipes", "RecipeID");
            AddForeignKey("dbo.Recipes", "Ingredient_IngredientID", "dbo.Ingredients", "IngredientID");
            DropTable("dbo.RecipeIngredients");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RecipeIngredients",
                c => new
                    {
                        Recipe_RecipeID = c.Int(nullable: false),
                        Ingredient_IngredientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Recipe_RecipeID, t.Ingredient_IngredientID });
            
            DropForeignKey("dbo.Recipes", "Ingredient_IngredientID", "dbo.Ingredients");
            DropForeignKey("dbo.Ingredients", "Recipe_RecipeID1", "dbo.Recipes");
            DropForeignKey("dbo.Ingredients", "Recipe_RecipeID", "dbo.Recipes");
            DropIndex("dbo.Recipes", new[] { "Ingredient_IngredientID" });
            DropIndex("dbo.Ingredients", new[] { "Recipe_RecipeID1" });
            DropIndex("dbo.Ingredients", new[] { "Recipe_RecipeID" });
            DropColumn("dbo.Recipes", "Ingredient_IngredientID");
            DropColumn("dbo.Ingredients", "Recipe_RecipeID1");
            DropColumn("dbo.Ingredients", "Recipe_RecipeID");
            CreateIndex("dbo.RecipeIngredients", "Ingredient_IngredientID");
            CreateIndex("dbo.RecipeIngredients", "Recipe_RecipeID");
            AddForeignKey("dbo.RecipeIngredients", "Ingredient_IngredientID", "dbo.Ingredients", "IngredientID", cascadeDelete: true);
            AddForeignKey("dbo.RecipeIngredients", "Recipe_RecipeID", "dbo.Recipes", "RecipeID", cascadeDelete: true);
        }
    }
}
