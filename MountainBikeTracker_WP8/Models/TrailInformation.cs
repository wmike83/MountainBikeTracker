using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBikeTracker_WP8.Models
{
    // TrailName will be associated with a City, Forest, and a DateTime Stamp
    public class TrailInformation
    {
        public string City { get; set; }
        public string Forest { get; set; }
        public DateTime Date { get; set; }
    }
}
