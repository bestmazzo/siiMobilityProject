using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SiiMobility.API.Models.TrafficManager.Requests
{
	/// <inheritdoc />
	public class TrafficManagerRequest : ITrafficManagerRequest
	{
		/// <inheritdoc />
		public DateTime STimestamp { get; set; }


		/// <inheritdoc />
		public DateTime ETimestamp { get; set; }


		/// <inheritdoc />
		public string Categories { get; set; }


		/// <inheritdoc />
		public int MaxResults { get; set; } = 9999;

		//public List<double> Selection { get; set; }

		/// <inheritdoc />
		[Required]
		public string Selection { get; set; }

		/// <inheritdoc />
		[JsonIgnore]
		[XmlIgnore]
		public Coordinates Coordinates { get; set; }


		/// <inheritdoc />
		public string ValidateAndParseSelection()
		{
			{
				if (!Selection.Contains(";"))
					return "Invalid selection.";
				var lat = Selection.Split(";")[0];
				var lon = Selection.Split(";")[1];
				if (string.IsNullOrWhiteSpace(lat))
				{
					return "Missing Latitude.";
				}

				if (string.IsNullOrWhiteSpace(lon))
				{
					return "Missing Longitude.";
				}

				double coordinatesLong;
				if (!double.TryParse(lat, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out coordinatesLong))
					return "Invalid Longitude";

				double coordinatesLat;
				if (!double.TryParse(lon, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out coordinatesLat))
					return "Invalid Latitude.";

				Coordinates = new Coordinates(coordinatesLat, coordinatesLong);

				return "";
			}
		}
	}
}
