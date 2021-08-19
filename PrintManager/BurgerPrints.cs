using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PrintManager
{
    public class BurgerPrints : IPrintProvider
    {
        private string ApiKey = "";
        private bool IsSandbox = false;
        public BurgerPrints(string apiKey, bool isSandBox = false)
        {
            ApiKey = apiKey;
            IsSandbox = isSandBox;
        }

        public async Task<object> GetOrderDetail(string orderId)
        {
            using(var client = new HttpClient())
            {
                string url = $"https://pro.burgerprints.com/pspfulfill/api/v1/dropship-api/order/v1/{orderId}?api_key={ApiKey}&sandbox={IsSandbox.ToString()}";
                var response = await client.GetAsync(url);
                return response;
            }
        }

        public async Task<object> MakeOrderAsync(OrderRequestModel request)
        {
            using (var client = new HttpClient())
            {
                List<object> items = new List<object>();
                foreach(var item in request.Items)
                {
                    items.Add(new
                    {
                        catalog_sku = "FRGSD-S",
                        design_url_front = item.Templete,
                        mockup_url_front = item.Mockup,
                        quantity = item.Quantity
                    });
                }

                var postData = new
                {
                    shipping_name= request.CustomerName,
                    shipping_address1= request.Adress,
                    shipping_city = request.City,
                    shipping_state = request.State,
                    shipping_zip = request.Zip,
                    shipping_country = request.Country,
                    shipping_email= request.Email,
                    ignore_address_check = false,
                    sandbox = IsSandbox,
                    api_key = ApiKey,
                    items
                };
                var content = new StringContent(JsonConvert.SerializeObject(postData));
                var response = await client.PostAsync("https://pro.burgerprints.com/pspfulfill/api/v1/dropship-api/order/v2", content);
                return response;
            }
        }

        public bool UploadImage()
        {
            throw new NotImplementedException();
        }
    }
}
