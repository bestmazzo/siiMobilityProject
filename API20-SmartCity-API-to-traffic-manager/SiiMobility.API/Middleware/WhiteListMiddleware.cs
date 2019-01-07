using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SiiMobility.API.Middleware
{
	public class WhiteListMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<WhiteListMiddleware> _logger;
		private readonly string[] _whiteList;

		public WhiteListMiddleware(RequestDelegate next, ILogger<WhiteListMiddleware> logger, string whiteList)
		{
			_whiteList = whiteList.Split(',').ToArray();
			_next = next;
			_logger = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			var remoteIp = context.Connection.RemoteIpAddress?.ToString();
			if (!IsValid(remoteIp))
			{
				_logger.LogError($"Forbidden Request from Remote IP address: {remoteIp}");
				context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
			}
			await _next.Invoke(context);
		}

		private bool IsValid(string remoteAddress)
		{
			if (string.IsNullOrEmpty(remoteAddress)) return false;
			if (IPAddress.TryParse(remoteAddress, out var address))
			{
				if (IPAddress.IsLoopback(address)) return true;
				if (remoteAddress.Contains("localhost") || remoteAddress == "127.0.0.1") return true;
			}

			remoteAddress = GetIPv4Address(remoteAddress);
			_logger.LogInformation($"Request from Remote IP address: {remoteAddress}");
			return _whiteList == null || _whiteList.Length <= 0 || _whiteList.Any(x => CheckAddress(x, remoteAddress));
		}

		private static bool CheckAddress(string address, string remote)
		{
			if (!address.Contains("*"))
			{
				return Equals(address, remote);
			}
			var splittedAddress = address.Split('.');
			var splittedRemote = remote.Split('.');
			var ret = true;
			for (var i = 0; i < 4; i++)
			{
				ret &= splittedAddress[i].Equals("*") || splittedAddress[i].Equals(splittedRemote[i]);
				if (!ret)
				{
					break;
				}
			}
			return ret;
		}

		private string GetIPv4Address(string sHostNameOrAddress)
		{
			try
			{
				// Get the list of IP addresses for the specified host
				var aIpHostAddresses = Dns.GetHostAddresses(sHostNameOrAddress);

				// First try to find a real IPV4 address in the list
				foreach (var ipHost in aIpHostAddresses)
					if (ipHost.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
						return ipHost.ToString();

				// If that didn't work, try to lookup the IPV4 addresses for IPV6 addresses in the list
				foreach (var ipHost in aIpHostAddresses)
					if (ipHost.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
					{
						var ihe = Dns.GetHostEntry(ipHost);
						foreach (var ipEntry in ihe.AddressList)
							if (ipEntry.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
								return ipEntry.ToString();
					}
			}
			catch (Exception ex)
			{
				_logger.LogError(new EventId(), ex, "ip not valid");
				throw new Exception("ip not valid", ex);
			}
			return null;
		}
	}
}
