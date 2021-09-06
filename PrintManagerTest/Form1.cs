using System;
using System.Windows.Forms;
using PrintManager;
using Newtonsoft.Json;
using static System.Environment;
namespace PrintManagerTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //EmailExtractTest();
            BurgerPrintTest();
        }
        private void EmailExtractTest()
        {
            textBox1.Text += $"{NewLine}============  EmailExtractTest  =============={NewLine}";
            EmailChecker.EmailSet("yourmail", "yourpassword", "imap-mail.outlook.com", 993);
            EmailChecker checker = new EmailChecker();
            var result = checker.Check(new DateTime(2021, 8, 18));
            string display = "";
            foreach (var r in result)
            {
                display = $"{display}\r\n======================\r\n" +
                    $"OrderCode: {r.OrderCode}\r\n" +
                    $"OrderTime: {r.OrderTime.ToString()}\r\n" +
                    $"Store: {r.Store}\r\n" +
                    $"CustomerName: {r.CustomerName}\r\n" +
                    $"CustomerEmail: {r.CustomerEmail}\r\n" +
                    $"Street: {r.Street}\r\n" +
                    $"City: {r.City}\r\n" +
                    $"State: {r.State}\r\n" +
                    $"ZipCode: {r.ZipCode}\r\n" +
                    $"Country: {r.Country}\r\n" +
                    $"OrderTotal: {r.OrderTotal}\r\n" +
                    $"OrderTotalCurrency: {r.OrderTotalCurrency}\r\n" +
                    $"OrderTotal: {r.OrderTotal}";
                foreach (var i in r.Items)
                {
                    display = $"{display}\r\n----------------------\r\n" +
                        $"ItemName: {i.ItemName}\r\n" +
                        $"Size and stype: {i.SizeSType}\r\n" +
                        $"Color: {i.Color}\r\n" +
                        $"Quantity: {i.Quantity}\r\n" +
                        $"Price: {i.Price}\r\n" +
                        $"PriceCurrency: {i.PriceCurrency}\r\n" +
                        $"";
                }
                textBox1.Text += display;
            }
        }
        private async void BurgerPrintTest()
        {
            textBox1.Text += $"{NewLine}============  BurgerPrintTest  =============={NewLine}";
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
            textBox1.Text += $"{NewLine}------- MakeOrderTest --------{NewLine}";
            if (resBPOrder.is_success)
            {
                textBox1.Text += $"Message: {resBPOrder.message}{NewLine}OrderId: {resBPOrder.order_id}";
                var resGetOrder = await printProvider.GetOrderDetail(resBPOrder.order_id);
                textBox1.Text += $"{NewLine}------- GetOrderTest --------{NewLine}";
                textBox1.Text += JsonConvert.SerializeObject(resGetOrder);
            }
            else
            {
                textBox1.Text += $"Failed.{NewLine}Message: {resBPOrder.message}";
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
            textBox1.Text += $"{NewLine}------- MakeOrderTestWithCustomData --------{NewLine}";
            if (resBPOrderWithCustomData.is_success)
            {
                string resId = resBPOrderWithCustomData.log_id ?? resBPOrderWithCustomData.order_id;
                textBox1.Text += $"Message: {resBPOrderWithCustomData.message}{NewLine}LogId: {resId}";
                var resGetLogOrder = await printProvider.GetLogOrderDetail(resId);
                textBox1.Text += $"{NewLine}------- GetLogOrderTest --------{NewLine}";
                textBox1.Text += JsonConvert.SerializeObject(resGetLogOrder);
            }
            else
            {
                textBox1.Text += $"Failed.{NewLine}Message: {resBPOrderWithCustomData.message}";
            }
        }
        private async void PrintfyTest()
        {
            Printify printProvider = new Printify("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIzN2Q0YmQzMDM1ZmUxMWU5YTgwM2FiN2VlYjNjY2M5NyIsImp0aSI6IjFmYjMwM2MzZjQ1MWU1Zjg5YTU3MDBlOTMzMjM2YmZlZDIwMTExZmIwM2Q5ZTk0ODA5OGEzYzQ1YjhjOGQxZTMyZGE0MmM3YzhmZTYwOTlhIiwiaWF0IjoxNjI5NDE1Mjk4LCJuYmYiOjE2Mjk0MTUyOTgsImV4cCI6MTY2MDk1MTI5OCwic3ViIjoiODYyMzg2MyIsInNjb3BlcyI6WyJzaG9wcy5tYW5hZ2UiLCJzaG9wcy5yZWFkIiwiY2F0YWxvZy5yZWFkIiwib3JkZXJzLnJlYWQiLCJvcmRlcnMud3JpdGUiLCJwcm9kdWN0cy5yZWFkIiwicHJvZHVjdHMud3JpdGUiLCJ3ZWJob29rcy5yZWFkIiwid2ViaG9va3Mud3JpdGUiLCJ1cGxvYWRzLnJlYWQiLCJ1cGxvYWRzLndyaXRlIiwicHJpbnRfcHJvdmlkZXJzLnJlYWQiXX0.AfHEfxqxhaSmer1WOZZguA2khEYxxoo7-oT0Xzwjwwke4suKhiqLbkmd4LRax10LvRNQk_J9vWN_YykX7P4", "test");
            var response = await printProvider.UploadImage(@"D:\test.jpg");
            textBox1.Text = response.ToString();
        }
    }
}
