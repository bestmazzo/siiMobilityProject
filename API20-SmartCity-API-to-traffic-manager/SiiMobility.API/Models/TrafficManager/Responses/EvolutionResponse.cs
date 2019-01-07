using System.Collections.Generic;

namespace SiiMobility.API.Models.TrafficManager.Responses
{
	/// <summary>
	/// Evolution response model
	/// </summary>
	public class EvolutionResponse : TrafficManagerResponse<Event.EvolutionEvent>
	{
		/// <inheritdoc />
		public EvolutionResponse(List<Event.EvolutionEvent> events)
			:base(events, "CarFlowChanges", true)
		{
		}

		/// <inheritdoc />
		public EvolutionResponse()
		{
			
		}

	}
}
