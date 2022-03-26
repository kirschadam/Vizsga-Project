using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrontendWPF.Templates
{
    public class Users
    {
        // returns a list of users
        public static List<UserService.User> GetUsers()
        {

            UserService.UserServiceClient client = new UserService.UserServiceClient();
            UserService.User[] user = { };

            try
            {
                // string query = $"WHERE Username='{userName}' AND Password='{CreateMD5(password)}'";
                string query = $"WHERE 1";  // összes user lekérdezése
                //user = client.UserList(query);

                if (user == null)
                {
                    MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    // return;
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
            return user.ToList();


        }

        
    }
}
