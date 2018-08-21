namespace Stargazer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventReminderSent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "ReminderSent", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "ReminderSent");
        }
    }
}
