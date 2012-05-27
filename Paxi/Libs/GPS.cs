using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Device.Location;
namespace Paxi.Libs.GPS
{
    /// <summary>
    /// Arguments for distance change events.
    /// </summary>
    public class DistanceEventArgs : EventArgs
    {
        double _diff;

        public double Diff
        {
            get { return _diff; }
            set { _diff = value; }
        }
        public DistanceEventArgs(double diff) { Diff = diff; }
    }
    /// <summary>
    /// GPS class setuping all logic required to get gps location.
    /// Provieds events to notify all listeners about important changes
    /// </summary>
    public class GPS
    {
        IGeoPositionWatcher<GeoCoordinate> _watcher;
        public event EventHandler<DistanceEventArgs> DistanceChanged;
        public IGeoPositionWatcher<GeoCoordinate> Watcher
        {
            get { return _watcher; }
            set { _watcher = value; }
        }
        double _totalDistance, _prevLatitude, _prevLongitude;

        /// <summary>
        /// Longitude of previuous read from GPS instruments
        /// </summary>
        public double PrevLongitude
        {

            get { return _prevLongitude; }
            set { _prevLongitude = value; }
        }

        /// <summary>
        /// Latttitude of previuous read from GPS instruments
        /// </summary>
        public double PrevLatitude
        {
            get { return _prevLatitude; }
            set { _prevLatitude = value; }
        }

        /// <summary>
        /// Total distance made by this instance of GPS class since startTravel() method was called
        /// </summary>
        public double TotalDistance
        {
            get { return _totalDistance; }
            set { _totalDistance = value; }
        }

        /// <summary>
        /// Initializes a GPS distance tracking utility.
        /// </summary>
        public GPS()
        {
            Watcher = null;
            TotalDistance = 0;
            PrevLatitude = 0;
            PrevLongitude = 0;
        }
        /// <summary>
        /// Starts GPS distance tracking
        /// </summary>
        public void startTracking()
        {
            string deviceName = Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceName").ToString();
            if (deviceName == "XDeviceEmulator")
            {
                Watcher = new GeoCoordinateSimulator(34.0568, -117.195);
            }
            else
            {
                Watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
            }

            //Default, High
            Watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            Watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            Watcher.Start();
        }
        /// <summary>
        /// Stops GPS tracking
        /// </summary>
        /// <returns>Distance of trip in KM</returns>
        public double stopTracking()
        {
            Watcher.Stop();
            return TotalDistance;
        }

        /// <summary>
        /// Adds distance from last GPS cooridnates to current GPS cooridinates to current distance.
        /// </summary>
        /// <param name="latitude">GPS latitude</param>
        /// <param name="longitude">GPS longitude</param>
        private void addDistanceByNextCooridinates(double latitude, double longitude)
        {
            if (this.PrevLatitude == 0 || this.PrevLongitude == 0)
            {
                this.PrevLatitude = latitude;
                this.PrevLongitude = longitude;
                return;
            }
            double diff = this.CalculateGreatCircleDistance(this.PrevLatitude, this.PrevLongitude, latitude, longitude);
            OnDistanceChanged(new DistanceEventArgs(diff));
            this.TotalDistance += diff;
            this.PrevLatitude = latitude;
            this.PrevLongitude = longitude;
        }
        /// <summary>
        /// Calculates distance between 2 points on Earth Hemisphere
        /// </summary>
        /// <param name="lat1">Latitude of starting point</param>
        /// <param name="long1">Longitude of starting point</param>
        /// <param name="lat2">Latitude of end point</param>
        /// <param name="long2">Longitude of end point</param>
        /// <returns></returns>
        private double CalculateGreatCircleDistance(double lat1, double long1, double lat2, double long2)
        {
            return 6367 * Math.Acos(
                Math.Sin(lat1) * Math.Sin(lat2)
                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(long2 - long1));
        }



        #region Delegates
        /// <summary>
        /// Delegate for watcher StatusChanged event - handles errors of gps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            string currentStatus = e.Status.ToString();
            System.Diagnostics.Debug.WriteLine(currentStatus);
        }

        /// <summary>
        /// Delegate for PositionChanged event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (!e.Position.Location.IsUnknown)
            {
                double latitude = e.Position.Location.Latitude;
                double longitude = e.Position.Location.Longitude;
                double altitude = e.Position.Location.Altitude;
                double course = e.Position.Location.Course;
                double speed = e.Position.Location.Speed;
                double hAccuracy = e.Position.Location.HorizontalAccuracy;
                double vAccuracy = e.Position.Location.VerticalAccuracy;
                DateTimeOffset time = e.Position.Timestamp;
                this.addDistanceByNextCooridinates(latitude, longitude);
                System.Diagnostics.Debug.WriteLine("Total distance: " + this.TotalDistance);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Location Unknown");
            }
        }
        protected virtual void OnDistanceChanged(DistanceEventArgs e)
        {
            if (DistanceChanged != null)
                DistanceChanged(this, e);
        }
        #endregion
    }
}
