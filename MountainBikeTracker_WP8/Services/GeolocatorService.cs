using MountainBikeTracker_WP8.Resources;
using System;
using Windows.Devices.Geolocation;

namespace MountainBikeTracker_WP8.Services
{
    public class GeolocatorService
    {
        #region Singleton Instance
        private static GeolocatorService _instance = null;
        public static GeolocatorService Instance
        { get { return _instance ?? (_instance = new GeolocatorService()); } }
        #endregion

        #region Constructor
        private GeolocatorService()
        {
        }
        #endregion

        #region Fields
        private Geolocator locator;
        private Geoposition lastPosition;
        #endregion

        #region Properties
        public Action<Geocoordinate> OnPositionChanged { get; set; }
        public Action<PositionStatus> OnStatusChanged { get; set; }
        public Geocoordinate LastPoint
        {
            get
            {
                if (this.lastPosition == null)
                    return null;
                else
                    return this.lastPosition.Coordinate;
            }
        }
        public string LastCity
        {
            get
            {
                return "";
            }
        }
        #endregion

        #region Helper Methods
        public void Pause()
        {
            if (this.CanStart())
                if (this.HasStarted())
                    this.locator.PositionChanged -= this.locator_PositionChanged;
        }
        public void Start()
        {
            if (this.CanStart())
                if (this.HasStarted())
                    this.locator.PositionChanged += this.locator_PositionChanged;
        }
        public bool CanStart()
        {
            bool consent;
            bool consentExist = ServiceLocator.IsolatedStorageService
                .TryGetValue(AppResources.LocationConsentText, out consent);
            return consentExist && consent;
        }
        public bool HasStarted()
        {
            if (this.locator == null)
            {
                this.locator = new Geolocator();
                this.locator.DesiredAccuracy = PositionAccuracy.High;
                this.locator.DesiredAccuracyInMeters = 5;
                this.locator.MovementThreshold = 1;

                this.locator.StatusChanged += this.locator_StatusChanged;

                return false;
            }
            else if (this.lastPosition == null)
                return false;
            else
                return true;
        }
        public void Dispose()
        {
            Object thisLock = new Object();

            lock (thisLock)
            {
                if (this.locator != null)
                {
                    if (this.LastPoint != null)
                        this.locator.StatusChanged -= this.locator_StatusChanged;
                    this.locator.PositionChanged -= this.locator_PositionChanged;

                    this.locator = null;
                }
            }
        }
        #endregion

        #region Events
        private void locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            this.lastPosition = args.Position;
            if (this.OnPositionChanged != null)
                this.OnPositionChanged(args.Position.Coordinate);
        }
        private async void locator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            if (args.Status == PositionStatus.Ready)
            {
                this.lastPosition = await this.locator.GetGeopositionAsync();

                //// Windows phone 8.1 code only
                //// reverse geocoding
                //BasicGeoposition myLocation = new BasicGeoposition
                //{
                //    Longitude = this._lastPosition.Coordinate.Longitude,
                //    Latitude = this._lastPosition.Coordinate.Latitude
                //};
                //Geopoint pointToReverseGeocode = new Geopoint(myLocation);

                //MapLocationFinderResult result = await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);

                //// here also it should be checked if there result isn't null and what to do in such a case
                //string city = result.Locations[0].Address.Country;
            }

            if (this.OnStatusChanged != null)
                this.OnStatusChanged(args.Status);
        }
        #endregion
    }
}
