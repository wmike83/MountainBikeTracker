using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace MountainBikeTracker_WP8.Views.UserControls
{
    public partial class LineGraph : UserControl
    {
        public LineGraph()
        {
            InitializeComponent();
            this.plnElevation.Points = new PointCollection();
        }

        public void AddPointsToGraph(double[] points, double max, double min)
        {
            double normalizedX = 0;
            double x_difference = this.cvsLineGraph.ActualWidth / (points.Length - 1);
            double y_difference = this.cvsLineGraph.ActualHeight;
            double normalizedY = 0;
            this.plnElevation.Points.Add(new Point(0, y_difference));
            foreach (var point in points)
            {
                normalizedY = ((point - min) / (max - min)) * y_difference;

                this.plnElevation.Points.Add(new Point(normalizedX, normalizedY));
                normalizedX += x_difference;
            }
            this.plnElevation.Points.Add(new Point(this.cvsLineGraph.ActualWidth, y_difference));
        }
    }
}
