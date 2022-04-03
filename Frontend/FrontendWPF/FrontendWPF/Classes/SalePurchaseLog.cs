using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using FrontendWPF.Classes;

namespace FrontendWPF.Classes
{
    public class Filter_SalePurchaseLog
    {
        // class requires getter-setter to be visible in DataGrid!
        public string LogDate { set; get; }
        public string LogUsername { set; get; }
        public string LogOperation { set; get; }
        public string Id { set; get; }
        public string Product { set; get; }
        public string Quantity { set; get; }
        public string TotalPrice { set; get; }
        public string Date { set; get; }
        public string Location { set; get; }
        public string Username { set; get; }

        public Filter_SalePurchaseLog()
        {
        }
    }

    public class SalePurchaseLog
    {
        // class requires getter-setter to be visible in DataGrid!
        public DateTime? LogDate { set; get; }
        public string LogUsername { set; get; }
        public string LogOperation { set; get; }
        public int? Id { set; get; }
        public string Product { set; get; }
        public int? Quantity { set; get; }
        public int? TotalPrice { set; get; }
        public DateTime? Date { set; get; }
        public string Location { set; get; }
        public string Username { set; get; }

        public SalePurchaseLog()
        {
        }

        public SalePurchaseLog(DateTime? logDate, string logUsername, string logOperation, int? id, string product, int? quantity, int? totalPrice, DateTime? date, string location, string username)
        {
            LogDate = logDate;
            LogUsername = logUsername;
            LogOperation = logOperation;
            Id = id;
            Product = product;
            Quantity = quantity;
            TotalPrice = totalPrice;
            Date = date;
            Location = location;
            Username = username;
        }


        // returns a list of sales/purchases log
        public static List<SalePurchaseLog> GetSalesPurchasesLog(string type, DateTime startDate, DateTime endDate)
        {
            string filename = type == "purchase" ? @".\Logs\managePurchases.log" : @".\Logs\manageSales.log";

            StreamReader sr = new StreamReader(filename);
            // check header correctness
            string header_row = sr.ReadLine();
            if (header_row != "LogDate;LogUsername;LogOperation;Id;Product;Quantity;TotalPrice;Date;Location;Username")
            {
                MessageBox.Show($"Incorrect file content! Expected header is 'LogDate;LogUsername;LogOperation;Id;Product;Quantity;TotalPrice;Date;Location;Username', but received '{header_row}'", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            SalePurchaseLog salePurchaseLog;
            List<SalePurchaseLog> logList = new List<SalePurchaseLog>();
            List<StockService.Product> dbProductsList = new List<StockService.Product>();
            List<UserService.User> dbUsersList = new List<UserService.User>();
            List<LocationService.Store> dbLocationsList = new List<LocationService.Store>();

            int row_index = 0;
            string[] row;
            int logAdded = 0;
            string errorMessage = "";
            dbLocationsList = FrontendWPF.Classes.Location.GetLocations("", "", "", "");
            if (dbLocationsList == null) { return null; } // stop on any error
            dbProductsList = FrontendWPF.Classes.Product.GetProducts("", "", "", "", "", "", "");
            if (dbProductsList == null) { return null; } // stop on any error
            dbUsersList = User.GetUsers("", "", "", "", "", "", "", "");
            if (dbUsersList == null) { return null; } // stop on any error

            while (sr.EndOfStream == false)
            {
                row_index++;
                string error = "";
                row = sr.ReadLine().Split(';');
                if (row.Length != 10) // skip row if number of columns is incorrect
                {
                    errorMessage += $"{type} in line {row_index}: The required {10} fields are not available!\n";
                    continue;
                }

                string logDate = row[0];
                string logUsername = row[1];
                string logOperation = row[2];
                string id = row[3];
                string product = row[4];
                string quantity = row[5];
                string totalPrice = row[6];
                string date = row[7];
                string location = row[8];
                string username = row[9];

                // check data correctness
                bool dateExists = DateTime.TryParse(logDate, out DateTime date_entered);
                if (dateExists == false)
                {
                    error += $"{type} in line {row_index}: Log date value '{logDate}' is incorrect!\n";
                }
                else if (date_entered > DateTime.Now)
                {
                    error += $"{type} in line {row_index}: Date '{logDate}' cannot be a future date!\n";
                }
                else if ((DateTime.Now - date_entered).TotalDays > 3650)
                {
                    error += $"{type} in line {row_index}: Date '{logDate}' cannot be earlier than 3650 days!\n";
                }
                if (logOperation != "update")
                {
                    if (product.Length < 5) // if wrong (product) Name value is entered
                    {
                        error += $"{type} in line {row_index}: Product name must be at least 5 characters!\n";
                    }
                    if (dbProductsList.Any(p => p.Name == product) == false)
                    {
                        error += $"{type} in line {row_index}: product '{product}' does not exist!\n";
                    }
                    int? int_val = Int32.TryParse(quantity, out var tempVal) ? tempVal : (int?)null;
                    if (int_val == null || (int_val <= 0))
                    {
                        error += $"{type} in line {row_index}: Quantity '{quantity}' is incorrect!\n";
                    }
                    else if (int_val > 1000000000)
                    {
                        error += $"{type} in line {row_index}: Quantity '{quantity}' cannot exceed 1,000,000,000!\n";
                    }
                    int_val = Int32.TryParse(totalPrice, out var tempVal1) ? tempVal1 : (int?)null;
                    if (int_val == null || (int_val < 0))
                    {
                        error += $"{type} in line {row_index}: Total price '{quantity}' is incorrect!\n";
                    }
                    else if (int_val > 1000000000)
                    {
                        error += $"{type} in line {row_index}: Total price '{quantity}' cannot exceed 1,000,000,000!\n";
                    }
                    dateExists = DateTime.TryParse(date, out DateTime date_entered1);
                    if (dateExists == false)
                    {
                        error += $"{type} in line {row_index}: Date value '{date}' is incorrect!\n";
                    }
                    else if (date_entered1 > DateTime.Now)
                    {
                        error += $"{type} in line {row_index}: Date '{date}' cannot be a future date!\n";
                    }
                    else if ((DateTime.Now - date_entered1).TotalDays > 3650)
                    {
                        error += $"{type} in line {row_index}: Date '{date}' cannot be earlier than 3650 days!\n";
                    }
                    if (dbLocationsList.Any(p => p.Name == location) == false) // if wrong Location name is entered
                    {
                        error += $"{type} in line {row_index}: Location '{location}' does not exist!\n";
                    }
                    if (username.Length < 5)
                    {
                        error += $"{type} in line {row_index}: Username '{username}' must be at least 5 charachters long!\n";
                    }
                    if (dbUsersList.Any(p => p.Username == username) == false) // if user does not exist in database
                    {
                        error += $"{type} in line {row_index + 1}: User '{username}' does not exist!\n";
                    }
                }
                errorMessage += error;
                

                if (error != "") { continue; } // skip on error
                if (DateTime.Parse(logDate) < startDate || DateTime.Parse(logDate) > endDate) { continue; } // skip log date before and after limit dates

                int? quantityVal = Int32.TryParse(quantity, out var tempVal2) ? tempVal2 : (int?)null;
                int? totalPriceVal = Int32.TryParse(totalPrice, out var tempVal3) ? tempVal3 : (int?)null;
                DateTime? dateVal = DateTime.TryParse(date, out var tempVal4) ? tempVal4 : (DateTime?)null;
                salePurchaseLog = new SalePurchaseLog
                {
                    LogDate = DateTime.Parse(logDate),
                    LogUsername = logUsername,
                    LogOperation = logOperation,
                    Id = int.Parse(id),
                    Product = product,
                    Quantity = quantityVal,
                    TotalPrice = totalPriceVal,
                    Date = dateVal,
                    Location = location,
                    Username = username
                };
                logAdded++;
                logList.Add(salePurchaseLog);

            }
            sr.Close();

            if (errorMessage != "") { MessageBox.Show($"Following error occurred during the data import:\n\n{errorMessage}", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
            
            return logList;
        }

    }
}