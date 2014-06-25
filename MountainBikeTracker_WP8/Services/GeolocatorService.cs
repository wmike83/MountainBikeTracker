using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace MountainBikeTracker_WP8.Services
{
    class GeolocatorService
    {
        #region Fields
        private Geolocator _locator;
        #endregion

        #region Properties
        public Action<Geocoordinate> OnPositionChanged { get; set; }
        public Action<PositionStatus> OnStatusChanged { get; set; }
        public Geocoordinate LastPoint { get; private set; }
        private bool IsListening { get; set; }
        #endregion

        #region Constructor
        public GeolocatorService()
        {
            IsListening = false;
        }
        #endregion

        #region Helper Methods
        public void Pause()
        {
            if (HasStarted() && this.IsListening)
            {
                //this._locator.PositionChanged -= this._locator_PositionChanged;
                this.IsListening = false;
            }
        }

        public void Start()
        {
            if (HasStarted() && !this.IsListening)
            {
                this._locator.PositionChanged += this._locator_PositionChanged;
                this.IsListening = true;
            }
        }
        public bool HasStarted()
        {
            if (this._locator == null)
            {
                this._locator = new Geolocator();
                this._locator.DesiredAccuracy = PositionAccuracy.High;
                this._locator.DesiredAccuracyInMeters = 5;
                this._locator.MovementThreshold = 1;

                this._locator.StatusChanged += this._locator_StatusChanged;

                return false;
            }
            return true;
        }
        #endregion

        #region Events
        private void _locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            this.LastPoint = args.Position.Coordinate;
            if (this.OnPositionChanged != null)
            {
                this.OnPositionChanged(args.Position.Coordinate);
            }
        }
        private async void _locator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            if (args.Status == PositionStatus.Ready)
                this.LastPoint = (await this._locator.GetGeopositionAsync()).Coordinate;

            if (this.OnStatusChanged != null)
            {
                this.OnStatusChanged(args.Status);
            }
        }
        #endregion
    }
}
