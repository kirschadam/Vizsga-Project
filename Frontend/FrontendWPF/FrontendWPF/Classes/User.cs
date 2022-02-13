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
        public int Id;
        public string Username;
        public string Password;

        public User()
        {
        }

        public User(int id, string username, string password)
        {
            Id = id;
            Username = username;
            Password = password;
        }

        // returns a list of users
        public static List<UserService.User> GetUsers(string id, string username, string location, string region, string limit)
        {
            UserService.UserServiceClient client = new UserService.UserServiceClient();
 
            UserService.User[] usersArray;
            List<UserService.User> usersList = new List<UserService.User>();
 
            try
            {
                string hostMessage = client.ListUser(Shared.uid, id, username, location, region, limit).Message;
                if (hostMessage.Contains("Unable to connect"))
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
                    // string query = $"WHERE Username='{userName}' AND Password='{CreateMD5(password)}'";
                    usersArray = client.ListUser(Shared.uid, id, username, location, region, limit).Users;
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