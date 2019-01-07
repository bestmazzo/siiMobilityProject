using System.Net.Http;
using System.Threading.Tasks;

namespace SiiMobility.API.RestClient
{
	public interface IHttpClient
	{
		Task<dynamic> GetAsync(string uri);
		Task<HttpResponseMessage> PostAsync<T>(string uri, T request);
		Task<HttpResponseMessage> DeleteAsync(string uri);
		Task<HttpResponseMessage> PutAsync<T>(string uri, T request);
	}
}