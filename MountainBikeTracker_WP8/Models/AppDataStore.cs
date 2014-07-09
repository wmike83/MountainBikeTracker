using MountainBikeTracker_WP8.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBikeTracker_WP8.Models
{
    public class AppDataStore : BindableBase
    {
        private ulong _index = 0;

        public Dictionary<ulong, TrailInformation> Trails { get; private set; }
        
        public AppDataStore()
        {
            this.Trails = new Dictionary<ulong, TrailInformation>();
        }

        public void AddNewTrail(TrailInformation trailInformation)
        {
            this.Trails.Add(_index++, trailInformation);
        }

        public TrailInformation RetrieveTrail(ulong index)
        {
            TrailInformation trail = null;
            try
            {
                this.Trails.TryGetValue(index, out trail);
                return trail;
            }
            catch
            {
                return null;
            }
        }
    }
}
