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
using System.Collections.ObjectModel;
using FrontendWPF.Classes;
using System.ComponentModel;

namespace FrontendWPF
{
    /// <summary>
    /// Interaction logic for DataBinding_ObservableCollection.xaml
    /// </summary>
    public partial class DataGrid_Users : Window
    {
        //An ObservableCollection Represents a dynamic data collection that provides 
        //notifications when items get added, removed, or when the whole list is refreshed.
        //Basically a fancy list but we cannot use lists to make bindings to, it just does not
        //work, because lists don't inherit from INotifyCollectionChanged thus they cannot send
        //notifications when items get added or removed so the view does not know if things are
        //added or deleted
        // public ObservableCollection<UserService.User> AvailableNumbers { get; set; }
        public List<UserService.User> dbUsersList { get; set; }
        public List<User> usersList { get; set; }

        public DataGrid_Users()
        {
            InitializeComponent();
            dataGrid1.RowHeaderWidth = 0;
            // dataGrid1.IsSynchronizedWithCurrentItem = true;

            // dbUsersList = new List<UserService.User>();
            // string query = $"WHERE 1";  // összes user lekérdezése
            dbUsersList = User.GetUsers("", "", "", "", "", "", "", ""); // query users from database

            // close window and stop if no user is retrieved
            if (dbUsersList.Count == 0)
            {
                IsEnabled = false;
                Close();
                return;
            }


            usersList = new List<User>();
            for (int i = 0; i < dbUsersList.Count; i++)
            {
                usersList.Add(new User((int)dbUsersList[i].Id, dbUsersList[i].Username, dbUsersList[i].Password, dbUsersList[i].Location, dbUsersList[i].Permission, dbUsersList[i].Active));
                // AvailableNumbers.Add(usersList[i]);
            }
            dataGrid1.ItemsSource = dbUsersList;
            // dataGrid1.ItemsSource = usersList;

            SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);


            /*
            GridView myGridView = new GridView();
            myGridView.AllowsColumnReorder = true;
            myGridView.ColumnHeaderToolTip = "Id";

            GridViewColumn gvc1 = new GridViewColumn();
            gvc1.DisplayMemberBinding = new Binding("Id");
            gvc1.Header = "Id";
            gvc1.Width = 100;
            myGridView.Columns.Add(gvc1);
            GridViewColumn gvc2 = new GridViewColumn();
            gvc2.DisplayMemberBinding = new Binding("Username");
            gvc2.Header = "Username";
            gvc2.Width = 100;
            myGridView.Columns.Add(gvc2);
            GridViewColumn gvc3 = new GridViewColumn();
            gvc3.DisplayMemberBinding = new Binding("Password");
            gvc3.Header = "Password";
            gvc3.Width = 100;
            myGridView.Columns.Add(gvc3);
            _ = stackpanel.Children.Add((UIElement)myGridView);
            */

            // listview.ItemsSource = DataContext
            //TODO: Step 7: Set DataContext to this
            //The DataContext property is the default source of your bindings, unless you specifically  
            //declare another source. It's defined on the FrameworkElement class, which most UI controls, 
            //including the WPF Window, inherits from. Simply put, it allows you to specify a basis for your bindings
            //There's no default source for the DataContext property (it's simply null from the start), 
            //but since a DataContext is inherited 
            //down through the control hierarchy, you can set a DataContext for the Window itself and then 
            //use it throughout all of the child controls. 
            //You can either set it in C# or XAML in this example it's already written in the XAML
            //And below if how you would write it in C#
            //this.DataContext = this;
        }
        //TODO: Step 8: Add an element to the list
        private void AddNumber(object sender, RoutedEventArgs e)
        {

            UserService.User user = new UserService.User()
            {
                Id = 10,
                Username = "user10",
                Password = "user10",
            };
            /*
            Classes.User user = new Classes.User()
            {
                Id = 10,
                Username = "user10",
                Password = "user10"
            };
            */
            dbUsersList.Add(user);
            // dataGrid1.Items.Refresh();
            dataGrid1.ItemsSource = null;
            dataGrid1.ItemsSource = dbUsersList;

            // AvailableNumbers.Add(user);
            // dataGrid1.Items.Add(user);
            // dataGrid1.ItemsSource = usersList;


            // AvailableNumbers.Add(user);
        }
        //TODO: Step 9: Remove the first element of the list 
        private void DeleteNumber(object sender, RoutedEventArgs e)
        {
            if (dbUsersList.Count > 0)
            {
                dbUsersList.RemoveAt(dbUsersList.Count - 1);
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = dbUsersList;
            }
        }

        // https://stackoverflow.com/questions/16956251/sort-a-wpf-datagrid-programmatically
        public static void SortDataGrid(DataGrid dataGrid, int columnIndex = 0, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            var column = dataGrid.Columns[columnIndex];

            // Clear current sort descriptions
            dataGrid.Items.SortDescriptions.Clear();

            // Add the new sort description
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));

            // Apply sort
            foreach (var col in dataGrid.Columns)
            {
                col.SortDirection = null;
            }
            column.SortDirection = sortDirection;

            // Refresh items to display sort
            dataGrid.Items.Refresh();
        }


        private void listview_Loaded(object sender, RoutedEventArgs e)
        {
            // AvailableNumbers.OrderBy(p => p.Username);
        }


        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            /*
            // not needed if AutoGenerateColumns = false;
            int columnsToDelete = dataGrid1.Columns.Count - 3;
            if (columnsToDelete > 0)
            {
                for (int i = 0; i < columnsToDelete; i++)
                {
                    dataGrid1.Columns.RemoveAt(3);
                }
            }
            */
        }


        private void dataGrid1_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
