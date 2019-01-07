using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace SiiMobility.API.RestClient
{
	public static class HttpExtensions
	{
		public static T ContentAsType<T>(this HttpResponseMessage response)
		{
			var data = response.Content.ReadAsStringAsync().Result;
			return string.IsNullOrEmpty(data) ?
				default(T) :
				JsonConvert.DeserializeObject<T>(data);
		}

		public static dynamic ContentAsDynamic(this HttpResponseMessage response)
		{
			var data = response.Content.ReadAsStringAsync().Result;
			return string.IsNullOrEmpty(data) ? null : JsonConvert.DeserializeObject(data);
		}

		public static string ContentAsJson(this HttpResponseMessage response)
		{
			var data = response.Content.ReadAsStringAsync().Result;
			return JsonConvert.SerializeObject(data);
		}

		public static string ContentAsString(this HttpResponseMessage response)
		{
			return response.Content.ReadAsStringAsync().Result;
		}

		public static string ToQueryString<T>(this T request, char separator = ',')
		{
			if (request == null)
				throw new ArgumentNullException("request");

			// Get all properties on the object
			var properties = request.GetType().GetProperties()
				.Where(x => x.CanRead)
				.Where(x => x.GetValue(request, null) != null)
				.ToDictionary(x => x.Name, x => x.GetValue(request, null));

			// Get names for all IEnumerable properties (excl. string)
			var propertyNames = properties
				.Where(x => !(x.Value is string) && x.Value is IEnumerable)
				.Select(x => x.Key)
				.ToList();

			// Concat all IEnumerable properties into a comma separated string
			foreach (var key in propertyNames)
			{
				var valueType = properties[key].GetType();
				var valueElemType = valueType.IsGenericType
					? valueType.GetGenericArguments()[0]
					: valueType.GetElementType();
				if (valueElemType.IsPrimitive || valueElemType == typeof(string))
				{
					var enumerable = properties[key] as IEnumerable;
					properties[key] = string.Join(separator, enumerable.Cast<object>());
				}
			}

			// Concat all key/value pairs into a string separated by ampersand
			return string.Join("&", properties
				.Select(x => string.Concat(
					Uri.EscapeDataString(x.Key), "=",
					Uri.EscapeDataString(x.Value.ToString()))));
		}
	}
}