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
            this.lnhSpeed.AddAverageLineToGraph((App.SaveCurrentRideViewModel.CurrentTrail.AverageSpeed * 0.4470400004105615), App.SaveCurrentRideViewModel.MaxSpeed, App.SaveCurrentRideViewModel.MinSpeed);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            App.SaveCurrentRideViewModel.Reset();
            NavigationService.GoBack();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("HELL");
        }
    }
}