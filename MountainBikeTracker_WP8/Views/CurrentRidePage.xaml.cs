using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Device.Location;
using MountainBikeTracker_WP8.Models;
using MountainBikeTracker_WP8.Resources;
using MountainBikeTracker_WP8.Helpers;
using Windows.Devices.Geolocation;
using System.Windows.Data;

namespace MountainBikeTracker_WP8.Views
{
    public partial class CurrentRidePage : PhoneApplicationPage
    {
        #region Fields
        private ApplicationBarIconButton startPauseAppBarButton;
        private ApplicationBarIconButton stopDeleteAppBarButton;
        private ApplicationBarMenuItem saveAppBarMenuItem;

        private MapPolyline mapCurrentRidePolyline = null;
        #endregion

        #region Constructor
        public CurrentRidePage()
        {
            InitializeComponent();
            
            // Remove auto disable
            PhoneApplicationService phoneServices = PhoneApplicationService.Current;
            phoneServices.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            this.BuildLocalizedApplicationBar();

            // Setting all the binding objects
            this.DataContext = App.CurrentRideViewModel;

            // Setup Map
            this.SetupMap();
        }
        #endregion

        #region Map Stuff
        private void SetupMap()
        {
            this.SetupMapCurrentRideLayer();
        }
        private void SetupMapCurrentRideLayer()
        {
            this.mapCurrentRidePolyline = new MapPolyline();
            this.mapCurrentRidePolyline.Path = App.CurrentRideViewModel.CurrentTrail.Points;
            this.mapCurrentRidePolyline.StrokeThickness = 5;
            this.mapCurrentRidePolyline.StrokeColor = Colors.Red;

            this.mapCurrentRide.MapElements.Add(this.mapCurrentRidePolyline);
        }
        private void SetMapCenter(GeoCoordinate geoCoord)
        {
            this.mapCurrentRide.Dispatcher.BeginInvoke(() =>
            {
                this.mapCurrentRide.SetView(geoCoord,
                                            15,
                                            MapAnimationKind.Parabolic);
            });
        }
        #endregion

        #region Application Bar Helper Methods
        private void StartPauseGeolocatorHandler()
        {
            if (this.startPauseAppBarButton.Text == AppResources.StartAppBarButtonText)
            {
                this.StartGeolocatorAndUpdateAppBar();
            }
            else
            {
                this.PauseGeolocatorAndUpdateAppBar();
            }
                this.SetCurrentListener();
        }
        private void StartGeolocatorAndUpdateAppBar()
        {
            ApplicationBarHelper.UpdateAppBarIconButton(this.startPauseAppBarButton,
                                                        "/Assets/AppBar/appbar.transport.pause.rest.png",
                                                        AppResources.PauseAppBarButtonText);

            this.stopDeleteAppBarButton.IsEnabled = false;

            App.CurrentRideViewModel.StartRecording();
        }
        private void PauseGeolocatorAndUpdateAppBar()
        {
            ApplicationBarHelper.UpdateAppBarIconButton(this.startPauseAppBarButton,
                                                        "/Assets/AppBar/appbar.transport.play.rest.png",
                                                        AppResources.StartAppBarButtonText);

            this.stopDeleteAppBarButton.IsEnabled = true;

            App.CurrentRideViewModel.PauseRecording();
        }
        #endregion

        #region Application Bar
        // Sample code for building a localized ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // Set the page's ApplicationBar to a new instance of ApplicationBar.
            this.ApplicationBar = new ApplicationBar();

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarHelper.SetupAppBarIconButton(this.ApplicationBar,
                                                       out this.startPauseAppBarButton,
                                                       AppResources.StartAppBarButtonText,
                                                       "/Assets/AppBar/appbar.transport.play.rest.png",
                                                       this.startPauseAppBarButton_Click);

            // Create a new button and set the text value to the localized string from AppResources.
            ApplicationBarHelper.SetupAppBarIconButton(this.ApplicationBar,
                                                       out this.stopDeleteAppBarButton,
                                                       AppResources.DeleteAppBarButtonText,
                                                       "/Assets/AppBar/appbar.delete.rest.png",
                                                       this.stopDeleteAppBarButton_Click);

            // Create a new menu item with the localized string from AppResources.
            ApplicationBarHelper.SetupAppBarMenuItem(this.ApplicationBar,
                                                     out this.saveAppBarMenuItem,
                                                     AppResources.SaveAppBarMenuItemText,
                                                     this.saveAppBarMenuItem_Click);
        }
        #endregion

        #region Application Bar Events
        private void stopDeleteAppBarButton_Click(object sender, EventArgs e)
        {
            ApplicationBarHelper.UpdateAppBarIconButton(this.startPauseAppBarButton,
                                                        "/Assets/AppBar/appbar.transport.play.rest.png",
                                                        AppResources.StartAppBarButtonText);

            App.CurrentRideViewModel.ResetTrail();
        }
        private void startPauseAppBarButton_Click(object sender, EventArgs e)
        {
            this.StartPauseGeolocatorHandler();
        }
        private void saveAppBarMenuItem_Click(object sender, EventArgs e)
        {
            //            this.ProgressBar(true);
            //            SystemTray.ProgressIndicator.Text = "Saving...";

            //            SystemTray.ProgressIndicator.IsIndeterminate = false;
            //            SystemTray.ProgressIndicator.IsVisible = false;
            //            SystemTray.ProgressIndicator.Text = "";

            // For now email the ride
            //var task = new EmailComposeTask();
            //task.To = "wmike83@yahoo.com";
            //task.Subject = "Ride " + DateTime.Now;
            //task.Body = App.User.CurrentRideViewModel.ToString();
            //task.Show();

            //NavigationService.Navigate(new Uri("/Views/SaveCurrentRidePage.xaml", UriKind.Relative));
        }
        #endregion

        #region Navigation Overrides
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }
        #endregion

        #region Screen Events
        private void stkCurrentSpeed_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.StartPauseGeolocatorHandler();
        }
        private void grdMapStatistics_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            // Make simple view show
            this.SimpleView.Visibility = Visibility.Visible;
            this.MapView.Visibility = Visibility.Collapsed;
            this.SetCurrentListener();
        }

        private Point _initialPoint;
        private void SimpleView_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            this._initialPoint = e.ManipulationOrigin;
        }

        private void SimpleView_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (!e.IsInertial)
            {
                Point currentpoint = e.DeltaManipulation.Translation;
                if (currentpoint.Y < 2)
                {
                    this.SimpleView.Visibility = Visibility.Collapsed;
                    this.MapView.Visibility = Visibility.Visible;
                    this.SetMapCenter(MountainBikeTrail.CreateGeoCoordinate(Services.ServiceLocator.GeolocatorService.LastPoint));
                    this.SetCurrentListener();
                    e.Complete();
                }
            }
        }
        #endregion

        #region Helper Methods
        private void SetCurrentListener()
        {
            if (this.startPauseAppBarButton.Text == AppResources.StartAppBarButtonText)
            {
                if (this.MapView.Visibility == Visibility.Visible)
                {
                    App.CurrentRideViewModel.StartListening();
                }
                else
                {
                    App.CurrentRideViewModel.StopListening();
                }
            }
        }
        #endregion
    }
}