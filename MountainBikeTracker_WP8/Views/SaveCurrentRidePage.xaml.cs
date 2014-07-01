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
            this.usrLineGraph.AddPointsToGraph(App.SaveCurrentRideViewModel.Points, App.SaveCurrentRideViewModel.MaxElevation, App.SaveCurrentRideViewModel.MinElevation);
        }


    }
}