using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBikeTracker_WP8.Models
{
    // TrailName will be associated with a City, Forest, and a DateTime Stamp
    [Table]
    public class TrailInformation
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int ID { get; set; }
        [Column]
        public string City { get; set; }
        [Column]
        public string Forest { get; set; }
        [Column]
        public DateTime Date { get; set; }
        [Column]
        public MountainBikeTrail Trail { get; set; }
    }
}
