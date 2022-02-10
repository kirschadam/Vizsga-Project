using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FrontendWPF
{
    /// <summary>
    /// Interaction logic for ViewUsers.xaml
    /// </summary>
    public partial class ViewUsers : Window
    {
        // public static ServiceReference3.UserManagementClient client;
        // public ServiceReference3.User[] user = { };

        public ViewUsers()
        {
            InitializeComponent();

            ReadDatabase();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }



        private void ReadDatabase()
        {
            ServiceReference3.UserServiceClient client = new ServiceReference3.UserServiceClient();
            ServiceReference3.User[] user = { };

            // string query = $"WHERE Username='{userName}' AND Password='{CreateMD5(password)}'";
            string query = $"WHERE 1";  // összes user lekérdezése

            // ServiceReference3.User[] user = { };
            try
            {
                //user = client.UserList(query);

                if (user == null)
                {
                    MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    return;
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Unable to connect to the remote server") || ex.ToString().Contains("EndpointNotFoundException"))
                {
                    MessageBox.Show("The remote server is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    return;
                }
                else
                {
                    MessageBox.Show("An error occurred, the details are the following:\n" + ex.ToString(), caption: "Error message");
                    return;
                }
            }


            // if the query has a result, read database
            if (user.Length > 0)
            {
                /*
                NewContactWindow newContactWindow = new NewContactWindow();
                newContactWindow.ShowDialog();
                */
                // dBListView.ItemsSource = user;
                List<string> dbFieldsList = new List<string>();
                string dbFields;
                foreach (var field in user)
                {
                    dbFields = $"Id: {field.Id}\tName: {field.Username}\tPassword: {field.Password} ";
                    dbFieldsList.Add(dbFields);
                }
                dBListView.ItemsSource = dbFieldsList;



                /*
                List<Contact> contacts;

                using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.databasePath))
                {
                    conn.CreateTable<Contact>();
                    contacts = conn.Table<Contact>().ToList();
                }

                if (contacts != null)
                {
                    contactsListView.ItemsSource = contacts;
                }
                */
            }
            else
            {
                MessageBox.Show("The query resulted no data!");
                return;
            }
            
        }
    }
}
