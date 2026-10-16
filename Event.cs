
using System;

namespace RaceDayApi1.Models
{
    public class Event
    {
        public int EventID { get; set; }

        public int UserID { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string? RouteInfo { get; set; } = string.Empty;
    }
}
