namespace WindAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationUri : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationUris",
                c => new
                    {
                        ChannelUri = c.String(nullable: false, maxLength: 400),
                        InsertDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChannelUri);
            
            DropTable("dbo.PushUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PushUsers",
                c => new
                    {
                        ChannelUri = c.String(nullable: false, maxLength: 300),
                        DeviceId = c.Guid(nullable: false),
                        Number = c.String(maxLength: 40),
                        Username = c.String(maxLength: 40),
                        Password = c.String(maxLength: 40),
                        TileID = c.String(maxLength: 40),
                        InsertDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChannelUri);
            
            DropTable("dbo.NotificationUris");
        }
    }
}
