using System.Collections.Generic;

namespace SiiMobility.API.Models.TrafficManager.Responses
{
	/// <summary>
	/// Base traffic manager response model
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ITrafficManagerResponse<T>
	{
		/// <summary>
		/// Number of event elements
		/// </summary>
		int FullCount { get; set; }

		/// <summary>
		/// Category type of the event list
		/// </summary>
		string Type { get; set; }

		/// <summary>
		/// Events' list
		/// </summary>
		List<T> Events { get; set; }

		/// <summary>
		/// Utility method for events descending ordering 
		/// </summary>
		void OrderResults();

		/// <summary>
		/// Utility method for events ordering
		/// </summary>
		void OrderResultsDesc();

		/// <summary>
		/// Utility method for events counting
		/// </summary>
		void CountResults();
	}
}
