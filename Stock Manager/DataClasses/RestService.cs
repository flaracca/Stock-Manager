using Newtonsoft.Json;
using Stock_Manager.Classes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Stock_Manager.DataClasses
{
    public class RestService
    {
        HttpClient client;


        public RestService()
        {
            client = new HttpClient();
            

        }
        
        public async Task<T> GetResponse<T>(string weburl) where T : class
        {
            Constants c = new Constants();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Constants.ApiKey, Constants.PwdUrl);


            try
            {
                var response = await client.GetAsync(weburl);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var JsonResult = response.Content.ReadAsStringAsync().Result;

                    try
                    {
                        var ContentResp = JsonConvert.DeserializeObject<T>(JsonResult);

                        return ContentResp;
                    }
                    catch (Exception ex)
                    {

                        
                        return null;
                    }
                }


            }
            catch (Exception e)
            {

                
                return null;
            }


            return null;
        }


        public async Task<string> PostResponse(string weburl)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Constants.ApiKey, Constants.PwdUrl);
            var postContent = new List<KeyValuePair<string, string>>
                {

                    new KeyValuePair<string, string>("datoInutile", "")

                };

            FormUrlEncodedContent content = new FormUrlEncodedContent(postContent);

            var result = await client.PostAsync(weburl, content);
            result.EnsureSuccessStatusCode();

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var JsonResult = result.Content.ReadAsStringAsync().Result;
                    return JsonResult;
                }
                catch { return null; }
            }

            return null;

        }

        public async Task<string> PostResponseWithData(string weburl, StringContent postContent)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Constants.ApiKey, Constants.PwdUrl);

            //FormUrlEncodedContent content = new FormUrlEncodedContent(postContent);

            var result = await client.PostAsync(weburl, postContent);
            result.EnsureSuccessStatusCode();

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                try
                {
                    var JsonResult = result.Content.ReadAsStringAsync().Result;
                    return JsonResult;
                }
                catch { return null; }
            }

            return null;

        }



    }
}
