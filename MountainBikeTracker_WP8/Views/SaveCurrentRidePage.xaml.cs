using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MountainBikeTracker_WP8.Views
{
    public partial class SaveCurrentRidePage : PhoneApplicationPage
    {
        public SaveCurrentRidePage()
        {
            InitializeComponent();

            this.DataContext = App.SaveCurrentRideViewModel;

            this.Loaded += SaveCurrentRidePage_Loaded;
        }

        void SaveCurrentRidePage_Loaded(object sender, RoutedEventArgs e)
        {
            this.lnhElevation.AddPointsToGraph(App.SaveCurrentRideViewModel.ElevationPoints, App.SaveCurrentRideViewModel.MaxElevation, App.SaveCurrentRideViewModel.MinElevation);
            this.lnhSpeed.AddPointsToGraph(App.SaveCurrentRideViewModel.SpeedPoints, App.SaveCurrentRideViewModel.MaxSpeed, App.SaveCurrentRideViewModel.MinSpeed);
            this.lnhSpeed.AddAverageLineToGraph(App.SaveCurrentRideViewModel.CurrentTrail.AverageSpeed, App.SaveCurrentRideViewModel.MaxSpeed, App.SaveCurrentRideViewModel.MinSpeed);
        }
    }
}