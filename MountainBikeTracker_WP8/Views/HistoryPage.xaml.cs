using Microsoft.Phone.Controls;
using MountainBikeTracker_WP8.Resources;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace MountainBikeTracker_WP8.Views
{
    /// <summary>
    /// Code behind for History Page
    /// </summary>
    public partial class HistoryPage : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HistoryPage()
        {
            InitializeComponent();

            // Set Data Binding object
            this.DataContext = App.HistoryViewModel;
        }

        /// <summary>
        /// Event triggered when the page is navigated to
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Load history settings data
            App.HistoryViewModel.LoadHistorySettingsData();

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Event triggered with an item in the list is held for a period of time.
        /// Event will delete the selected item.  Must select the item first.
        /// </summary>
        /// <param name="sender">object that triggered the click event</param>
        /// <param name="e">event args</param>
        private void lstHistory_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // Get the selected index
            var selectedIndex = this.lstHistory.SelectedIndex;
            // If selected index is valid then delete
            if (selectedIndex > -1)
            {
                // Verify the delete process
                var response = MessageBox.Show(AppResources.DeleteHistoryRequest
                                               + App.HistoryViewModel.History[selectedIndex].Date + "?",
                                               AppResources.DeleteCurrentHistoryLabel,
                                               MessageBoxButton.OKCancel);
                
                // If user consented to deleting then delete
                if (response == MessageBoxResult.OK)
                    App.HistoryViewModel.DeleteHistoryAsync(selectedIndex);
            }
        }

        /// <summary>
        /// Event triggered when an item in the list is tapped.
        /// Event will navigate to the selected item
        /// </summary>
        /// <param name="sender">object that triggered the click event</param>
        /// <param name="e">event args</param>
        private void lstHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Grab the selected item and move to the details view
            if (this.lstHistory.SelectedItem != null)
            {
                // Navigate to the details ride page with the selected ride
                NavigationService.Navigate(new Uri("/Views/DetailsRidePage.xaml?index="
                                                   + this.lstHistory.SelectedIndex.ToString(),
                                                   UriKind.Relative));

                this.lstHistory.SelectedItem = null;
            }
        }
    }
}