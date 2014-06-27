using MountainBikeTracker_WP8.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBikeTracker_WP8.ViewModels
{
    /// <summary>
    /// This view model will prepare data to be displayed in the information of the ride
    /// I will ask for a name and trail to association to the ride
    /// Also it will allow the user to finalize the file to the SkyDrive account
    /// </summary>
    public class SaveCurrentRideViewModel
    {
        private MountainBikeTrail _currentTrail = null;
        private TrailInformation _trailInfo;



        public MountainBikeTrail CurrentTrail
        {
            get { return this._currentTrail; }
        }

        public TrailInformation TrailInfo
        {
            get { return this._trailInfo; }
        }
        public SaveCurrentRideViewModel()
        {
            this.GetTrailData();
        }

        public void GetTrailData()
        {
            this._currentTrail = App.CurrentRideViewModel.CurrentTrail;
            this._trailInfo = new TrailInformation()
            {
                Date = App.CurrentRideViewModel.CurrentTrail.TimeStamps.FirstOrDefault<DateTime>(),
                City = Services.ServiceLocator.GeolocatorService.LastCity
            };
        }
    }
}
