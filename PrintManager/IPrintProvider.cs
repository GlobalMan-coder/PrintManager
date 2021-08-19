using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrintManager
{
    public interface IPrintProvider
    {
        bool UploadImage();
        Task<Object> MakeOrderAsync(OrderRequestModel request);
        Task<Object> GetOrderDetail(string orderId);
    }
    public class OrderRequestModel
    {
        public string CustomerName { get; }
        public string Adress { get; }
        public string City { get; }
        public string State { get; }
        public string Zip { get; }
        public string Country { get; }
        public string Email { get; }
        public Item[] Items { get; }
        public OrderRequestModel(
            string customerName
            , string adress
            , string city
            , string state
            , string zip
            , string country
            , string email
            , Item[] items)
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
    public class Item
    {
        public string Templete { get; }
        public string Mockup { get; }
        public int Quantity { get; }
        public Item(
            string templete
            , string mockup
            , int quantity)
        {
            Templete = templete;
            Mockup = mockup;
            Quantity = quantity;
        }
    }
}
