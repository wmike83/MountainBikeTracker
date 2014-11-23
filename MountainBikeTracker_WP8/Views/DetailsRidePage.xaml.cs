using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using System;
using System.Device.Location;
using System.Windows.Media;
using System.Windows.Navigation;

namespace MountainBikeTracker_WP8.Views
{
    /// <summary>
    /// Code behind for the Details Ride Page
    /// Used to read history
    /// </summary>
    public partial class DetailsRidePage : PhoneApplicationPage
    {
        #region Fields
        /// <summary>
        /// Map polyline used to project the ride
        /// </summary>
        private MapPolyline mapCurrentRidePolyline = null;
        #endregion

        #region Cosntructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DetailsRidePage()
        {
            InitializeComponent();

            // Setting up the binding object
            this.DataContext = App.DetailsRideViewModel;
        }
        #endregion

        #region Map Methods
        /// <summary>
        /// Setup the map on the UI and create the map current line
        /// </summary>
        private void SetupMap()
        {
            // Create and add ride layer
            this.SetupMapRideLayer();
            // Set map center to center point
            this.SetMapCenter(App.DetailsRideViewModel.CenterPoint);
        }

        /// <summary>
        /// Creates map ride layer
        /// </summary>
        private void SetupMapRideLayer()
        {
            // Create map ride polyline
            this.mapCurrentRidePolyline = new MapPolyline();
            // Connect to geo coordinate collection
            this.mapCurrentRidePolyline.Path = App.DetailsRideViewModel.Trail.Points;
            // Set visual settings
            this.mapCurrentRidePolyline.StrokeThickness = 5;
            this.mapCurrentRidePolyline.StrokeColor = Colors.Red;

            // Add to map on screen
            this.mapRide.MapElements.Add(this.mapCurrentRidePolyline);
        }

        /// <summary>
        /// Set map center and update UI
        /// </summary>
        /// <param name="geoCoord"></param>
        private void SetMapCenter(GeoCoordinate geoCoord)
        {
            // Marshal to UI thread
            this.mapRide.Dispatcher.BeginInvoke(() =>
            {
                // Set map view to geo point with default zoom level and with animation
                this.mapRide.SetView(geoCoord,
                                     this.mapRide.ZoomLevel,
                                     MapAnimationKind.Parabolic);
            });
        }
        #endregion

        #region Navigation Events
        /// <summary>
        /// Event triggered when the page is navigated to
        /// </summary>
        /// <param name="e">event args</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Store navigation string
            string msg = "";
            if (NavigationContext.QueryString.TryGetValue("index", out msg))
            {
                // Read value and convert to int
                int index = Convert.ToInt32(msg);
                // Read point
                await App.DetailsRideViewModel.ReadTrailAsync(index);
                // Setup map
                this.SetupMap();
            }

            base.OnNavigatedTo(e);
        }
        #endregion
    }
}