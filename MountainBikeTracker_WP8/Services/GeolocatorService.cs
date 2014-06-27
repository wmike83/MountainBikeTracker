﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace MountainBikeTracker_WP8.Services
{
    class GeolocatorService
    {
        #region Fields
        private Geolocator _locator;
        private Geoposition _lastPosition;
        #endregion

        #region Properties
        public Action<Geocoordinate> OnPositionChanged { get; set; }
        public Action<PositionStatus> OnStatusChanged { get; set; }
        public Geocoordinate LastPoint
        {
            get
            {
                return this._lastPosition.Coordinate;
            }
        }
        public string LastCity
        {
            get
            {
                return "";
            }
        }
        #endregion

        #region Constructor
        public GeolocatorService()
        {
        }
        #endregion

        #region Helper Methods
        public void Pause()
        {
            if (HasStarted())
            {
                this._locator.PositionChanged -= this._locator_PositionChanged;
            }
        }

        public void Start()
        {
            if (HasStarted())
                this._locator.PositionChanged += this._locator_PositionChanged;
        }
        public bool HasStarted()
        {
            if (this._locator == null)
            {
                this._locator = new Geolocator();
                this._locator.DesiredAccuracy = PositionAccuracy.High;
                this._locator.DesiredAccuracyInMeters = 5;
                this._locator.MovementThreshold = 1;

                this._locator.StatusChanged += this._locator_StatusChanged;

                return false;
            }
            return true;
        }
        #endregion

        #region Events
        private void _locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            this._lastPosition = args.Position;
            if (this.OnPositionChanged != null)
            {
                this.OnPositionChanged(args.Position.Coordinate);
            }
        }
        private async void _locator_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            if (args.Status == PositionStatus.Ready)
            {
                this._lastPosition = await this._locator.GetGeopositionAsync();

                //// Windows phone 8.1 code only
                //// reverse geocoding
                //BasicGeoposition myLocation = new BasicGeoposition
                //{
                //    Longitude = this._lastPosition.Coordinate.Longitude,
                //    Latitude = this._lastPosition.Coordinate.Latitude
                //};
                //Geopoint pointToReverseGeocode = new Geopoint(myLocation);

                //MapLocationFinderResult result = await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);

                //// here also it should be checked if there result isn't null and what to do in such a case
                //string city = result.Locations[0].Address.Country;
            }
            if (this.OnStatusChanged != null)
            {
                this.OnStatusChanged(args.Status);
            }
        }
        #endregion
    }
}
