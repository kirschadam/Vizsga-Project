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
        public int Id;
        public int ProductId;
        public int Quantity;
        public int UnitPrice;
        public DateTime Date;
        public int LocationId;
        public int UserId;

        public SalePurchase()
        {
        }

        public SalePurchase(int id, int productId, int quantity, int unitPrice, DateTime date, int locationId, int userId)
        {
            Id = id;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Date = date;
            LocationId = locationId;
            UserId = userId;
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
                if (hostMessage.Contains("Unable to connect")) //  temporary solultion, I will need error message Unable to connect...

                {
                    MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    return null;
                }
                else if (hostMessage == "Unauthorized user!")
                {
                    MessageBox.Show("The connection to the server was interrupted. Please log in again to continue.", caption: "Error message");

                    // logout
                    Shared.StartWindow.button_login.Content = "LOGIN";
                    Shared.StartWindow.button_login.Foreground = Brushes.LightSalmon;
                    Shared.loggedInUser = null;
                    Shared.loggedIn = false;
                    Shared.StartWindow.button_ManageUsersWindow.IsEnabled = false;
                    Shared.StartWindow.button_ManageUsersWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF9D9D9D");
                    Shared.StartWindow.button_ManageProductsWindow.IsEnabled = false;
                    Shared.StartWindow.button_ManageProductsWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF989898");
                    Shared.StartWindow.button_ManagePurchasesWindow.IsEnabled = false;
                    Shared.StartWindow.button_ManagePurchasesWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF989898");
                    Shared.StartWindow.button_ManageSalesWindow.IsEnabled = false;
                    Shared.StartWindow.button_ManageSalesWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF8C8C8C");
                    Shared.StartWindow.button_ManageLocationsWindow.IsEnabled = false;
                    Shared.StartWindow.button_ManageLocationsWindow.Foreground = Brushes.Gray;

                    // login
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow = new LoginWindow();
                    loginWindow.Show();
                    
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
                    MessageBox.Show("The remote server is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    return null;
                }
                else
                {
                    MessageBox.Show("An error occurred, the details are the following:\n" + ex.ToString(), caption: "Error message");
                    return null;
                }
            }
            // return product.ToList();
            return salesPurchasesList;
            
        }
        
    }
}