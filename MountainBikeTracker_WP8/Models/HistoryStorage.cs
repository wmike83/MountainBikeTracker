using MountainBikeTracker_WP8.Models;
using System;
using System.Linq;

namespace GeoGenius.WindowsPhone8._0.Models
{
    public class HistoryStorage : StorageBase
    {
        #region History Properties
        public DateTime Date { get; set; }
        public double Distance { get; set; }
        public double AverageSpeed { get; set; }
        #endregion

        #region History Static Variables
        private static string _historyFolder = "HistoryFolder";
        public static string HistoryFolder { get { return _historyFolder; } }

        private static string _historyFilePrefix = "HistoryFile";
        public static string HistoryFilePrefix { get { return _historyFilePrefix; } }

        private static string _historyFileExtension = ".json";
        public static string HistoryFileExtension { get { return _historyFileExtension; } }
        #endregion

        public static HistoryStorage BuildHistoryStorage(MountainBikeTrail ride)
        {
            var timeStamp = ride.TimeStamps.FirstOrDefault();
            string filePath = HistoryStorage.HistoryFilePrefix + timeStamp.Ticks + HistoryStorage.HistoryFileExtension;

            return new HistoryStorage()
            {
                AverageSpeed = MountainBikeTrail.ConvertMetersPerSecondToMilesPerHour(ride.AverageSpeed),
                Distance = MountainBikeTrail.ConvertMetersToMiles(ride.Distance),
                Date = timeStamp,
                FolderPath = HistoryStorage.HistoryFolder,
                FilePath = filePath
            };
        }
    }
}
