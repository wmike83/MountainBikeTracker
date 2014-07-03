using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBikeTracker_WP8.Models
{
    public class AppContext : DataContext
    {
        public Table<TrailInformation> Trails;
        
        public AppContext()
            : base("DataSource=isostore:/data/myapp.sdf")
        {
        }


    }
}
