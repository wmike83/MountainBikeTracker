using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Windows.Devices.Geolocation;
using System;
using MountainBikeTracker_WP8.Resources;

namespace MountainBikeTracker_WP8.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent") && (bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"] == true)
            {
                SystemTray.ProgressIndicator = new ProgressIndicator();

                this.StartAndListenToGeoLocator();

                // User has already consented
                return;
            }
            else
            {
                MessageBoxResult result =
                    MessageBox.Show(AppResources.LocationRequest,
                    AppResources.LocationLabel,
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;

                    SystemTray.ProgressIndicator = new ProgressIndicator();
                    this.StartAndListenToGeoLocator();
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        private void StartAndListenToGeoLocator()
        {
            if (!Services.ServiceLocator.GeolocatorService.HasStarted())
            {

                Services.ServiceLocator.GeolocatorService.OnStatusChanged += this.OnStatusChanged;

                this.ProgressBar(true);
                SystemTray.ProgressIndicator.Text = AppResources.AcquiringGPS;
            }
            else
            {
                this.btnStartNewRide.IsEnabled = true;
            }
        }

        private void OnStatusChanged(Windows.Devices.Geolocation.PositionStatus obj)
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (obj == PositionStatus.Ready)
                {
                    this.btnStartNewRide.IsEnabled = true;

                    this.ProgressBar(false);

                    Services.ServiceLocator.GeolocatorService.OnStatusChanged -= this.OnStatusChanged;
                }
            });
        }
        private void ProgressBar(bool isVisible)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = isVisible;
            SystemTray.ProgressIndicator.IsVisible = isVisible;
        }

        private void btnStartNewRide_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/CurrentRidePage.xaml", UriKind.Relative));
        }
        private void btnViewHistory_Click(object sender, RoutedEventArgs e)
        {
            if (btnStartNewRide.IsEnabled)
                NavigationService.Navigate(new Uri("/Views/HistoryPage.xaml", UriKind.Relative));
        }
    }
}