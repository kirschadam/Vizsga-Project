using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrontendWPF.Classes
{
    public class Product
    {
        public int Id;
        public string name;
        public int unitPrice;

        public Product()
        {
        }

        public Product(int id, string productname, int unitprice)
        {
            Id = id;
            name = productname;
            unitPrice = unitprice;
        }

        // returns a list of products
        public static List<StockService.Product> GetProducts(string id, string name, string qOver, string qUnder, string limit)
        {
            StockService.StockServiceClient client = new StockService.StockServiceClient();

            StockService.Product[] productsArray;
            List<StockService.Product> productsList = new List<StockService.Product>();
 
            try
            {
                // string query = $"WHERE name='{productName}' AND unitPrice='{CreateMD5(unitprice)}'";
                productsArray = client.ListProduct(Shared.uid, id, name, qOver, qUnder, limit).Products;
                // ServiceReference3.Response_Product response_Product = new ServiceReference3.Response_Product();
                // string uid = response_Product.Uid;

                // if (productsArray == null)
                if (productsArray.Length == 0) // { FrontendWPF.ServiceReference3.Product[0]} // TODO: ez jön vissza akkor is, ha elérhető az adatbázis, de üres a lekérés! Módosítani kellene, hogy null értékkel térjen vissza, ha nem tud kapcsolódni az adatbázishoz!

                {
                    MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    // return;
                }
                else
                {
                    productsList = productsArray.ToList();
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
            return productsList;
            
        }
        
    }
}