using System.Collections.Generic;
using MailKit.Net.Imap;
using MailKit;
using HtmlAgilityPack;
using System;
using System.Linq;
using System.Text.RegularExpressions;

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
            try
            {
                using (ImapClient client = new ImapClient())
                {
                    client.Connect(Settings.Host, Settings.Port);
                    client.Authenticate(Settings.UserName, Settings.Password);
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);
                    if (inbox.Count == 0) return result;
                    for (int i = inbox.Count - 1; i >= 0; i--)
                    {
                        var message = inbox.GetMessage(i);
                        if (startTime != null && message.Date.DateTime < startTime) break;
                        var email = Extract(message.HtmlBody);
                        if(email != null)
                        {
                            email.OrderTime = message.Date.ToString();
                            result.Add(email);
                        }
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal EmailModel Extract(string html)
        {
            if (!html.Contains("Etsy sale")) return null;
            EmailModel result = new EmailModel();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            var orderId = doc.DocumentNode.Descendants("a").
                Where(node => node.GetAttributeValue("href", "").Contains("orders/")).FirstOrDefault();
            if (orderId == null) return null;
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

            var customerEmail = Regex.Match(html, "people/[0-9a-zA-Z@.]+");
            if (customerEmail.Success) result.CustomerEmail = customerEmail.Value.Split('/')[1];

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
                        item.Quantity = innerText.ToValue();
                    }
                    else if (innerText.Contains("Price"))
                    {
                        var prices = Regex.Matches(innerText.ToValue(), @"([0-9.]+|[^0-9.\s]+)");
                        item.PriceCurrency = prices[0].Value;
                        item.Price = prices[1].Value;
                    }
                    else if (innerText.Contains("Shop") && result.Store == null)
                    {
                        result.Store = innerText.ToValue();
                    }
                }
                result.Items.Add(item);
            }

            var orderTotal = doc.DocumentNode.Descendants("td")
                .Where(node => node.GetAttributeValue("style", "")
                .Contains("text-align:left") && node.InnerText.Contains("Order total:")).FirstOrDefault()
                .ParentNode.ChildNodes[3].ChildNodes[1];
            var price = Regex.Matches(orderTotal.InnerText.Trim(), @"([0-9.]+|[^0-9.\s]+)");
            result.OrderTotal = price[1].Value;
            result.OrderTotalCurrency = price[0].Value;
            return result;
        }
    }
    public class EmailModel
    {
        public string OrderCode { get; internal set; }
        public string OrderTime { get; internal set; }
        public string Account { get; internal set; }
        public string Store { get; internal set; }
        public string CustomerName { get; internal set; }
        public string CustomerEmail { get; internal set; }
        public string Street { get; internal set; }
        public string City { get; internal set; }
        public string State { get; internal set; }
        public string ZipCode { get; internal set; }
        public string Country { get; internal set; }
        public string OrderTotal { get; internal set; }
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
        public string Quantity { get; internal set; }
        public string Price { get; internal set; }
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
