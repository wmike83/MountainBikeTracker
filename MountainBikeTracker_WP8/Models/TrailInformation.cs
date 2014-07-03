using System;

namespace MountainBikeTracker_WP8.Models
{
    // TrailName will be associated with a City, Forest, and a DateTime Stamp
    public class TrailInformation
    {
        public string City { get; set; }
        public string Forest { get; set; }
        public DateTime Date { get; set; }
        public MountainBikeTrail Trail { get; set; }

        public TrailInformation()
        {
            this.Date = DateTime.Now;
            this.City = "";
            this.Forest = "";
        }
    }
}
