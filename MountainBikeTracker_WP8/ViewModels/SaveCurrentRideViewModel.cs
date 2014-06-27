using MountainBikeTracker_WP8.Models;
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
        private double _maxSpeed;

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
        public double MaxSpeed
        {
            get
            {
                return MountainBikeTrail.ConvertMetersPerSecondToMilesPerHour(this._maxSpeed);
            }
        }
        public SaveCurrentRideViewModel()
        {
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
            GeoCoordinate lastGeo = App.CurrentRideViewModel.CurrentTrail.Points.FirstOrDefault<GeoCoordinate>();
            foreach (GeoCoordinate geo in App.CurrentRideViewModel.CurrentTrail.Points)
            {
                double deltaAltitude = geo.Altitude - lastGeo.Altitude;
                double currentSpeed = geo.Speed;
                if( deltaAltitude >= 0.00000000 )
                    this._totalAscend += deltaAltitude;
                else
                    this._totalDescend += (deltaAltitude * -1);

                if (currentSpeed > this.MaxSpeed)
                    this._maxSpeed = currentSpeed;
            }
        }
    }
}
