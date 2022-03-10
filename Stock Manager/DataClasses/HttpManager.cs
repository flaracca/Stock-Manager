using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stock_Manager.DataClasses
{
    public class HttpManager
    {
        public HttpManager()
        {
        }

        public async Task<List<T>> GetAsync<T>(string requestUrl) where T : class
        {
            var client = new System.Net.Http.HttpClient();
            var response = await client.GetAsync(requestUrl);
            var responseJson = await response.Content.ReadAsStringAsync();
            var JsonObject = JsonConvert.DeserializeObject<List<T>>(responseJson);
            return JsonObject;
        }

        public async Task CodibePostAsync(string requestUrl)
        {
            var client = new System.Net.Http.HttpClient();
            var response = await client.GetAsync(requestUrl);
            var responseJson = await response.Content.ReadAsStringAsync();
            //var JsonObject = JsonConvert.DeserializeObject(responseJson);

        }
    }
}
