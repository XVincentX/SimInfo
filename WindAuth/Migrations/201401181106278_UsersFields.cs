namespace WindAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsersFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LoggedUsers", "Type", c => c.String(maxLength: 1));
            AddColumn("dbo.LoggedUsers", "device_id", c => c.Guid(nullable: false));
            AlterColumn("dbo.PushUsers", "Number", c => c.String(maxLength: 40));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PushUsers", "Number", c => c.String(maxLength: 4000));
            DropColumn("dbo.LoggedUsers", "device_id");
            DropColumn("dbo.LoggedUsers", "Type");
        }
    }
}
