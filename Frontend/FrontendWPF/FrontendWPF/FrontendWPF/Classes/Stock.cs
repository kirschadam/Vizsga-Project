using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FrontendWPF.Classes
{
    public class Stock
    {
        public int Id { set; get; }
        public string Product { set; get; }
        public int Quantity { set; get; }
        public string Location { set; get; }

        public Stock()
        {
        }

        public Stock(int id, string product, int quantity, string location)
        {
            Id = id;
            Product = product;
            Quantity = quantity;
            Location = location;
        }

        // returns a list of stocks
        public static List<StockService.Stock> GetStocks(string id, string product, string location, string quantityOver, string quantityUnder, string limit)
        {
            StockService.StockServiceClient client = new StockService.StockServiceClient();

            StockService.Stock[] stocksArray;
            List<StockService.Stock> stocksList = new List<StockService.Stock>();
 
            try
            {
                string hostMessage = client.ListStock(Shared.uid, id, product, location,       quantityOver, quantityUnder, limit).Message;
                if (hostMessage.Contains("Unable to connect"))
                {
                    MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    return null;
                }
                else if (hostMessage == "Unauthorized user!")
                {
                    Shared.Logout();
                    return null;
                }
                else
                {
                    stocksArray = client.ListStock(Shared.uid, id, product, location, quantityOver, quantityUnder, limit).Stocks;
                    stocksList = stocksArray.ToList();
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Unable to connect to the remote server") || ex.ToString().Contains("EndpointNotFoundException"))
                {
                    MessageBox.Show("The remote server is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    return null;
                }
                else
                {
                    MessageBox.Show("An error occurred, the details are the following:\n" + ex.ToString(), caption: "Error message");
                    return null;
                }
            }
            return stocksList;
            
        }
        
    }
}