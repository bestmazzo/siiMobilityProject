using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace SiiMobility.API.Helpers
{
	public static class HelperExtensions
	{
		public static Encoding GetUTF8Encoding(this MultipartSection section)
		{
			var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var mediaType);
			// UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
			// most cases.
			if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
			{
				return Encoding.UTF8;
			}
			return mediaType.Encoding;
		}
	}
}
