using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrontendWPF.Classes
{
    public class SalePurchase
    {
        public int Id;
        public int ProductId;
        public int Quantity;
        public DateTime Date;
        public int LocationId;
        public int UserId;

        public SalePurchase()
        {
        }

        public SalePurchase(int id, int productId, int quantity, DateTime date, int locationId, int userId)
        {
            Id = id;
            ProductId = productId;
            Quantity = quantity;
            Date = date;
            LocationId = locationId;
            UserId = userId;
        }

        // returns a list of sales/purchases
        public static List<StockService.SalePurchase> GetSalesPurchases(string type, string id, string product, string qOver, string qUnder, string before, string after, string location, string user, string limit)
        {
            StockService.StockServiceClient client = new StockService.StockServiceClient();

            StockService.SalePurchase[] salesPurchasesArray;
            List<StockService.SalePurchase> salesPurchasesList = new List<StockService.SalePurchase>();
 
            try
            {
                // string query = $"WHERE type='{type}'";
                salesPurchasesArray = client.ListSalePurchase(Shared.uid, type, id, product, qOver, qUnder, before, after, location, user, limit).SalesPurchases;


                if (salesPurchasesArray.Length == 0)

                {
                    MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    // return;
                }
                else
                {
                    salesPurchasesList = salesPurchasesArray.ToList();
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Unable to connect to the remote server") || ex.ToString().Contains("EndpointNotFoundException"))
                {
                    MessageBox.Show("The remote server is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    // return;
                }
                else
                {
                    MessageBox.Show("An error occurred, the details are the following:\n" + ex.ToString(), caption: "Error message");
                    //  return;
                }
            }
            // return product.ToList();
            return salesPurchasesList;
            
        }
        
    }
}