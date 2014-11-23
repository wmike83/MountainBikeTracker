using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Shell;
using MountainBikeTracker_WP8.Helpers;
using MountainBikeTracker_WP8.Models;
using MountainBikeTracker_WP8.Resources;
using System;
using System.Device.Location;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;

namespace MountainBikeTracker_WP8.Views
{
    /// <summary>
    /// Code behind for Current Ride Page
    /// </summary>
    public partial class CurrentRidePage : PhoneApplicationPage
    {
        #region Fields
        private bool lockCenterMap;
        private ApplicationBarIconButton startPauseAppBarButton;
        private ApplicationBarIconButton stopDeleteAppBarButton;
        private ApplicationBarIconButton lockUnlockAppBarButton;
        private ApplicationBarMenuItem saveAppBarMenuItem;

        private MapPolyline mapCurrentRidePolyline = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CurrentRidePage()
        {
            InitializeComponent();

            // Flag to lock center of screen
            this.lockCenterMap = false;

            // Remove auto disable
            PhoneApplicationService phoneServices = PhoneApplicationService.Current;
            phoneServices.UserIdleDetectionMode = IdleDetectionMode.Disabled;

            this.BuildLocalizedApplicationBar();

            // Setting all the binding objects
            this.DataContext = App.CurrentRideViewModel;

            this.SetupMap();
        }
        #endregion

        #region Map Stuff
        /// <summary>
        /// Setup map which creates the current ride layer
        /// </summary>
        private void SetupMap()
        {
            this.SetupMapCurrentRideLayer();
        }

        /// <summary>
        /// Setup map current ride layer
        /// </summary>
        private void SetupMapCurrentRideLayer()
        {
            // Create map polyline
            this.mapCurrentRidePolyline = new MapPolyline();
            // Connect to geo coordinate collection
            this.mapCurrentRidePolyline.Path = App.CurrentRideViewModel.CurrentTrail.Points;
            // set visual elements
            this.mapCurrentRidePolyline.StrokeThickness = 5;
            this.mapCurrentRidePolyline.StrokeColor = Colors.Red;

            // Add map to screen
            this.mapCurrentRide.MapElements.Add(this.mapCurrentRidePolyline);
        }

        /// <summary>
        /// Sets maps center with animation
        /// </summary>
        /// <param name="geoCoord">Geo point to center map too</param>
        private void SetMapCenterAnimated(GeoCoordinate geoCoord)
        {
            this.mapCurrentRide.Dispatcher.BeginInvoke(() =>
            {
                this.mapCurrentRide.SetView(geoCoord,
                                            this.mapCurrentRide.ZoomLevel,
                                            MapAnimationKind.Parabolic);
            });
        }

        /// <summary>
        /// Sets maps center without animation
        /// </summary>
        /// <param name="geoCoord"></param>
        private void SetMapCenter(GeoCoordinate geoCoord)
        {
            this.mapCurrentRide.Dispatcher.BeginInvoke(() =>
            {
                this.mapCurrentRide.SetView(geoCoord,
                                            this.mapCurrentRide.ZoomLevel,
                                            geoCoord.Course,
                                            MapAnimationKind.Linear);
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
                this.stkCurrentSpeedData.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                this.PauseGeolocatorAndUpdateAppBar();

                this.saveAppBarMenuItem.IsEnabled = true;
                this.stkCurrentSpeedData.Background = new SolidColorBrush(Color.FromArgb(255, 160, 160, 160));
            }
        }
        private void StartGeolocatorAndUpdateAppBar()
        {
            this.UpdateStartAppBarToPause();

            App.CurrentRideViewModel.StartRecording();
        }
        private void UpdateStartAppBarToPause()
        {
            ApplicationBarHelper.UpdateAppBarIconButton(this.startPauseAppBarButton,
                                            "/Assets/AppBar/appbar.transport.pause.rest.png",
                                            AppResources.PauseAppBarButtonText);

            this.stopDeleteAppBarButton.IsEnabled = false;
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

            this.saveAppBarMenuItem.IsEnabled = false;

            this.lockUnlockAppBarButton = new ApplicationBarIconButton();
            this.UpdateLockMap();
            this.lockUnlockAppBarButton.Click += this.imgLockUnlock_Click;
        }
        #endregion

        #region Application Bar Events
        private void stopDeleteAppBarButton_Click(object sender, EventArgs e)
        {
            var response = MessageBox.Show(AppResources.DeleteCurrentRequest,
                                           AppResources.DeleteCurrentHistoryLabel,
                                           MessageBoxButton.OKCancel);

            if (response == MessageBoxResult.Cancel)
                return;

            ApplicationBarHelper.UpdateAppBarIconButton(this.startPauseAppBarButton,
                                                        "/Assets/AppBar/appbar.transport.play.rest.png",
                                                        AppResources.StartAppBarButtonText);

            App.CurrentRideViewModel.ResetTrail();

            this.SetMapCenterAnimated(MountainBikeTrail.CreateGeoCoordinate(
                                      Services.ServiceLocator.GeolocatorService.LastPoint));

            this.saveAppBarMenuItem.IsEnabled = false;
        }

        private void startPauseAppBarButton_Click(object sender, EventArgs e)
        {
            this.StartPauseGeolocatorHandler();
        }


        public void AddLockImage()
        {
            this.lockCenterMap = true;
            this.UpdateLockMap();
            // Create a new button and set the text value to the localized string from AppResources.
            this.ApplicationBar.Buttons.Insert(0, this.lockUnlockAppBarButton);
        }
        public void RemoveLockImage()
        {
            this.lockCenterMap = false;
            this.UpdateLockMap();

            if (this.ApplicationBar.Buttons.Contains(this.lockUnlockAppBarButton))
                this.ApplicationBar.Buttons.Remove(this.lockUnlockAppBarButton);
        }

        private void imgLockUnlock_Click(object sender, EventArgs e)
        {
            this.LockUnlockMap();
        }

        private void LockUnlockMap()
        {
            this.lockCenterMap = !this.lockCenterMap;
            this.UpdateLockMap();
        }

        private bool UpdateLockIcon()
        {
            if (this.lockCenterMap)
            {
                this.MapLockAnimation.Begin();
                ApplicationBarHelper.UpdateAppBarIconButton(this.lockUnlockAppBarButton,
                                                            "/Assets/AppBar/appbar.map.unlock.png",
                                                            AppResources.UnlockAppBarButtonText);
            }
            else
            {
                this.MapUnlockAnimation.Begin();
                ApplicationBarHelper.UpdateAppBarIconButton(this.lockUnlockAppBarButton,
                                                            "/Assets/AppBar/appbar.map.lock.png",
                                                            AppResources.LockAppBarButtonText);
            }
            return this.lockCenterMap;
        }
        private void UpdateLockMap()
        {
            if (this.UpdateLockIcon())
            {
                // Lock
                App.CurrentRideViewModel.PropertyChanged += this.CurrentRideViewModel_PropertyChanged;
                this.SetMapCenter(App.CurrentRideViewModel.CurrentLocation);
                this.mapCurrentRide.Pitch = 60;
            }
            else
            {
                // Unlock
                App.CurrentRideViewModel.PropertyChanged -= this.CurrentRideViewModel_PropertyChanged;
                this.mapCurrentRide.Pitch = 0;
                this.mapCurrentRide.Heading = 0;
            }
        }

        void CurrentRideViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentLocation")
            {
                this.SetMapCenter(App.CurrentRideViewModel.CurrentLocation);
            }
        }

        private async void saveAppBarMenuItem_Click(object sender, EventArgs e)
        {
            bool result = await App.CurrentRideViewModel.SaveRideAsync();
            if (!result)
            {
                MessageBox.Show(AppResources.SaveRideFailure);
            }
            else
            {
                NavigationService.GoBack();
            }
        }
        #endregion

        #region Navigation Overrides
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ResummingFromBackground)
            {
                App.CurrentRideViewModel.NavigatingTo();
            }
            else
            {
                if (e.NavigationMode == NavigationMode.New && App.CurrentRideViewModel.IsRecording)
                    this.UpdateStartAppBarToPause();
                App.ResummingFromBackground = false;
            }

            base.OnNavigatedTo(e);
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New && e.IsCancelable)
                App.ResummingFromBackground = true;
            else if (e.NavigationMode == NavigationMode.Back)
            {
                if (App.CurrentRideViewModel.IsRecording)
                {
                    var response = MessageBox.Show(AppResources.DeleteRecordingRequest,
                                                   AppResources.DeleteRecordingLabel,
                                                   MessageBoxButton.OKCancel);

                    if (response == MessageBoxResult.OK)
                    {
                        App.CurrentRideViewModel.NavigatingFrom();
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else if (App.CurrentRideViewModel.CurrentTrail.Points.Count > 0)
                {
                    var response = MessageBox.Show(AppResources.DeleteRecordedRequest,
                                                   AppResources.DeleteCurrentHistoryLabel,
                                                   MessageBoxButton.OKCancel);

                    if (response == MessageBoxResult.OK)
                    {
                        App.CurrentRideViewModel.NavigatingFrom();
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }

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
            this.RemoveLockImage();
        }
        private void SimpleView_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (!e.IsInertial)
            {
                Point currentpoint = e.CumulativeManipulation.Translation;
                if (currentpoint.Y < -15)
                {
                    this.SimpleView.Visibility = Visibility.Collapsed;
                    this.MapView.Visibility = Visibility.Visible;
                    this.SetMapCenterAnimated(MountainBikeTrail.CreateGeoCoordinate(
                                              Services.ServiceLocator.GeolocatorService.LastPoint));
                    e.Complete();
                    this.AddLockImage();
                }
            }
        }
        #endregion

        private void imgMapIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LockUnlockMap();
        }
    }
}