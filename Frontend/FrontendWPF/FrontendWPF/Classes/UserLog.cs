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
    public class UserLog
    {
        // class requires getter-setter to be visible in DataGrid!
        public DateTime? LogDate { set; get; }
        public string LogUsername { set; get; }
        public string LogOperation { set; get; }
        public int?  Id { set; get; }
        public string Username { set; get; }
        public string Password { set; get; }
        public string Location { set; get; }
        public int? Permission { set; get; }
        public int? Active { set; get; }

        public UserLog()
        {
        }

        public UserLog(DateTime? logDate, string logUsername, string logOperation, int? id, string username, string password, string location, int? permission, int? active)
        {
            LogDate = logDate;
            LogUsername = logUsername;
            LogOperation = logOperation;
            Id = id;
            Username = username;
            Password = password;
            Location = location;
            Permission = permission;
            Active = active;
        }

        // returns a list of users log
        public static List<UserLog> GetUsersLog(DateTime startDate, DateTime endDate)
        {
            string filename = "manageUsers.log";

            StreamReader sr = new StreamReader(filename);
            // check header correctness
            string header_row = sr.ReadLine();
            if (header_row != "LogDate;LogUsername;LogOperation;Id;Username;Password;Location;Permission;Active")
            {
                MessageBox.Show($"Incorrect file content! Expected header is 'LogDate;LogUsername;LogOperation;Id;Username;Password;Location;Permission;Active', but received '{header_row}'", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }

            UserLog userLog;
            List<UserLog> logList = new List<UserLog>();
            List<UserService.User> dbUsersList = new List<UserService.User>();

            int row_index = 0;
            string[] row;
            int logAdded = 0;
            string errorMessage = "";
            dbUsersList = User.GetUsers("", "", "", "", "", "", "", "");
            if (dbUsersList == null) { return null; } // stop on any error

            while (sr.EndOfStream == false)
            {
                row_index++;
                string error = "";
                row = sr.ReadLine().Split(';');
                if (row.Length != 9) // skip row if number of columns is incorrect
                {
                    errorMessage += $"User in line {row_index}: The required {9} fields are not available!\n";
                    continue;
                }

                string logDate = row[0];
                string logUsername = row[1];
                string logOperation = row[2];
                string id = row[3];
                string username = row[4];
                string password = row[5];
                string location = row[6];
                string permission = row[7];
                string active = row[8];

                // check data correctness
                bool dateExists = DateTime.TryParse(logDate, out DateTime date_entered);
                if (dateExists == false)
                {
                    error += $"User in line {row_index}: Log date value '{logDate}' is incorrect!\n";
                }
                else if (date_entered > DateTime.Now)
                {
                    error += $"User in line {row_index}: Date '{logDate}' cannot be a future date!\n";
                }
                else if ((DateTime.Now - date_entered).TotalDays > 3650)
                {
                    error += $"User in line {row_index}: Date '{logDate}' cannot be earlier than 3650 days!\n";
                }
                if (logOperation != "update")
                {
                    if (username.Length < 5) // if wrong Username value is entered
                    {
                        error += $"User in line {row_index}: Username '{username}' must be at least 5 characters!\n";
                    }
                    if(password.Length < 5)
                    {
                        error += $"'{username}': Password must be at least 5 charachters long!\n";
                    }
                    if (Shared.permissionList.Any(p => p == permission) == false) // if wrong Permission value is entered
                    {
                        error += $"'{username}': Persmission value '{permission}' does not exist!\n";
                    }
                    if ((active == "0" || active == "1") == false) // if wrong Active value is entered
                    {
                        error += $"'{username}': Active value '{active}' does not exist!\n";
                    }
                }
                errorMessage += error;


                if (error != "") { continue; } // skip on error
                if (DateTime.Parse(logDate) < startDate || DateTime.Parse(logDate) > endDate) { continue; } // skip log date before and after limit dates

                int? permissionVal = Int32.TryParse(permission, out var tempVal2) ? tempVal2 : (int?)null;
                int? activeVal = Int32.TryParse(active, out var tempVal3) ? tempVal3 : (int?)null;
                userLog = new UserLog
                {
                    LogDate = DateTime.Parse(logDate),
                    LogUsername = logUsername,
                    LogOperation = logOperation,
                    Id = int.Parse(id),
                    Username = username,
                    Password = password,
                    Location = location,
                    Permission = permissionVal,
                    Active = activeVal
                };
                logAdded++;
                logList.Add(userLog);

            }
            sr.Close();

            if (errorMessage != "") { MessageBox.Show($"Following error occurred during the data import:\n\n{errorMessage}", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }

            return logList;
        }

    }
}