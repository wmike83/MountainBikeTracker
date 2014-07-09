﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MountainBikeTracker_WP8.Models;

namespace MountainBikeTracker_WP8.Views
{
    public partial class HistoryPage : PhoneApplicationPage
    {
        public HistoryPage()
        {
            InitializeComponent();

            this.DataContext = App.DataStore;
        }

        private void MainLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show("");
            NavigationService.Navigate(new Uri("/Views/SaveCurrentRidePage.xaml?msg=SelectedIndex?ind=" + (lstActiveRiders.SelectedItem as Dictionary<string, TrailInformation>).Keys, UriKind.Relative));
        }
    }
}