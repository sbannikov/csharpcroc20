namespace CrocCSharpBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _002 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MessageHistories",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        User_ID = c.Guid(nullable: false),
                        Message = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Users", t => t.User_ID, cascadeDelete: true)
                .Index(t => new { t.User_ID, t.TimeStamp }, name: "IX_USER");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MessageHistories", "User_ID", "dbo.Users");
            DropIndex("dbo.MessageHistories", "IX_USER");
            DropTable("dbo.MessageHistories");
        }
    }
}
