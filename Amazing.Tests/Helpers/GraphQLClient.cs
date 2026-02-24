using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Amazing.Tests.Helpers
{
    public class GraphQLClient
    {
        private readonly HttpClient _http;
        private const string Endpoint = "http://localhost:5000/graphql";

        public GraphQLClient(string bearerToken = null)
        {
            _http = new HttpClient();
            if (!string.IsNullOrEmpty(bearerToken))
                _http.DefaultRequestHeaders.Add("authorization", bearerToken);
        }

        public async Task<JObject> SendAsync(string query)
        {
            var body = JsonConvert.SerializeObject(new { query });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync(Endpoint, content);
            var json = await response.Content.ReadAsStringAsync();
            return JObject.Parse(json);
        }
    }
}
