namespace SiiMobility.API.Models.TrafficManager.Requests
{
	/// <summary>
	/// Evolution request model
	/// </summary>
	public class EvolutionRequest : TrafficManagerRequest
	{
		/// <summary>
		/// Max search distance from the GPS point
		/// </summary>
		public int MaxDist { get; set; }
	}
}
