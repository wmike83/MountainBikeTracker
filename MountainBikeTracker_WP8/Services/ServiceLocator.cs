using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBikeTracker_WP8.Services
{
    class ServiceLocator
    {
        private static GeolocatorService _geolocatorService;

        public static GeolocatorService GeolocatorService
        {
            get { return ServiceLocator._geolocatorService ?? (ServiceLocator._geolocatorService = new GeolocatorService()); }
        }
    }
}
