using GeoGenius.WindowsPhone8._0.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace MountainBikeTracker_WP8.Services
{
    public class StorageService
    {
        #region Singleton Instance
        private static StorageService _instance = null;
        public static StorageService Instance { get { return _instance ?? (_instance = new StorageService()); } }
        #endregion

        #region Constructor
        public StorageService()
        {
            this.local = Windows.Storage.ApplicationData.Current.LocalFolder;
        }
        #endregion

        public StorageFolder local;

        public async Task<bool> SaveCurrentRideDataAsync<T>(T ride, StorageBase storage)
        {
            // Create the json data from the ride. 
            byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ride));

            try
            {
                // Create a new folder name HistoryFolder.
                var dataFolder = await this.local.CreateFolderAsync(storage.FolderPath,
                                                                    CreationCollisionOption.OpenIfExists);

                // Create a new file named HistoryFile{Date}.json.
                var file = await dataFolder.CreateFileAsync(storage.FilePath,
                                                            CreationCollisionOption.ReplaceExisting);

                // Write the data from the ride.
                using (var s = await file.OpenStreamForWriteAsync())
                {
                    s.Write(fileBytes, 0, fileBytes.Length);
                }

                return true;
            }
            catch
            {
                // Don't know
            }
            return false;
        }

        public async Task<T> ReadSavedRideDataAsync<T>(StorageBase history)
        {
            if (this.local != null)
            {
                try
                {
                    // Get the DataFolder folder.
                    var dataFolder = await this.local.GetFolderAsync(history.FolderPath);

                    // Get the file.
                    var file = await dataFolder.OpenStreamForReadAsync(history.FilePath);

                    // Read the data.
                    using (StreamReader streamReader = new StreamReader(file))
                    {
                        var jsonRide = streamReader.ReadToEnd();
                        var ride = JsonConvert.DeserializeObject<T>(jsonRide);
                        return ride;
                    }
                }
                catch
                {
                    // Don't know
                }
            }
            return default(T);
        }

        public async Task<bool> DeleteSavedRideAsync(StorageBase history)
        {
            var folderPath = history.FolderPath;
            var filePath = history.FilePath;

            if (this.local != null)
            {
                try
                {
                    // Get the DataFolder folder.
                    var dataFolder = await this.local.GetFolderAsync(folderPath);

                    var dataFile = await dataFolder.GetFileAsync(filePath);
                    await dataFile.DeleteAsync();
                    return true;
                }
                catch
                {
                    // Something happened
                }
            }

            return false;
        }
    }
}
