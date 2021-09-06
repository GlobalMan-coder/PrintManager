using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PrintManager
{
    public class BurgerPrints
    {
        internal static string ApiKey = "";
        internal static bool IsSandbox = false;
        public BurgerPrints(string apiKey, bool isSandBox = false)
        {
            ApiKey = apiKey;
            IsSandbox = isSandBox;
        }
        public async Task<object> GetOrderDetail(string OrderId)
        {
            string url = $"https://seller.burgerprints.com/pspfulfill/api/v1/dropship-api/order/v1/{OrderId}?api_key={ApiKey}&sandbox={IsSandbox.ToString()}";
            var response = await API(url, Method.GET);
            return JsonConvert.DeserializeObject<BPOrderDetail>(response);
        }
        public async Task<object> GetLogOrderDetail(string LogId)
        {
            string url = $"https://seller.burgerprints.com/pspfulfill/api/v1/dropship-api/order/v2/check-log/{LogId}";
            var response = await API(url, Method.GET);
            return JsonConvert.DeserializeObject<BPLogOrderDetail>(response);
        }

        public async Task<BPOrderResponse> MakeOrderWithCustomDataAsync(BPOrderRequest orderItem)
        {
            var content = new StringContent(JsonConvert.SerializeObject(orderItem));
            var response = await API("https://seller.burgerprints.com/pspfulfill/api/v1/dropship-api/order/v2", Method.POST, content);
            return JsonConvert.DeserializeObject<BPOrderResponse>(response);
        }
        public async Task<BPOrderResponse> MakeOrderAsync(BPOrderRequest orderItem)
        {
            var content = new StringContent(JsonConvert.SerializeObject(orderItem));
            var response = await API("https://seller.burgerprints.com/pspfulfill/api/v1/dropship-api/order/v1", Method.POST, content);
            return JsonConvert.DeserializeObject<BPOrderResponse>(response);
        }
        async Task<string> API(string apiUrl, Method method, HttpContent content = null)
        {
            using (var client = new HttpClient())
            {
                string result = "";
                if (method == Method.POST)
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.PostAsync(apiUrl, content);
                    result = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    var response = await client.GetAsync(apiUrl, HttpCompletionOption.ResponseContentRead);
                    result = await response.Content.ReadAsStringAsync();
                }
                return result;
            }
        }
    }
    public class BPOrderRequest
    {
        public string shipping_name { get; }
        public string shipping_address1 { get; }
        public string shipping_address2 { get; set; }
        public string shipping_city { get; }
        public string shipping_state { get; }
        public string shipping_zip { get; }
        // https://www.iso.org/obp/ui/#search
        public string shipping_country { get; }
        public string shipping_email { get; }
        public string shipping_phone { get; set; }
        public bool ignore_address_check { get; }
        public string reference_order_id { get; set; }
        public bool sandbox { get; }
        public string api_key { get; }
        public BPRequestItem[] items { get; }
        public BPOrderRequest(
            string ShippingName
            , string ShippingAddress1
            , string ShippingCity
            , string ShippingState
            , string ShippingZip
            , string ShippingCountry
            , string ShippingEmail
            , BPRequestItem[] Items
            , bool IgnoreAddressCheck = false)
        {
            shipping_name = ShippingName;
            shipping_address1 = ShippingAddress1;
            shipping_city = ShippingCity;
            shipping_state = ShippingState;
            shipping_zip = ShippingZip;
            shipping_country = ShippingCountry;
            shipping_email = ShippingEmail;
            sandbox = BurgerPrints.IsSandbox;
            api_key = BurgerPrints.ApiKey;
            items = Items;
            ignore_address_check = IgnoreAddressCheck;
        }
    }
    public class BPRequestItem
    {
        // https://docs.google.com/spreadsheets/d/1t5wtuFbOU8AVXBvkVWLANOD5R7tMhpWKpRgw7w91UdA/edit#gid=0
        public string catalog_sku { get; }
        public int quantity { get; }
        public BPRequestItem(string CatalogSku, int Quantity)
        {
            catalog_sku = CatalogSku;
            quantity = Quantity;
        }
    }
    public class BPRequestItemWithCustomData : BPRequestItem
    {
        public string design_url_front { get; }
        public string mockup_url_front { get; }
        public string design_url_back { get; }
        public string mockup_url_back { get; }
        public BPRequestItemWithCustomData(
            string CatalogSku
            , string DesignUrlFront
            , string MockupUrlFront
            , string DesignUrlBack
            , string MockupUrlBack
            , int Quantity)
            : base(CatalogSku, Quantity)
        {
            design_url_front = DesignUrlFront;
            mockup_url_front = MockupUrlFront;
            design_url_back = DesignUrlBack;
            mockup_url_back = MockupUrlBack;
        }
    }
    public class BPOrderResponse
    {
        public bool is_success { get; set; }
        public string message { get; set; }
        public string order_id { get; set; }
        public string log_id { get; set; }
    }
    public class BPOrderDetail
    {
        public string id { get; set; }
        public string status { get; set; }
        public string amount { get; set; }
        public string sub_amount { get; set; }
        public string shipping_fee { get; set; }
        public Dictionary<string, Tracking> trackings { get; set;}
    }
    public class Tracking
    {
        public string carrier { get; set; }
        public string code { get; set; }
        public string url { get; set; }
    }
    public class BPLogOrderDetail
    {
        public string order_id { get; set;}
	    public string shipping_name { get; set;}
	    public string shipping_address1 { get; set;}
	    public string shipping_address2  { get; set;}
	    public string shipping_city { get; set;}
	    public string shipping_state { get; set;}
	    public string shipping_zip { get; set;}
	    public string shipping_country { get; set;}
	    public string shipping_email { get; set;}
	    public string shipping_phone { get; set;}
	    public bool ignore_address_check { get; set;}
	    public string reference_order_id  { get; set;}
	    public BPResponseItem[] items { get; set; }
    }
    public class BPResponseItem
    {
        public string lineitem_sku { get; set;}
        public string item_quantity { get; set;}
        public string design_front_url { get; set;}
        public string design_back_url { get; set;}
        public string mockup_front_url { get; set;}
        public string mockup_back_url { get; set;}
        public string status { get; set;}
        public string notes { get; set; }
    }
}