namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recipetypedifficulty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "RecipeType", c => c.String());
            AddColumn("dbo.Recipes", "RecipeDifficultyLevel", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "RecipeDifficultyLevel");
            DropColumn("dbo.Recipes", "RecipeType");
        }
    }
}
