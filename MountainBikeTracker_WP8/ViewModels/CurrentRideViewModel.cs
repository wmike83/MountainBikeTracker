using MountainBikeTracker_WP8.Helpers;
using MountainBikeTracker_WP8.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MountainBikeTracker_WP8.ViewModels
{
    public class CurrentRideViewModel : BindableBase
    {
        #region Fields
        private MountainBikeTrail _currentTrail = null;
        private double _currentSpeed;
        private double _altitude;
        private GeoCoordinate _currentLocation;
        #endregion

        #region Properties
        public MountainBikeTrail CurrentTrail
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
            this._currentTrail = new MountainBikeTrail();
            this.CurrentLocation = MountainBikeTrail.CreateGeoCoordinate(Services.ServiceLocator.GeolocatorService.LastPoint);
            this.Altitude = 0;
            this.CurrentSpeed = 0;
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Start listening directly to locator
        /// Only called when map is in view and not recording
        /// </summary>
        public void StartListening()
        {
            Services.ServiceLocator.GeolocatorService.OnPositionChanged += App.CurrentRideViewModel.OnPositionChanged;
        }
        /// <summary>
        /// Stop listening directly to locator
        /// Called when when map is in view and user starts recording
        /// </summary>
        public void StopListening()
        {
            Services.ServiceLocator.GeolocatorService.OnPositionChanged -= App.CurrentRideViewModel.OnPositionChanged;
        }
        #endregion

        #region Current Ride Relays
        /// <summary>
        /// Start recording
        /// Stop listening directly to locator
        /// </summary>
        public void StartRecording()
        {
            App.CurrentRideViewModel.CurrentTrail.Start();
            App.CurrentRideViewModel.CurrentTrail.PropertyChanged += App.CurrentRideViewModel.CurrentTrail_PropertyChanged;
            App.CurrentRideViewModel.StopListening();
        }
        /// <summary>
        /// Pause recording
        /// Start listening directly to locator
        /// </summary>
        public void PauseRecording()
        {
            App.CurrentRideViewModel.CurrentTrail.Pause();
            App.CurrentRideViewModel.CurrentTrail.PropertyChanged -= App.CurrentRideViewModel.CurrentTrail_PropertyChanged;
            App.CurrentRideViewModel.StartListening();
        }
        /// <summary>
        /// Reset trail
        /// </summary>
        public void ResetTrail()
        {
            App.CurrentRideViewModel.CurrentTrail.Reset();
            this.OnPropertyChanged("CurrentTrail");
            this.OnPropertyChanged("Points");
            
        }
        #endregion

        #region Current Ride Event Listeners
        /// <summary>
        /// This is used for when the user is recording and for both views
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentTrail_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentTrail")
            {
                GeoCoordinate geoPoint = this.CurrentTrail.Points.Last();
                App.CurrentRideViewModel.CurrentLocation = geoPoint;
                App.CurrentRideViewModel.CurrentSpeed = MountainBikeTrail.ConvertMetersPerSecondToMilesPerHour(geoPoint.Speed);
                App.CurrentRideViewModel.Altitude = MountainBikeTrail.ConvertMetersToFeet(geoPoint.Altitude);
                base.OnPropertyChanged(e.PropertyName);
            }
        }
        /// <summary>
        /// This is used for when looking at the app and not recording
        /// </summary>
        /// <param name="obj"></param>
        private void OnPositionChanged(Windows.Devices.Geolocation.Geocoordinate obj)
        {
            GeoCoordinate geoPoint = Models.MountainBikeTrail.CreateGeoCoordinate(obj);
            App.CurrentRideViewModel.CurrentLocation = geoPoint;
            App.CurrentRideViewModel.CurrentSpeed = MountainBikeTrail.ConvertMetersPerSecondToMilesPerHour(geoPoint.Speed);
            App.CurrentRideViewModel.Altitude = MountainBikeTrail.ConvertMetersToFeet(geoPoint.Altitude);
        }
        #endregion
    }
}
