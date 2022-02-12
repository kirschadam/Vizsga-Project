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

namespace FrontendWPF.Templates
{
    /// <summary>
    /// Interaction logic for DataBinding_ObservableCollection.xaml
    /// </summary>
    public partial class DataBinding_ObservableCollection_Users : Window
    {
        //TODO: Step 5: Add the ObservableCollection that we will make the binding to
        //An ObservableCollection Represents a dynamic data collection that provides 
        //notifications when items get added, removed, or when the whole list is refreshed.
        //Basically a fancy list but we cannot use lists to make bindings to, it just does not
        //work, because lists don't inherit from INotifyCollectionChanged thus they cannot send
        //notifications when items get added or removed so the view does not know if things are
        //added or deleted
        public ObservableCollection<UserService.User> AvailableNumbers { get; set; }

        public DataBinding_ObservableCollection_Users()
        {
            //TODO: Step 6: Initialize the observable collection
            //and add elements to it
            // AvailableNumbers = new ObservableCollection<int>();
            AvailableNumbers = new ObservableCollection<UserService.User>();

            List<UserService.User> usersList = new List<UserService.User>();
            usersList = Templates.Users.GetUsers();

            // usersList.OrderBy(p => p.Username);
            for (int i = 0; i < usersList.Count ; i++)
            {
                AvailableNumbers.Add(usersList[i]);
            }
            /*
            int counter = 0; 
            for (int i = 0; i < 15; i++)
            {
                AvailableNumbers.Add(counter++);
            }
            */

            InitializeComponent();
            
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

            AvailableNumbers.Add(user);
        }
        //TODO: Step 9: Remove the first element of the list 
        private void DeleteNumber(object sender, RoutedEventArgs e)
        {
            AvailableNumbers.RemoveAt(0);
        }

        private void listview_Loaded(object sender, RoutedEventArgs e)
        {
            // AvailableNumbers.OrderBy(p => p.Username);
        }

        private void listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
