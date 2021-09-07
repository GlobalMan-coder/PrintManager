using System;
using System.Windows.Forms;
using PrintManager;
using Newtonsoft.Json;
using static System.Environment;
using System.Threading.Tasks;

namespace PrintManagerTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            //EmailExtractTest();
            await BurgerPrintTest();
            await PrintfyTest();
        }
        private void EmailExtractTest()
        {
            textBox1.AppendText($"{NewLine}{NewLine}{NewLine}============  EmailExtractTest  =============={NewLine}");
            EmailChecker.EmailSet("yourmail", "yourpassword", "imap-mail.outlook.com", 993);
            EmailChecker checker = new EmailChecker();
            var result = checker.Check(new DateTime(2021, 8, 18));
            textBox1.AppendText(JsonConvert.SerializeObject(result));
        }
        private async Task<bool> BurgerPrintTest()
        {
            textBox1.AppendText($"{NewLine}{NewLine}{NewLine}============  BurgerPrintTest  =============={NewLine}");
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
                    , shipping_phone = "phone"
                    //, reference_order_id = "V3"
                });
            textBox1.AppendText($"{NewLine}{NewLine}------- MakeOrderTest --------{NewLine}");
            if (resBPOrder.is_success)
            {
                textBox1.AppendText($"Message: {resBPOrder.message}{NewLine}OrderId: {resBPOrder.order_id}");
                var resGetOrder = await printProvider.GetOrderDetail(resBPOrder.order_id);
                textBox1.AppendText($"{NewLine}{NewLine}------- GetOrderTest --------{NewLine}");
                textBox1.AppendText(JsonConvert.SerializeObject(resGetOrder));
            }
            else
            {
                textBox1.AppendText($"Failed.{NewLine}Message: {resBPOrder.message}");
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
                    , shipping_phone = "phone"
                    //, reference_order_id = "V3"
                });
            textBox1.AppendText($"{NewLine}{NewLine}------- MakeOrderTestWithCustomData --------{NewLine}");
            if (resBPOrderWithCustomData.is_success)
            {
                string resId = resBPOrderWithCustomData.log_id ?? resBPOrderWithCustomData.order_id;
                textBox1.AppendText($"Message: {resBPOrderWithCustomData.message}{NewLine}LogId: {resId}");
                var resGetLogOrder = await printProvider.GetLogOrderDetail(resId);
                textBox1.AppendText($"{NewLine}{NewLine}------- GetLogOrderTest --------{NewLine}");
                textBox1.AppendText(JsonConvert.SerializeObject(resGetLogOrder));
            }
            else
            {
                textBox1.AppendText($"Failed.{NewLine}Message: {resBPOrderWithCustomData.message}");
            }
            return true;
        }
        private async Task<bool> PrintfyTest()
        {
            textBox1.AppendText($"{NewLine}{NewLine}{NewLine}============  PrintfyTest  =============={NewLine}");
            Printify printProvider = new Printify("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIzN2Q0YmQzMDM1ZmUxMWU5YTgwM2FiN2VlYjNjY2M5NyIsImp0aSI6ImEyYmYwNDlkZmY0ZDhkNTc2OWRhNzcwZDJmMDkwY2YyYzQ0MDIyZTk0N2ZjNzk4NmQzNGNlYjE1MDgxN2M1OTQ1ZGE4OTcyMmEwM2IzMTczIiwiaWF0IjoxNjMwOTY3MDg5LCJuYmYiOjE2MzA5NjcwODksImV4cCI6MTY2MjUwMzA4OSwic3ViIjoiODYyMzg2MyIsInNjb3BlcyI6WyJzaG9wcy5tYW5hZ2UiLCJzaG9wcy5yZWFkIiwiY2F0YWxvZy5yZWFkIiwib3JkZXJzLnJlYWQiLCJvcmRlcnMud3JpdGUiLCJwcm9kdWN0cy5yZWFkIiwicHJvZHVjdHMud3JpdGUiLCJ3ZWJob29rcy5yZWFkIiwid2ViaG9va3Mud3JpdGUiLCJ1cGxvYWRzLnJlYWQiLCJ1cGxvYWRzLndyaXRlIiwicHJpbnRfcHJvdmlkZXJzLnJlYWQiXX0.ALrhsgi_Pf63aviX9FaX1OqY7CFDVH08_ycWC5GyULXwiMbaRQAe4oirTBtb_UDdfbFRqIuxwDdc9dyVvj0");

            textBox1.AppendText($"{NewLine}{NewLine}------- ImageUploadTest --------{NewLine}");
            var resUpload = await printProvider.UploadImage("D:\\test.jpg");
            textBox1.AppendText(JsonConvert.SerializeObject(resUpload));

            textBox1.AppendText($"{NewLine}{NewLine}------- GetShopListTest --------{NewLine}");
            var resShopList = await printProvider.GetShopList();
            textBox1.AppendText(JsonConvert.SerializeObject(resShopList));

            textBox1.AppendText($"{NewLine}{NewLine}------- GetAllBlueprintsTest --------{NewLine}");
            var resBlueprintList = await printProvider.GetAllBlueprint();
            textBox1.AppendText($"{resBlueprintList.Count} blueprints exist.");
            int firstBluePrintId = resBlueprintList[0].id;

            textBox1.AppendText($"{NewLine}{NewLine}------- GetBlueprintTest --------{NewLine}");
            var resBlueprint = await printProvider.GetAllBlueprint();
            textBox1.AppendText(JsonConvert.SerializeObject(resBlueprint));

            textBox1.AppendText($"{NewLine}{NewLine}------- GetPrintProvidersTest --------{NewLine}");
            var resPrintProviderList = await printProvider.GetPrintProviders(firstBluePrintId);
            textBox1.AppendText(JsonConvert.SerializeObject(resPrintProviderList));
            int firstPrintProviderid = resPrintProviderList[0].id;

            textBox1.AppendText($"{NewLine}{NewLine}------- GetVariantsTest --------{NewLine}");
            var resVariants = await printProvider.GetVariants(firstBluePrintId, firstPrintProviderid);
            textBox1.AppendText($"{resVariants.variants.Count} variants exist");

            textBox1.AppendText($"{NewLine}{NewLine}------- CreateProductTest --------{NewLine}");
            var resCreateProduct = await printProvider.CreateProduct(
                resShopList[0].id
                , new CreateProductRequest(
                    "TestProductTitle"
                    , "TestDescription"
                    , firstBluePrintId
                    , firstPrintProviderid
                    , new [] {
                        new RequestVariant(resVariants.variants[0].id, 400, true)
                        , new RequestVariant(resVariants.variants[1].id, 400, true)
                        , new RequestVariant(resVariants.variants[2].id, 400, true)
                    }
                    , new [] {
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

            textBox1.AppendText(JsonConvert.SerializeObject(resCreateProduct));

            textBox1.AppendText($"{NewLine}{NewLine}------- CreateOrderTest --------{NewLine}");
            var resCreateOrder = await printProvider.CreateOrder(
                resShopList[0].id
                , new CreateOrderRequest(
                    DateTime.Now.Millisecond.ToString()
                    , "Label"
                    , 1
                    , new []
                    {
                        new OrderLineItem(resCreateProduct.id, resCreateProduct.variants[0].id, 1)
                    }
                    , new Address("FirstName", "LastName", "Email", "Phone", "US", "Region", "Ad1", "Ad2", "Zip")
                ));
            textBox1.AppendText(JsonConvert.SerializeObject(resCreateOrder));

            if(resCreateOrder.id != null)
            {
                textBox1.AppendText($"{NewLine}{NewLine}------- GetOrderDetailTest --------{NewLine}");
                var resGetOrder = await printProvider.GetOrder(resShopList[0].id, resCreateOrder.id);
                textBox1.AppendText(JsonConvert.SerializeObject(resGetOrder));
            }

            textBox1.AppendText($"{NewLine}{NewLine}------- GetAllOrderTest --------{NewLine}");
            var resGetOrderList = await printProvider.GetOrders(resShopList[0].id);
            textBox1.AppendText(JsonConvert.SerializeObject(resGetOrderList));
            return true;
        }
    }
}
