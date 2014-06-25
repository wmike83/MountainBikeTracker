using MountainBikeTracker_WP8.Helpers;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountainBikeTracker_WP8.ViewModels
{
    public class CurrentRideViewModel : BindableBase
    {
        #region Fields
        private Dictionary<string, ActiveRiderViewModel> _activeRiders = new Dictionary<string, ActiveRiderViewModel>();

        private GeoOutdoors_Trail _currentTrail = null;
        private double _currentSpeed;
        private double _altitude;
        private GeoCoordinate _currentLocation;
        #endregion

        #region Properties
        public Dictionary<string, ActiveRiderViewModel> ActiveRiders
        {
            get { return this._activeRiders; }
        }
        public GeoOutdoors_Trail CurrentTrail
        {
            get { return this._currentTrail; }
        }
        public double CurrentSpeed
        {
            get { return this._currentSpeed; }
            private set { this.SetProperty(ref this._currentSpeed, value); }
        }
        public double Altitude
        {
            get { return this._altitude; }
            private set { this.SetProperty(ref this._altitude, value); }
        }
        public GeoCoordinate CurrentLocation
        {
            get { return this._currentLocation; }
            private set { this.SetProperty(ref this._currentLocation, value); }
        }
        #endregion

        #region Constructor
        public CurrentRideViewModel()
        {
            this._currentTrail = App.User.CurrentRide.CurrentTrail;
            this.CurrentLocation = Models.GeoOutdoors_Trail.CreateGeoCoordinate(Services.GeoOutdoors_ServiceLocator.GeolocatorService.LastPoint);
            this.Altitude = 0;
            this.CurrentSpeed = 0;

            this._activeRiders = new Dictionary<string, ActiveRiderViewModel>();
            this.FillActiveRidersColorsList();
        }

        #endregion

        #region Helper Methods
        private void FillActiveRidersColorsList()
        {
            if (App.User.CurrentRide.ActiveRiders.Count > 0)
            {
                List<SolidColorBrush> fullList = new List<SolidColorBrush>();
                fullList.Add(new SolidColorBrush(Colors.Blue));
                fullList.Add(new SolidColorBrush(Colors.Brown));
                fullList.Add(new SolidColorBrush(Colors.Cyan));
                fullList.Add(new SolidColorBrush(Colors.DarkGray));
                fullList.Add(new SolidColorBrush(Colors.Gray));
                fullList.Add(new SolidColorBrush(Colors.Green));
                fullList.Add(new SolidColorBrush(Colors.Orange));
                fullList.Add(new SolidColorBrush(Colors.Purple));
                fullList.Add(new SolidColorBrush(Colors.Yellow));

                for (int i = 0, list_i = 0; i < App.User.CurrentRide.ActiveRiders.Count; i++, list_i++)
                {
                    if (list_i >= fullList.Count)
                    {
                        list_i = 0;
                    }
                    var rider = App.User.CurrentRide.ActiveRiders[i];
                    App.CurrentRideViewModel.ActiveRiders.Add(rider, new ActiveRiderViewModel(rider, fullList[list_i]));
                }
            }
        }
        public void StartListening()
        {
            Services.GeoOutdoors_ServiceLocator.WebService.OnDataReturned += App.CurrentRideViewModel.OnDataReturned;
            GeoCoordinate geoPoint = Models.GeoOutdoors_Trail.CreateGeoCoordinate(Services.GeoOutdoors_ServiceLocator.GeolocatorService.LastPoint);
            DateTime geoTime = Services.GeoOutdoors_ServiceLocator.GeolocatorService.LastPoint.Timestamp.DateTime;
            Services.GeoOutdoors_ServiceLocator.WebService.SendGeoCoordinateToWebService(App.User.UserName, geoPoint, geoTime);

            Services.GeoOutdoors_ServiceLocator.GeolocatorService.OnPositionChanged += App.CurrentRideViewModel.OnPositionChanged;
        }
        public void StopListening()
        {
            if (App.CurrentRideViewModel.ActiveRiders.Count > 0)
            {
                Services.ServiceLocator.GeolocatorService.OnPositionChanged -= App.CurrentRideViewModel.OnPositionChanged;
            }
        }
        #endregion

        #region Current Ride Relays
        public void Start()
        {
            App.CurrentRideViewModel.CurrentTrail.Start();
        }
        public void Pause()
        {
            App.CurrentRideViewModel.CurrentTrail.Pause();

        }
        public void Reset()
        {
            App.CurrentRideViewModel.CurrentTrail.Reset();
        }
        #endregion

        #region Current Ride Event Listeners
        private void OnDataReturned(Dictionary<string, GeoCoordinate> obj)
        {
            if (obj.Count > 0)
            {
                foreach (var rider in obj.Keys)
                {
                    App.CurrentRideViewModel.ActiveRiders[rider].RiderPoint = obj[rider];
                }
                App.CurrentRideViewModel.OnPropertyChanged("ActiveRiders");
            }
        }
        private void OnPositionChanged(Windows.Devices.Geolocation.Geocoordinate obj)
        {
            GeoCoordinate geoPoint = Models.GeoOutdoors_Trail.CreateGeoCoordinate(obj);
            App.CurrentRideViewModel.CurrentLocation = geoPoint;
            App.CurrentRideViewModel.CurrentSpeed = GeoOutdoors_Trail.ConvertMetersPerSecondToMilesPerHour(App.CurrentRideViewModel.CurrentLocation.Speed);
            App.CurrentRideViewModel.Altitude = GeoOutdoors_Trail.ConvertMetersToFeet(App.CurrentRideViewModel.CurrentLocation.Altitude);
        }
        #endregion
    }
}
