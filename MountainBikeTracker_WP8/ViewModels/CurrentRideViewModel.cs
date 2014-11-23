using GeoGenius.WindowsPhone8._0.Models;
using MountainBikeTracker_WP8.Models;
using MountainBikeTracker_WP8.Services;
using System.Device.Location;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace MountainBikeTracker_WP8.ViewModels
{
    public class CurrentRideViewModel : BindableBase
    {
        #region Fields
        private MountainBikeTrail currentTrail = null;
        private double currentSpeed;
        private double distance;
        private double altitude;
        private double averageSpeed;
        private GeoCoordinate currentLocation;
        private bool isRecording;
        #endregion

        #region Properties
        public MountainBikeTrail CurrentTrail
        {
            get { return this.currentTrail; }
            set { this.SetProperty(ref this.currentTrail, value); }
        }
        public double CurrentSpeed
        {
            get { return this.currentSpeed; }
            set { this.SetProperty(ref this.currentSpeed, value); }
        }
        public double Distance
        {
            get { return this.distance; }
            set { this.SetProperty(ref this.distance, value); }
        }
        public double Altitude
        {
            get { return this.altitude; }
            set { this.SetProperty(ref this.altitude, value); }
        }
        public double AverageSpeed
        {
            get { return this.averageSpeed; }
            set { this.SetProperty(ref this.averageSpeed, value); }
        }
        public GeoCoordinate CurrentLocation
        {
            get { return this.currentLocation; }
            set { this.SetProperty(ref this.currentLocation, value); }
        }
        public bool IsRecording
        {
            get { return this.isRecording; }
            set { this.isRecording = value; }
        }
        #endregion

        #region Constructor
        public CurrentRideViewModel()
        {
            this.CurrentTrail = new MountainBikeTrail();
            if (ServiceLocator.GeolocatorService.LastPoint != null)
                this.CurrentLocation =
                    MountainBikeTrail.CreateGeoCoordinate(ServiceLocator.GeolocatorService.LastPoint);
            this.Altitude = 0;
            this.CurrentSpeed = 0;
            this.AverageSpeed = 0;
            this.Distance = 0;
            this.IsRecording = false;
        }
        #endregion

        #region Helper Methods
        public void NavigatingTo()
        {
            App.CurrentRideViewModel.ResetTrail();
            ServiceLocator.GeolocatorService.Start();
            App.CurrentRideViewModel.StartListening();
        }
        public void NavigatingFrom()
        {
            ServiceLocator.GeolocatorService.Pause();
            App.CurrentRideViewModel.PauseRecording();
            App.CurrentRideViewModel.StopListening();
            App.CurrentRideViewModel.ResetTrail();
        }
        /// <summary>
        /// Start listening directly to locator
        /// Only called when map is in view and not recording
        /// </summary>
        public void StartListening()
        {
            // Start Geolocator and registering to event Position Changed
            ServiceLocator.GeolocatorService.OnPositionChanged = App.CurrentRideViewModel.OnPositionChanged + ServiceLocator.GeolocatorService.OnPositionChanged;
        }
        /// <summary>
        /// Stop listening directly to locator
        /// Called when when map is in view and user starts recording
        /// </summary>
        public void StopListening()
        {
            // Pausing Geolocator and unregistering to event Position Changed
            ServiceLocator.GeolocatorService.OnPositionChanged -= App.CurrentRideViewModel.OnPositionChanged;
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

            App.CurrentRideViewModel.IsRecording = true;
        }
        /// <summary>
        /// Pause recording
        /// Start listening directly to locator
        /// </summary>
        public void PauseRecording()
        {
            App.CurrentRideViewModel.IsRecording = false;
        }
        /// <summary>
        /// Reset trail
        /// </summary>
        public void ResetTrail()
        {
            App.CurrentRideViewModel.CurrentTrail.Reset();
            App.CurrentRideViewModel.Distance = 0;
            App.CurrentRideViewModel.AverageSpeed = 0;
        }
        #endregion

        #region Current Ride Event Listeners
        /// <summary>
        /// This is called when at page.
        /// It always updates the current location, speed, and altitude.
        /// If recording then updates the trail
        /// </summary>
        /// <param name="obj"></param>
        private void OnPositionChanged(Geocoordinate obj)
        {
            // update the point if not listening so the user can see on a map while not riding
            // if listening then add new point to trail
            GeoCoordinate geoPoint;
            if (App.CurrentRideViewModel.IsRecording)
            {
                geoPoint = App.CurrentRideViewModel.CurrentTrail.AddNewPoint(obj);

                // Allow for user to select later with enum
                App.CurrentRideViewModel.AverageSpeed =
                    MountainBikeTrail.ConvertMetersPerSecondToMilesPerHour(
                    App.CurrentRideViewModel.CurrentTrail.AverageSpeed);
                App.CurrentRideViewModel.Distance =
                    MountainBikeTrail.ConvertMetersToMiles(
                    App.CurrentRideViewModel.CurrentTrail.Distance);
            }
            else
            {
                geoPoint = Models.MountainBikeTrail.CreateGeoCoordinate(obj);
            }

            App.CurrentRideViewModel.CurrentLocation = geoPoint;

            // Allow for user to select later with enum
            App.CurrentRideViewModel.CurrentSpeed =
                MountainBikeTrail.ConvertMetersPerSecondToMilesPerHour(geoPoint.Speed);
            App.CurrentRideViewModel.Altitude =
                MountainBikeTrail.ConvertMetersToFeet(geoPoint.Altitude);

            if (App.RunningInBackground)
            {
                // Send to tile
            }
        }
        #endregion

        public async Task<bool> SaveRideAsync()
        {
            MountainBikeTrail ride = this.CurrentTrail;
            HistoryStorage fileInfo = HistoryStorage.BuildHistoryStorage(ride);
            bool result = await ServiceLocator.StorageService
                                .SaveCurrentRideDataAsync(ride, fileInfo);
            if (result)
            {
                // Add pointer to File for History Page
                App.HistoryViewModel.AddToHistory(fileInfo);
                App.CurrentRideViewModel.ResetTrail();
                return true;
            }
            else
            {
                // No file to add try again
                return false;
            }
        }
    }
}
