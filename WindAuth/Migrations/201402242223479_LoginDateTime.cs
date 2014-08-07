namespace WindAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LoginDateTime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoggedUsers", "LastLogin", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.LoggedUsers", "LastLogin");
        }
    }
}
