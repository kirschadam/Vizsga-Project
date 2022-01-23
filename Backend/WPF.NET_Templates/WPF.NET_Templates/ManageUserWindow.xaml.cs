using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPF.NET_Templates.Classes;
// using WPF.NET_Templates.Components;
// using System.ServiceModel;
// using System.ServiceModel.Description;
// using WPF.NET_Templates.ServiceReference3;



namespace WPF.NET_Templates
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>

    public partial class ManageUserWindow : Window
    {
        private ServiceReference3.UserServiceClient client = new ServiceReference3.UserServiceClient();
        private bool closeCompleted = false;

        public List<ServiceReference3.User> dbUsersList { get; set; }

        // DataGrid datagrid_selected;
        System.Collections.IList selectedItems;
        List<ServiceReference3.User> selectedUsersList { get; set; }
        int PK_column_index = 0;
        string edit_mode = "read";
        public List<User> usersList { get; set; }
        private int[] fieldsEntered = new int[5];
        ScrollViewer scrollViewer;
        string lastLocation = "";
        int? lastPersmission = null;
        int? lastActive = null;

        public ManageUserWindow()
        {
            InitializeComponent();

            selectedItems = dataGrid1.SelectedItems; // to make sure it is not null;

            // get all users
            dbUsersList = User.GetUsers("","","","",""); // query all users from database

            // close window and stop if no user is retrieved
            if (dbUsersList.Count == 0)
            {
                IsEnabled = false;
                closeCompleted = true;
                IsEnabled = false; // so that it cannot be opened
                Close();
                return;
            }

            usersList = new List<User>();
            for (int i = 0; i < dbUsersList.Count; i++)
            {
                usersList.Add(new User((int)dbUsersList[i].Id, dbUsersList[i].Username, dbUsersList[i].Password));
            }
            // dataGrid1.ItemsSource = usersList;
            dataGrid1.ItemsSource = dbUsersList;

            SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);


            Dispatcher.InvokeAsync(() => {
                dataGrid1.Width = window.ActualWidth - 250 - 10; // -ColumnDefinition2 width - stackPanel left margin
                // dataGrid1.Width = stackPanel.ActualWidth -0 ; // to view vertical scrollbar

                ScrollDown();


                // DataGridRow row = dataGrid1.ItemContainerGenerator.ContainerFromItem(dataGrid1.Items[dataGrid1.Items.Count - 1]) as DataGridRow;
                // row.Focus();

                // DataGridCell cell = dataGrid1.Columns[1].GetCellContent(row).Parent as DataGridCell;
                // cell.IsEditing = true;
                // dataGrid1.BeginEdit(); 
                // TextBox textBox = (TextBox)cell.Content;
                // textBox.Focus();
                // Keyboard.Focus(textBox);
                // row.Focus();
                // row.IsSelected = true;
            }, DispatcherPriority.ApplicationIdle);

            // var scrollerViewer = Shared.GetScrollViewer(dataGrid1);
            // if (scrollerViewer != null) scrollerViewer.ScrollToEnd();





            gifImage.StartAnimation();


        }



        private void Button_AddNumber_Click(object sender, RoutedEventArgs e)
        {

            ServiceReference3.User user = new ServiceReference3.User()
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
        private void Button_DeleteNumber_Click(object sender, RoutedEventArgs e)
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
            // dataGrid1.Width = dataGrid1.ActualWidth -50; // to view vertical scrollbar
            // Shared.StyleDatagridCell(dataGrid1, row_index: 1, column_index: 1, backgroundColor: Brushes.Green, foregroundColor: Brushes.White);

        }

        /*
        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        */

        private async void Button_Login_ClickAsync(object sender, RoutedEventArgs e)
        {
            // client = new ServiceReference3.UserManagementClient();
            // string uid = null;
            // string userName = Textbox_UserName.Text;
            // string password = PasswordBox_Password.Password;

            // string query = $"WHERE Username='{userName}' AND Password='{Shared.CreateMD5(password)}'";

            ServiceReference3.User[] user = new ServiceReference3.User[0];
            try
            {
                user = client.ListUser("", "", "", "", "", "").Users.ToArray();

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


            // login check
            //if (Textbox_UserName.Text == "admin" && PasswordBox_Password.Password == "admin")
            if (user.Length == 1)
            {
                // Button_Register.Click -= Button_Register_Click; // remove event handlers
                // Button_Login.Click -= Button_Login_ClickAsync;

                // Image_Login.Source = new BitmapImage(new Uri("/Resources/Images/success.png", UriKind.Relative));
                //this.WebBrowser.Source = new Uri(String.Format("file:///
                gifImage.GifSource = "/WPF.NET_Templates;component/Resources/Images/success.gif";
                gifImage.Width = 65;
                gifImage.StopAnimation();
                gifImage.StartAnimation();


                // TextBlock_Login.Text = $"Hello {Textbox_UserName.Text}, you are logged in.";
                // TextBlock_Login.Foreground = Brushes.LightGreen;

                Shared.startWindow_With_PinPanels.button_login.Foreground = Brushes.Green;
                Shared.startWindow_With_PinPanels.button_login.Content = "Log out";

                // how to run a method as an Action: https://stackoverflow.com/questions/13260322/how-to-use-net-action-to-execute-a-method-with-unknown-number-of-parameters

                // await Shared.Delay(() => CloseWindow(), 2500);
                await Task.Delay(2500); // delays below code with 2500ms
                Close();
            }
            else
            {
                // TextBlock_Login.Text = "Wrong login data, please retry!";
                // TextBlock_Login.Foreground = Brushes.LightSalmon;
            }
        }
        // login when pressing the Enter key
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // if(e.Key == Key.Enter) { Button_Login_ClickAsync(sender, e); }


        }




        // Fade-in-out animation




        private void WindowFadeOut_Completed(object sender, EventArgs e)
        {
            closeCompleted = true;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (!closeCompleted)
            {
                WindowFadeOut.Begin();
                e.Cancel = true;
            }
        }

        private void MessageFadeOut_Completed(object sender, EventArgs e)
        {
            // checkBox_fadeInOut.IsChecked = true;
            TextBlock_message.Text = "";  // the Storyboard restores visibility due to FillBehavior="Stop", therefore the text is cleared to remain hidden
        }

        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            // Button_Register.Click -= Button_Register_Click; // remove event handler
            // Button_Login.Click -= Button_Login_ClickAsync;

            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            Close();
        }

        private void Textbox_UserName_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            /*
            if (Textbox_UserName.Text == "Username")
            {
                Textbox_UserName.Text = "";
            }
            */
        }

        private void PasswordBox_Password_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            /*
            if (PasswordBox_Password.Password == "Password")
            {
                PasswordBox_Password.Password = "";
            }
            */
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();

        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Button_Close.IsEnabled = false;
            Close();
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedItems = dataGrid1.SelectedItems;
            
            
            
            // restore selection to new row and edited cell in case of insert if user clicks elsewhere
            Dispatcher.InvokeAsync(() => {
                if (edit_mode == "insert" && dataGrid1.SelectedIndex != row_index) // && cell.IsSelected == false
                {
                    dataGrid1.SelectedItem = dataGrid1.Items[row_index]; // select last row containing the user to be added
                    cell.Focus();
                }
            }, DispatcherPriority.ApplicationIdle);



            // datagrid_selected = (DataGrid)sender;
            // selectedItems = datagrid_selected.SelectedItems;
        }

        private void Button_DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (selectedItems.Count > 0) //  || selectedItems.Count > 0)
            {
                selectedUsersList = new List<ServiceReference3.User>();
                foreach (ServiceReference3.User user in selectedItems)
                {
                    selectedUsersList.Add(user);
                }
                dataGrid1.ItemsSource = selectedUsersList;
                
                // set row background color to Salmon
                dataGrid1.Dispatcher.InvokeAsync(() => {
                    for (int i = 0; i < selectedUsersList.Count; i++)
                    {
                        /*
                        DataGridRow row = dataGrid1.ItemContainerGenerator.ContainerFromItem(dataGrid1.Items[i]) as DataGridRow;
                        DataGridCell cell = dataGrid1.Columns[1].GetCellContent(row).Parent as DataGridCell;
                        cell.Background = Brushes.Salmon;
                        */
                        Shared.StyleDatagridCell(dataGrid1, row_index: i, column_index: 1, Brushes.Salmon, Brushes.White);
                    }
                    dataGrid1.Focus();
                },
                DispatcherPriority.ContextIdle);

                int selectedUsers = selectedUsersList.Count;
                string deleteMessage = selectedUsers == 1 ? "Are you sure to delete the selected user?" : $"Are you sure to delete the selected {selectedUsers} users?";
                MessageBoxResult result = MessageBox.Show(deleteMessage, caption: "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    int usersToDelete = selectedUsersList.Count;
                    for (int i = usersToDelete-1; i >= 0; i--)
                    {
                        try
                        {
                            // DELETE user(s) from database
                            deleteMessage = client.DeleteUser(Shared.uid, selectedUsersList[i].Id.ToString());
                            if (deleteMessage == "User successfully deleted!")
                            {
                                dbUsersList.Remove(selectedUsersList[i]); // remove user also from dbUsersList
                                selectedUsersList.RemoveAt(i);
                            }
                            else
                            {
                                MessageBox.Show(deleteMessage, caption: "Error message");
                            }

                        }
                        catch (Exception ex)
                        {
                            if (ex.ToString().Contains("XXXXX"))
                            {
                                MessageBox.Show($"This will be a specific error. Details:\n{ex.Message}", caption: "Error message");
                                return;
                            }
                            else
                            {
                                MessageBox.Show("An error occurred, with the following details:\n" + ex.Message, caption: "Error message");
                                return;
                            }
                        }
                    }

                    if (selectedUsersList.Count == 0)
                    {
                        deleteMessage = usersToDelete == 1 ? "The user has been deleted." : "The users have been deleted.";
                    }
                    else
                    {
                        deleteMessage = selectedUsersList.Count == 1 ? "The user shown in the table could not be deleted, as reported in the error message." : "The user shown in the table could not be deleted, as reported in the error message.";
                    }
                    // list the users that could not be deleted (empty if all deleted)
                    dataGrid1.ItemsSource = null;
                    dataGrid1.ItemsSource = selectedUsersList;
                    MessageBox.Show(deleteMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                }
                dataGrid1.Focus();
                dataGrid1.ItemsSource = dbUsersList;
                // ScrollDown();

            }
            else
            {
                MessageBox.Show("Nothing has been selected. Please select at least one user. ", caption: "Information");
            }


        }
        
        
        private void ScrollDown()
        {
            // dataGrid1.Focus();
            scrollViewer = Shared.GetScrollViewer(dataGrid1);
            if (scrollViewer != null) scrollViewer.ScrollToEnd();
        }

        
        
        
        
        
        
        
        
        
        
        
        private void Button_UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.Columns[0].SortDirection != ListSortDirection.Ascending)
            {
                SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
            }

            dataGrid1.CanUserSortColumns = false;


            if (edit_mode == "read") // if 'Add user' is completed (or window just opened), 'Add user'
            {
                dataGrid1.IsReadOnly = false; // CanUserAddRows="False" must be set in XAML
                ScrollDown(); // scroll down if user scrolled up to avoid DataGridRow error of invisible rows
                row_index = dataGrid1.Items.Count - 1;
                dataGrid1.SelectedItem = dataGrid1.Items[row_index]; // select last row

                // List<ServiceReference3.User> dbUsersInsertedList = dbUsersList;
                // dataGrid1.ItemsSource = null;
                //dataGrid1.ItemsSource = dbUsersInsertedList;

                // gifImage.StopAnimation();
                // delay execution after dataGrid1 is re-rendered (after new itemsource binding)!
                // https://stackoverflow.com/questions/44272633/is-there-a-datagrid-rendering-complete-event
                // https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
                dataGrid1.Dispatcher.InvokeAsync(() => {
                    // Shared.StyleDatagridCell(dataGrid1, dataGrid1.Items.Count - 1, PK_column_index, Brushes.Salmon, Brushes.White); // no color for id
                    dataGrid1.Focus();
                    row = dataGrid1.ItemContainerGenerator.ContainerFromItem(dataGrid1.Items[row_index]) as DataGridRow;
                    cell = dataGrid1.Columns[1].GetCellContent(row).Parent as DataGridCell;

                    // cell.IsEditing = true; // no edit mode for update
                    cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down)); // move to Username
                    // dataGrid1.BeginEdit(); 

                    // TextBox textBox = (TextBox)cell.Content;
                    // textBox.Focus();
                    // Keyboard.Focus(textBox);

                    // Keyboard.Focus(cell);
                    // textBox.SelectAll();
                    // textBox.Focus();
                    // cell.IsEditing = true;
                    // dataGrid1.BeginEdit();
                    // textBox.CaretIndex = textBox.Text.Length;
                    // textBox.SelectAll();
                    // cell.Focus();
                    // cell.IsSelected = true;

                    // var scrollerViewer = Shared.GetScrollViewer(dataGrid1);
                    // ScrollDown();
                    // cell.IsEditing = true;
                    // Keyboard.Focus(cell);
                    // Press [Tab] key programatically.



                    /*
                                        cell.IsSelected = true;
                                        cell.Focus();
                                        dataGrid1.BeginEdit();
                    */
                    // Keyboard.Focus(textBox);
                    // cell.IsEditing = true;
                    // _ = textBox.SelectionStart;
                    // Keyboard.Focus(textBox);
                    // textBox.SelectionLength = 0;
                    // cell.ForceCursor = true;
                    // textBox.Select(2, 0);

                },
                DispatcherPriority.ContextIdle); // style the id cell of the new user



                edit_mode = "update";
            }
            else
            {
                MessageBox.Show("Please insert new data into a cell, then press Enter.", caption: "Information");
                dataGrid1.Focus();
                // dataGrid1.BeginEdit();
            }
        }



        private void Button_AddUser_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.Columns[0].SortDirection != ListSortDirection.Ascending)
            {
                SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
            }

            dataGrid1.CanUserSortColumns = false;
            Array.Clear(fieldsEntered, 0, fieldsEntered.Length);

            // string textblock = TextBlock_message.Visibility.ToString();
            if (edit_mode == "read") // if 'Add user' is completed (or window just opened), 'Add user'
            {

                // in db select last user with highest Id
                int? highestId = dbUsersList[dbUsersList.Count - 1].Id;
                ServiceReference3.User newUser = new ServiceReference3.User() // create new user with suggested values
                {
                    Id = highestId + 1,
                    Username = "",
                    Password = "",
                    Location = lastLocation = lastLocation != "" ? lastLocation : Shared.loggedInUser.Location,
                    Permission = lastPersmission = lastPersmission != null ? lastPersmission : 1,
                    Active =  lastActive = lastActive != null ? lastActive : 1
                };
                List<ServiceReference3.User> dbUsersInsertedList = dbUsersList;
                dbUsersInsertedList.Add(newUser);
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = dbUsersInsertedList;

                dataGrid1.IsReadOnly = false; // CanUserAddRows="False" must be set in XAML
                ScrollDown(); // scroll down if user scrolled up to avoid DataGridRow error of invisible rows
                row_index = dataGrid1.Items.Count -1;
                dataGrid1.SelectedItem = dataGrid1.Items[row_index]; // select last row containing the user to be added
                
                // DataGridRow row = dataGrid1.ItemContainerGenerator.ContainerFromItem(dataGrid1.Items[dataGrid1.Items.Count - 1]) as DataGridRow;


                // gifImage.StopAnimation();
                // delay execution after dataGrid1 is re-rendered (after new itemsource binding)!
                // https://stackoverflow.com/questions/44272633/is-there-a-datagrid-rendering-complete-event
                // https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
                dataGrid1.Dispatcher.InvokeAsync(() => {
                    Shared.StyleDatagridCell(dataGrid1, dataGrid1.Items.Count - 1, PK_column_index, Brushes.Salmon, Brushes.White);
                    dataGrid1.Focus();
                    row = dataGrid1.ItemContainerGenerator.ContainerFromItem(dataGrid1.Items[row_index]) as DataGridRow;
                    cell = dataGrid1.Columns[1].GetCellContent(row).Parent as DataGridCell;
                    cell.IsEditing = true;
                    cell.MoveFocus(new TraversalRequest( FocusNavigationDirection.Down));
                    // dataGrid1.BeginEdit(); 

                    // TextBox textBox = (TextBox)cell.Content;
                    // textBox.Focus();
                    // Keyboard.Focus(textBox);

                    // Keyboard.Focus(cell);
                    // textBox.SelectAll();
                    // textBox.Focus();
                    // cell.IsEditing = true;
                    // dataGrid1.BeginEdit();
                    // textBox.CaretIndex = textBox.Text.Length;
                    // textBox.SelectAll();
                    // cell.Focus();
                    // cell.IsSelected = true;

                    // var scrollerViewer = Shared.GetScrollViewer(dataGrid1);
                    // ScrollDown();
                    // cell.IsEditing = true;
                    // Keyboard.Focus(cell);
                    // Press [Tab] key programatically.



                    /*
                                        cell.IsSelected = true;
                                        cell.Focus();
                                        dataGrid1.BeginEdit();
                    */
                    // Keyboard.Focus(textBox);
                    // cell.IsEditing = true;
                    // _ = textBox.SelectionStart;
                    // Keyboard.Focus(textBox);
                    // textBox.SelectionLength = 0;
                    // cell.ForceCursor = true;
                    // textBox.Select(2, 0);

                },
                DispatcherPriority.ContextIdle); // style the id cell of the new user



                edit_mode = "insert";
            }
            else
            {
                MessageBox.Show("Please fill in all user data, then press Enter.", caption: "Information");
                dataGrid1.Focus();
                dataGrid1.BeginEdit();
            }
        }

        DataGridRow row;
        DataGridColumn column;
        DataGridCell cell;
        TextBox textBox;
        string old_value;
        string new_value = "";
        int row_index;
        int column_index;
        string changed_property_name;
        ServiceReference3.User user_edited, user_edited0;

        private void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                
                row = e.Row;
                row_index = row.GetIndex();
                column = e.Column;
                column_index = column.DisplayIndex;
                user_edited = row.Item as ServiceReference3.User; // read out current (old) values from the row, because the entry is a new value
                user_edited0 = user_edited;

                cell = dataGrid1.Columns[column_index].GetCellContent(row).Parent as DataGridCell;
                textBox = (TextBox)cell.Content;
                new_value = textBox.Text;

                changed_property_name = dataGrid1.Columns[column_index].Header.ToString();
                // get old property value of user by property name
                // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
                old_value = user_edited.GetType().GetProperty(changed_property_name).GetValue(user_edited).ToString();

                
                
                if (changed_property_name == "Password" && new_value != old_value)
                {
                    new_value = Shared.CreateMD5(new_value); // encypt new password (the old password is already encrypted
                }


                // check data correctness
                string stopMessage = "";
                if (new_value == "") // stop if new value is empty
                {
                    stopMessage = "New value cannot be empty!";
                }
                else if (changed_property_name == "Username" && new_value != old_value  && dbUsersList.Any(p => p.Username == new_value)) // stop if user already exists in database, IF new username is different
                {
                    stopMessage = $"The username '{new_value}' is already taken, please enter another username!";
                }
                else if (changed_property_name == "Location" && Shared.locationsList.Any(p => p == new_value) == false) // stop if wrong Location name is entered
                {
                    stopMessage = $"The location '{new_value}' does not exist, please enter the correct location!";
                }
                else if (changed_property_name == "Permission" && Shared.permissionList.Any(p => p == new_value) == false) // stop if wrong Location name is entered
                {
                    stopMessage = $"The Permission value '{new_value}' does not exist, please enter the correct value (between 0-9)!";
                }
                else if (changed_property_name == "Active" && (new_value == "0" || new_value =="1") == false) // stop if wrong Location name is entered
                {
                    stopMessage = $"The Active value '{new_value}' does not exist, please enter the correct value (1 or 0)!";
                }

                if (stopMessage != "")  // warn user, and stop
                {
                    // textBox.Text = old_value;
                    // cell.Content = old_value;
                    MessageBox.Show(stopMessage, caption: "Warning");
                    // dataGrid1.CancelEdit();

                    // this Dispatcher will be executed after: this CellEditEnding + CelectionChanged, ... !
                    Dispatcher.InvokeAsync(() => {

                        bool focused = cell.IsFocused;
                        bool selected = cell.IsSelected;
                        if (cell.IsSelected == false)
                        {
                            dataGrid1.SelectedItem = dataGrid1.Items[row_index]; // select edited cell if user selected another cell
                        }

                        // remove event handler from wrong new cell (BegindEdit can also be removed if needed)
                        (sender as DataGrid).CellEditEnding -= new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid1_CellEditEnding);

                        // select empty cell (if user eventually selected another one
                        // cell.Focus();
                        // dataGrid1.SelectedItem = cell;
                        // cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                        Button_AddUser.Focus();
                        cell.IsEditing = true;
                        cell.Focus(); // set focus on cell
                        cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down)); // move focus to textBox

                        textBox = (TextBox)cell.Content;
                        // Keyboard.Focus(textBox);
                        textBox.SelectAll();

                        // restore event handler
                        (sender as DataGrid).CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid1_CellEditEnding);
                    }, DispatcherPriority.ApplicationIdle);
                    return;
                }
                // stop in insert mode if new and old value are the same AND the field was already updated (in insert mode the suggested old values of columns Location, Permission and Active can be same as old values if accepted) OR in each case in update mode; 
                else if (old_value == new_value && (fieldsEntered[column_index -1] == 1 || edit_mode == "update")) // && column_index < 3
                {
                    return;
                }

                if (edit_mode == "insert") // mark edited cell with salmon in insert mode
                {
                Shared.StyleDatagridCell(dataGrid1, row_index, column_index, Brushes.Salmon, Brushes.White); // style the updated cell
                }
                                                                                                            
                // start saving new valid value
                fieldsEntered[column_index - 1] = 1; // register the entered property's column index

                if (column_index < 4) // // update string-type fields with new value (Username / Password / Location)
                {
                    user_edited.GetType().GetProperty(changed_property_name).SetValue(user_edited, new_value);
                }
                else // update int?-type fields with new value (Permission / Active)
                {
                    int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                    user_edited.GetType().GetProperty(changed_property_name).SetValue(user_edited,  Convert.ToInt32(new_value));

                }

                // check if all properties are entered, then insert into database
                // if (user_edited.Username != "" && user_edited.Password !="")
                if (fieldsEntered.All(n => n == 1) || edit_mode == "update") // if all fields have been updated OR update mode for 1 field
                {
                    string registerMessage = "";
                    string updateMessage = "";
                    try
                    {
                        if (edit_mode == "insert")
                        {
                            // REGISTER into database
                            // client = new ServiceReference3.UserManagementClient();
                            registerMessage = client.RegisterUser(Shared.uid, user_edited.Username, user_edited.Password, user_edited.Location, user_edited.Permission.ToString());
                            if (registerMessage != "User successfully registered!")
                            {
                                MessageBox.Show(registerMessage, caption: "Error message");
                                // restore old value // TODO: restore cell values? (or simply reload entire list?)
                                user_edited = user_edited0;
                                return;
                            }
                        }
                        else if (edit_mode == "update")
                        {
                            updateMessage = client.UpdateUser(Shared.uid, user_edited.Id.ToString(), user_edited.Username, user_edited.Password, user_edited.Location, user_edited.Permission.ToString(), user_edited.Active.ToString());
                            if (updateMessage != "User successfully updated!")
                            {
                                MessageBox.Show(updateMessage + " Field was not updated in the database!", caption: "Error message");
                                // restore old value // TODO: restore cell value? 
                                user_edited = user_edited0;
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.ToString().Contains("XXXXX"))
                        {
                            MessageBox.Show("This will be a specific error.", caption: "Error message");
                            return;
                        }
                        else
                        {
                            MessageBox.Show("An error occured, with the following details:\n" + ex.ToString(), caption: "Error message");
                            return;
                        }
                    }
                    
                    
                    if (edit_mode == "insert")
                    {
                        lastLocation = user_edited.Location;
                        lastPersmission = user_edited.Permission;
                        lastActive = user_edited.Active;

                        // set background color of added user to green
                        for (int i = 0; i < dataGrid1.Columns.Count; i++)
                        {
                            cell = dataGrid1.Columns[i].GetCellContent(row).Parent as DataGridCell;
                            cell.Background = Brushes.OliveDrab;
                        }
                        TextBlock_message.Text = $"The user '{user_edited.Username}' has been added.";
                        Array.Clear(fieldsEntered, 0, fieldsEntered.Length);
                        edit_mode = "read";
                        dataGrid1.CanUserSortColumns = true;
                    }
                    else if (edit_mode == "update")
                    {
                        cell.Background = Brushes.OliveDrab;
                        TextBlock_message.Text = $"The user '{user_edited.Username}' has been updated with {changed_property_name}.";
                    }
                    old_value = new_value; // update old_value after successful update
                    TextBlock_message.Foreground = Brushes.LightGreen;
                    
                    
      
                    // gifImage.GifSource = "/WPF.NET_Templates;component/Resources/Images/success.gif";
                    // gifImage.Width = 65;
                    // gifImage.StopAnimation();
                    // gifImage.StartAnimation();
                    // Thread thread = new Thread(StopAnimation);
                    // thread.Start();

                    checkBox_fadeInOut.IsChecked = false;
                    checkBox_fadeInOut.IsChecked = true; // show gifImage

                    // await Shared.Delay(() => StopAnimation(), 3000);
                    // await Task.Delay(3000); // delays below code with 3500ms
                    // gifImage.StopAnimation();




                }
                else // move to next cell
                {
                    dataGrid1.Focus();
                    dataGrid1.Dispatcher.InvokeAsync(() => {
                        
                        cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));

                        // select next unchanged column; if last 'Active' column is reached, return to first 'Username' column
                        int column_shift = 0;
                        while (fieldsEntered[column_index + column_shift -1] != 0)
                        {
                            column_shift = column_index + column_shift == 5 ? -column_index + 1 : column_shift + 1;
                        }
                        cell = dataGrid1.Columns[column_index + column_shift].GetCellContent(row).Parent as DataGridCell;
                        
                        // turn off eventual editing mode causes e.g. by tab key on data entry
                        if (cell.IsEditing) { cell.IsEditing = false; }

                        // cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                        // cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                        Button_AddUser.Focus();
                        cell.IsEditing = true;
                        
                        cell.Focus(); // set focus on cell
                        cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down)); // move focus to textBox
                        
                        textBox = (TextBox)cell.Content;
                        // Keyboard.Focus(textBox);
                        textBox.SelectAll();


                        /*
                        row = dataGrid1.ItemContainerGenerator.ContainerFromItem(dataGrid1.Items[row_index]) as DataGridRow;
                        cell = dataGrid1.Columns[column_index + 1].GetCellContent(row).Parent as DataGridCell;
                        cell.IsEditing = true;
                        cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                        */

                        /*
                    cell.IsSelected = true;
                    cell.Focus();
                    dataGrid1.BeginEdit();
                        */


                    },
                DispatcherPriority.ContextIdle); // style the id cell of the new user
                }

            }
            else
            {
                // restore old value if user changed it
                if (new_value != "" && new_value != old_value)
                {
                    // e.Cancel = true;
                    dataGrid1.Dispatcher.InvokeAsync(() => {

                        textBox.Text = old_value;
                        // cell.Content = old_value;
                        // user_edited = user_edited0;

                        fieldsEntered[column_index - 1] = 0; // UNregister the entered property's column index

                        if (column_index < 4) // // update string-type fields with new value (Username / Password / Location)
                        {
                            user_edited.GetType().GetProperty(changed_property_name).SetValue(user_edited, old_value);
                        }
                        else // update int?-type fields with new value (Permission / Active)
                        {
                            int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                            user_edited.GetType().GetProperty(changed_property_name).SetValue(user_edited, Convert.ToInt32(old_value));

                        }

                    },
                DispatcherPriority.ContextIdle);

                }
                return;
            }
        }

        private void StopAnimation()
        {
            // to avoid error "The calling thread cannot access this object because a different thread owns it"
            // https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
            this.Dispatcher.Invoke(async () =>
            {
                await Task.Delay(2000);
                //Thread.Sleep(5000);
                gifImage.StopAnimation();
            }, DispatcherPriority.ContextIdle);
        }

        private void dataGrid1_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {

            if (edit_mode == "insert" &&  e.Row.GetIndex() != row_index)
            {
                e.Cancel = true;
            }
            /*
            DataGridRow row = e.Row;
            DataGridCell cell = dataGrid1.Columns[1].GetCellContent(row).Parent as DataGridCell;
            cell.IsEditing = true;
            */


        }

        private void dataGrid1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                return;
            }
        }

        private void dataGrid1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }




        /*
        private bool AlreadyFaded;
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!AlreadyFaded)
            {
                AlreadyFaded = true;
                e.Cancel = true;
                DoubleAnimation anim = new DoubleAnimation(0, TimeSpan.FromSeconds(4));
                WindowFadeOut.Completed += new EventHandler(WindowFadeOut_Completed);
                BeginAnimation(OpacityProperty, anim);
            }
        }
        */

    }
}

