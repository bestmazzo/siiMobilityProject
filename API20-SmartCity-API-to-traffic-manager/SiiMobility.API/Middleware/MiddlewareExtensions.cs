using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;

namespace SiiMobility.API.Middleware
{
	public static class MiddlewareExtensions
	{
		public static IApplicationBuilder UseHttpStatusCodeExceptionMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<HttpStatusCodeExceptionMiddleware>();
		}

		public static IApplicationBuilder UseWhiteListMiddleware(this IApplicationBuilder builder, string whiteList)
		{
			return builder.UseMiddleware<WhiteListMiddleware>(whiteList);
		}

		// Custom authentication extension method
		public static AuthenticationBuilder AddCustomAuth(this AuthenticationBuilder builder, Action<CustomAuthOptions> configureOptions)
		{
			// Add custom authentication scheme with custom options and custom handler
			return builder.AddScheme<CustomAuthOptions, CustomAuthHandler>(CustomAuthOptions.DefaultScheme, configureOptions);
		}
	}
}