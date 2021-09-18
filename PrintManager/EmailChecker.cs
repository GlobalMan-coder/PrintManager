using System.Collections.Generic;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Text.RegularExpressions;
//using Chilkat;
using MailKit.Net.Imap;
using MailKit.Search;

namespace PrintManager
{
    internal class Settings
    {
        public static string Host = "pop.gmail.com";
        public static int Port = 143;
        public static string UserName;
        public static string Password;
    }
    public class EmailChecker
    {
        public static void EmailSet(string UserName, string Password, string Host = null, int? Port = null)
        {
            Settings.UserName = UserName;
            Settings.Password = Password;
            if(Host != null) Settings.Host = Host;
            if (Port != null) Settings.Port = Port.Value;
        }

        public List<EmailModel> Check(DateTime? startTime = null)
        {
            List <EmailModel> result = new List<EmailModel>();
            using (ImapClient client = new ImapClient())
            {
                client.Connect(Settings.Host, Settings.Port, true);
                client.Authenticate(Settings.UserName, Settings.Password);
                var inbox = client.Inbox;
                inbox.Open(MailKit.FolderAccess.ReadOnly);
                var ids = inbox.Search(SearchQuery.All);
                if(startTime != null)
                {
                    ids = inbox.Search(SearchQuery.DeliveredAfter(startTime.Value));
                }
                if (ids.Count == 0) return result;
                foreach(var id in ids)
                {
                    var mail = inbox.GetMessage(id);
                    var model = Extract(mail.HtmlBody);
                    if(model != null)
                    {
                        model.OrderTime = mail.Date.Date;
                        result.Add(model);
                    }
                }
                return result;
            }
        }

        internal EmailModel Extract(string html)
        {
            if (!html.Contains("Etsy sale")) return null;
            EmailModel result = new EmailModel();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var atags = doc.DocumentNode.Descendants("a");
            var orderId = atags.Where(node => node.GetAttributeValue("href", "").Contains("orders/")).FirstOrDefault();
            if (orderId == null) return null;
            var mails = atags.Where(node => node.GetAttributeValue("href", "").Contains("mailto")).ToList();
            int mailCount = mails.Count;
            if(mailCount > 1)
            {
                result.CustomerEmail = mails[mailCount - 1].GetAttributeValue("href", "").Substring(7);
                result.Account = mails[mailCount - 2].GetAttributeValue("href", "").Substring(7);
            }
            
            result.OrderCode = orderId.InnerText;

            var Address = doc.DocumentNode.Descendants("address").FirstOrDefault();

            var lines = Regex.Split(Address.InnerHtml, @"\s*<br>\s*");
            int isStreet2 = (lines.Count() > 5) ? 1 : 0;
            result.CustomerName = lines[0].ToText();
            result.Street = lines[1].ToText() + ((isStreet2 == 0) ? "" : " " + lines[2].ToText());
            result.Country = lines[3 + isStreet2].ToText();
            var citylines = Regex.Split(lines[2 + isStreet2].Trim(), @"\s*<span>\s*");
            foreach(var info in citylines)
            {
                var infoText = info.ToText();
                if (infoText.Length != 0) {
                    if (Regex.Match(info, "[0-9]").Success)
                    {
                        result.ZipCode = infoText;
                    }
                    else if(infoText.Length == 2)
                    {
                        result.State = infoText;
                    }
                    else
                    {
                        result.City = infoText;
                    }
                }
            }

            //.Descendants("span").ToArray();
            //if (Address.Count() > 5)
            //{
            //    result.CustomerName = Address[0].InnerText;
            //    result.Street = Address[1].InnerText;
            //    result.City = Address[2].InnerText;
            //    result.State = Address[3].InnerText;
            //    result.ZipCode = Address[4].InnerText;
            //    result.Country = Address[5].InnerText;
            //}

            //var customerEmail = Regex.Match(html, "people/[0-9a-zA-Z@.]+");
            //if (customerEmail.Success) result.CustomerEmail = customerEmail.Value.Split('/')[1];

            var ItemNameNodes = doc.DocumentNode.Descendants("a").
                Where(node => node.GetAttributeValue("href", "").Contains("transaction/")
                    && node.GetAttributeValue("style", "") != "");
            foreach(var itemname in ItemNameNodes)
            {
                var item = new ItemModel();
                item.ItemName = itemname.InnerText.Trim();
                var itemDetailNodes = itemname.ParentNode.ParentNode.Descendants("div");
                foreach(var detailNode in itemDetailNodes)
                {
                    string innerText = detailNode.InnerText;
                    if (innerText.Contains("Size"))
                    {
                        item.SizeSType = innerText.ToValue();
                    }
                    else if (innerText.Contains("Color"))
                    {
                        item.Color = innerText.ToValue();
                    }
                    else if (innerText.Contains("Quantity"))
                    {
                        item.Quantity = int.Parse(innerText.ToValue());
                    }
                    else if (innerText.Contains("Price"))
                    {
                        var prices = Regex.Matches(innerText.ToValue(), @"([0-9.]+|[^0-9.\s]+)");
                        item.PriceCurrency = prices[0].Value;
                        item.Price = float.Parse(prices[1].Value);
                    }
                    else if (innerText.Contains("Shop") && result.Store == null)
                    {
                        result.Store = innerText.ToValue();
                    }
                }
                result.Items.Add(item);
            }

            var orderSumary = doc.DocumentNode.Descendants("td")
                .Where(node => node.GetAttributeValue("style", "")
                .Contains("text-align:left"));
            var orderTotalNode = orderSumary.Where(node => node.InnerText.Contains("Order total:")).FirstOrDefault();
            if (orderTotalNode != null)
            {
                var orderPrice = Regex.Matches(orderTotalNode.ParentNode.ChildNodes[3].ChildNodes[1].InnerText.Trim(), @"([0-9.]+|[^0-9.\s]+)");
                result.OrderTotal = float.Parse(orderPrice[1].Value);
                result.OrderTotalCurrency = orderPrice[0].Value;
            }

            var itemTotalNode = orderSumary.Where(node => node.InnerText.Contains("Item total:")).FirstOrDefault();
            if (itemTotalNode != null)
            {
                var itemPrice = Regex.Matches(itemTotalNode.ParentNode.ChildNodes[3].ChildNodes[1].InnerText.Trim(), @"([0-9.]+|[^0-9.\s]+)");
                result.ItemTotal = float.Parse(itemPrice[1].Value);
            }

            var subTotalNode = orderSumary.Where(node => node.InnerText.Contains("Subtotal:")).FirstOrDefault();
            if (subTotalNode != null)
            {
                var subTotal = Regex.Matches(subTotalNode.ParentNode.ChildNodes[3].ChildNodes[1].InnerText.Trim(), @"([0-9.]+|[^0-9.\s]+)");
                result.Subtotal = float.Parse(subTotal[1].Value);
            }

            var discountNode = orderSumary.Where(node => node.InnerText.Contains("Discount:")).FirstOrDefault();
            if (discountNode != null)
            {
                var discount = Regex.Matches(discountNode.ParentNode.ChildNodes[3].ChildNodes[1].InnerText.Trim(), @"([0-9.]+|[^0-9.\s]+)");
                result.Discount = float.Parse(discount[2].Value) * -1;
            }

            var shippingNode = orderSumary.Where(node => node.InnerText.Contains("Shipping:")).FirstOrDefault();
            if (shippingNode != null)
            {
                var shipping = Regex.Matches(shippingNode.ParentNode.ChildNodes[3].ChildNodes[1].InnerText.Trim(), @"([0-9.]+|[^0-9.\s]+)");
                result.Shipping = float.Parse(shipping[1].Value);
            }

            var taxNode = orderSumary.Where(node => node.InnerText.Contains("Tax:")).FirstOrDefault();
            if (taxNode != null)
            {
                var tax = Regex.Matches(taxNode.ParentNode.ChildNodes[3].ChildNodes[1].InnerText.Trim(), @"([0-9.]+|[^0-9.\s]+)");
                result.Tax = float.Parse(tax[1].Value);
            }

            return result;
        }
    }
    public class EmailModel
    {
        public string OrderCode { get; internal set; }
        public DateTime OrderTime { get; internal set; }
        public string Account { get; internal set; }
        public string Store { get; internal set; }
        public string CustomerName { get; internal set; }
        public string CustomerEmail { get; internal set; }
        public string Street { get; internal set; }
        public string City { get; internal set; }
        public string State { get; internal set; }
        public string ZipCode { get; internal set; }
        public string Country { get; internal set; }
        public float OrderTotal { get; internal set; }
        public float ItemTotal { get; internal set; }
        public float Discount { get; internal set; }
        public float Subtotal { get; internal set; }
        public float Shipping { get; internal set; }
        public float Tax { get; internal set; }

        public string OrderTotalCurrency { get; internal set; }
        public List<ItemModel> Items { get; internal set; }
        public EmailModel()
        {
            Items = new List<ItemModel>();
        }
    }
    public class ItemModel
    {
        public string ItemName { get; internal set; }
        public string SizeSType { get; internal set; }
        public string Color { get; internal set; }
        public int Quantity { get; internal set; }
        public float Price { get; internal set; }
        public string PriceCurrency { get; internal set; }
    }
    internal static class Extension
    {
        public static string ToValue(this string str, string pattern = ":")
        {
            if(str.Contains(pattern))
            {
                return str.Substring(str.IndexOf(pattern) + 1).Trim();
            }
            else
            {
                return "";
            }
        }
        public static string ToText(this string str, string pattern = ":")
        {
            return Regex.Replace(str, @"<\/?\w+>", "").Trim();
        }
    }
}
