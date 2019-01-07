using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SiiMobility.API.Models.TrafficManager.Requests
{
	/// <summary>
	/// Base request class for TrafficManager's endpoints
	/// </summary>
	public interface ITrafficManagerRequest
	{
		/// <summary>
		/// From date filter
		/// </summary>
		DateTime STimestamp { get; set; }

		/// <summary>
		/// To date filter
		/// </summary>
		DateTime ETimestamp { get; set; }

		/// <summary>
		/// Categories of events to be fetched
		/// </summary>
		string Categories { get; set; }

		/// <summary>
		/// Specifies how many events should be returned
		/// </summary>
		int MaxResults { get; set; }

		//List<double> Selection { get; set; }

		/// <summary>
		/// Latitude and longitude separated by ';' EX: 43.7768463;11.2478617
		/// </summary>
		string Selection { get; set; }

		[JsonIgnore]//TODO: Check attributes utility inside interfaces
		[XmlIgnore]
		Coordinates Coordinates { get; set; }

		/// <summary>
		/// Utility method for validating the selection property and valorizing the coordinates objects with its values
		/// </summary>
		/// <returns>Returns a string containing the error if any</returns>
		string ValidateAndParseSelection();

	}
}
