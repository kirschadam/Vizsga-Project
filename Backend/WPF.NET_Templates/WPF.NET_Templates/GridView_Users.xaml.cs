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
using WPF.NET_Templates.Classes;

namespace WPF.NET_Templates
{
    /// <summary>
    /// Interaction logic for DataBinding_ObservableCollection.xaml
    /// </summary>
    public partial class GridView_Users : Window
    {
        //TODO: Step 5: Add the ObservableCollection that we will make the binding to
        //An ObservableCollection Represents a dynamic data collection that provides 
        //notifications when items get added, removed, or when the whole list is refreshed.
        //Basically a fancy list but we cannot use lists to make bindings to, it just does not
        //work, because lists don't inherit from INotifyCollectionChanged thus they cannot send
        //notifications when items get added or removed so the view does not know if things are
        //added or deleted
        public ObservableCollection<ServiceReference3.User> AvailableNumbers { get; set; }
        public List<ServiceReference3.User> dbUsersList { get; set; }

        public GridView_Users()
        {
            InitializeComponent();
            //TODO: Step 6: Initialize the observable collection
            //and add elements to it
            // AvailableNumbers = new ObservableCollection<int>();
            AvailableNumbers = new ObservableCollection<ServiceReference3.User>();

            // usersList = new List<ServiceReference3.User>();
            // string query = $"WHERE 1";  // összes user lekérdezése
            dbUsersList = User.GetUsers("", "", "", "", "");
            
            // close window and stop if no user is retrieved
            if (dbUsersList.Count == 0)
            {
                IsEnabled = false;
                Close();
                return;
            }

            // usersList.OrderBy(p => p.Username);
            for (int i = 0; i < dbUsersList.Count ; i++)
            {
                AvailableNumbers.Add(dbUsersList[i]);
            }
            /*
            int counter = 0; 
            for (int i = 0; i < 15; i++)
            {
                AvailableNumbers.Add(counter++);
            }
            */
            // listview.ItemsSource = AvailableNumbers;
            listview.ItemsSource = dbUsersList;
            // set MinWidth for the first column's header
            var newHeader = new GridViewColumnHeader()
            {
                MinWidth = 40,
                Content = "Id ",
            };
            gridview.Columns[0].Header = newHeader;



            // listview.IsSynchronizedWithCurrentItem = true;
            



            /*
            GridView myGridView = new GridView();
            myGridView.AllowsColumnReorder = true;
            myGridView.ColumnHeaderToolTip = "Employee Information";

            GridViewColumn gvc1 = new GridViewColumn();
            gvc1.DisplayMemberBinding = new Binding("Username");
            gvc1.Header = "FirstName";
            gvc1.Width = 100;
            myGridView.Columns.Add(gvc1);
            GridViewColumn gvc2 = new GridViewColumn();
            gvc2.DisplayMemberBinding = new Binding("LastName");
            gvc2.Header = "Last Name";
            gvc2.Width = 100;
            myGridView.Columns.Add(gvc2);
            GridViewColumn gvc3 = new GridViewColumn();
            gvc3.DisplayMemberBinding = new Binding("EmployeeNumber");
            gvc3.Header = "Employee No.";
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

            ServiceReference3.User user = new ServiceReference3.User()
            {
                Id = 10,
                Username = "user10",
                Password = "user10",
            };

            //AvailableNumbers.Add(user);
            dbUsersList.Add(user);
            listview.ItemsSource = null;
            listview.ItemsSource = dbUsersList;

        }
        //TODO: Step 9: Remove the first element of the list 
        private void DeleteNumber(object sender, RoutedEventArgs e)
        {
            // AvailableNumbers.RemoveAt(0);
            if (dbUsersList.Count > 0)
            {
                dbUsersList.RemoveAt(dbUsersList.Count - 1);
                listview.ItemsSource = null;
                listview.ItemsSource = dbUsersList;
            }
        }

        private void listview_Loaded(object sender, RoutedEventArgs e)
        {
            // AvailableNumbers.OrderBy(p => p.Username);

            // listview.Width += 30;
            
            // resize columns due to the added triangle sortglyph:
            /*
            GridView view = (GridView)this.listview.View;
            foreach (GridViewColumn c in view.Columns)
            {
                c.Width = 1;
                c.ClearValue(GridViewColumn.WidthProperty);
            }
            */
        }

        private void listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LastNameCM_Clic(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
