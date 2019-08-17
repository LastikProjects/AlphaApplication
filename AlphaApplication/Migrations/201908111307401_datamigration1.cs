namespace AlphaApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datamigration1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RoomReservations", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RoomReservations", "UserId", c => c.Int(nullable: false));
        }
    }
}
