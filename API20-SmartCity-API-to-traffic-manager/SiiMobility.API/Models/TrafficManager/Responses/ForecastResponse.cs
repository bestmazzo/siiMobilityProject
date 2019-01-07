using System.Collections.Generic;

namespace SiiMobility.API.Models.TrafficManager.Responses
{
	/// <summary>
	/// Forecast response model
	/// </summary>
	public class ForecastResponse : TrafficManagerResponse<Event.ForecastEvent>
	{
		/// <summary>
		/// Constructor with integrated ordering, type assignment and counting
		/// </summary>
		/// <param name="events">List of events</param>
		public ForecastResponse(List<Event.ForecastEvent> events)
		:base(events, "HighWayFlowChanges")
		{
		}

		/// <summary>
		/// Parameterless constructor
		/// </summary>
		public ForecastResponse()
		{
			
		}

	}
}
