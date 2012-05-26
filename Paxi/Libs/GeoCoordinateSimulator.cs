using System;
using System.Device.Location;
using System.Windows.Threading;

namespace Paxi.Libs.GPS
{
	/// <summary>
	/// Simulates the GeoCoordinateWatcher
	/// </summary>
	public sealed class GeoCoordinateSimulator : IGeoPositionWatcher<GeoCoordinate>, IDisposable
	{
		private const double MaxMovement = 0.0004; //Degrees
		
		#region Private Fields
		private double threshold = 0d;
		private volatile bool disposed;
		private DispatcherTimer timer;
		private GeoPositionStatus status = GeoPositionStatus.NoData;
		private GeoPosition<GeoCoordinate> position;
		private Random random = new Random();
		private double accumulatedLongitude = double.NaN;
		private double accumulatedLatitude = double.NaN;
		private double altitude = double.NaN;
		private double lastlatitude = double.NaN;
		private double lastlongitude = double.NaN;
		private DateTime lastTime;
		private double startLatitude;
		private double startLongitude;
		private double lastCourse = 0;
		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="GeoCoordinateSimulator"/> class.
		/// </summary>
		/// <param name="startLatitude">The start latitude where the simulation should begin.</param>
		/// <param name="startLongitude">The start longitude where the simulation should begin.</param>
		public GeoCoordinateSimulator(double startLatitude, double startLongitude)
		{
			this.startLatitude = startLatitude;
			this.startLongitude = startLongitude;
			timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
			timer.Tick += timer_Tick;
		}

		/// <summary>
		/// Fired every time a simulated GPS measurement needs to occur
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timer_Tick(object sender, EventArgs e)
		{
			double lat;
			double lon;
			double speed = 0;
			double course = 0;
			DateTime now = DateTime.Now;
			double distance = double.PositiveInfinity;
			if (double.IsNaN(accumulatedLatitude) || double.IsNaN(accumulatedLongitude) || double.IsNaN(altitude))
			{
				lon = startLongitude;
				lat = startLatitude;
				altitude = 500;
				course = random.NextDouble() * 360;
			}
			else
			{
				course = lastCourse + (random.NextDouble() * 15 - 7.5);
				var result = GetPointFromHeadingGeodesic(new double[] { accumulatedLongitude, accumulatedLatitude },
					random.NextDouble() * 50, course);
				lon = result[0];
				lat = result[1];
				altitude += (random.NextDouble() - .5) * 10;
				while (lat < -90) lat += 180;
				while (lat > 90) lat -= 180;
				while (lon < -180) lon += 360;
				while (lon > 180) lon -= 360;
				distance = GetDistanceGeodesic(lastlongitude, lastlatitude, lon, lat);
				if (Status == GeoPositionStatus.Ready)
				{
					TimeSpan time = now - lastTime;
					speed = (distance / time.TotalSeconds);
				}
			}

			accumulatedLongitude = lon;
			accumulatedLatitude = lat;
			lastTime = now;
			lastCourse = course;
			if (distance >= MovementThreshold) //Only fire if the amount moved since last event is greater than the threshold
			{
				while (course < 0) course += 360;
				while (course > 360) course -= 360;
				Position = new GeoPosition<GeoCoordinate>()
				{
					Location = new GeoCoordinate(
						accumulatedLatitude, accumulatedLongitude, altitude,  //Location
						random.NextDouble() * 100, random.NextDouble() * 200, //Accuracy
						speed, course),										  //Movement
					Timestamp = now
				};
				Status = GeoPositionStatus.Ready;
				lastlatitude = accumulatedLatitude;
				lastlongitude = accumulatedLongitude;
				lastTime = now;
			}
		}

		#region Geographic calculations

		/// <summary>
		/// Calculates the distance between two points in meters
		/// </summary>
		/// <param name="lon1"></param>
		/// <param name="lat1"></param>
		/// <param name="lon2"></param>
		/// <param name="lat2"></param>
		/// <returns></returns>
		private static double GetDistanceGeodesic(double lon1, double lat1, double lon2, double lat2)
		{
			lon1 = lon1 / 180 * Math.PI;
			lon2 = lon2 / 180 * Math.PI;
			lat1 = lat1 / 180 * Math.PI;
			lat2 = lat2 / 180 * Math.PI;
			return 2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((lat1 - lat2) / 2)), 2) +
			 Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin((lon1 - lon2) / 2), 2))) * 6378137;
		}

		/// <summary>
		/// Calculates the bearing between two points.
		/// </summary>
		/// <param name="lon1"></param>
		/// <param name="lat1"></param>
		/// <param name="lon2"></param>
		/// <param name="lat2"></param>
		/// <param name="distance"></param>
		/// <returns></returns>
		private static double GetTrueBearingGeodesic(double lon1, double lat1, double lon2, double lat2)
		{
			lat1 = lat1 / 180 * Math.PI;
			lon1 = lon1 / 180 * Math.PI;
			lat2 = lat2 / 180 * Math.PI;
			lon2 = lon2 / 180 * Math.PI;
			double tc1 = Math.Atan2(Math.Sin(lon1 - lon2) * Math.Cos(lat2),
				Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2)) % (2 * Math.PI);
			return 360 - (tc1 / Math.PI * 180);
		}

		public static double[] GetPointFromHeadingGeodesic(double[] start, double distance, double heading)
		{
			double brng = heading / 180 * Math.PI;
			double lon1 = start[0] / 180 * Math.PI;
			double lat1 = start[1] / 180 * Math.PI;
			double dR = distance / 6378137; //Angular distance in radians
			double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dR) + Math.Cos(lat1) * Math.Sin(dR) * Math.Cos(brng));
			double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(dR) * Math.Cos(lat1), Math.Cos(dR) - Math.Sin(lat1) * Math.Sin(lat2));
			double lon = lon2 / Math.PI * 180;
			double lat = lat2 / Math.PI * 180;
			while (lon < -180) lon += 360;
			while (lat < -90) lat += 180;
			while (lon > 180) lon -= 360;
			while (lat > 90) lat -= 180;
			return new double[] { lon, lat };
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the position.
		/// </summary>
		/// <value>The position.</value>
		public GeoPosition<GeoCoordinate> Position
		{
			get
			{
				return position;
			}
			private set
			{
				if (value != position)
				{
					position = value;
					RaisePositionChanged(new GeoPositionChangedEventArgs<GeoCoordinate>(position));
				}
			}
		}
		/// <summary>
		///  The status of the location service.
		/// </summary>
		public GeoPositionStatus Status
		{
			get
			{
				return status;
			}
			private set
			{
				if (value != status)
				{
					status = value;
					RaiseStatusChanged(new GeoPositionStatusChangedEventArgs(value));
				}
			}
		}
		
		/// <summary>
		/// The minimum distance that must be travelled between successive 
		/// <see cref="PositionChanged"/> events.
		/// </summary>
		/// <value>The distance, in meters, that must be travelled between
		/// successive <see cref="PositionChanged"/> events.</value>
		public double MovementThreshold
		{
			get
			{
				DisposeCheck();
				return threshold;
			}
			set
			{
				DisposeCheck();
				if ((value < 0.0) || double.IsNaN(value))
				{
					throw new ArgumentOutOfRangeException("value", "Argument must be non negative");
				}
				threshold = value;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Starts the acquisition of data from the location service.
		/// </summary>
		/// <param name="suppressPermissionPrompt">This parameter is not used..</param>
		public void Start(bool suppressPermissionPrompt)
		{
			Start();
		}

		/// <summary>
		/// Starts the acquisition of data from the location service.
		/// </summary>
		public void Start()
		{
			this.DisposeCheck();
			timer.Start();
			Status = GeoPositionStatus.Initializing;
		}

		/// <summary>
		/// Stops the acquisition of data from the location service.
		/// </summary>
		public void Stop()
		{
			this.DisposeCheck();
			timer.Stop();
			this.Position = null;
			Status = GeoPositionStatus.Disabled;
		}

		/// <summary>
		/// Attempts to start the acquisition of data from the location service. If the
		/// provided timeout interval is exceeded before the location service responds,
		/// the request for location is stopped and the method returns false.
		/// </summary>
		/// <param name="suppressPermissionPrompt">This parameter is not used.</param>
		/// <param name="timeout"> A TimeSpan object specifying the amount of time to wait for location data
		/// acquisition to begin.</param>
		/// <returns><c>true</c> if the location service responds within the
		/// timeout window. Otherwise, false.</returns>
		public bool TryStart(bool suppressPermissionPrompt, TimeSpan timeout)
		{
			Start();
			return true;
		}

		#endregion

		#region Events

		private void RaisePositionChanged(GeoPositionChangedEventArgs<GeoCoordinate> args)
		{
			if (PositionChanged != null)
			{
				PositionChanged(this, args);
			}
		}

		/// <summary>
		/// Occurs when the location service detects a change in position.
		/// </summary>
		public event EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>> PositionChanged;

		private void RaiseStatusChanged(GeoPositionStatusChangedEventArgs args)
		{
			if (StatusChanged != null)
			{
				StatusChanged(this, args);
			}
		}

		/// <summary>
		/// Occurs when the status of the location service changes.
		/// </summary>
		public event EventHandler<GeoPositionStatusChangedEventArgs> StatusChanged;

		#endregion

		#region IDisposable
		/// <summary>
		/// Releases managed and unmanaged resources used by the <see cref="GeoCoordinateSimulator"/>
		/// and stops the acquisition of data from the location service.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
		}
		private void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.disposed)
				{
					this.Stop();
					this.disposed = true;
				}
			}
		}
		private void DisposeCheck()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("GeoCoordinateSimulator");
			}
		}
		~GeoCoordinateSimulator()
		{
			this.Dispose(false);
	    }

		#endregion
	}
}
