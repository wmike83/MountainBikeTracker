using MountainBikeTracker_WP8.Models;
using System.Device.Location;
using System.Threading.Tasks;

namespace MountainBikeTracker_WP8.ViewModels
{
    public class DetailsRideViewModel : BindableBase
    {
        #region Fields
        private int historyIndex;
        private MountainBikeTrail trail = null;
        private double distance;
        private double maxAltitude;
        private double minAltitude;
        private double elevationGain;
        private double elevationLoss;
        private double averageSpeed;
        private double maxSpeed;
        private double minSpeed;
        private GeoCoordinate centerPoint;
        #endregion

        #region Properties
        public MountainBikeTrail Trail
        {
            get { return this.trail; }
            set { this.SetProperty(ref this.trail, value); }
        }
        public double Distance
        {
            get { return this.distance; }
            set { this.SetProperty(ref this.distance, value); }
        }
        public double MaxAltitude
        {
            get { return this.maxAltitude; }
            set { this.SetProperty(ref this.maxAltitude, value); }
        }
        public double MinAltitude
        {
            get { return this.minAltitude; }
            set { this.SetProperty(ref this.minAltitude, value); }
        }
        public double ElevationGain
        {
            get { return this.elevationGain; }
            set { this.SetProperty(ref this.elevationGain, value); }
        }
        public double ElevationLoss
        {
            get { return this.elevationLoss; }
            set { this.SetProperty(ref this.elevationLoss, value); }
        }
        public double AverageSpeed
        {
            get { return this.averageSpeed; }
            set { this.SetProperty(ref this.averageSpeed, value); }
        }
        public double MaxSpeed
        {
            get { return this.maxSpeed; }
            set { this.SetProperty(ref this.maxSpeed, value); }
        }
        public double MinSpeed
        {
            get { return this.minSpeed; }
            set { this.SetProperty(ref this.minSpeed, value); }
        }
        public GeoCoordinate CenterPoint
        {
            get { return centerPoint; }
            set { this.SetProperty(ref this.centerPoint, value); }
        }
        #endregion

        #region Constructor
        public DetailsRideViewModel()
        {
            this.Trail = new MountainBikeTrail();
            this.Distance = 0;
            this.MaxAltitude = 0;
            this.MinAltitude = 0;
            this.ElevationGain = 0;
            this.ElevationLoss = 0;
            this.AverageSpeed = 0;
            this.MaxSpeed = 0;
            this.MinSpeed = 0;
            this.CenterPoint = new GeoCoordinate(0, 0);
        }
        #endregion

        #region Helper Methods
        public async Task<bool> ReadTrailAsync(int index)
        {
            // Read file
            // Get the local folder.
            var ride = await App.HistoryViewModel.LoadHistoryAsync(index);

            this.historyIndex = index;

            // Figure out stats data
            double altitudeMax = double.MinValue;
            double altitudeMin = double.MaxValue;
            double lastElevation = ride.Points[0].Altitude;
            double elevationGain = 0;
            double elevationLoss = 0;
            double speedMax = double.MinValue;
            double speedMin = double.MaxValue;

            foreach (var point in ride.Points)
            {
                var altitude = point.Altitude;
                if (altitude > altitudeMax)
                    altitudeMax = point.Altitude;
                if (altitude < altitudeMin)
                    altitudeMin = point.Altitude;
                if (altitude > lastElevation)
                    elevationGain += altitude - lastElevation;
                else
                    elevationLoss += lastElevation - altitude;

                lastElevation = altitude;

                var speed = point.Speed;
                if (speed > speedMax)
                    speedMax = speed;
                if (speed < speedMin)
                    speedMin = speed;
            }

            // Set all stats
            App.DetailsRideViewModel.Distance = MountainBikeTrail.ConvertMetersToMiles(ride.Distance);
            App.DetailsRideViewModel.AverageSpeed = MountainBikeTrail.ConvertMetersPerSecondToMilesPerHour(ride.AverageSpeed);
            App.DetailsRideViewModel.Trail = ride;
            App.DetailsRideViewModel.MaxAltitude = MountainBikeTrail.ConvertMetersToFeet(altitudeMax);
            App.DetailsRideViewModel.MinAltitude = MountainBikeTrail.ConvertMetersToFeet(altitudeMin);
            App.DetailsRideViewModel.MaxSpeed = MountainBikeTrail.ConvertMetersPerSecondToMilesPerHour(speedMax);
            App.DetailsRideViewModel.MinSpeed = MountainBikeTrail.ConvertMetersPerSecondToMilesPerHour(speedMin);
            App.DetailsRideViewModel.ElevationGain = MountainBikeTrail.ConvertMetersToFeet(elevationGain);
            App.DetailsRideViewModel.ElevationLoss = MountainBikeTrail.ConvertMetersToFeet(elevationLoss);
            App.DetailsRideViewModel.CenterPoint = ride.Points[0];

            return true;
        }
        #endregion
    }
}
