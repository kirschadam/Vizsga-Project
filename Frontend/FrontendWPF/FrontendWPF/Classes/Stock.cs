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
        public string Id { set; get; }
        public string Product { set; get; }
        public string Quantity { set; get; }
        public string Location { set; get; }

        public Stock()
        {
        }

        public Stock(string id, string product, string quantity, string location)
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
                    stocksArray = client.ListStock(Shared.uid, id, product, location, quantityOver, quantityUnder, limit).Stocks;
                    stocksList = stocksArray.ToList();
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
            return stocksList;
            
        }
        
    }
}