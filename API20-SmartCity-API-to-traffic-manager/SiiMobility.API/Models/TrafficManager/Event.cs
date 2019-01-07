using System;
using System.ComponentModel.DataAnnotations;

namespace SiiMobility.API.Models.TrafficManager
{
	/// <summary>
	/// Base event class
	/// </summary>
	public abstract class Event : IEvent
	{
		/// <inheritdoc />
		public int Id { get; set; }

		/// <inheritdoc />
		public DateTime TimeStamp { get; set; }

		/// <inheritdoc />
		public Coordinates Position { get; set; }

		/// <inheritdoc />
		public EventDetails Details { get; set; }

		/// <summary>
		/// Parameterless constructor
		/// </summary>
		protected Event()
		{
			
		}

		/// <summary>
		/// Evolution event class
		/// </summary>
		public class EvolutionEvent : Event
		{

			/// <summary>
			/// Parameterless constructor
			/// </summary>
			public EvolutionEvent()
			{
				
			}
		}

		/// <summary>
		/// Forecast event class
		/// </summary>
		public class ForecastEvent : Event
		{
			/// <summary>
			/// Probability of the event to take place
			/// </summary>
			[Range(0d, 100d)]
			public double Affordability { get; set; }

			/// <summary>
			/// Parameterless constructor
			/// </summary>
			public ForecastEvent()
			{
				
			}
		}
	}
}
