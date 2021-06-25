namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedingredient : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Ingredients", "Recipe_RecipeID", "dbo.Recipes");
            DropForeignKey("dbo.Ingredients", "Recipe_RecipeID1", "dbo.Recipes");
            DropForeignKey("dbo.Recipes", "Ingredient_IngredientID", "dbo.Ingredients");
            DropIndex("dbo.Ingredients", new[] { "Recipe_RecipeID" });
            DropIndex("dbo.Ingredients", new[] { "Recipe_RecipeID1" });
            DropIndex("dbo.Recipes", new[] { "Ingredient_IngredientID" });
            CreateTable(
                "dbo.RecipeIngredients",
                c => new
                    {
                        Recipe_RecipeID = c.Int(nullable: false),
                        Ingredient_IngredientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Recipe_RecipeID, t.Ingredient_IngredientID })
                .ForeignKey("dbo.Recipes", t => t.Recipe_RecipeID, cascadeDelete: true)
                .ForeignKey("dbo.Ingredients", t => t.Ingredient_IngredientID, cascadeDelete: true)
                .Index(t => t.Recipe_RecipeID)
                .Index(t => t.Ingredient_IngredientID);
            
            DropColumn("dbo.Ingredients", "Recipe_RecipeID");
            DropColumn("dbo.Ingredients", "Recipe_RecipeID1");
            DropColumn("dbo.Recipes", "Ingredient_IngredientID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipes", "Ingredient_IngredientID", c => c.Int());
            AddColumn("dbo.Ingredients", "Recipe_RecipeID1", c => c.Int());
            AddColumn("dbo.Ingredients", "Recipe_RecipeID", c => c.Int());
            DropForeignKey("dbo.RecipeIngredients", "Ingredient_IngredientID", "dbo.Ingredients");
            DropForeignKey("dbo.RecipeIngredients", "Recipe_RecipeID", "dbo.Recipes");
            DropIndex("dbo.RecipeIngredients", new[] { "Ingredient_IngredientID" });
            DropIndex("dbo.RecipeIngredients", new[] { "Recipe_RecipeID" });
            DropTable("dbo.RecipeIngredients");
            CreateIndex("dbo.Recipes", "Ingredient_IngredientID");
            CreateIndex("dbo.Ingredients", "Recipe_RecipeID1");
            CreateIndex("dbo.Ingredients", "Recipe_RecipeID");
            AddForeignKey("dbo.Recipes", "Ingredient_IngredientID", "dbo.Ingredients", "IngredientID");
            AddForeignKey("dbo.Ingredients", "Recipe_RecipeID1", "dbo.Recipes", "RecipeID");
            AddForeignKey("dbo.Ingredients", "Recipe_RecipeID", "dbo.Recipes", "RecipeID");
        }
    }
}
