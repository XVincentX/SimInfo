namespace WindAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LoggedUsers",
                c => new
                    {
                        Username = c.String(nullable: false, maxLength: 20),
                        Password = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.Username);
            
            CreateTable(
                "dbo.PushUsers",
                c => new
                    {
                        ChannelUri = c.String(nullable: false, maxLength: 300),
                        DeviceId = c.Guid(nullable: false),
                        Number = c.String(maxLength: 4000),
                        Username = c.String(maxLength: 40),
                        Password = c.String(maxLength: 40),
                        TileID = c.String(maxLength: 40),
                        InsertDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ChannelUri);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PushUsers");
            DropTable("dbo.LoggedUsers");
        }
    }
}
