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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string msg = "";
            if (NavigationContext.QueryString.TryGetValue("msg", out msg))
                switch ( msg )
                {
                    case "SaveButton":
                        {
                            App.SaveCurrentRideViewModel.GetCurrentTrailData();
                            break;
                        }
                    case"SelectedIndex":
                        {
                            string ind = "";
                            if (NavigationContext.QueryString.TryGetValue("ind", out ind))
                            {
                                App.SaveCurrentRideViewModel.GetTrailData(Convert.ToUInt64(ind));
                                this.btnSave.IsEnabled = false;

                            }
                            break;
                        }
                    default:
                         {
                             // Don't know!
                             break;
                         }
                }
        }
        void SaveCurrentRidePage_Loaded(object sender, RoutedEventArgs e)
        {
            double[] values1 = new double[1000];
            double[] values2 = new double[1000];
            Random rand = new Random();
            for (int i = 0; i < 1000; i++ )
            {
                values1[i] = rand.Next(2, 25);
                values2[i] = rand.Next(0, 12);
            }
            this.lnhElevation.AddPointsToGraph(values1, values2.Max(), values2.Min());
            this.lnhSpeed.AddPointsToGraph(values2, values2.Max(), values2.Min());
            this.lnhSpeed.AddAverageLineToGraph(values2.Average(), values2.Max(), values2.Min());

            //this.lnhElevation.AddPointsToGraph(App.SaveCurrentRideViewModel.ElevationPoints, App.SaveCurrentRideViewModel.MaxElevation, App.SaveCurrentRideViewModel.MinElevation);
            //this.lnhSpeed.AddPointsToGraph(App.SaveCurrentRideViewModel.SpeedPoints, App.SaveCurrentRideViewModel.MaxSpeed, App.SaveCurrentRideViewModel.MinSpeed);
            //this.lnhSpeed.AddAverageLineToGraph((App.SaveCurrentRideViewModel.TrailInfo.Trail.AverageSpeed * 0.4470400004105615), App.SaveCurrentRideViewModel.MaxSpeed, App.SaveCurrentRideViewModel.MinSpeed);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to Current Ride
            NavigationService.GoBack();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            App.SaveCurrentRideViewModel.TrailInfo.Date = this.dtpDate.Value ?? DateTime.Now;
            // Removing all backstack
            NavigationService.RemoveBackEntry();
            // Navigate back to Main Page
            NavigationService.GoBack();
        }
    }
}