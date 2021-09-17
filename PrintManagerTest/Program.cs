using PrintManager;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Environment;
namespace PrintManagerTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Test();
        }

        private static async Task Test()
        {
            EmailExtractTest();
            await BurgerPrintTest();
            await PrintfyTest();
        }

        private static void EmailExtractTest()
        {
            Console.WriteLine($"{NewLine}{NewLine}{NewLine}============  EmailExtractTest  =============={NewLine}");
            EmailChecker.EmailSet("yourmail", "yourpassword", "imap-mail.outlook.com", 993);
            EmailChecker checker = new EmailChecker();
            var result = checker.Check(new DateTime(2021, 9, 15));
            Console.WriteLine(JsonSerializer.Serialize(result));
        }
        private static async Task<bool> BurgerPrintTest()
        {
            Console.WriteLine($"{NewLine}{NewLine}{NewLine}============  BurgerPrintTest  =============={NewLine}");
            BurgerPrints printProvider = new BurgerPrints("bd703d21-ca16-4170-8280-60c7ec0c8ee4");
            var resBPOrder = await printProvider.MakeOrderAsync(
                new BPOrderRequest(
                "testName"
                , "testStreet"
                , "testCity"
                , "testState"
                , "testZip"
                , "US"
                , "thanhtuan@gmail.com"
                , new[] {
                    new BPRequestItem(
                    "calY6v692fKj84sF|34ktwfGTwdqrDImB"
                    , 3)
                    , new BPRequestItem(
                    "cDCOea4JdTspaW4j|pYxW6nvrPvlM3lLB"
                    , 3
                    )}
                , true)
                // Not required param
                {
                    shipping_address2 = "testAddress2"
                    ,
                    shipping_phone = "phone"
                    //, reference_order_id = "V3"
                });
            Console.WriteLine($"{NewLine}{NewLine}------- MakeOrderTest --------{NewLine}");
            if (resBPOrder.is_success)
            {
                Console.WriteLine($"Message: {resBPOrder.message}{NewLine}OrderId: {resBPOrder.order_id}");
                var resGetOrder = await printProvider.GetOrderDetail(resBPOrder.order_id);
                Console.WriteLine($"{NewLine}{NewLine}------- GetOrderTest --------{NewLine}");
                Console.WriteLine(JsonSerializer.Serialize(resGetOrder));
            }
            else
            {
                Console.WriteLine($"Failed.{NewLine}Message: {resBPOrder.message}");
            }

            var resBPOrderWithCustomData = await printProvider.MakeOrderWithCustomDataAsync(new BPOrderRequest(
                "testName"
                , "testStreet"
                , "testCity"
                , "testState"
                , "testZip"
                , "US"
                , "thanhtuan@gmail.com"
                , new[] {
                    new BPRequestItemWithCustomData(
                    "FRGSD-S"
                    , "https://d1ud88wu9m1k4s.cloudfront.net/isp/2021/03/04/A2075_store_b7vinpbi8brtf.jpg"
                    , "https://d1ud88wu9m1k4s.cloudfront.net/isp/2021/03/04/A2075_store_b7vinpbi8brtf.jpg"
                    , "https://d1ud88wu9m1k4s.cloudfront.net/isp/2021/03/04/A2075_store_b7vinpbi8brtf.jpg"
                    , "https://d1ud88wu9m1k4s.cloudfront.net/isp/2021/03/04/A2075_store_b7vinpbi8brtf.jpg"
                    , 1)
                    , new BPRequestItemWithCustomData(
                    "FRGSD-S"
                    , "https://d1ud88wu9m1k4s.cloudfront.net/isp/2021/03/04/A2075_store_b7vinpbi8brtf.jpg"
                    , "https://d1ud88wu9m1k4s.cloudfront.net/isp/2021/03/04/A2075_store_b7vinpbi8brtf.jpg"
                    , "https://d1ud88wu9m1k4s.cloudfront.net/isp/2021/03/04/A2075_store_b7vinpbi8brtf.jpg"
                    , "https://d1ud88wu9m1k4s.cloudfront.net/isp/2021/03/04/A2075_store_b7vinpbi8brtf.jpg"
                    , 2
                    )}
                , true)
            // Not required param
            {
                shipping_address2 = "testAddress2"
                    ,
                shipping_phone = "phone"
                //, reference_order_id = "V3"
            });
            Console.WriteLine($"{NewLine}{NewLine}------- MakeOrderTestWithCustomData --------{NewLine}");
            if (resBPOrderWithCustomData.is_success)
            {
                string resId = resBPOrderWithCustomData.log_id ?? resBPOrderWithCustomData.order_id;
                Console.WriteLine($"Message: {resBPOrderWithCustomData.message}{NewLine}LogId: {resId}");
                var resGetLogOrder = await printProvider.GetLogOrderDetail(resId);
                Console.WriteLine($"{NewLine}{NewLine}------- GetLogOrderTest --------{NewLine}");
                Console.WriteLine(JsonSerializer.Serialize(resGetLogOrder));
            }
            else
            {
                Console.WriteLine($"Failed.{NewLine}Message: {resBPOrderWithCustomData.message}");
            }
            return true;
        }
        private static async Task<bool> PrintfyTest()
        {
            Console.WriteLine($"{NewLine}{NewLine}{NewLine}============  PrintfyTest  =============={NewLine}");
            Printify printProvider = new Printify("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIzN2Q0YmQzMDM1ZmUxMWU5YTgwM2FiN2VlYjNjY2M5NyIsImp0aSI6ImEyYmYwNDlkZmY0ZDhkNTc2OWRhNzcwZDJmMDkwY2YyYzQ0MDIyZTk0N2ZjNzk4NmQzNGNlYjE1MDgxN2M1OTQ1ZGE4OTcyMmEwM2IzMTczIiwiaWF0IjoxNjMwOTY3MDg5LCJuYmYiOjE2MzA5NjcwODksImV4cCI6MTY2MjUwMzA4OSwic3ViIjoiODYyMzg2MyIsInNjb3BlcyI6WyJzaG9wcy5tYW5hZ2UiLCJzaG9wcy5yZWFkIiwiY2F0YWxvZy5yZWFkIiwib3JkZXJzLnJlYWQiLCJvcmRlcnMud3JpdGUiLCJwcm9kdWN0cy5yZWFkIiwicHJvZHVjdHMud3JpdGUiLCJ3ZWJob29rcy5yZWFkIiwid2ViaG9va3Mud3JpdGUiLCJ1cGxvYWRzLnJlYWQiLCJ1cGxvYWRzLndyaXRlIiwicHJpbnRfcHJvdmlkZXJzLnJlYWQiXX0.ALrhsgi_Pf63aviX9FaX1OqY7CFDVH08_ycWC5GyULXwiMbaRQAe4oirTBtb_UDdfbFRqIuxwDdc9dyVvj0");

            Console.WriteLine($"{NewLine}{NewLine}------- ImageUploadTest --------{NewLine}");
            var resUpload = await printProvider.UploadImage("D:\\test.jpg");
            Console.WriteLine(JsonSerializer.Serialize(resUpload));

            Console.WriteLine($"{NewLine}{NewLine}------- GetShopListTest --------{NewLine}");
            var resShopList = await printProvider.GetShopList();
            Console.WriteLine(JsonSerializer.Serialize(resShopList));

            Console.WriteLine($"{NewLine}{NewLine}------- GetAllBlueprintsTest --------{NewLine}");
            var resBlueprintList = await printProvider.GetAllBlueprint();
            Console.WriteLine($"{resBlueprintList.Count} blueprints exist.");
            int firstBluePrintId = resBlueprintList[0].id;

            Console.WriteLine($"{NewLine}{NewLine}------- GetBlueprintTest --------{NewLine}");
            var resBlueprint = await printProvider.GetAllBlueprint();
            Console.WriteLine(JsonSerializer.Serialize(resBlueprint));

            Console.WriteLine($"{NewLine}{NewLine}------- GetPrintProvidersTest --------{NewLine}");
            var resPrintProviderList = await printProvider.GetPrintProviders(firstBluePrintId);
            Console.WriteLine(JsonSerializer.Serialize(resPrintProviderList));
            int firstPrintProviderid = resPrintProviderList[0].id;

            Console.WriteLine($"{NewLine}{NewLine}------- GetVariantsTest --------{NewLine}");
            var resVariants = await printProvider.GetVariants(firstBluePrintId, firstPrintProviderid);
            Console.WriteLine($"{resVariants.variants.Count} variants exist");

            Console.WriteLine($"{NewLine}{NewLine}------- CreateProductTest --------{NewLine}");
            var resCreateProduct = await printProvider.CreateProduct(
                resShopList[0].id
                , new CreateProductRequest(
                    "TestProductTitle"
                    , "TestDescription"
                    , firstBluePrintId
                    , firstPrintProviderid
                    , new[] {
                        new RequestVariant(resVariants.variants[0].id, 400, true)
                        , new RequestVariant(resVariants.variants[1].id, 400, true)
                        , new RequestVariant(resVariants.variants[2].id, 400, true)
                    }
                    , new[] {
                        new RequestPrintArea(
                        new [] { resVariants.variants[0].id
                            , resVariants.variants[1].id
                            , resVariants.variants[2].id
                        }
                        , new [] {
                            new RequestPlaceholder(
                                "front"
                                , new []
                                {
                                    new RequestImage(resUpload.id, 0.5f,0.5f, 0.2f, 0)
                                })})}));

            Console.WriteLine(JsonSerializer.Serialize(resCreateProduct));

            Console.WriteLine($"{NewLine}{NewLine}------- CreateOrderTest --------{NewLine}");
            var resCreateOrder = await printProvider.CreateOrder(
                resShopList[0].id
                , new CreateOrderRequest(
                    DateTime.Now.Millisecond.ToString()
                    , "Label"
                    , 1
                    , new[]
                    {
                        new OrderLineItem(resCreateProduct.id, resCreateProduct.variants[0].id, 1)
                    }
                    , new Address("FirstName", "LastName", "Email", "Phone", "US", "Region", "Ad1", "Ad2", "Zip")
                ));
            Console.WriteLine(JsonSerializer.Serialize(resCreateOrder));

            if (resCreateOrder.id != null)
            {
                Console.WriteLine($"{NewLine}{NewLine}------- GetOrderDetailTest --------{NewLine}");
                var resGetOrder = await printProvider.GetOrder(resShopList[0].id, resCreateOrder.id);
                Console.WriteLine(JsonSerializer.Serialize(resGetOrder));
            }

            Console.WriteLine($"{NewLine}{NewLine}------- GetAllOrderTest --------{NewLine}");
            var resGetOrderList = await printProvider.GetOrders(resShopList[0].id);
            Console.WriteLine(JsonSerializer.Serialize(resGetOrderList));
            return true;
        }

    }
}
