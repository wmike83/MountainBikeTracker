using MountainBikeTracker_WP8.Helpers;
using MountainBikeTracker_WP8.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBikeTracker_WP8.ViewModels
{
    public class GhostCurrentRideViewModel : BindableBase
    {
        #region Fields
        private MountainBikeTrail _currentTrail = null;
        private MountainBikeTrail _ghostTrail = null;
        private MountainBikeTrail _ghostFinishedTrail = null;
        private double _currentSpeed;
        private GeoCoordinate _currentLocation;
        private GeoCoordinate _ghostLocation;
        #endregion

        #region Properties
        public MountainBikeTrail CurrentTrail
        {
            get { return this._currentTrail; }
        }
        public MountainBikeTrail GhostTrail
        {
            get { return this._ghostTrail; }
        }
        public double CurrentSpeed
        {
            get { return this._currentSpeed; }
            private set { this.SetProperty(ref this._currentSpeed, value); }
        }
        public GeoCoordinate CurrentLocation
        {
            get { return this._currentLocation; }
            private set { this.SetProperty(ref this._currentLocation, value); }
        }
        public GeoCoordinate GhostLocation
        {
            get { return this._ghostLocation; }
            private set { this.SetProperty(ref this._ghostLocation, value); }
        }
        #endregion

        #region Constructor
        public GhostCurrentRideViewModel()
        {
            this._currentTrail = new MountainBikeTrail();
            this._ghostTrail = new MountainBikeTrail();
            this.CurrentLocation = MountainBikeTrail.CreateGeoCoordinate(Services.ServiceLocator.GeolocatorService.LastPoint);
            this.CurrentSpeed = 0;
        }
        #endregion

        #region Helper Methods
        public void SetGhostFinishedTrail(MountainBikeTrail ghostTrail)
        {
            this._ghostFinishedTrail = ghostTrail;
            this._ghostLocation = ghostTrail.Points.First();
        }

        #endregion
    }
}
