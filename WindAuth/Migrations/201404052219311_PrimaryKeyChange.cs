namespace WindAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PrimaryKeyChange : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.NotificationUris");
            AlterColumn("dbo.NotificationUris", "ChannelUri", c => c.String());
            AlterColumn("dbo.NotificationUris", "stringNumbers", c => c.String(nullable: false, maxLength: 400));
            AddPrimaryKey("dbo.NotificationUris", "stringNumbers");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.NotificationUris");
            AlterColumn("dbo.NotificationUris", "stringNumbers", c => c.String());
            AlterColumn("dbo.NotificationUris", "ChannelUri", c => c.String(nullable: false, maxLength: 400));
            AddPrimaryKey("dbo.NotificationUris", "ChannelUri");
        }
    }
}
