using System;
using System.Threading.Tasks;

namespace PrintManager
{
    public class Printify : IPrintProvider
    {
        string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIzN2Q0YmQzMDM1ZmUxMWU5YTgwM2FiN2VlYjNjY2M5NyIsImp0aSI6IjFmYjMwM2MzZjQ1MWU1Zjg5YTU3MDBlOTMzMjM2YmZlZDIwMTExZmIwM2Q5ZTk0ODA5OGEzYzQ1YjhjOGQxZTMyZGE0MmM3YzhmZTYwOTlhIiwiaWF0IjoxNjI5NDE1Mjk4LCJuYmYiOjE2Mjk0MTUyOTgsImV4cCI6MTY2MDk1MTI5OCwic3ViIjoiODYyMzg2MyIsInNjb3BlcyI6WyJzaG9wcy5tYW5hZ2UiLCJzaG9wcy5yZWFkIiwiY2F0YWxvZy5yZWFkIiwib3JkZXJzLnJlYWQiLCJvcmRlcnMud3JpdGUiLCJwcm9kdWN0cy5yZWFkIiwicHJvZHVjdHMud3JpdGUiLCJ3ZWJob29rcy5yZWFkIiwid2ViaG9va3Mud3JpdGUiLCJ1cGxvYWRzLnJlYWQiLCJ1cGxvYWRzLndyaXRlIiwicHJpbnRfcHJvdmlkZXJzLnJlYWQiXX0.AfHEfxqxhaSmer1WOZZguA2khEYxxoo7-oT0Xzwjwwke4suKhiqLbkmd4LRax10LvRNQk_J9vWN_YykX7P4";
        string tokenName = "test";
        string endpoint = "https://api.printify.com/v1/";
        public Task<object> GetOrderDetail(string orderId)
        {
            throw new NotImplementedException();
        }

        public bool UploadImage()
        {
            throw new NotImplementedException();
        }

        Task<object> IPrintProvider.MakeOrderAsync(OrderRequestModel request)
        {
            throw new NotImplementedException();
        }
    }
}
