namespace Stargazer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PictureModels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pictures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BodyName = c.String(),
                        ImgurHash = c.String(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pictures", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.Pictures", new[] { "UserId" });
            DropTable("dbo.Pictures");
        }
    }
}
