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
            this.cvsLineGraph.Width = points.Length > this.cvsLineGraph.ActualWidth ? points.Length*4 : this.cvsLineGraph.ActualWidth;
            this.UpdateLayout();
            this.plnLineGraph.Width = this.cvsLineGraph.ActualWidth;
            this.UpdateLayout();

            double normalizedX = 0;
            double x_difference = this.cvsLineGraph.ActualWidth / (points.Length - 1);
            double y_difference = (this.cvsLineGraph.ActualHeight * (1.0 / 2.0));
            double normalizedY = 0;
            this.plnLineGraph.Points.Add(new Point(0, this.cvsLineGraph.ActualHeight));
            foreach (var point in points)
            {
                normalizedY = (((point - min) / (max - min)) * y_difference) + (y_difference / 2.0);

                this.plnLineGraph.Points.Add(new Point(normalizedX, normalizedY));
                normalizedX += x_difference;
            }
            this.plnLineGraph.Points.Add(new Point(this.cvsLineGraph.ActualWidth, this.cvsLineGraph.ActualHeight));
        }

        public void AddAverageLineToGraph(double average, double max, double min)
        {
            double y_difference = (this.cvsLineGraph.ActualHeight * (1.0 / 2.0));
            double normalizedY = (((average - min) / (max - min)) * y_difference) + (y_difference / 2.0);
            Polyline p = new Polyline();
            p.Height = this.cvsLineGraph.ActualHeight;
            p.Stroke = new SolidColorBrush(Colors.Red);
            p.StrokeThickness = 3;
            p.Width = this.cvsLineGraph.ActualWidth;
            p.Points.Add(new Point(0, normalizedY));
            p.Points.Add(new Point(this.cvsLineGraph.ActualWidth, normalizedY));
            this.cvsLineGraph.Children.Add(p);
        }
    }
}
