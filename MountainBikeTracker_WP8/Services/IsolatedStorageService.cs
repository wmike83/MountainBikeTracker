using System.IO.IsolatedStorage;

namespace MountainBikeTracker_WP8.Services
{
    public class IsolatedStorageService
    {
        #region Singleton Instance
        private static IsolatedStorageService _instance = null;
        public static IsolatedStorageService Instance { get { return _instance ?? (_instance = new IsolatedStorageService()); } }
        #endregion

        #region Constructor
        private IsolatedStorageService()
        {
            this.storage = IsolatedStorageSettings.ApplicationSettings;
        }
        #endregion

        private IsolatedStorageSettings storage;
        
        public bool TryUpdateValue<T>(string key, T Value)
        {
            if (this.storage.Contains(key))
            {
                this.storage[key] = Value;
                this.storage.Save();
                return true;
            }
            else
                return false;
        }

        public bool TryAddValue<T>(string key, T value)
        {
            if (!this.storage.Contains(key))
            {
                this.storage.Add(key, value);
                this.storage.Save();
                return true;
            }
            else
                return false;
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            return this.storage.TryGetValue(key, out value);
        }
    }
}
