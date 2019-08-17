using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AlphaApplication.Models
{

    public class MeetingRoom
    {
        public int Id { get; set; }
        public double NumberRoom { get; set; }
        public int CountSeats { get; set; }
        public bool Projector { get; set; }
        public bool MarkerBoard { get; set; }
        public string Description { get; set; }
    }
}