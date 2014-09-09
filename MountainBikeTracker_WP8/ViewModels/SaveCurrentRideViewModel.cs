using MountainBikeTracker_WP8.Helpers;
using MountainBikeTracker_WP8.Models;
using System;
using System.Device.Location;
using System.Linq;

namespace MountainBikeTracker_WP8.ViewModels
{
    /// <summary>
    /// This view model will prepare data to be displayed in the information of the ride
    /// I will ask for a name and trail to association to the ride
    /// Also it will allow the user to finalize the file to the SkyDrive account
    /// </summary>
    public class SaveCurrentRideViewModel// : BindableBase
    {
        #region Fields
        private double _totalAscend;
        private double _totalDescend;
        private double _maxElevation;
        private double _minElevation;
        private double[] _elevationPoints;
        private double _maxSpeed;
        private double _minSpeed;
        private double[] _speedPoints;
        private string _emailAddress;
        #endregion

        #region Properties
        public TrailInformation TrailInfo { get; private set; }
        public double TotalAscend
        {
            get
            {
                return MountainBikeTrail.ConvertMetersToFeet(this._totalAscend);
            }
        }
        public double TotalDescend
        {
            get
            {
                return MountainBikeTrail.ConvertMetersToFeet(this._totalDescend);
            }
        }
        public double MaxSpeedMPH
        {
            get
            {
                return MountainBikeTrail.ConvertMetersPerSecondToMilesPerHour(this._maxSpeed);
            }
        }
        public double MaxElevation
        {
            get
            {
                return this._maxElevation;
            }
        }
        public double MinElevation
        {
            get
            {
                return this._minElevation;
            }
        }
        public double[] ElevationPoints
        {
            get
            {
                return this._elevationPoints;
            }
        }
        public double MaxSpeed
        {
            get
            {
                return this._maxSpeed;
            }
        }
        public double MinSpeed
        {
            get
            {
                return this._minSpeed;
            }
        }
        public double[] SpeedPoints
        {
            get
            {
                return this._speedPoints;
            }
        }
        public RelayCommand CancelCommand
        {
            get;
            private set;
        }
        public RelayCommand SaveCommand
        {
            get;
            private set;
        }
        public RelayCommand GhostCommand
        {
            get;
            private set;
        }
        public string EmailAddress
        {
            get { return this._emailAddress; }
            set { this._emailAddress = value; }
        }
        public System.Windows.Visibility IsNotSaved
        {
            get;
            private set;
        }
        public System.Windows.Visibility IsSaved
        {
            get
            {
                if (this.IsNotSaved == System.Windows.Visibility.Visible)
                    return System.Windows.Visibility.Collapsed;
                else
                    return System.Windows.Visibility.Visible;
            }
        }
        #endregion

        #region Constructor
        public SaveCurrentRideViewModel()
        {
            this.CancelCommand = new RelayCommand(ExecuteCancelCommand);
            this.SaveCommand = new RelayCommand(ExecuteSaveCommand);
            this.GhostCommand = new RelayCommand(ExecuteGhostCommand);

            this.Reset();
        }
        #endregion

        #region Helper Methods
        public void GetCurrentTrailData()
        {
            this.IsNotSaved = System.Windows.Visibility.Visible;
            this.TrailInfo.Trail = App.CurrentRideViewModel.CurrentTrail;

            this.GetTrailInfo();
        }
        public void GetTrailData(ulong index)
        {
            this.IsNotSaved = System.Windows.Visibility.Collapsed;
            this.TrailInfo = App.DataStore.Trails[index];

            this.GetTrailInfo();
        }
        private void GetTrailInfo()
        {
            double lastAltitude = this.TrailInfo.Trail.Points.FirstOrDefault<GeoCoordinate>().Altitude;
            this._elevationPoints = new double[this.TrailInfo.Trail.Points.Count];
            this._speedPoints = new double[this.TrailInfo.Trail.Points.Count];
            int index = 0;

            foreach (GeoCoordinate geo in this.TrailInfo.Trail.Points)
            {
                double currentAltitude = geo.Altitude;
                double deltaAltitude = currentAltitude - lastAltitude;
                double currentSpeed = geo.Speed;

                if (deltaAltitude >= 0.00000000)
                    this._totalAscend += deltaAltitude;
                else
                    this._totalDescend += (deltaAltitude * -1);

                if (currentAltitude > this._maxElevation)
                    this._maxElevation = currentAltitude;

                if (currentAltitude < this._minElevation)
                    this._minElevation = currentAltitude;

                this._elevationPoints[index] = currentAltitude;

                if (currentSpeed > this._maxSpeed)
                    this._maxSpeed = currentSpeed;

                if (currentSpeed < this._minSpeed)
                    this._minSpeed = currentSpeed;

                this._speedPoints[index++] = currentSpeed;

                lastAltitude = currentAltitude;
            }
        }
        public void Reset()
        {
            this._elevationPoints = null;
            this._maxElevation = double.MinValue;
            this._maxSpeed = double.MinValue;
            this._minElevation = double.MaxValue;
            this._minSpeed = double.MaxValue;
            this._speedPoints = null;
            this._totalAscend = 0;
            this._totalDescend = 0;
            this.TrailInfo = new TrailInformation();
        }

        private void ExecuteCancelCommand()
        {
            // Do Cancel Here

            this.Reset();
        }
        private void ExecuteSaveCommand()
        {
            // Do Save Here
            App.DataStore.AddNewTrail(this.TrailInfo);

            this.Reset();
        }
        #endregion



        public void ExecuteGhostCommand()
        {
            // Do Ghost Command Here
        }
    }
}
