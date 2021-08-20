using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
namespace PrintManager
{
    public enum Method
    {
        POST,
        GET
    }
    public class Printify
    {
        string Token = "";
        string endpoint = "https://api.printify.com/v1/";
        public Printify(string token)
        {
            Token = token;
        }
        public Task<object> GetOrderDetail(string orderId)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> UploadImage(string fileUrl)
        {
            var file = File.ReadAllBytes(fileUrl);
            string base64String = Convert.ToBase64String(file);
            var temp = fileUrl.Split('/', '\\', '.');
            string fileName = (temp.Length > 1) ? temp[temp.Length - 2] : "";
            var postData = new Dictionary<string, string>
            {
                { "file_name",  fileName },
                { "contents", base64String }
            };
            var content = new FormUrlEncodedContent(postData);
            return await API("uploads/images.json", Method.POST, content);
        }
        async Task<HttpResponseMessage> API(string apiUrl, Method method, HttpContent content = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                string url = endpoint + apiUrl;
                HttpResponseMessage response;
                if(method == Method.POST)
                {
                    response = await client.PostAsync(url, content);
                }
                else
                {
                    response = await client.GetAsync(url);
                }
                return response;
            }
        }
    }
}
