namespace AlphaApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datamigration2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RoomReservations", "EventDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RoomReservations", "EventDescription");
        }
    }
}
