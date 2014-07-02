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
using System.Windows.Shapes;

namespace MountainBikeTracker_WP8.Views.UserControls
{
    public partial class LineGraph : UserControl
    {
        public LineGraph()
        {
            InitializeComponent();
            this.plnLineGraph.Points = new PointCollection();
        }

        public void AddPointsToGraph(double[] points, double max, double min)
        {

            double normalizedX = 0;
            double x_difference = this.cvsLineGraph.ActualWidth / (points.Length - 1);
            double y_difference = (this.cvsLineGraph.ActualHeight * (1 / 3));
            double normalizedY = 0;
            this.plnLineGraph.Points.Add(new Point(0, y_difference));
            foreach (var point in points)
            {
                normalizedY = (((point - min) / (max - min)) * y_difference) + y_difference;

                this.plnLineGraph.Points.Add(new Point(normalizedX, normalizedY));
                normalizedX += x_difference;
            }
            this.plnLineGraph.Points.Add(new Point(this.cvsLineGraph.ActualWidth, y_difference));
        }

        public void AddAverageLineToGraph(double average, double max, double min)
        {
            double y_difference = (this.cvsLineGraph.ActualHeight * (1 / 3));
            double normalizedY = (((average - min) / (max - min)) * y_difference) + y_difference;
            Polyline p = new Polyline();
            p.Height = this.plnLineGraph.ActualHeight;
            p.Stroke = new SolidColorBrush(Colors.Red);
            p.StrokeThickness = 3;
            p.Width = this.plnLineGraph.ActualHeight;
            p.Points.Add(new Point(0, normalizedY));
            p.Points.Add(new Point(p.Width, normalizedY));
        }
    }
}
