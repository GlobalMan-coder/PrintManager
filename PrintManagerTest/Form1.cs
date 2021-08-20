using System;
using System.Windows.Forms;
using PrintManager;

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
            PrintfyTest();
        }
        private void EmailExtractTest()
        {
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
                textBox1.Text = display;
            }
        }
        private async void BurgerPrintTest()
        {
            BurgerPrints printProvider = new BurgerPrints("bd703d21-ca16-4170-8280-60c7ec0c8ee4", true);
            var response  = await printProvider.MakeOrderAsync(new BurgerPrintOrderRequest(
                "testName"
                , "testAdress"
                , "testCity"
                , "testStae"
                , "testzip"
                , "testCountry"
                , "testEmail"
                , new[] {
                    new BurgerPrintOrderItem(
                    "FRGSD-S"
                    , "https://d1ud88wu9m1k4s.cloudfront.net/isp/2021/03/04/A2075_store_b7vinpbi8brtf.jpg"
                    , "https://d1ud88wu9m1k4s.cloudfront.net/isp/2021/03/04/A2075_store_b7vinpbi8brtf.jpg"
                    , 1)
                    , new BurgerPrintOrderItem(
                    "FRGSD-S"
                    , "https://d1ud88wu9m1k4s.cloudfront.net/isp/2021/03/04/A2075_store_b7vinpbi8brtf.jpg"
                    , "https://d1ud88wu9m1k4s.cloudfront.net/isp/2021/03/04/A2075_store_b7vinpbi8brtf.jpg"
                    , 2
                    )}));
            textBox1.Text = response.ToString();
        }
        private async void PrintfyTest()
        {
            Printify printProvider = new Printify("eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJhdWQiOiIzN2Q0YmQzMDM1ZmUxMWU5YTgwM2FiN2VlYjNjY2M5NyIsImp0aSI6IjFmYjMwM2MzZjQ1MWU1Zjg5YTU3MDBlOTMzMjM2YmZlZDIwMTExZmIwM2Q5ZTk0ODA5OGEzYzQ1YjhjOGQxZTMyZGE0MmM3YzhmZTYwOTlhIiwiaWF0IjoxNjI5NDE1Mjk4LCJuYmYiOjE2Mjk0MTUyOTgsImV4cCI6MTY2MDk1MTI5OCwic3ViIjoiODYyMzg2MyIsInNjb3BlcyI6WyJzaG9wcy5tYW5hZ2UiLCJzaG9wcy5yZWFkIiwiY2F0YWxvZy5yZWFkIiwib3JkZXJzLnJlYWQiLCJvcmRlcnMud3JpdGUiLCJwcm9kdWN0cy5yZWFkIiwicHJvZHVjdHMud3JpdGUiLCJ3ZWJob29rcy5yZWFkIiwid2ViaG9va3Mud3JpdGUiLCJ1cGxvYWRzLnJlYWQiLCJ1cGxvYWRzLndyaXRlIiwicHJpbnRfcHJvdmlkZXJzLnJlYWQiXX0.AfHEfxqxhaSmer1WOZZguA2khEYxxoo7-oT0Xzwjwwke4suKhiqLbkmd4LRax10LvRNQk_J9vWN_YykX7P4");
            var response = await printProvider.UploadImage(@"D:\test.jpg");
            textBox1.Text = response.ToString();
        }
    }
}
