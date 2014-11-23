using GeoGenius.WindowsPhone8._0.Helpers;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MountainBikeTracker_WP8.Resources;
using MountainBikeTracker_WP8.Services;
using System;
using System.Windows;
using Windows.Devices.Geolocation;

namespace MountainBikeTracker_WP8.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            bool consent;
            if (ServiceLocator.IsolatedStorageService.TryGetValue(AppResources.LocationConsentText, out consent) && consent)
            {
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
                    if (!ServiceLocator.IsolatedStorageService.TryUpdateValue(AppResources.LocationConsentText, true))
                        ServiceLocator.IsolatedStorageService.TryAddValue(AppResources.LocationConsentText, true);

                    this.StartAndListenToGeoLocator();
                }
                else
                {
                    if (!ServiceLocator.IsolatedStorageService.TryUpdateValue(AppResources.LocationConsentText, false))
                        ServiceLocator.IsolatedStorageService.TryAddValue(AppResources.LocationConsentText, false);
                }
            }
        }

        private void StartAndListenToGeoLocator()
        {
            if (!Services.ServiceLocator.GeolocatorService.HasStarted())
            {

                Services.ServiceLocator.GeolocatorService.OnStatusChanged += this.OnStatusChanged;

                ProgressBarHelper.ProgressBar(true);
                ProgressBarHelper.ProgressBarText = AppResources.AcquiringGPS;
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

                    ProgressBarHelper.ProgressBar(false);

                    Services.ServiceLocator.GeolocatorService.OnStatusChanged -= this.OnStatusChanged;
                }
            });
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