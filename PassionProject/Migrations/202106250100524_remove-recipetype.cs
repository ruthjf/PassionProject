namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removerecipetype : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Recipes", "RecipeType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Recipes", "RecipeType", c => c.String());
        }
    }
}
