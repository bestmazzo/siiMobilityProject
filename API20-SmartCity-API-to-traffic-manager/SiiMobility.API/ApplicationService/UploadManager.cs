using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using SiiMobility.API.Helpers;

namespace SiiMobility.API.ApplicationService
{
	public class UploadManager : IFileManager
	{
		private readonly ILogger<UploadManager> _logger;
		private readonly IHostingEnvironment _env;

		public UploadManager(ILogger<UploadManager> logger, IHostingEnvironment env)
		{
			_logger = logger;
			_env = env;
		}
		private static readonly FormOptions DefaultFormOptions = new FormOptions();

		public async Task<string> Execute(HttpContext context, KeyValueAccumulator formAccumulator)
		{
			var boundary = MultipartRequestHelper.GetBoundary(
				MediaTypeHeaderValue.Parse(context.Request.ContentType),
				DefaultFormOptions.MultipartBoundaryLengthLimit);

			var reader = new MultipartReader(boundary, context.Request.Body);
			var targetFilePath = string.Empty;
			var section = await reader.ReadNextSectionAsync();
			while (section != null)
			{
				var hasContentDispositionHeader =
					ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

				if (hasContentDispositionHeader)
				{
					if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
					{
						targetFilePath = Path.Combine(_env.ContentRootPath, "Upload",
							HeaderUtilities.RemoveQuotes(contentDisposition.FileName).Value);
						using (var targetStream = File.Create(targetFilePath))
						{
							await section.Body.CopyToAsync(targetStream);
							_logger.LogInformation($"Copied the uploaded file '{targetFilePath}'");
						}
					}
					else if (MultipartRequestHelper.HasFormDataContentDisposition(contentDisposition))
					{
						// Content-Disposition: form-data; name="key"
						//
						// value

						// Do not limit the key name length here because the 
						// multipart headers length limit is already in effect.
						var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;
						using (var streamReader = new StreamReader(section.Body, section.GetUTF8Encoding(), true, 1024, true))
						{
							// The value length limit is enforced by MultipartBodyLengthLimit
							var value = await streamReader.ReadToEndAsync();
							if (string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase))
							{
								value = string.Empty;
							}
							formAccumulator.Append(key, value);

							if (formAccumulator.ValueCount > DefaultFormOptions.ValueCountLimit)
							{
								throw new InvalidDataException($"Form key count limit {DefaultFormOptions.ValueCountLimit} exceeded.");
							}
						}
					}
				}

				// Drains any remaining section body that has not been consumed and
				// reads the headers for the next section.
				section = await reader.ReadNextSectionAsync();
			}

			return targetFilePath;
		}
	}
}
