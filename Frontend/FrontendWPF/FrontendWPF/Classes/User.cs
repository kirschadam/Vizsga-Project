using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FrontendWPF.Classes
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Location { get; set; }
        public string Permission { get; set; }
        public string Active { get; set; }


    public User()
        {
        }

        public User(string id, string username, string password, string location, string permission, string active)
        {
            Id = id;
            Username = username;
            Password = password;
            Location = location;
            Permission = permission;
            Active = active;
        }

        // returns a list of users
        public static List<UserService.User> GetUsers(string id, string username, string location, string permissionover, string permissionunder, string active,  string region, string limit)
        {
            UserService.UserServiceClient client = new UserService.UserServiceClient();
 
            UserService.User[] usersArray;
            List<UserService.User> usersList = new List<UserService.User>();
 
            try
            {
                string hostMessage = client.ListUser(Shared.uid, id, username, location, permissionover, permissionunder, active, region, limit).Message;
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
                    // string query = $"WHERE Username='{userName}' AND Password='{CreateMD5(password)}'";
                    usersArray = client.ListUser(Shared.uid, id, username, location, permissionover, permissionunder, active, region, limit).Users;
                    usersList = usersArray.ToList();
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
            // return user.ToList();
            return usersList;
        }

    }
}