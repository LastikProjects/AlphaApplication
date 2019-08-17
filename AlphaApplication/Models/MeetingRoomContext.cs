using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AlphaApplication.Models
{
    public class MeetingRoomContext : DbContext
    {
        public DbSet<MeetingRoom> MeetingRooms { get; set; }
        public DbSet<RoomReservation> RoomReservations { get; set; }
    }
}