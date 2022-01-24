using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public static List<ServiceReference3.User> GetUsers(string id, string username, string location, string region, string limit)
        {
            ServiceReference3.UserServiceClient client = new ServiceReference3.UserServiceClient();
            ServiceStorageManger.StockServiceClient stockClient = new ServiceStorageManger.StockServiceClient();
 
            ServiceReference3.User[] usersArray;
            ServiceStorageManger.Stock[] stockArray;
            // ServiceStorageManger.Response_Storage[] responseArray = { };
            List<ServiceReference3.User> usersList = new List<ServiceReference3.User>();
 
            try
            {
                // string query = $"WHERE Username='{userName}' AND Password='{CreateMD5(password)}'";
                usersArray = client.ListUser(Shared.uid, id, username, location, region, limit).Users;
                // ServiceReference3.Response_User response_User = new ServiceReference3.Response_User();
                // string uid = response_User.Uid;

                stockArray = stockClient.ListStock(Shared.uid, "", "").Stocks;

                // if (usersArray == null)
                if (usersArray.Length == 0) // { FrontendWPF.ServiceReference3.User[0]} // TODO: ez jön vissza akkor is, ha elérhető az adatbázis, de üres a lekérés! Módosítani kellene, hogy null értékkel térjen vissza, ha nem tud kapcsolódni az adatbázishoz!

                {
                    MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    // return;
                }
                else
                {
                    usersList = usersArray.ToList();
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
            // return user.ToList();
            return usersList;
        }

    }
}