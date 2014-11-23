using Microsoft.Phone.Maps.Controls;
using MountainBikeTracker_WP8.ViewModels;
using System;
using System.Collections.Generic;
using System.Device.Location;
using Windows.Devices.Geolocation;

namespace MountainBikeTracker_WP8.Models
{
    public class MountainBikeTrail : BindableBase
    {
        #region Fields
        // Data that describes ride
        private double averageSpeed; // Measured in meters per second
        private double distance; // Measured in meters
        private GeoCoordinateCollection points = new GeoCoordinateCollection();
        private TimeSpan duration;
        private TimeSpan totalTime;
        private List<DateTime> timeStamps = new List<DateTime>();

        // Time tracking objects
        private long startTime;
        private long lastTime;
        #endregion

        #region Properties
        public double AverageSpeed
        {
            get { return this.averageSpeed; }
            set { this.averageSpeed = value; }
        }
        public double Distance
        {
            get { return this.distance; }
            set { this.distance = value; }
        }
        public TimeSpan Duration
        {
            get { return this.duration; }
            set { this.SetProperty(ref this.duration, value); }
        }
        public TimeSpan TotalTime
        {
            get { return this.totalTime; }
            set { this.SetProperty(ref this.totalTime, value); }
        }
        public GeoCoordinateCollection Points
        {
            get { return this.points; }
            set { this.points = value; }
        }
        public List<DateTime> TimeStamps
        {
            get { return this.timeStamps; }
            set { this.timeStamps = value; }
        }
        public int TrailID { get; set; }
        #endregion

        #region Constructor
        public MountainBikeTrail()
        {
            // Set values to defaults
            this.Reset();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Resets the trail iformation
        /// <ul>
        ///     <li>Average Speed</li>
        ///     <li>Distance</li>
        ///     <li>Duration</li>
        ///     <li>Points</li>
        /// </ul>
        /// </summary>
        public void Reset()
        {
            // Reset all ride components
            this.AverageSpeed = 0;
            this.Distance = 0;
            this.Duration = new TimeSpan();
            this.Points.Clear();
            this.TimeStamps.Clear();

            this.startTime = 0;
            this.lastTime = 0;
        }
        public GeoCoordinate AddNewPoint(Geocoordinate obj)
        {
            // Getting new and last GeoCoordinate objects
            GeoCoordinate newPoint = MountainBikeTrail.CreateGeoCoordinate(obj);
            GeoCoordinate lastPoint = this.Points.Count > 0 ? this.Points[this.Points.Count - 1] : newPoint;

            // Update the new distance
            double distance = lastPoint.GetDistanceTo(newPoint);

            this.Distance += distance;

            // Get new time stamp
            long newTime = obj.Timestamp.TimeOfDay.Ticks;
            // Update the duration
            // If new time is later than old time
            if (newTime > this.lastTime)
            {
                this.Duration += TimeSpan.FromTicks(newTime - this.lastTime);
                this.lastTime = newTime;
            }

            // Update the new average speed
            double averageSpeed = this.Distance / this.Duration.TotalSeconds;
            this.AverageSpeed = double.IsNaN(averageSpeed) ? 0 : averageSpeed;

            // Update the points
            this.Points.Add(newPoint);

            // Update the timestamps
            this.TimeStamps.Add(new DateTime(obj.Timestamp.LocalDateTime.Ticks));

            this.TotalTime = new TimeSpan(this.lastTime - this.startTime);
            return newPoint;
        }
        #endregion

        #region GeolocatorService Helper Methods
        public void Start()
        {
            // Started new Tracking
            if (this.Duration.TotalMilliseconds == 0)
            {
                this.startTime = DateTime.Now.TimeOfDay.Ticks;
                this.lastTime = this.startTime;
            }
            else // if resuming old tracking
            {
                this.lastTime = DateTime.Now.TimeOfDay.Ticks;
            }
        }
        #endregion

        #region Static Helper Methods
        public static double CalculateDistance(GeoCoordinate start, GeoCoordinate end)
        {
            // This is a big one found from the internet.
            // One of many different ways to calculate distance
            // between two GPS GeoCoordinates.
            // I did add the hypontenus altitude change calculation
            var startLatRads = (start.Latitude * Math.PI) / 180.0;
            var startLonRads = (start.Longitude * Math.PI) / 180.0;
            var destLatRads = (end.Latitude * Math.PI) / 180.0;
            var destLonRads = (end.Longitude * Math.PI) / 180.0;
            var distance = Math.Acos(Math.Sin(startLatRads) * Math.Sin(destLatRads) +
                            Math.Cos(startLatRads) * Math.Cos(destLatRads) *
                            Math.Cos(startLonRads - destLonRads)) * 3961.0; // Half the Diameter in Miles across.  If KM then change to 6375
            double altDelta = (start.Altitude - end.Altitude) * 0.000621371192; // Meters to Feet conversion
            // Adjusting for change in altitude using the formula a^2 + b^2 = c^2
            distance = Math.Sqrt((distance * distance) + (altDelta * altDelta));
            return distance;
        }
        public static double ConvertMetersPerSecondToMilesPerHour(double speed)
        {
            // Obvious
            return speed * 2.23693629;
        }
        public static double ConvertMetersToFeet(double meters)
        {
            return meters * 3.28084;
        }
        public static double ConvertMetersToMiles(double meters)
        {
            return meters * 0.000632371;
        }

        public static Random rand = new Random();
        public static double lastAltitude = double.MinValue;
        public static double lastSpeed = double.MinValue;

        public static GeoCoordinate CreateGeoCoordinate(Geocoordinate geocoordinate)
        {
            // Removing all double? to values or zero
            GeoCoordinate location = new GeoCoordinate()
            {
                Altitude = (geocoordinate.Altitude.HasValue && !double.IsNaN(geocoordinate.Altitude.Value)) ? geocoordinate.Altitude.Value : 0,
                Course = (geocoordinate.Heading.HasValue && !double.IsNaN(geocoordinate.Heading.Value)) ? geocoordinate.Heading.Value : 0,
                HorizontalAccuracy = geocoordinate.Accuracy,
                Latitude = geocoordinate.Latitude,
                Longitude = geocoordinate.Longitude,
                Speed = (geocoordinate.Speed.HasValue && !double.IsNaN(geocoordinate.Speed.Value)) ? geocoordinate.Speed.Value : 0,
                VerticalAccuracy = (geocoordinate.AltitudeAccuracy.HasValue && !double.IsNaN(geocoordinate.AltitudeAccuracy.Value)) ? geocoordinate.AltitudeAccuracy.Value : 0
            };

            // For testing using debugger
            if (System.Diagnostics.Debugger.IsAttached)
            {
                if (lastAltitude == double.MinValue)
                    lastAltitude = 133.4;
                if (lastSpeed == double.MinValue)
                    lastSpeed = 5.3;

                location.Altitude = ((rand.NextDouble() - 0.5) * 10) + lastAltitude;
                location.Speed = ((rand.NextDouble() - 0.5) * 3) + lastSpeed;

                lastAltitude = location.Altitude;
                lastSpeed = location.Speed;
            }

            return location;
        }
        #endregion

    }

}
