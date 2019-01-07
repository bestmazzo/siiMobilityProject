using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SiiMobility.API.Models.Exceptions;

namespace SiiMobility.API.Middleware
{
	public class HttpStatusCodeExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<HttpStatusCodeExceptionMiddleware> _logger;

		public HttpStatusCodeExceptionMiddleware(RequestDelegate next, ILogger<HttpStatusCodeExceptionMiddleware> logger)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
			_logger = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (HttpStatusCodeException ex)
			{
				if (context.Response.HasStarted)
				{
					_logger.LogWarning("The response has already started, the http status code middleware will not be executed.");
					throw;
				}

				context.Response.Clear();
				context.Response.StatusCode = ex.StatusCode;
				context.Response.ContentType = ex.ContentType;

				await context.Response.WriteAsync(ex.Message);
			}
		}
	}
}