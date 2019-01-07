using System;

namespace SiiMobility.API.Models.TrafficManager
{
	public interface IEvent
	{
		/// <summary>
		/// Event Id
		/// </summary>
		int Id { get; set; }

		/// <summary>
		/// TimeStamp of the event
		/// </summary>
		DateTime TimeStamp { get; set; }

		/// <summary>
		/// Event position in coordinates
		/// </summary>
		Coordinates Position { get; set; }

		/// <summary>
		/// Event details
		/// </summary>
		EventDetails Details { get; set; }
	}
}
