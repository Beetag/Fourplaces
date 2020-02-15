using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ApiClient
{
	private readonly HttpClient _client = new HttpClient();

	public async Task<HttpResponseMessage> Execute(HttpMethod method, string url, object data = null, string token = null)
	{
		HttpRequestMessage request = new HttpRequestMessage(method, url);

		if (data != null)
		{
			request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
		}

		if (token != null)
		{
			request.Headers.Add("Authorization", $"Bearer {token}");
		}
		return await _client.SendAsync(request);
	}

	public async Task<T> ReadFromResponse<T>(HttpResponseMessage response)
	{
		string result = await response.Content.ReadAsStringAsync();

		return JsonConvert.DeserializeObject<T>(result);
	}
}