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
    public class LocationLog
    {
        // class requires getter-setter to be visible in DataGrid!
        public DateTime? LogDate { set; get; }
        public string LogUsername { set; get; }
        public string LogOperation { set; get; }
        public int?  Id { set; get; }
        public string Name { set; get; }
        public string Region { set; get; }

        public LocationLog()
        {
        }

        public LocationLog(DateTime? logDate, string logUsername, string logOperation, int id, string name, string region)
        {
            LogDate = logDate;
            LogUsername = logUsername;
            LogOperation = logOperation;
            Id = id;
            Name = name;
            Region = region;
        }

        // returns a list of locations log
        public static List<LocationLog> GetLocationsLog(DateTime startDate, DateTime endDate)
        {
            string filename = @".\Logs\manageLocations.log";

            StreamReader sr = new StreamReader(filename);
            // check header correctness
            string header_row = sr.ReadLine();
            if (header_row != "LogDate;LogUsername;LogOperation;Id;Name;Region")
            {
                MessageBox.Show($"Incorrect file content! Expected header is 'LogDate;LogUsername;LogOperation;Id;Name;Region', but received '{header_row}'", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            LocationLog locationLog;
            List<LocationLog> logList = new List<LocationLog>();
            List<UserService.User> dbUsersList = new List<UserService.User>();
            List<LocationService.Store> dbLocationsList = new List<LocationService.Store>();
            List<LocationService.Region> dbRegionsList = new List<LocationService.Region>();

            int row_index = 0;
            string[] row;
            int logAdded = 0;
            string errorMessage = "";
            dbUsersList = User.GetUsers("", "", "", "", "", "", "", "");
            if (dbUsersList == null) { return null; } // stop on any error
            dbRegionsList = Classes.Region.GetRegions("", "", "", "");

            while (sr.EndOfStream == false)
            {
                row_index++;
                string error = "";
                row = sr.ReadLine().Split(';');
                if (row.Length != 6) // skip row if number of columns is incorrect
                {
                    errorMessage += $"Product in line {row_index}: The required {6} fields are not available!\n";
                    continue;
                }

                string logDate = row[0];
                string logUsername = row[1];
                string logOperation = row[2];
                string id = row[3];
                string name = row[4];
                string region = row[5];

                // check data correctness
                bool dateExists = DateTime.TryParse(logDate, out DateTime date_entered);
                if (dateExists == false)
                {
                    error += $"Location in line {row_index}: Log date value '{logDate}' is incorrect!\n";
                }
                else if (date_entered > DateTime.Now)
                {
                    error += $"Location in line {row_index}: Date '{logDate}' cannot be a future date!\n";
                }
                else if ((DateTime.Now - date_entered).TotalDays > 3650)
                {
                    error += $"Location in line {row_index}: Date '{logDate}' cannot be earlier than 3650 days!\n";
                }
                if (logOperation != "update")
                {
                    if (name.Length < 3) // if wrong (product) Name value is entered
                    {
                        error += $"Location in line {row_index}: Location name must be at least 3 characters!\n";
                    }
                    if (region.Length < 3)
                    {
                        error += $"Name must be et least 3 characters long!\n";
                    }
                    else if (dbRegionsList.Any(p => p.Name == region) == false) // if region does not exist in database
                    {
                        error += $"The region '{region}' does not exist!\n";
                    }
                }
                errorMessage += error;


                if (error != "") { continue; } // skip on error
                if (DateTime.Parse(logDate) < startDate || DateTime.Parse(logDate) > endDate) { continue; } // skip log date before and after limit dates

                locationLog = new LocationLog
                {
                    LogDate = DateTime.Parse(logDate),
                    LogUsername = logUsername,
                    LogOperation = logOperation,
                    Id = int.Parse(id),
                    Name = name,
                    Region = region
                };
                logAdded++;
                logList.Add(locationLog);

            }
            sr.Close();

            if (errorMessage != "") { MessageBox.Show($"Following error occurred during the data import:\n\n{errorMessage}", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }

            return logList;
        }

    }
}