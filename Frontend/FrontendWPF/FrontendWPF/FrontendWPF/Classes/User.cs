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
        public int? Id;
        public string Username;
        public string Password;
        public string Location;
        public int? Permission;
        public int? Active;


        public User()
        {
        }

        public User(int? id, string username, string password, string location, int? permission, int? active)
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
                    // string query = $"WHERE Username='{userName}' AND Password='{CreateMD5(password)}'";
                    usersArray = client.ListUser(Shared.uid, id, username, location, permissionover, permissionunder, active, region, limit).Users;
                    usersList = usersArray.ToList();
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
            // return user.ToList();
            return usersList;
        }

    }
}