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
        #endregion

        #region Properties
        public TrailInformation TrailInfo { get; private set; }
        public double TotalAscend
        {
            get
            {
                return MountainBikeTrail.ConvertMetersToFeet( this._totalAscend );
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
        #endregion

        #region Constructor
        public SaveCurrentRideViewModel()
        {
            this.CancelCommand = new RelayCommand(ExecuteCancelCommand);
            this.SaveCommand = new RelayCommand(ExecuteSaveCommand);

            this.Reset();
        }
        #endregion

        #region Helper Methods
        public void GetCurrentTrailData()
        {
            this.TrailInfo.Trail = App.CurrentRideViewModel.CurrentTrail;

            double lastAltitude = App.CurrentRideViewModel.CurrentTrail.Points.FirstOrDefault<GeoCoordinate>().Altitude;
            this._elevationPoints = new double[App.CurrentRideViewModel.CurrentTrail.Points.Count];
            this._speedPoints = new double[App.CurrentRideViewModel.CurrentTrail.Points.Count];
            int index = 0;

            foreach (GeoCoordinate geo in App.CurrentRideViewModel.CurrentTrail.Points)
            {
                double currentAltitude = geo.Altitude;
                double deltaAltitude = currentAltitude - lastAltitude;
                double currentSpeed = geo.Speed;
                
                if( deltaAltitude >= 0.00000000 )
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
        public void GetTrailData(string msg)
        {
            throw new NotImplementedException();
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


    }
}
