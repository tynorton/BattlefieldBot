namespace BattlefieldBot.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BattlelogUser",
                c => new
                    {
                        UserID = c.Long(nullable: false, identity: true),
                        UserName = c.String(),
                        IsOnline = c.Boolean(nullable: false),
                        IsPlaying = c.Boolean(nullable: false),
                        GameType = c.Int(nullable: false),
                        ServerID = c.String(),
                        LastUpdatedTicks = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BattlelogUser");
        }
    }
}
