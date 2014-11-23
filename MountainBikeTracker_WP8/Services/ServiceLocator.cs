
namespace MountainBikeTracker_WP8.Services
{
    class ServiceLocator
    {
        public static GeolocatorService GeolocatorService
        {
            get { return GeolocatorService.Instance; }
        }

        public static IsolatedStorageService IsolatedStorageService
        {
            get { return IsolatedStorageService.Instance; }
        }

        public static StorageService StorageService
        {
            get { return StorageService.Instance; }
        }

    }
}
