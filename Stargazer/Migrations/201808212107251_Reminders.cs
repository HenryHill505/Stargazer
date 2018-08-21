namespace Stargazer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Reminders : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "GetReminders", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "ReminderTime", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ReminderTime");
            DropColumn("dbo.AspNetUsers", "GetReminders");
        }
    }
}
