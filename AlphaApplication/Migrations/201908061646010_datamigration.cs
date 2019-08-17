namespace AlphaApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datamigration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RoomReservations", "Confirmation", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RoomReservations", "Confirmation");
        }
    }
}
