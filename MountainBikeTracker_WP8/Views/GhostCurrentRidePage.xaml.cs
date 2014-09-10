using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using System.Windows.Media;
using MountainBikeTracker_WP8.Resources;
using MountainBikeTracker_WP8.Helpers;
using MountainBikeTracker_WP8.Models;

namespace MountainBikeTracker_WP8.Views
{
    public partial class GhostCurrentRidePage : PhoneApplicationPage
    {
        #region Fields
        private ApplicationBarIconButton startPauseAppBarButton;
        private ApplicationBarIconButton stopDeleteAppBarButton;
        private ApplicationBarMenuItem saveAppBarMenuItem;

        private MapPolyline mapCurrentRidePolyline = null;
        private MapPolyline mapGhostRidePolyline = null;
        #endregion

        #region Constructor
        public GhostCurrentRidePage()
        {
            InitializeComponent();
            
            // Remove auto disable
            PhoneApplicationService phoneServices = PhoneApplicationService.Current;
            phoneServices.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            this.BuildLocalizedApplicationBar();

            // Setting all the binding objects
            this.DataContext = App.GhostCurrentRideViewModel;

            // Setup Map
            this.SetupMap();
        }
        #endregion

        #region Map Stuff
        private void SetupMap()
        {
            this.SetupMapCurrentRideLayer();
            this.SetupMapGhostRideLayer();
        }
        private void SetupMapGhostRideLayer()
        {
            this.mapGhostRidePolyline = new MapPolyline();
            this.mapGhostRidePolyline.Path = App.GhostCurrentRideViewModel.GhostTrail;
            this.mapGhostRidePolyline.StrokeThickness = 5;
            this.mapGhostRidePolyline.StrokeColor = Colors.Gray;

            this.mapCurrentRide.MapElements.Add(this.mapGhostRidePolyline);
        }
        private void SetupMapCurrentRideLayer()
        {
            this.mapCurrentRidePolyline = new MapPolyline();
            this.mapCurrentRidePolyline.Path = App.GhostCurrentRideViewModel.CurrentTrail.Points;
            this.mapCurrentRidePolyline.StrokeThickness = 5;
            this.mapCurrentRidePolyline.StrokeColor = Colors.Red;

            this.mapCurrentRide.MapElements.Add(this.mapCurrentRidePolyline);
        }
        private void SetMapCenter(GeoCoordinate geoCoord)
        {
            this.mapCurrentRide.Dispatcher.BeginInvoke(() =>
            {
                this.mapCurrentRide.SetView(geoCoord,
                                            this.mapCurrentRide.ZoomLevel,
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

                this.saveAppBarMenuItem.IsEnabled = false;
            }
            else
            {
                this.PauseGeolocatorAndUpdateAppBar();

                this.saveAppBarMenuItem.IsEnabled = true;
            }
        }
        private void StartGeolocatorAndUpdateAppBar()
        {
            ApplicationBarHelper.UpdateAppBarIconButton(this.startPauseAppBarButton,
                                                        "/Assets/AppBar/appbar.transport.pause.rest.png",
                                                        AppResources.PauseAppBarButtonText);

            this.stopDeleteAppBarButton.IsEnabled = false;

            App.GhostCurrentRideViewModel.StartRecording();
        }
        private void PauseGeolocatorAndUpdateAppBar()
        {
            ApplicationBarHelper.UpdateAppBarIconButton(this.startPauseAppBarButton,
                                                        "/Assets/AppBar/appbar.transport.play.rest.png",
                                                        AppResources.StartAppBarButtonText);

            this.stopDeleteAppBarButton.IsEnabled = true;

            App.GhostCurrentRideViewModel.PauseRecording();
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

            this.saveAppBarMenuItem.IsEnabled = false;
        }
        #endregion

        #region Application Bar Events
        private void stopDeleteAppBarButton_Click(object sender, EventArgs e)
        {
            ApplicationBarHelper.UpdateAppBarIconButton(this.startPauseAppBarButton,
                                                        "/Assets/AppBar/appbar.transport.play.rest.png",
                                                        AppResources.StartAppBarButtonText);

            App.GhostCurrentRideViewModel.ResetTrail();
            this.SetMapCenter(MountainBikeTrail.CreateGeoCoordinate(Services.ServiceLocator.GeolocatorService.LastPoint));
            this.saveAppBarMenuItem.IsEnabled = false;
        }
        private void startPauseAppBarButton_Click(object sender, EventArgs e)
        {
            this.StartPauseGeolocatorHandler();
        }
        private void saveAppBarMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/SaveCurrentRidePage.xaml?msg=SaveButton", UriKind.Relative));
        }
        #endregion

        #region Navigation Overrides
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string msg = "";
            if (NavigationContext.QueryString.TryGetValue("msg", out msg))
            {
                ulong key = Convert.ToUInt64(msg);
                App.GhostCurrentRideViewModel.SetGhostFinishedTrail(App.DataStore.Trails[key].Trail);
                App.GhostCurrentRideViewModel.ResetTrail();
                Services.ServiceLocator.GeolocatorService.Start();
                App.GhostCurrentRideViewModel.StartListening();
            }

        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            Services.ServiceLocator.GeolocatorService.Pause();
            App.GhostCurrentRideViewModel.StopListening();
        }
        #endregion


    }
}