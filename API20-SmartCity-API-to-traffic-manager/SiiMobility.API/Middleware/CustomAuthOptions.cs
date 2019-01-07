using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;

namespace SiiMobility.API.Middleware
{
	public class CustomAuthOptions : AuthenticationSchemeOptions
	{
		public const string DefaultScheme = "apiKey";
		public static string Scheme => DefaultScheme;
		public StringValues AuthKey { get; set; }
	}
}