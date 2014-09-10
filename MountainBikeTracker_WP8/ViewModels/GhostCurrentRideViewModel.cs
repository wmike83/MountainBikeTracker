using Microsoft.Phone.Maps.Controls;
using MountainBikeTracker_WP8.Helpers;
using MountainBikeTracker_WP8.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MountainBikeTracker_WP8.ViewModels
{
    public class GhostCurrentRideViewModel : BindableBase
    {
        #region Fields
        private MountainBikeTrail _currentTrail = null;
        private GeoCoordinateCollection _ghostTrail = null;
        private MountainBikeTrail _ghostFinishedTrail = null;
        private double _currentSpeed;
        private double _currentGhostDistance;
        private double _currentDistanceDelta;
        private GeoCoordinate _currentLocation;
        private GeoCoordinate _ghostLocation;
        private TimeSpan _duration;
        #endregion

        #region Properties
        public MountainBikeTrail CurrentTrail
        {
            get { return this._currentTrail; }
        }
        public GeoCoordinateCollection GhostTrail
        {
            get { return this._ghostTrail; }
        }
        public MountainBikeTrail GhostFinishedTrail
        {
            get { return this._ghostFinishedTrail; }
            set { this._ghostFinishedTrail = value; }
        }
        public double CurrentSpeed
        {
            get { return this._currentSpeed; }
            private set { this.SetProperty(ref this._currentSpeed, value); }
        }
        public double CurrentGhostDistance
        {
            get { return this._currentGhostDistance; }
            private set { this._currentGhostDistance = value; }
        }
        public double CurrentDistanceDelta
        {
            get { return this._currentDistanceDelta; }
            private set { this.SetProperty(ref this._currentDistanceDelta, value); }
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
        public TimeSpan Duration
        {
            get { return this._duration; }
            private set { this._duration = value; }
        }
        #endregion

        #region Constructor
        public GhostCurrentRideViewModel()
        {
            this._currentTrail = new MountainBikeTrail();
            this._ghostTrail = new GeoCoordinateCollection();
            this.CurrentLocation = MountainBikeTrail.CreateGeoCoordinate(Services.ServiceLocator.GeolocatorService.LastPoint);
            this.CurrentSpeed = 0;
            this.CurrentGhostDistance = 0;
            this.CurrentDistanceDelta = 0;
        }
        #endregion

        #region Helper Methods
        public void SetGhostFinishedTrail(MountainBikeTrail ghostTrail)
        {
            App.GhostCurrentRideViewModel.GhostFinishedTrail = ghostTrail;
            App.GhostCurrentRideViewModel.GhostLocation = ghostTrail.Points.First();
        }
        /// <summary>
        /// Start listening directly to locator
        /// Only called when map is in view and not recording
        /// </summary>
        public void StartListening()
        {
            Services.ServiceLocator.GeolocatorService.OnPositionChanged += App.GhostCurrentRideViewModel.OnPositionChanged;
        }
        /// <summary>
        /// Stop listening directly to locator
        /// Called when when map is in view and user starts recording
        /// </summary>
        public void StopListening()
        {
            Services.ServiceLocator.GeolocatorService.OnPositionChanged -= App.GhostCurrentRideViewModel.OnPositionChanged;
        }

        /// <summary>
        /// Start the ghosting thread
        /// </summary>
        public void StartGhosting()
        {
            // Creates timer thread
            DispatcherTimer time = new DispatcherTimer();
            // Set first interval to 100 millisecond
            time.Interval = new TimeSpan(0, 0, 0, 0, 100);
            // Tick event to add next point when triggered
            time.Tick += ((s, e) =>
            {
                // Stop timer if last point
                if (!App.GhostCurrentRideViewModel.AddNextPoint())
                    time.Stop();

                // Set next duration
                time.Interval = App.GhostCurrentRideViewModel.Duration;
            });
            // Start thread
            time.Start();
        }
        /// <summary>
        /// Adds GPS Point to ghost collection
        /// </summary>
        /// <returns>True if still have points, False if done</returns>
        public bool AddNextPoint()
        {
            // If position is valid then add point
            if (App.GhostCurrentRideViewModel.GhostTrail.Count < App.GhostCurrentRideViewModel.GhostFinishedTrail.Points.Count)
            {
                int index = App.GhostCurrentRideViewModel.GhostTrail.Count;
                GeoCoordinate newPoint = App.GhostCurrentRideViewModel.GhostFinishedTrail.Points[index];
                GeoCoordinate lastPoint = App.GhostCurrentRideViewModel.GhostTrail.Last();
                App.GhostCurrentRideViewModel.GhostTrail.Add(newPoint);
                // Calculate a new duration for the next time interval
                App.GhostCurrentRideViewModel.Duration = App.GhostCurrentRideViewModel.GhostTrail.Count == App.GhostCurrentRideViewModel.GhostFinishedTrail.Points.Count - 1 ? new TimeSpan() : App.GhostCurrentRideViewModel.GhostFinishedTrail.TimeStamps[index + 1] - App.GhostCurrentRideViewModel.GhostFinishedTrail.TimeStamps[index];
                App.GhostCurrentRideViewModel.GhostLocation = newPoint;
                App.GhostCurrentRideViewModel.CurrentGhostDistance += MountainBikeTrail.ConvertMetersToMiles(lastPoint.GetDistanceTo(newPoint));
                App.GhostCurrentRideViewModel.CurrentDistanceDelta = App.GhostCurrentRideViewModel.CurrentTrail.Distance - App.GhostCurrentRideViewModel.CurrentGhostDistance;
                return true;
            }
            else
            {
                // Done
                return false;
            }
        }

        #endregion

        #region Current Ride Relays
        /// <summary>
        /// Start recording
        /// Stop listening directly to locator
        /// </summary>
        public void StartRecording()
        {
            App.GhostCurrentRideViewModel.CurrentTrail.Start();
            App.GhostCurrentRideViewModel.CurrentTrail.PropertyChanged += App.GhostCurrentRideViewModel.CurrentTrail_PropertyChanged;
            App.GhostCurrentRideViewModel.StopListening();

            // Start Ghosting
            App.GhostCurrentRideViewModel.StartGhosting();
        }
        /// <summary>
        /// Pause recording
        /// Start listening directly to locator
        /// </summary>
        public void PauseRecording()
        {
            //App.GhostCurrentRideViewModel.CurrentTrail.Pause();
            //App.GhostCurrentRideViewModel.CurrentTrail.PropertyChanged -= App.GhostCurrentRideViewModel.CurrentTrail_PropertyChanged;
            //App.GhostCurrentRideViewModel.StartListening();

            // Pause Ghosting
        }
        /// <summary>
        /// Reset trail
        /// </summary>
        public void ResetTrail()
        {
 
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
                GeoCoordinate currentGeoPoint = App.GhostCurrentRideViewModel.CurrentTrail.Points.Last();
                GeoCoordinate ghostGeoPoint = App.GhostCurrentRideViewModel.GhostTrail.Last();
                App.GhostCurrentRideViewModel.CurrentLocation = currentGeoPoint;
                App.GhostCurrentRideViewModel.GhostLocation = ghostGeoPoint;
                App.GhostCurrentRideViewModel.CurrentSpeed = MountainBikeTrail.ConvertMetersPerSecondToMilesPerHour(currentGeoPoint.Speed);
                App.GhostCurrentRideViewModel.CurrentDistanceDelta = App.GhostCurrentRideViewModel.CurrentTrail.Distance - App.GhostCurrentRideViewModel.CurrentGhostDistance;
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
            App.GhostCurrentRideViewModel.CurrentLocation = geoPoint;
            App.GhostCurrentRideViewModel.CurrentSpeed = MountainBikeTrail.ConvertMetersPerSecondToMilesPerHour(geoPoint.Speed);
        }
        #endregion

    }
}
