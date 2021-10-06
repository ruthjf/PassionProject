namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recipexingredient : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RecipeXIngredients",
                c => new
                    {
                        RecipeXIngredientID = c.Int(nullable: false, identity: true),
                        RecipeID = c.Int(nullable: false),
                        IngredientID = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Unit = c.String(),
                    })
                .PrimaryKey(t => t.RecipeXIngredientID)
                .ForeignKey("dbo.Ingredients", t => t.IngredientID, cascadeDelete: true)
                .ForeignKey("dbo.Recipes", t => t.RecipeID, cascadeDelete: true)
                .Index(t => t.RecipeID)
                .Index(t => t.IngredientID);
            
            DropTable("dbo.AppUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AppUsers",
                c => new
                    {
                        AppUserID = c.Int(nullable: false, identity: true),
                        AppUsername = c.String(),
                        AppUserFirstName = c.String(),
                        AppUserLastName = c.String(),
                    })
                .PrimaryKey(t => t.AppUserID);
            
            DropForeignKey("dbo.RecipeXIngredients", "RecipeID", "dbo.Recipes");
            DropForeignKey("dbo.RecipeXIngredients", "IngredientID", "dbo.Ingredients");
            DropIndex("dbo.RecipeXIngredients", new[] { "IngredientID" });
            DropIndex("dbo.RecipeXIngredients", new[] { "RecipeID" });
            DropTable("dbo.RecipeXIngredients");
        }
    }
}
