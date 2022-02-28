using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FrontendWPF.Classes
{
    public class ProductLog
    {
        // class requires getter-setter to be visible in DataGrid!
        public DateTime? LogDate { set; get; }
        public string LogUsername { set; get; }
        public string LogOperation { set; get; }
        public int? Id { set; get; }
        public string Name { set; get; }
        public int? BuyUnitPrice { set; get; }
        public int? SellUnitPrice { set; get; }

        public ProductLog()
        {
        }

        public ProductLog(DateTime? logDate, string logUsername, string logOperation, int id, string name, int? buyUnitPrice, int? sellUnitPrice)
        {
            LogDate = logDate;
            LogUsername = logUsername;
            LogOperation = logOperation;
            Id = id;
            Name = name;
            BuyUnitPrice = buyUnitPrice;
            SellUnitPrice = sellUnitPrice;
        }

        // returns a list of products log
        public static List<ProductLog> GetProductsLog(DateTime startDate, DateTime endDate)
        {
            string filename = "manageProducts.log";

            StreamReader sr = new StreamReader(filename);
            // check header correctness
            string header_row = sr.ReadLine();
            if (header_row != "LogDate;LogUsername;LogOperation;Id;Name;BuyUnitPrice;SellUnitPrice")
            {
                MessageBox.Show($"Incorrect file content! Expected header is 'LogDate;LogUsername;LogOperation;Id;Name;BuyUnitPrice;SellUnitPrice', but received '{header_row}'", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            ProductLog productLog;
            List<ProductLog> logList = new List<ProductLog>();
            List<UserService.User> dbUsersList = new List<UserService.User>();

            int row_index = 0;
            string[] row;
            int logAdded = 0;
            string errorMessage = "";
            dbUsersList = User.GetUsers("", "", "", "", "","", "", "");
            if (dbUsersList == null) { return null; } // stop on any error

            while (sr.EndOfStream == false)
            {
                row_index++;
                string error = "";
                row = sr.ReadLine().Split(';');
                if (row.Length != 7) // skip row if number of columns is incorrect
                {
                    errorMessage += $"Product in line {row_index}: The required {7} fields are not available!\n";
                    continue;
                }

                string logDate = row[0];
                string logUsername = row[1];
                string logOperation = row[2];
                string id = row[3];
                string name = row[4];
                string buyUnitPrice = row[5];
                string sellUnitPrice = row[6];

                // check data correctness
                bool dateExists = DateTime.TryParse(logDate, out DateTime date_entered);
                if (dateExists == false)
                {
                    error += $"Product in line {row_index}: Log date value '{logDate}' is incorrect!\n";
                }
                else if (date_entered > DateTime.Now)
                {
                    error += $"Product in line {row_index}: Date '{logDate}' cannot be a future date!\n";
                }
                else if ((DateTime.Now - date_entered).TotalDays > 3650)
                {
                    error += $"Product in line {row_index}: Date '{logDate}' cannot be earlier than 3650 days!\n";
                }
                if (logOperation != "update")
                {
                    if (name.Length < 5) // if wrong (product) Name value is entered
                    {
                        error += $"Product in line {row_index}: Product name must be at least 5 characters!\n";
                    }
                    int? int_val = Int32.TryParse(buyUnitPrice, out var tempVal) ? tempVal : (int?)null;
                    if (int_val == null || (int_val < 0))
                    {
                        error += $"Product in line {row_index}: Purchase price '{buyUnitPrice}' is incorrect!\n";
                    }
                    else if (int_val > 1000000000)
                    {
                        error += $"Product in line {row_index}: Purchase price '{buyUnitPrice}' cannot exceed 1,000,000,000!\n";
                    }
                    int_val = Int32.TryParse(sellUnitPrice, out var tempVal1) ? tempVal1 : (int?)null;
                    if (int_val == null || (int_val < 0))
                    {
                        error += $"Product in line {row_index}: Sales price '{sellUnitPrice}' is incorrect!\n";
                    }
                    else if (int_val > 1000000000)
                    {
                        error += $"Product in line {row_index}: Sales price '{sellUnitPrice}' cannot exceed 1,000,000,000!\n";
                    }
                }
                errorMessage += error;


                if (error != "") { continue; } // skip on error
                if (DateTime.Parse(logDate) < startDate || DateTime.Parse(logDate) > endDate) { continue; } // skip log date before and after limit dates

                int? buyUnitPriceVal = Int32.TryParse(buyUnitPrice, out var tempVal2) ? tempVal2 : (int?)null;
                int? sellUnitPriceVal = Int32.TryParse(sellUnitPrice, out var tempVal3) ? tempVal3 : (int?)null;
                productLog = new ProductLog
                {
                    LogDate = DateTime.Parse(logDate),
                    LogUsername = logUsername,
                    LogOperation = logOperation,
                    Id = int.Parse(id),
                    Name = name,
                    BuyUnitPrice = buyUnitPriceVal,
                    SellUnitPrice = sellUnitPriceVal,
                };
                logAdded++;
                logList.Add(productLog);

            }
            sr.Close();

            if (errorMessage != "") { MessageBox.Show($"Following error occurred during the data import:\n\n{errorMessage}", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }

            return logList;
        }

    }
}