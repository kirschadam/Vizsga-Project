using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FrontendWPF.Classes
{
    public class SalePurchase
    {
        // class requires getter-setter to be visible in DataGrid!
        public string Id { set; get; }
        public string Product { set; get; }
        public string Quantity { set; get; }
        public string TotalPrice { set; get; }
        public string Date { set; get; }
        public string Location { set; get; }
        public string Username { set; get; }

        public SalePurchase()
        {
        }

        public SalePurchase(string id, string product, string quantity, string totalPrice, string date, string location, string username)
        {
            Id = id;
            Product = product;
            Quantity = quantity;
            TotalPrice = totalPrice;
            Date = date;
            Location = location;
            Username = username;
        }


        // returns a list of sales/purchases
        public static List<StockService.SalePurchase> GetSalesPurchases(string type, string id, string product, string qOver, string qUnder, string priceOver, string priceUnder, string before, string after, string location, string user, string limit)
        {
            StockService.StockServiceClient client = new StockService.StockServiceClient();

            StockService.SalePurchase[] salesPurchasesArray;
            List<StockService.SalePurchase> salesPurchasesList = new List<StockService.SalePurchase>();
 
            try
            {
                string hostMessage = client.ListSalePurchase(Shared.uid, type, id, product, qOver, qUnder, priceOver, priceUnder, before, after, location, user, limit).Message;
                if (hostMessage.Contains("Unable to connect") || hostMessage.Contains("One or more errors occurred") || hostMessage.Contains("Egy vagy több hiba történt")) // returns 0 item (instead of null) if backend cannot connect to database
                {
                    MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return null;
                }
                else if (hostMessage == "Unauthorized user!")
                {
                    Shared.Logout();
                    return null;
                }
                else
                {
                    // string query = $"WHERE type='{type}'";
                    salesPurchasesArray = client.ListSalePurchase(Shared.uid, type, id, product, qOver, qUnder, priceOver, priceUnder, before, after, location, user, limit).SalesPurchases;
                    // Message does not give Unable to connect... error, and Array.Length is 0 if no database or no record...
                    salesPurchasesList = salesPurchasesArray.ToList();
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Unable to connect to the remote server") || ex.ToString().Contains("EndpointNotFoundException"))
                {
                    MessageBox.Show("The remote server is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
                else
                {
                    MessageBox.Show("An error occurred, the details are the following:\n" + ex.ToString(), caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
            // return product.ToList();
            return salesPurchasesList;
            
        }
        
    }
}