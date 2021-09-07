using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
namespace PrintManager
{
    public class Printify
    {
        string Token = "";
        string endpoint = "https://api.printify.com/v1/";
        public Printify(string token)
        {
            Token = token;
        }
        public async Task<UploadImageResponse> UploadImage(string fileUrl)
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
            var content = new StringContent(JsonConvert.SerializeObject(postData));
            var response = await API("uploads/images.json", Method.POST, content);
            return JsonConvert.DeserializeObject<UploadImageResponse>(response);
        }
        public async Task<List<Shop>> GetShopList()
        {
            var response = await API("shops.json", Method.GET);
            return JsonConvert.DeserializeObject<List<Shop>>(response);
        }
        public async Task<List<Blueprint>> GetAllBlueprint()
        {
            var response = await API("catalog/blueprints.json", Method.GET);
            return JsonConvert.DeserializeObject<List<Blueprint>>(response);
        }
        public async Task<Blueprint> GetBlueprint(int blueprintId)
        {
            var response = await API($"catalog/blueprints/{blueprintId}.json", Method.GET);
            return JsonConvert.DeserializeObject<Blueprint>(response);
        }
        public async Task<List<PrintProvider>> GetPrintProviders(int blueprintId)
        {
            var response = await API($"catalog/blueprints/{blueprintId}/print_providers.json", Method.GET);
            return JsonConvert.DeserializeObject<List<PrintProvider>>(response);
        }
        public async Task<Variants> GetVariants(int blueprintId, int printProviderId)
        {
            var response = await API($"catalog/blueprints/{blueprintId}/print_providers/{printProviderId}/variants.json", Method.GET);
            return JsonConvert.DeserializeObject<Variants>(response);
        }
        public async Task<ResponseProduct> CreateProduct(string ShopId, CreateProductRequest request)
        {
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await API($"shops/{ShopId}/products.json", Method.POST, content);
            return JsonConvert.DeserializeObject<ResponseProduct>(response);
        }
        public async Task<ResponseOrder> CreateOrder(string ShopId, CreateOrderRequest request)
        {
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await API($"shops/{ShopId}/orders.json", Method.POST, content);
            return JsonConvert.DeserializeObject<ResponseOrder>(response);
        }

        public async Task<object> GetOrders(string ShopId)
        {
            var response = await API($"shops/{ShopId}/orders.json", Method.GET);
            return JsonConvert.DeserializeObject(response);
        }

        public async Task<object> GetOrder(string ShopId, string OrderId)
        {
            var response = await API($"shops/{ShopId}/orders/{OrderId}.json", Method.GET);
            return JsonConvert.DeserializeObject(response);
        }

        async Task<string> API(string apiUrl, Method method, HttpContent content = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                string url = endpoint + apiUrl;
                HttpResponseMessage response;
                if (method == Method.POST)
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    response = await client.PostAsync(url, content);
                }
                else
                {
                    response = await client.GetAsync(url);
                }
                return await response.Content.ReadAsStringAsync();
            }
        }
    }

    public class UploadImageResponse
    {
        public string id { get; set; }
        public string file_name { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public float size { get; set; }
        public string mime_type { get; set; }
        public string preview_url { get; set; }
        public string upload_time { get; set; }
    }
    public class Shop
    {
        public string id { get; set; }
        public string title { get; set; }
        public string sales_channel { get; set; }
    }
    public class Blueprint
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string brand { get; set; }
        public string model { get; set; }
        public string[] images { get; set; }
    }
    public class PrintProvider
    {
        public int id { get; set; }
        public string title { get; set; }
        public Location location { get; set; }
        public class Location
        {
            public string address1 { get; set; }
            public string address2 { get; set; }
            public string city { get; set; }
            public string country { get; set; }
            public string region { get; set; }
            public string zip { get; set; }
        }
    }
    public class Variants
    {
        public int id { get; set; }
        public string title { get; set; }
        public List<Variant> variants { get; set; }
    }
    public class Variant
    {
        public int id { get; set; }
        public string title { get; set; }
        public Dictionary<string, string> options { get; set; }
        public List<Placeholder> placeholders { get; set; }
        public class Placeholder
        {
            public string position { get; set; }
            public int height { get; set; }
            public int width { get; set; }
        }
    }
    
    public class CreateProductRequest
    {
        public string title { get; }
        public string description { get; }
        public int blueprint_id { get; }
        public int print_provider_id { get; }
        public RequestVariant[] variants { get; }
        public RequestPrintArea[] print_areas { get; }
        public CreateProductRequest(
            string Title
            , string Description
            , int BlueprintId
            , int PrintProviderId
            , RequestVariant[] Variants
            , RequestPrintArea[] PrintAreas)
        {
            title = Title;
            description = Description;
            blueprint_id = BlueprintId;
            print_provider_id = PrintProviderId;
            variants = Variants;
            print_areas = PrintAreas;
        }
    }
    public class RequestVariant
    {
        public int id { get; }
        public int price { get; }
        public bool is_enabled { get; }
        public RequestVariant(int Id, int Price, bool IsEnabled)
        {
            id = Id;
            price = Price;
            is_enabled = IsEnabled;
        }
    }
    public class RequestPrintArea
    {
        public int[] variant_ids { get; }
        public RequestPlaceholder[] placeholders { get; }
        public RequestPrintArea(int[] VariantIds, RequestPlaceholder[] Placeholders)
        {
            variant_ids = VariantIds;
            placeholders = Placeholders;
        }
    }
    public class RequestPlaceholder
    {
        public string position { get; }
        public RequestImage[] images { get; }
        public RequestPlaceholder(string Position, RequestImage[] Images)
        {
            position = Position;
            images = Images;
        }
    }
    public class RequestImage
    {
        public string id { get; }
        public float x { get; }
        public float y { get; }
        public float scale { get; }
        public float angle { get; }
        public RequestImage(string Id, float X, float Y, float Scale, float Angle)
        {
            id = Id; x = X; y = Y; scale = Scale; angle = Angle;
        }
    }

    public class ResponseProduct
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string[] tags { get; set; }
        public dynamic[] options { get; set; }
        public ProductVariant[] variants { get; set; }
        public class ProductVariant
        {
            public int id { get; set; }
            public string sku { get; set; }
            public int cost { get; set; }
            public int price { get; set; }
            public string title { get; set; }
            public int grams { get; set; }
            public bool is_enabled { get; set; }
            public bool is_default { get; set; }
            public bool is_available { get; set; }
            public string[] options { get; set; }
            public int quantity { get; set; }
        }
        public class ProductImage
        {
            public string src { get; set; }
            public int[] variant_ids { get; set; }
            public string position { get; set; }
            public bool is_default { get; set; }
        }
    }

    public class CreateOrderRequest
    {
        public string external_id { get; }
        public OrderLineItem[] line_items { get; }
        public int shipping_method { get; }
        public string label { get; }
        public bool send_shipping_notification { get; set;  }
        public Address address_to { get; }
        public CreateOrderRequest(string ExternalId, string Label, int ShippingMethod, OrderLineItem[] LineItems, Address AddressTo)
        {
            external_id = ExternalId;
            label = Label;
            shipping_method = ShippingMethod;
            line_items = LineItems;
            address_to = AddressTo;
        }
    }
    public class OrderLineItem
    {
        public string product_id { get; }
        public int variant_id { get; }
        public int quantity { get; }
        public OrderLineItem(string ProductId, int VariantId, int Quantity)
        {
            product_id = ProductId;
            variant_id = VariantId;
            quantity = Quantity;
        }
    }
    public class Address
    {
        public string first_name { get; }
        public string last_name { get; }
        public string email { get; }
        public string phone { get; }
        public string country { get; }
        public string region { get; }
        public string address1 { get; }
        public string address2 { get; }
        public string city { get; }
        public string zip { get; }
        public Address(
            string FristName
            , string LastName
            , string Email
            , string Phone
            , string Country
            , string Region
            , string Address1
            , string Address2
            , string Zip)
        {
            first_name = FristName;
            last_name = LastName;
            email = Email;
            phone = Phone;
            country = Country;
            region = Region;
            address1 = Address1;
            address2 = Address2;
            zip = Zip;
        }
    }

    public class ResponseOrder
    {
        public string id { get; }
    }
    
}
