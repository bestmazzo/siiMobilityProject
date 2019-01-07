using System.Collections.Generic;
using System.Linq;
using SiiMobility.API.Models.TrafficManager;
using SiiMobility.API.Models.TrafficManager.Responses;

namespace SiiMobility.API.Models
{
	/// <inheritdoc />
	public class TrafficManagerResponse<T> : ITrafficManagerResponse<T> where T:IEvent
	{
		/// <inheritdoc />
		public int FullCount { get; set; }

		/// <inheritdoc />
		public string Type { get; set; }

		/// <inheritdoc />
		public List<T> Events { get; set; }

		/// <inheritdoc />
		public void OrderResultsDesc()
		{
			Events = Events.OrderByDescending(x => x.TimeStamp).ToList();
		}

		/// <inheritdoc />
		public void OrderResults()
		{
			Events = Events.OrderBy(x => x.TimeStamp).ToList();
		}

		/// <inheritdoc />
		public void CountResults()
		{
			FullCount = Events.Count;
		}

		/// <summary>
		/// Constructor with integrated ordering, type assignment and counting
		/// </summary>
		/// <param name="events">List of events</param>
		/// <param name="type">Type name</param>
		/// <param name="orderByDescending">Descending ordering flag</param>
		public TrafficManagerResponse (List<T> events, string type = "BaseType", bool orderByDescending = false)
		{
			Events = orderByDescending ? events.OrderByDescending(x => x.TimeStamp).ToList() : events.OrderByDescending(x => x.TimeStamp).ToList();
			Type = type;
			FullCount = Events.Count;
		}

		/// <summary>
		/// Base constructor
		/// </summary>
		public TrafficManagerResponse()
		{
			
		}
	}
}
