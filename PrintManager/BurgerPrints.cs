using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PrintManager
{
    public class BurgerPrints
    {
        private string ApiKey = "";
        private bool IsSandbox = false;
        public BurgerPrints(string apiKey, bool isSandBox = false)
        {
            ApiKey = apiKey;
            IsSandbox = isSandBox;
        }

        public async Task<HttpResponseMessage> GetOrderDetail(string orderId)
        {
            using(var client = new HttpClient())
            {
                string url = $"https://pro.burgerprints.com/pspfulfill/api/v1/dropship-api/order/v1/{orderId}?api_key={ApiKey}&sandbox={IsSandbox.ToString()}";
                var response = await client.GetAsync(url);
                return response;
            }
        }

        public async Task<HttpResponseMessage> MakeOrderAsync(BurgerPrintOrderRequest request)
        {
            using (var client = new HttpClient())
            {
                List<object> items = new List<object>();
                foreach(var item in request.Items)
                {
                    items.Add(new
                    {
                        catalog_sku = item.CatalogSKU,
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
    }
    public  class BurgerPrintOrderRequest 
    {
        public string CustomerName { get; }
        public string Adress { get; }
        public string City { get; }
        public string State { get; }
        public string Zip { get; }
        public string Country { get; }
        public string Email { get; }
        public BurgerPrintOrderItem[] Items { get; }
        public BurgerPrintOrderRequest(
            string customerName
            , string adress
            , string city
            , string state
            , string zip
            , string country
            , string email
            , BurgerPrintOrderItem[] items)
        {
            Items = items;
            CustomerName = customerName;
            Adress = adress;
            City = city;
            State = state;
            Zip = zip;
            Country = country;
            Email = email;
        }
    }
    public class BurgerPrintOrderItem
    {
        public string CatalogSKU { get; }
        public string Templete { get; }
        public string Mockup { get; }
        public int Quantity { get; }
        public BurgerPrintOrderItem(
            string catalogSKU
            , string templete
            , string mockup
            , int quantity)
        {
            Templete = templete;
            Mockup = mockup;
            Quantity = quantity;
        }
    }
}
