using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            EmailChecker.EmailSet("your email", "emailpassword", "imap.gmail.com", 993);
            EmailChecker checker = new EmailChecker();
            var result = checker.Check(new DateTime(2021, 8, 18));
            string display = "";
            foreach(var r in result)
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
                foreach(var i in r.Items)
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
    }
}
