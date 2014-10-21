namespace WindAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class EnlargeUsername : DbMigration
    {
        public override void Up()
        {
            AlterColumn("LoggedUsers", "Username", w => w.String(maxLength: 30, nullable: false));
        }

        public override void Down()
        {
            AlterColumn("LoggedUsers", "Username", w => w.String(maxLength: 20, nullable: false));
        }
    }
}
