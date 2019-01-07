namespace SiiMobility.API.Models.TrafficManager
{
	/// <summary>
	/// Coordinates class
	/// </summary>
	public class Coordinates
	{
		/// <summary>
		/// Latitude
		/// </summary>
		public double Lat { get; set; }

		/// <summary>
		/// Longitude
		/// </summary>
		public double Long { get; set; }

		/// <summary>
		/// Parametrized constructor
		/// </summary>
		/// <param name="latitude">Latitude parameter</param>
		/// <param name="longitude">Longitude parameter</param>
		public Coordinates(double latitude, double longitude)
		{
			Lat = latitude;
			Long = longitude;
		}

		/// <summary>
		/// Base parameterless constructor
		/// </summary>
		public Coordinates()
		{

		}
	}
}
