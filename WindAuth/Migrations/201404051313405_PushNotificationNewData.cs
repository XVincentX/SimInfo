namespace WindAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PushNotificationNewData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.NotificationUris", "stringNumbers", c => c.String());
            DropColumn("dbo.NotificationUris", "InsertDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.NotificationUris", "InsertDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.NotificationUris", "stringNumbers");
        }
    }
}
