namespace Stargazer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedPictureModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pictures", "DeleteHash", c => c.String());
            AddColumn("dbo.Pictures", "Link", c => c.String());
            DropColumn("dbo.Pictures", "ImgurHash");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pictures", "ImgurHash", c => c.String());
            DropColumn("dbo.Pictures", "Link");
            DropColumn("dbo.Pictures", "DeleteHash");
        }
    }
}
