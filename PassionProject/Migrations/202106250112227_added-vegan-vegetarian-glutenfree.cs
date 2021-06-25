namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedveganvegetarianglutenfree : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Recipes", "Vegan", c => c.Boolean(nullable: false));
            AddColumn("dbo.Recipes", "Vegetarian", c => c.Boolean(nullable: false));
            AddColumn("dbo.Recipes", "GlutenFree", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Recipes", "GlutenFree");
            DropColumn("dbo.Recipes", "Vegetarian");
            DropColumn("dbo.Recipes", "Vegan");
        }
    }
}
