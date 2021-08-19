using System;
using System.Threading.Tasks;

namespace PrintManager
{
    public class Printify : IPrintProvider
    {
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
