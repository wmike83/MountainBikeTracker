﻿using MountainBikeTracker_WP8.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
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
        private double _totalAscend;
        private double _totalDescend;
        private double _maxElevation;
        private double _minElevation;
        private double[] _elevationPoints;
        private double _maxSpeed;
        private double _minSpeed;
        private double[] _speedPoints;

        public MountainBikeTrail CurrentTrail { get; private set; }
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
                return MountainBikeTrail.ConvertMetersPerSecondToMilesPerHour(this._maxSpeed);
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
        public SaveCurrentRideViewModel()
        {
            this._maxElevation = double.MinValue;
            this._minElevation = double.MaxValue;
            this._maxSpeed = double.MinValue;
            this._minSpeed = double.MaxValue;
            this.GetTrailData();
        }

        public void GetTrailData()
        {
            this.CurrentTrail = App.CurrentRideViewModel.CurrentTrail;
            this.TrailInfo = new TrailInformation()
            {
                Date = App.CurrentRideViewModel.CurrentTrail.TimeStamps.FirstOrDefault<DateTime>(),
                City = ""//Services.ServiceLocator.GeolocatorService.LastCity
            };
            //GeoCoordinate lastGeo = App.CurrentRideViewModel.CurrentTrail.Points.FirstOrDefault<GeoCoordinate>();
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
            }
        }
    }
}
