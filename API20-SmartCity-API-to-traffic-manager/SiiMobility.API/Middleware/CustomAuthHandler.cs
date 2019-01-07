using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace SiiMobility.API.Middleware
{
	public class CustomAuthHandler : AuthenticationHandler<CustomAuthOptions>
	{
		public CustomAuthHandler(IOptionsMonitor<CustomAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
			: base(options, logger, encoder, clock)
		{
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			// Get Authorization header value
			if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
			{
				Logger.LogError("Cannot read authorization header.");
				return Task.FromResult(AuthenticateResult.Fail("Cannot read authorization header."));
			}

			// The auth key from Authorization header check against the configured ones
			if (authorization.Any(key => Options.AuthKey.All(ak => ak != key)))
			{
				Logger.LogError("Invalid auth key.");
				return Task.FromResult(AuthenticateResult.Fail("Invalid auth key."));
			}

			// Create authenticated user
			var identities = new List<ClaimsIdentity> { new ClaimsIdentity("apiKey") };
			var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), CustomAuthOptions.Scheme);
			Logger.LogInformation("Logged In");
			return Task.FromResult(AuthenticateResult.Success(ticket));
		}
	}
}