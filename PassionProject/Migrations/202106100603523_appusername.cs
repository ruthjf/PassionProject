namespace PassionProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class appusername : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AppUsers", "AppUsername", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AppUsers", "AppUsername");
        }
    }
}
