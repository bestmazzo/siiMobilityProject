using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiiMobility.API.ApplicationService;
using SiiMobility.API.Models.TrafficManager;
using SiiMobility.API.Models.TrafficManager.Requests;
using SiiMobility.API.Models.TrafficManager.Responses;

namespace SiiMobility.API.Controllers
{
	[Route("api/v1/[controller]/[action]")]
	[Produces("application/json")]
	public class TrafficManagerController : Controller
    {

		/// <summary>
		/// Fetch all traffic events regarding a specific GPS point
		/// </summary>
		/// <param name="requestData"></param>
		/// <returns></returns>
	    [HttpGet]
	    [ProducesResponseType(typeof(EvolutionResponse), 200)]
		[AllowAnonymous]
		public IActionResult Evolution(EvolutionRequest requestData)
		{
			var validation = requestData.ValidateAndParseSelection();
			if (!string.IsNullOrWhiteSpace(validation))
				return BadRequest(validation);

			//var testResponse = new Event.EvolutionEvent
			//{
			//	Details = new EventDetails
			//	{
			//		Description = "Evento di test"
			//	},
			//	Id = 1,
			//	Position = requestData.Coordinates,
			//	//Position = new Evolution.Coordinates()
			//	//{
			//	//	Lat = 12512.512,
			//	//	Long = 51161.12
			//	//},
			//	TimeStamp = DateTime.Now

			//};

			var test = SeedGenerator.Generate<EvolutionResponse>(1, requestData.MaxResults).First();
			test.Type = "CarFlowChanges";
			test.CountResults();
			test.OrderResultsDesc();
		    return Ok(test);
	    }

		/// <summary>
		/// Fetch all scheduled traffic-related events and a forecast on events likely to have place
		/// </summary>
		/// <param name="requestData"></param>
		/// <returns></returns>
		[HttpGet]
		[ProducesResponseType(200, Type = typeof(ForecastResponse))]
		[ProducesResponseType(400)]
		[AllowAnonymous]
		public IActionResult Forecast(ForecastRequest requestData)
		{
			var validation = requestData.ValidateAndParseSelection();
			if (!string.IsNullOrWhiteSpace(validation))
				return BadRequest(validation);



			var test = SeedGenerator.Generate<ForecastResponse>(1, requestData.MaxResults).First();

			//var testResponse = new Event.ForecastEvent
			//{
			//	Details = new EventDetails
			//	{
			//		Description = "Evento di test"
			//	},
			//	Id = 1,
			//	Position = requestData.Coordinates,
			//	//Position = new Evolution.Coordinates()
			//	//{
			//	//	Lat = 12512.512,
			//	//	Long = 51161.12
			//	//},
			//	TimeStamp = DateTime.Now

			//};
			//var test = new ForecastResponse(new List<Event.ForecastEvent>{testResponse});

			test.Type = "HighWayFlowChanges";
			test.OrderResults();
			test.CountResults();
			return Ok(test);
		}
	}
}