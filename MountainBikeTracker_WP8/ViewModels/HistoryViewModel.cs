using GeoGenius.WindowsPhone8._0.Models;
using MountainBikeTracker_WP8.Models;
using MountainBikeTracker_WP8.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MountainBikeTracker_WP8.ViewModels
{
    public class HistoryViewModel : BindableBase
    {
        #region Class Variables
        private static string StorageKey = "historyData";
        #endregion

        #region Fields
        private ObservableCollection<HistoryStorage> history;
        #endregion

        #region Properties
        public ObservableCollection<HistoryStorage> History
        {
            get { return this.history; }
            set { this.SetProperty(ref this.history, value); }
        }
        #endregion

        #region Constructor
        public HistoryViewModel()
        {
            this.History = new ObservableCollection<HistoryStorage>();
        }
        #endregion

        #region Methods
        public void LoadHistorySettingsData()
        {
            ObservableCollection<HistoryStorage> history;
            if (ServiceLocator.IsolatedStorageService
                .TryGetValue(StorageKey, out history))
            {
                App.HistoryViewModel.History = history;
            }
        }

        public void SaveHistorySettingsData()
        {
            var history = App.HistoryViewModel.History;
            if (!ServiceLocator.IsolatedStorageService
                .TryUpdateValue(StorageKey, history))
            {
                ServiceLocator.IsolatedStorageService
                    .TryAddValue(StorageKey, history);
            }
        }

        public void AddToHistory(HistoryStorage history)
        {
            App.HistoryViewModel.LoadHistorySettingsData();
            App.HistoryViewModel.History.Add(history);
            App.HistoryViewModel.SaveHistorySettingsData();
        }

        public async Task<MountainBikeTrail> LoadHistoryAsync(int index)
        {
            App.HistoryViewModel.LoadHistorySettingsData();
            return await Services.ServiceLocator.StorageService.
                ReadSavedRideDataAsync<MountainBikeTrail>
                (App.HistoryViewModel.History[index]);
        }

        public async void DeleteHistoryAsync(int index)
        {
            if (await Services.ServiceLocator.StorageService.
                DeleteSavedRideAsync(App.HistoryViewModel.History[index]))
            {
                App.HistoryViewModel.History.RemoveAt(index);
                App.HistoryViewModel.SaveHistorySettingsData();
            }
        }
        #endregion
    }
}
