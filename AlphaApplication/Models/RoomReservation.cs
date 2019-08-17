using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlphaApplication.Models
{
    public class RoomReservation
    {
        public int Id { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string UserId { get; set; }
        public int RoomId { get; set; }
        public string EventDescription { get; set; }
        public bool Confirmation { get; set; }
    }
}