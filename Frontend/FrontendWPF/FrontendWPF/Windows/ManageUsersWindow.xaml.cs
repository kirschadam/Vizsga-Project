using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;  
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using FrontendWPF.Classes;
using Microsoft.Win32;

namespace FrontendWPF.Windows
{
    public partial class ManageUsersWindow : Window
    {
        private UserService.UserServiceClient client = new UserService.UserServiceClient();
        private bool closeCompleted = false;
        private List<UserService.User> dbUsersList { get; set; }
        
        System.Collections.IList selectedItems;
        List<UserService.User> selectedUsersList { get; set; }
        List<Classes.User> filterUsersList { get; set; }
        List<UserService.User> filteredUsersList { get; set; }
        List<UserService.User> importList { get; set; }
        List<LocationService.Store> dbLocationsList { get; set; }

        int PK_column_index = 0;
        string edit_mode;
        private int[] fieldsEntered = new int[5]; // Username, Password, Location, Permission, Active 
        ScrollViewer scrollViewer;
        string input = "";
        double windowLeft0;
        double windowTop0;
        double windowWidth0;
        double windowHeight0;
        string opId = "=";
        string opPermission = "=";
        string lastLocation = "";
        int? lastPersmission = null;
        int? lastActive = null;

        public ManageUsersWindow()
        {
            InitializeComponent();

            ReloadData();
        }

        private void Button_ReloadData_Click(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }

        private void ReloadData()
        {
            edit_mode = "read";
            dataGrid1.IsReadOnly = true;
            dataGrid1.CanUserSortColumns = true;
            dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
            dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
            TextBlock_message.Foreground = Brushes.White;
            
            // query all users from database
            dbUsersList = User.GetUsers("", "", "", "", "", "", "", "");
            if (dbUsersList == null) { IsEnabled = false; Close(); return; } // stop on any error
            TextBlock_message.Text = $"{dbUsersList.Count} users loaded.";

            dataGrid1.ItemsSource = dbUsersList;
            SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);

            filterUsersList = new List<Classes.User>();
            
            

            
            Dispatcher.InvokeAsync(() => {
                double stretch = Math.Max((borderLeft.ActualWidth - 10 - 67) / (550 - 10 - 140), 0.8); // Border width - left margin - a bit more because first column remains unchanged
                dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
                dataGrid0.Width = dataGrid1.Width;
                dataGrid0.Columns[0].Width = dataGrid1.Columns[0].ActualWidth;

                stackPanel1.Height = 442 + window.ActualHeight - 500; // original window.Height

                // stretch columns to dataGrid1 width
                for (int i = 1; i < dataGrid1.Columns.Count; i++)
                {
                    dataGrid1.Columns[i].Width = dataGrid1.Columns[i].MinWidth * stretch;
                    dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
                }
                dataGrid1.FontSize = 14 * Math.Min(stretch, 1); // reset font size to max 14 on large window width
                dataGrid1.Items.Refresh();
                ScrollDown();
                selectedItems = dataGrid1.SelectedItems; // to make sure it is not null;
            }, DispatcherPriority.Loaded);
            if (ePasswordBox != null) { HidePasswordBox();} // hide password box if left open
            EnableButtons();
            // create/reset user_filter item and add it to filter dataGrid0
            user_filter = new Classes.User()
            {
                Id = "",
                Username = "",
                Password = "",
                Location = "",
                Permission = "",
                Active = ""
            };
            filterUsersList.Clear();
            filterUsersList.Add(user_filter);
            dataGrid0.ItemsSource = null; // to avoid IsEditingItem error
            dataGrid0.ItemsSource = filterUsersList;
            dataGrid0.Items.Refresh();

            SetUserAccess();
        }

        // https://stackoverflow.com/questions/16956251/sort-a-wpf-datagrid-programmatically
        private static void SortDataGrid(DataGrid dataGrid, int columnIndex = 0, ListSortDirection sortDirection = ListSortDirection.Ascending)
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
        }

        private void dataGrid1_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            selectedItems = dataGrid1.SelectedItems;

            // in update mode  update selectedcells and user_edited (when SelecionUnit is Cell)
            // if (dataGrid1.SelectionUnit == DataGridSelectionUnit.Cell)
            if (edit_mode == "update")
            {
                // https://stackoverflow.com/questions/4714325/wpf-datagrid-selectionchanged-event-isnt-raised-when-selectionunit-cell
                IList<DataGridCellInfo> selectedcells = e.AddedCells;
                if (selectedcells.Count > 0) // ignore new selection when button is pressed and selection becomes 0; 
                {
                    user_edited = (UserService.User)selectedcells[0].Item;
                    user_edited0 = user_edited;
                }
            }
        }

        private void Button_DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (ePasswordBox != null) // stop if passwordBox is open
            {
                MessageBox.Show("Please confirm the password, or press Reset to cancel adding new user.", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                passwordTextBox.Focus();
                return;  
            } 
            
            if (edit_mode == "update")
            {
                edit_mode = "read";
                dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                DataGridCellInfo currentCell = dataGrid1.CurrentCell;

                dataGrid1.SelectedItems.Add(user_edited); // this triggers SelectionChanged and sets new selectedItems
            }
            
            if (selectedItems.Count > 0)
            {
                selectedUsersList = new List<UserService.User>();
                foreach (UserService.User user in selectedItems)
                {
                    if (selectedItems.Count == 1 || user.Username != "admin") // skip admin when more users are selected
                    {
                    selectedUsersList.Add(user);
                    }
                }
                dataGrid1.ItemsSource = selectedUsersList;

                // waits to render dataGrid1 and sets row background color to Salmon 
                dataGrid1.Dispatcher.InvokeAsync(() => {
                    for (int i = 0; i < selectedUsersList.Count; i++)
                    {
                        Shared.StyleDatagridCell(dataGrid1, row_index: i, column_index: 1, Brushes.Salmon, Brushes.White);
                    }

                    int selectedUsers = selectedUsersList.Count;
                    string deleteMessage = "";
                    // admin cannot be deleted
                    if (selectedUsers == 1 && selectedUsersList[0].Username == "admin")
                    {
                        deleteMessage = "The user 'admin' cannot be deleted!";
                    }
                    else
                    {
                        deleteMessage = selectedUsers == 1 ? "Are you sure to delete the selected user?" : $"Are you sure to delete the selected {selectedUsers} users?";

                        TextBlock_message.Text = selectedUsers == 1 ? "Delete user?" : $"Delete {selectedUsers} users?";
                        TextBlock_message.Foreground = Brushes.Salmon;
                    }
                    MessageBoxResult result = MessageBox.Show(deleteMessage, caption: "Warning", deleteMessage != "The user 'admin' cannot be deleted!" ? MessageBoxButton.YesNo : MessageBoxButton.OK, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        for (int i = selectedUsers - 1; i >= 0; i--)
                        {
                            try
                            {
                                // DELETE user(s) from database
                                deleteMessage = client.DeleteUser(Shared.uid, selectedUsersList[i].Id.ToString(), selectedUsersList[i].Username);
                                if (deleteMessage == "User successfully deleted!")
                                {
                                    user_edited = selectedUsersList[i]; // required to write the log
                                    Log("delete"); // write log to file
                                    dbUsersList.Remove(selectedUsersList[i]); // remove user also from dbUsersList
                                    selectedUsersList.RemoveAt(i);
                                }
                                else if (deleteMessage == "Unauthorized user!")
                                {
                                    Shared.Logout(); // stop on unauthorized user
                                    IsEnabled = false;
                                    Close();
                                    return; 
                                }
                                else
                                {
                                    MessageBox.Show(deleteMessage, caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("An error occurred, with the following details:\n" + ex.Message, caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }

                        if (selectedUsersList.Count == 0)
                        {
                            deleteMessage = selectedUsers == 1 ? "The user has been deleted." : "The users have been deleted.";
                            TextBlock_message.Text = selectedUsers == 1 ? "User deleted." : "Users deleted.";
                        }
                        else
                        {
                            deleteMessage = selectedUsersList.Count == 1 ? "The user shown in the table could not be deleted, as reported in the error message." : "The users shown in the table could not be deleted, as reported in the error message.";
                        }
                        // list the users that could not be deleted (empty if all deleted)
                        dataGrid1.ItemsSource = null;
                        dataGrid1.ItemsSource = selectedUsersList;

                        checkBox_fadeInOut.IsChecked = false;
                        checkBox_fadeInOut.IsChecked = true; // show gifImage
                        gifImage.StartAnimation();
                        MessageBox.Show(deleteMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    // dataGrid1.Focus();
                    dataGrid1.ItemsSource = dbUsersList;
                    // for some reason the sorting gets improper, so sort again by Id
                    SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);

                    ScrollDown();
                    TextBlock_message.Text = "Select an option.";
                    TextBlock_message.Foreground = Brushes.White;
                },
                DispatcherPriority.Loaded); // just a bit lower priority than Render (Loaded = 6, Render = 7)
            }
            else
            {
                MessageBox.Show("Nothing is selected. Please select at least one user. ", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            dataGrid1.CanUserSortColumns = true;
        }

      
        private void Button_UpdateUser_Click(object sender, RoutedEventArgs e)
        {
            UpdateUser();
        }

        private void UpdateUser()
        {
            if (edit_mode != "update") // if not in update mode, switch to update mode
            {
                if (edit_mode == "insert") // remove incomplete added user 
                {
                    dbUsersList.Remove(user_edited);
                    dataGrid1.ItemsSource = null;
                    dataGrid1.ItemsSource = dbUsersList;
                    EnableButtons();
                }
                if (dataGrid1.Columns[0].SortDirection != ListSortDirection.Ascending)
                {
                    SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
                }
                dataGrid1.CanUserSortColumns = false;

                dataGrid1.IsReadOnly = false; // CanUserAddRows="False" must be set in XAML
                dataGrid1.SelectionMode = DataGridSelectionMode.Single;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.Cell;
                TextBlock_message.Text = "Update user.";
                TextBlock_message.Foreground = Brushes.White;
                ScrollDown();
                if (ePasswordBox != null) { HidePasswordBox(); }
                edit_mode = "update";
            }
            else
            {
                MessageBox.Show("Please insert new data into a cell, then press Enter.", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                dataGrid1.Focus();
            }
        }

        private void Button_AddUser_Click(object sender, RoutedEventArgs e)
        {
            AddUser();
        }

        private void AddUser()
        {
            if (edit_mode != "insert") // if not in insert mode, switch to insert mode
            {
                Array.Clear(fieldsEntered, 0, fieldsEntered.Length);

                // in db select last user with highest Id
                int? highestId = dbUsersList.Max(u => u.Id);
                user_edited = new UserService.User() // create new user with suggested values
                {
                    Id = highestId + 1,
                    Username = "",
                    Password = "",
                    Location = lastLocation = lastLocation != "" ? lastLocation : Shared.loggedInUser.Location,
                    Permission = lastPersmission = lastPersmission != null ? lastPersmission : 1,
                    Active = lastActive = lastActive != null ? lastActive : 1
                };
                user_edited0 = user_edited;
              
                dbUsersList.Add(user_edited);
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = dbUsersList;

                if (dataGrid1.Columns[0].SortDirection != ListSortDirection.Ascending)
                {
                    SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
                }
                dataGrid1.CanUserSortColumns = false;

                dataGrid1.IsReadOnly = false; // CanUserAddRows="False" must be set in XAML
                ScrollDown();
                row_index = dataGrid1.Items.Count - 1;
                dataGrid1.SelectedItem = dataGrid1.Items[row_index]; // select last row containing the user to be added

                // delay execution after dataGrid1 is re-rendered (after new itemsource binding)!
                // https://stackoverflow.com/questions/44272633/is-there-a-datagrid-rendering-complete-event
                // https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
                dataGrid1.Dispatcher.InvokeAsync(() =>
                {
                    // style the id cell of the new user
                    Shared.StyleDatagridCell(dataGrid1, dataGrid1.Items.Count - 1, PK_column_index, Brushes.Salmon, Brushes.White);
                    dataGrid1.Focus();
                    row = dataGrid1.ItemContainerGenerator.ContainerFromItem(dataGrid1.Items[row_index]) as DataGridRow;
                    cell = dataGrid1.Columns[1].GetCellContent(row).Parent as DataGridCell;
                    cell.IsEditing = true;
                    cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                }, DispatcherPriority.Background); // Background to avoid row = null error

                edit_mode = "insert";
                dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                TextBlock_message.Text = "Add user.";
                TextBlock_message.Foreground = Brushes.White;
                if (ePasswordBox != null) { HidePasswordBox(); } // hide password box if left open
                Button_AddUser.IsEnabled = false;
                Button_DeleteUser.IsEnabled = false;
                Button_Filter.IsEnabled = false;
                Button_Export.IsEnabled = false;
                Button_Import.IsEnabled = false;
                Button_LogWindow.IsEnabled = false;
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
        int filterc_index;
        string changed_property_name;
        UserService.User user_edited, user_edited0;
        Classes.User user_filter;

        DataGridCellEditEndingEventArgs ePasswordBox;
        private void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (ePasswordBox != null) { e = ePasswordBox; } // change event if password box is opened
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if ( ePasswordBox == null && CellEditEnding_setup(e) == null) { return; } // setup rules: stop on null (do not check again if password box is open)

                string stopMessage = ePasswordBox == null? CellEditEnding_checkInput() : ""; // check data correctness (do not check again if password box is open)
                if (stopMessage == "stop") { return; } // stop on database error
                if (stopMessage != "")  // warn user, and stop
                {
                    MessageBox.Show(stopMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    textBox.Text = old_value; // restore correct cell value
                    // cell.Content = old_value;

                    Dispatcher.InvokeAsync(() => {
                        // select edited row/cell if user selected another row/cell
                        SelectEditedCell();
                        
                        // remove event handler from wrong new cell (BegindEdit can also be removed if needed)
                        (sender as DataGrid).CellEditEnding -= new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid1_CellEditEnding);

                        // select empty cell (if user eventually selected another one
                        Button_ReloadData.Focus();

                        SelectTextBox();

                        // restore event handler
                        (sender as DataGrid).CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid1_CellEditEnding);
                    }, DispatcherPriority.Loaded);
                    return;
                }
                // stop in insert mode if new and old value are the same AND the field was already updated (in insert mode the suggested old values of columns Location, Permission and Active can be same as old values if accepted) OR in each case in update mode; 
                else if (old_value == new_value && (fieldsEntered[column_index - 1] == 1 || edit_mode == "update")) // && column_index < 3
                {
                    CellEditEnding_nextCell_update();
                    return;
                }

                Dispatcher.InvokeAsync(() => {
                    SelectEditedCell(); // select edited row/cell if user selected another row/cell after data entry
                }, DispatcherPriority.Loaded);
                
                if (edit_mode == "insert") // mark edited cell with salmon in insert mode
                {
                    Shared.StyleDatagridCell(dataGrid1, row_index, column_index, Brushes.Salmon, Brushes.White); // style the updated cell
                }

                if (changed_property_name == "Password")
                {
                    if (Shared.CreateMD5(new_value) != old_value)
                    {
                        if (ePasswordBox == null) // 1. run (before password confirmation)
                        {
                            dataGrid1.CellEditEnding -= new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid1_CellEditEnding);
                            ePasswordBox = e; // setting passwordBox event
                            dataGrid1.IsReadOnly = true;
                            textBlock_username.Text = $"for {user_edited.Username}:";
                            passwordBox.Visibility = Visibility.Visible;
                            dataGrid1.Dispatcher.InvokeAsync(() => {
                            passwordTextBox.Focus();
                            }, DispatcherPriority.Loaded);
                            return;
                        }
                        else // 2. run (after password confirmation)
                        {
                            new_value = Shared.CreateMD5(new_value); // encyrpt new password (the old password is already encrypted
                            HidePasswordBox();
                        }
                    }
                    else
                    {
                        CellEditEnding_nextCell_update();
                        return;
                    }
                }

                // start saving new valid value
                fieldsEntered[column_index - 1] = 1; // register the entered property's column index

                if (column_index < 4) // update string-type fields with new value (Username / Password / Location)
                {
                    user_edited.GetType().GetProperty(changed_property_name).SetValue(user_edited, new_value);
                }
                else // update int?-type fields with new value (Permission / Active)
                {
                    int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                    user_edited.GetType().GetProperty(changed_property_name).SetValue(user_edited, Convert.ToInt32(new_value));

                }

                // check if all properties are entered, then insert into database
                if (fieldsEntered.All(n => n == 1) || edit_mode == "update") // if all fields have been updated OR update mode for one field
                {
                    string hostMessage = "";
                    try
                    {
                        if (edit_mode == "insert")
                        {
                            // REGISTER into database
                            hostMessage = client.RegisterUser(Shared.uid, user_edited.Username, user_edited.Password, user_edited.Location, user_edited.Permission.ToString());
                            if (hostMessage == "Unauthorized user!")
                            {
                                Shared.Logout(); // stop on unauthorized user
                                IsEnabled = false;
                                Close();
                                return;
                            }
                            else if (hostMessage != "User successfully registered!")
                            {
                                MessageBox.Show(hostMessage, caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                    // restore old value // TODO: restore cell values? (or simply reload entire list?)
                                    user_edited = user_edited0;
                                    return;
                            }
                        }
                        else if (edit_mode == "update")
                        {
                            hostMessage = client.UpdateUser(Shared.uid, user_edited.Id.ToString(), user_edited.Username, user_edited.Password, user_edited.Location, user_edited.Permission.ToString(), user_edited.Active.ToString());
                            if (hostMessage == "Unauthorized user!")
                            {
                                Shared.Logout(); // stop on unauthorized user
                                IsEnabled = false;
                                Close();
                                return;
                            }
                            else if (hostMessage != "User successfully updated!")
                            {
                                MessageBox.Show(hostMessage + " Field was not updated.", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                // restore old value // TODO: restore cell value? 
                                user_edited = user_edited0;
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occured, with the following details:\n" + ex.ToString(), caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                     
                    if (edit_mode == "insert")
                    {
                        lastLocation = user_edited.Location; // save last data to suggest them for next record
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
                        Log("insert"); // write log to file
                        edit_mode = "read";
                        dataGrid1.CanUserSortColumns = true;
                        dataGrid1.IsReadOnly = true;
                        dataGrid1.Dispatcher.InvokeAsync(() => {
                            Button_AddUser.Focus(); // set focus to allow repeatedly add user on pressing the Add user button
                        }, DispatcherPriority.Loaded);

                        Button_AddUser.IsEnabled = true;
                        Button_DeleteUser.IsEnabled = true;
                        Button_Filter.IsEnabled = true;
                        Button_Export.IsEnabled = true;
                        Button_Import.IsEnabled = true;
                        Button_LogWindow.IsEnabled = true;
                    }
                    else if (edit_mode == "update")
                    {
                        TextBlock_message.Text = $"The user '{user_edited.Username}' has been updated with {changed_property_name}.";
                        Log("update"); // write log to file
                        cell.Background = Brushes.OliveDrab;
                        // Shared.ChangeColor(cell, Colors.OliveDrab, Colors.Transparent);
                        CellEditEnding_nextCell_update();
                    }
                    old_value = new_value; // update old_value after successful update
                    TextBlock_message.Foreground = Brushes.LightGreen;
               
                    checkBox_fadeInOut.IsChecked = false;
                    checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                    gifImage.StartAnimation();
                }
                else // move to next cell when inserting
                {
                    CellEditEnding_nextCell_insert();
                }

            }
        }

        private string CellEditEnding_setup(DataGridCellEditEndingEventArgs e)
        {
            if (edit_mode != "update" && Button_UpdateUser.IsKeyboardFocused) // switch to insert mode if 'Update' is clicked
            {
                edit_mode = "read";
                dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                dbUsersList.RemoveAt(dbUsersList.Count - 1);
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = dbUsersList;
                EnableButtons();
                UpdateUser();
                return null;
            }
            else if (edit_mode != "insert" && Button_AddUser.IsKeyboardFocused) // switch to insert mode if 'Add' is clicked
            {
                edit_mode = "read";
                EnableButtons();
                AddUser();
                return null;
            }
            else if (Button_ReloadData.IsKeyboardFocused) // return if 'Reload data" is clicked
            {
                EnableButtons();
                return null;
            }
            else if (Button_Close.IsKeyboardFocused)
            {
                CloseWindow();
                return null;
            }

            row = e.Row;
            row_index = row.GetIndex();
            column = e.Column;
            column_index = column.DisplayIndex;
            //user_edited = row.Item as UserService.User; //  user_edited and user_edited0 are already defined in UpdateUser and AddUser (read out current (old) values from the row, because the entry is a new value)

            cell = dataGrid1.Columns[column_index].GetCellContent(row).Parent as DataGridCell;
            textBox = (TextBox)cell.Content;
            new_value = textBox.Text;
            
            changed_property_name = dataGrid1.Columns[column_index].Header.ToString();
            // get old property value of user by property name
            // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
            old_value = user_edited.GetType().GetProperty(changed_property_name).GetValue(user_edited).ToString();
            return "OK";
        }

        private string CellEditEnding_checkInput()
        {
            string stopMessage = "";
            if (new_value == "") // if new value is empty
            {
                stopMessage = "New value cannot be empty!";
            }
            else if (changed_property_name == "Username" && new_value.Length < 5)
            {
                stopMessage = $"The username must be at least 5 charachters long!";
            }
            else if (changed_property_name == "Username" && new_value != old_value && dbUsersList.Any(p => p.Username == new_value)) // stop if user already exists in database, AND if new username is different
            {
                stopMessage = $"The username '{new_value}' is already taken, please enter another username!";
            }
            else if (changed_property_name == "Username" && old_value == "admin") // admin cannot be renamed
            {
                stopMessage = $"The user '{old_value}' cannot be renamed!";
            }
            else if (changed_property_name == "Password" && new_value.Length < 5)
            {
                stopMessage = $"The password must be at least 5 charachters long!";
            }
            else if (changed_property_name == "Location") // if wrong Location name is entered
            {
                dbLocationsList = Location.GetLocations("", "", "", "");
                if (dbLocationsList == null) { IsEnabled = false; Close(); return "stop"; } // stop on any error
                else if (dbLocationsList.Any(p => p.Name == new_value) == false)
                {
                    stopMessage = $"The location '{new_value}' does not exist, please enter the correct location!";
                }
            }
            else if (changed_property_name == "Permission" && Shared.permissionList.Any(p => p == new_value) == false) // if wrong Permission value is entered
            {
                stopMessage = $"The Permission value '{new_value}' does not exist, please enter the correct value (between 0-9)!";
            }
            else if (changed_property_name == "Permission" && row_index == 0) // admin's permission level cannot be changed
            {
                stopMessage = $"The Permission value of admin cannot be changed!";
            }
            else if (changed_property_name == "Active" && (new_value == "0" || new_value == "1") == false) // if wrong Active value is entered
            {
                stopMessage = $"The Active value '{new_value}' does not exist, please enter the correct value (1 or 0)!";
            }
            else if (changed_property_name == "Active" && row_index == 0) // admin's active value cannot be changed
            {
                stopMessage = $"The Active value of admin cannot be changed!";
            }
            return stopMessage;
        }

        private void CellEditEnding_nextCell_update()
        {
            dataGrid1.Dispatcher.InvokeAsync(() => {
                // select next  column; if last 'Active' column is reached, return to first 'Username' column
                if (column_index == dataGrid1.Columns.Count - 1)
                {
                    column_index = 1;
                    // move 1 row down if it is not the last row
                    if (row_index < dataGrid1.Items.Count - 1)
                    {
                        row = dataGrid1.ItemContainerGenerator.ContainerFromItem(dataGrid1.Items[row_index + 1]) as DataGridRow;
                        row_index++;
                    }
                }
                else
                {
                    column_index++;
                }

                cell = dataGrid1.Columns[column_index].GetCellContent(row).Parent as DataGridCell;

                // go into edit mode if in insert mode
                cell.Focus(); // set focus on cell
                if (edit_mode == "insert")
                {
                    SelectTextBox();
                }

                if (edit_mode == "update") dataGrid1.SelectedCells.Clear();
                SelectEditedCell();
            },
            DispatcherPriority.Loaded);
        }

        private void CellEditEnding_nextCell_insert()
        {
            dataGrid1.Focus();
            dataGrid1.Dispatcher.InvokeAsync(() => {

                cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));

                // select next unchanged column; if last 'Active' column is reached, return to first 'Username' column
                int column_shift = 0;
                while (fieldsEntered[column_index + column_shift - 1] != 0)
                {
                    column_shift = column_index + column_shift == 5 ? -column_index + 1 : column_shift + 1;
                }
                cell = dataGrid1.Columns[column_index + column_shift].GetCellContent(row).Parent as DataGridCell;

                // turn off eventual editing mode caused e.g. by tab key on data entry
                if (cell.IsEditing) { cell.IsEditing = false; }
                Button_ReloadData.Focus();
                SelectTextBox();
            }, DispatcherPriority.Loaded);
        }

        private void SelectEditedCell()
        {
            // select edited row/cell if user selected another row/cell during data entry
            if (cell.IsSelected == false)
            {
                // if (dataGrid1.SelectionUnit == DataGridSelectionUnit.Cell)
                if (edit_mode == "update")
                {
                    cell.IsSelected = true;
                }
                else
                {
                    dataGrid1.SelectedItem = dataGrid1.Items[row_index];
                }
            }
        }

        private  void EnableButtons()
        {
            Button_AddUser.IsEnabled = true;
            Button_DeleteUser.IsEnabled = true;
            Button_Filter.IsEnabled = true;
            Button_Export.IsEnabled = true;
            Button_Import.IsEnabled = true;
            Button_LogWindow.IsEnabled = true;
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
            row = e.Row;
            row_index = row.GetIndex();
            column = e.Column;
            column_index = column.DisplayIndex;
            cell = dataGrid1.Columns[column_index].GetCellContent(row).Parent as DataGridCell;

            // in insert mode, do not allow user to edit a different row, and restore selection, focus and editing
            if (edit_mode == "insert" &&  e.Row.GetIndex() != row_index)
            {
                e.Cancel = true;
                SelectEditedCell();
                SelectTextBox();
            }
        }


        // https://stackoverflow.com/questions/27744097/wpf-fade-out-animation-cant-change-opacity-any-more
        private void ChangeOpacity()
        {
            DoubleAnimation animation = new DoubleAnimation
            {
                To = 0,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = TimeSpan.FromSeconds(5),
                FillBehavior = FillBehavior.Stop
            };

            animation.Completed += (s, a) => cell.Opacity = 0;

            cell.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void SelectTextBox()
        {
            cell.Focus();
            cell.IsEditing = true;

            cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down)); // move focus to textBox
            cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
            textBox = (TextBox)cell.Content;
            // Keyboard.Focus(textBox);
            textBox.SelectAll();
        }
        
        private void ScrollDown()
        {
            // dataGrid1.Focus();
            scrollViewer = Shared.GetScrollViewer(dataGrid1);
            if (scrollViewer != null) scrollViewer.ScrollToEnd();
        }

        private void MessageFadeOut_Completed(object sender, EventArgs e)
        {
            TextBlock_message.Text = "";  // the Storyboard restores visibility due to FillBehavior="Stop", therefore the text is cleared to remain hidden
            // TextBlock_message.Opacity = 1;
            if (edit_mode == "read")
            {
                TextBlock_message.Foreground = Brushes.White;
            }
            gifImage.StopAnimation();
        }

        // make window draggable
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();

        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }


        private void CloseWindow()
        {
            Button_Close.IsEnabled = false;
            TextBlock_message.Text = "Closing window.";
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

        private void WindowFadeOut_Completed(object sender, EventArgs e)
        {
            closeCompleted = true;
            Close();
        }

        // show/hide dataGrid0 with filter row
        private void Button_Filter_Click(object sender, RoutedEventArgs e)
        {
            filteredUsersList = new List<UserService.User>();

            // show filter dataGrid0
            if (stackPanel1.Height == 442 + window.ActualHeight - 500)
            {
                stackPanel1.Margin = new Thickness(0, 45 + 30, 0, 0);
                stackPanel1.Height = 442 - 30 + window.ActualHeight - 500;
                ScrollDown();
                TextBlock_message.Text = "Enter filter value(s).";

            }
            else
            {
                stackPanel1.Margin = new Thickness(0, 45, 0, 0);
                stackPanel1.Height = 442 + window.ActualHeight - 500;
                TextBlock_message.Text = "Select an option.";
            }
        }
        DataGridBeginningEditEventArgs eBeginningEdit;
        private void dataGrid0_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            row = e.Row;
            column = e.Column;
            eBeginningEdit = e;
        }

        private void dataGrid0_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            input = e.Text; // get character entered
        }

        int? user_filterId = null;
        int? user_filterPermission = null;
        int? user_filterActive = null;
        private void dataGrid0_KeyUp(object sender, KeyEventArgs e)
        // private void dataGrid0_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (row == null || column == null) { return; } // stop if column or row is not selected or not in edit mode

            // check whether the pressed key is digit or number, otherwise stop
            if (input.Length == 0) {
                return; // should not happen
            }
            int ASCII = (int)input[0];
            input = " "; // reset input to empty to avoid false value, becasuse KeyUp event may run on function keys as well
            if (((ASCII > 31 && ASCII < 256) || ASCII == 336 || ASCII == 337 || ASCII == 368 || ASCII == 369) == false) { return; } // stop if not number or digit expect Ő(336), ő(337), Ű(368), ű(369)
            // if (ASCII == 43 || ASCII == 60 || ASCII == 61 || ASCII == 62) { return; } // stop if +, <, =, >
            bool key = e.Key == Key.Back;

            // stop on most function keys, expect back, delete, <+í(Oem102), -, ., é(Oem1), ü(Oem2), ö(Oem3), ő(Oem4), Ű(Oem5), ú(Oem6), á(Oem7), Ó(OemPlus)
            if (e.Key != Key.Back && e.Key != Key.Delete && e.Key != Key.Oem102 && e.Key != Key.Subtract && e.Key != Key.OemPeriod && e.Key != Key.Oem1 && e.Key != Key.Oem2 && e.Key != Key.Oem3 && e.Key != Key.Oem4 && e.Key != Key.Oem5 && e.Key != Key.Oem6 && e.Key != Key.Oem7 && e.Key != Key.OemPlus && ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)) == false)
            {
                return;
            }
            
            // row = e.Row;
            // column = e.Column;
            filterc_index = column.DisplayIndex;
            cell = dataGrid1.Columns[filterc_index].GetCellContent(row).Parent as DataGridCell;
            if (cell.IsEditing == false) { return; } // stop if cell is not editing
            textBox = (TextBox)cell.Content;
            new_value = textBox.Text;
            if ( new_value !="" && new_value.Length == 1 && (textBox.Text == "<" || textBox.Text == ">" || textBox.Text == ">") && e.Key != Key.Back && e.Key != Key.Delete) { return; } // stop if < or > in empty cell (but continue to recalculate if last key was Key.Back or Key.Delete
            if (new_value != "" &&  new_value.Length < 3 && (textBox.Text.Substring(1) == "=")) { return; } // stop if '=' when there are no more characters       
            
            string firstChar = new_value != "" ? new_value.ToString().Substring(0, 1) : "";
            string op = firstChar != ">" &&  firstChar != "<" ? "=" : firstChar;
            if (new_value.Length > 1 && new_value.ToString().Substring(1, 1) == "=")
            {
                op = op == ">" ? ">=" : op == "<" ? "<=" : op; // setting >= or <= operator values
            }
            
            changed_property_name = dataGrid1.Columns[filterc_index].Header.ToString();

           // remove operator for integer columns Id and Permission
           // set operator value for specific column
           if (changed_property_name == "Id" || changed_property_name == "Permission")
            {
                // if (op != "=" || (new_value != "" && new_value.ToString().Substring(0, 1) == "=")) { new_value = new_value.Substring(op.Length); } // remove entered operator
                
                switch (changed_property_name)
                {
                    case "Id": opId = op; break;
                    case "Permission": opPermission = op; break;
                    default: break;
                }
            }
            else { op = ""; } // clear operator for string columns

            // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
            int? int_val = Int32.TryParse(user_filter.GetType().GetProperty(changed_property_name).GetValue(user_filter).ToString(), out var tempVal0) ? tempVal0 : (int?)null;
            old_value = int_val != null ? user_filter.GetType().GetProperty(changed_property_name).GetValue(user_filter).ToString() : "";


            // check data correctness
            string stopMessage = "";
            if (new_value != "")
            {
                if (changed_property_name == "Id")
                {
                    string user_filterId0 = new_value.Replace(">", "").Replace("<", "").Replace("=", "");
                    user_filterId = int.TryParse(user_filterId0, out var tempVal1) ? tempVal1 : (int?)null;
                    // int_val = Int32.TryParse(user_filterId0, out var tempVal) ? tempVal : (int?)null;
                    if ((user_filterId0 != "" && user_filterId == null) || (user_filterId < 0 || user_filterId > 10000000))
                        if ((user_filterId0 != "" && user_filterId == null) || (user_filterId < 0 || user_filterId > 10000000))
                        {
                            stopMessage = $"The Id '{user_filterId0}' does not exist, please enter a correct value for the Id!";
                        }
                }
                else if (changed_property_name == "Permission")
                {
                    string user_filterPermission0 = new_value.Replace(">", "").Replace("<", "").Replace("=", "");
                    user_filterPermission = int.TryParse(user_filterPermission0, out var tempVal2) ? tempVal2 : (int?)null;
                    if ((user_filterPermission0 != "" && Shared.permissionList.Any(p => p == user_filterPermission0) == false)) // if wrong Permission value is entered
                    {
                        stopMessage = $"The Permission value '{user_filterPermission0}' does not exist, please enter the correct value (between 0-9)!";
                    }
                }
                else if (changed_property_name == "Active")
                {
                    // string user_filterActive0 = new_value.Replace(">", "").Replace("<", "").Replace("=", "");
                    user_filterActive = int.TryParse(new_value, out var tempVal3) ? tempVal3 : (int?)null;
                    if ( (new_value == "0" || new_value == "1") == false) // if wrong Active value is entered
                    {
                        stopMessage = $"The Active value '{new_value}' does not exist, please enter the correct value (1 or 0)!";
                    }
                }

                if (stopMessage != "")  // warn user, and stop
                {
                    MessageBox.Show(stopMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    textBox.Text = old_value; // restore correct cell value if old value is not null
                    Shared.SendKey(Key.End);
                    return;
                }
            }
                
            // update filter fields
            user_filter.GetType().GetProperty(changed_property_name).SetValue(user_filter, new_value);
            
            string password = ""; // encrypt only as a separate variable, leaving user_password unchanged
            if (new_value !="" && new_value != "-1" && changed_property_name == "Password" && new_value != old_value)
            {
                password = Shared.CreateMD5(new_value); // encypt new password (the old password is already encrypted
            }

            // filter
            filteredUsersList.Clear();
            foreach (var user in dbUsersList)
            {

                if ((user_filterId == null || Compare(user.Id, user_filterId, opId)) && (user_filter.Username == "" || user.Username.ToLower().Contains(user_filter.Username.ToLower())) && (password == "" ||  user.Password == password) && (user_filter.Location == "" || user.Location.ToLower().Contains(user_filter.Location.ToLower())) && (user_filterPermission == null || Compare(user.Permission, user_filterPermission, opPermission)) && (user_filterActive == null || user.Active == user_filterActive))
                {
                    filteredUsersList.Add(user);
                }
            }
            // update dataGrid1 with filtered items                    
            dataGrid1.ItemsSource = filteredUsersList;
            SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
            dataGrid1.Items.Refresh();
        }
        
        private bool Compare(int? a, int? b, string op)
        {
            switch (op)
            {
                case "=": return a == b;
                case ">": return a > b;
                case "<": return a < b;
                case ">=": return a >= b;
                case "<=": return a <= b;
                default: return false;
            }
        }

        private void SetUserAccess()
        {
            // 0-2: view only 3-5: +insert/update 6-8: +delete 9: +user management (admin)
            if (Shared.loggedInUser.Permission < 6)
            {
                Button_DeleteUser.IsEnabled = false;
                Button_DeleteUser.Foreground = Brushes.Gray;
                Button_DeleteUser.ToolTip = "You do not have rights to delete data!";
            }
            if (Shared.loggedInUser.Permission < 3)
            {
                Button_AddUser.IsEnabled = false;
                Button_AddUser.Foreground = Brushes.Gray;
                Button_AddUser.ToolTip = "You do not have rights to add data!";
                Button_UpdateUser.IsEnabled = false;
                Button_UpdateUser.Foreground = Brushes.Gray;
                Button_UpdateUser.ToolTip = "You do not have rights to update data!";
                Button_Import.IsEnabled = false;
                Button_Import.Foreground = Brushes.Gray;
                Button_Import.ToolTip = "You do not have rights to update data!";
            }
        }

        private void Button_Export_Click(object sender, RoutedEventArgs e)
        {
            
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Comma separated text file (*.csv)|*.csv|Text file (*.txt)|*.txt",
                DefaultExt = ".csv",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = "dbUsers",
                Title = "Save users data as:"
            };

            Nullable<bool> result = saveFileDialog.ShowDialog(); // show saveFileDialog
            if (result == true)
            {
                // create file content
                StreamWriter sr = new StreamWriter(saveFileDialog.FileName, append: false, encoding: Encoding.UTF8);
                // write file header line
                string header_row = "Id;Username;Password;Location;Persmission;Active";
                /*
                int num_columns = dataGrid1.Columns.Count;
                for (int i = 0; i < num_columns; i++)
                {
                    header_row += dataGrid1.Columns[i].Header.ToString();
                    if (i + 1 < num_columns) header_row += ";";
                }
                */
                sr.WriteLine(header_row);

                // write file rows
                string rows = "";
                UserService.User user;
                int i = 0;
                for (i= 0; i < dataGrid1.Items.Count; i++)
                {
                   user = dataGrid1.Items[i] as UserService.User;
                   rows += $"{user.Id};{user.Username};{user.Password};{user.Location};{user.Permission};{user.Active}\n";
                }
                sr.Write(rows);
                sr.Close();
                // Save document
                // File.WriteAllText(saveFileDialog.FileName, rows);

                TextBlock_message.Text = $"Database content ({i} records) printed to '{saveFileDialog.FileName}' file.";
                TextBlock_message.Foreground = Brushes.LightGreen;
                checkBox_fadeInOut.IsChecked = false;
                checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                gifImage.StartAnimation();
            }
        }
        private void Button_Import_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Encrypt password? (Choose No if passwords are already encrpyted!)", caption: "Question", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            bool encrypt = result == MessageBoxResult.Yes;
            if (result == MessageBoxResult.Cancel) { return; }

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Comma separated file (*.csv) |*.csv|Text file (*.txt)|*.txt",
                DefaultExt = ".csv",
                Title = "Open file for import to 'Users' table"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                StreamReader sr = new StreamReader(openFileDialog.FileName);
                // check header correctness
                string header_row = sr.ReadLine();
                int first_colum = header_row.Split(';').Length == 5 ? 0 : 1; // 1 if Id column is provided

                if (header_row != "Id;Username;Password;Location;Persmission;Active" && header_row != "Username;Password;Location;Persmission;Active")
                {
                    MessageBox.Show($"Incorrect file content! Expected header is 'Id; Username; Password; Location; Persmission; Active' (Id is optional), but received '{header_row}'");
                    return;
                }


                UserService.User user;
                importList = new List<UserService.User>();
                int row_index = 0;
                string[] row;
                int usersAdded = 0;
                string hostMessage = "";
                string errorMessage = "";
                dbLocationsList = Location.GetLocations("", "", "", "");
                if (dbLocationsList == null) { IsEnabled = false; Close(); return; } // stop on any error
                int? id = dbUsersList.Max(u => u.Id) + 1;
                while (sr.EndOfStream == false)
                {
                    string error = "";
                    row = sr.ReadLine().Split(';');
                    if (row.Length != 5 + first_colum) // skip row if number of columns is incorrect
                    {
                        continue;
                    }
                    string username = row[first_colum];
                    string password = row[first_colum + 1];
                    string location = row[first_colum + 2];
                    string permission = row[first_colum + 3];
                    string active = row[first_colum + 4];
                    
                    // check data correctness
                    if (username.Length < 5)
                    {
                        error += $"'{username}': Username must be at least 5 charachters long!\n";
                    }
                    if (dbUsersList.Any(p => p.Username == username)) // if user already exists in database
                    {
                        error += $"'{username}': User already exists in database!\n";
                    }
                    if (password.Length < 5)
                    {
                        error += $"'{username}': Password must be at least 5 charachters long!\n";
                    }
                    if (dbLocationsList.Any(p => p.Name == location) == false) // if wrong Location name is entered
                    {
                        error += $"'{username}': Location '{location}' does not exist!\n";
                    }
                    if (Shared.permissionList.Any(p => p == permission) == false) // if wrong Permission value is entered
                    {
                        error += $"'{username}': Persmission value '{permission}' does not exist!\n";
                    }
                    if ((active == "0" || active == "1") == false) // if wrong Active value is entered
                    {
                        error += $"'{username}': Active value '{active}' does not exist!\n";
                    }
                    errorMessage += error;
                    if (error != "") { continue; } // skip on error

                    // REGISTER into database
                    hostMessage = client.RegisterUser(Shared.uid, username, (encrypt ? Shared.CreateMD5(password) : password), location, permission);
                    if (hostMessage == "Unauthorized user!")
                    {
                        Shared.Logout(); // stop on unauthorized user
                        IsEnabled = false;
                        Close();
                        return;
                    }
                    else if (hostMessage != "User successfully registered!")
                    {
                        errorMessage += $"'{username}': {hostMessage}\n";
                        continue;
                    }

                    user = new UserService.User
                    {
                        Id = id,
                        Username = username,
                        Password = password,
                        Location = location,
                        Permission = int.Parse(permission),
                        Active = int.Parse(active)
                    };

                    usersAdded++;
                    importList.Add(user);
                    Log("insert"); // write log to file
                    id++;
                    row_index++;
                }
                sr.Close();

                if (errorMessage != "") { MessageBox.Show($"Following error occurred during the data import:\n\n{errorMessage}", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
                if (importList.Count > 0)
                {
                dataGrid1.ItemsSource = importList;
                
                TextBlock_message.Text = $"{usersAdded} {(usersAdded == 1 ? "record" : "records")} added into the 'Users' table.";
                TextBlock_message.Foreground = Brushes.LightGreen;
                checkBox_fadeInOut.IsChecked = false;
                checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                gifImage.StartAnimation();
                }
            }
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double stretch = Math.Max((borderLeft.ActualWidth - 10 -67) / (550 - 10 - 140), 0.8); // Border width - left margin - a bit more because first column remains unchanged
            dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
            dataGrid0.Width = dataGrid1.Width;
            dataGrid0.Columns[0].Width = dataGrid1.Columns[0].ActualWidth;

            stackPanel1.Height = 442 + window.ActualHeight - 500 - stackPanel1.Margin.Top + 45; // original window.Height

            // stretch columns to dataGrid1 width
            for (int i = 1; i < dataGrid1.Columns.Count; i++)
            {
                dataGrid1.Columns[i].Width = dataGrid1.Columns[i].MinWidth * stretch;
                dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
                dataGrid0.Columns[i].MaxWidth = dataGrid1.Columns[i].ActualWidth * stretch;
            }
            dataGrid1.FontSize = 14 * Math.Max(stretch*0.85, 0.85);
        }

        private void Log(string operation)
        {
            string row = "";
            // save operation into log file
            StreamWriter sr = new StreamWriter(@".\Logs\manageUsers.log", append: true, encoding: Encoding.UTF8);
            // write file header line
            // string header_row = "Date;Username;Operation;Id;Username;Location;Permission;Active";
            // sr.WriteLine(header_row);

            // write file rows
            UserService.User user;
            user = user_edited;

            if (operation == "update") // in update mode add the old value in a new line
            {
                int index = column_index;
                row = $"{DateTime.Now.ToString("yy.MM.dd HH:mm:ss")};{Shared.loggedInUser.Username};{operation};{user.Id};{(column_index == 1 ? old_value : null)};{(column_index == 2 ? old_value : null)};{(column_index == 3 ? old_value : null)};{(column_index == 4 ? old_value : null)};{(column_index == 5 ? old_value : null)}\n";
            }

            row += $"{DateTime.Now.ToString("yy.MM.dd HH:mm:ss")};{Shared.loggedInUser.Username};{operation};{user.Id};{user.Username};{user.Password};{user.Location};{user.Permission};{user.Active}";
            sr.WriteLine(row);
            sr.Close();
        }

        private void Button_Maximize_Click(object sender, RoutedEventArgs e)
        {
            windowWidth0 = window.Width;
            windowHeight0 = window.Height;
            windowLeft0 = window.Left;
            windowTop0 = window.Top;
            window.Width = Shared.screenWidth;
            window.Height = Shared.screenHeight;
            window.Left = 0;
            window.Top = 0;
            Button_Restore.IsEnabled = true;
            Button_Maximize.IsEnabled = false;

        }

        private void Button_Restore_Click(object sender, RoutedEventArgs e)
        {
            window.Width = windowWidth0;
            window.Height = windowHeight0;
            window.Left = windowLeft0;
            window.Top = windowTop0;
            Button_Restore.IsEnabled = false;
            Button_Maximize.IsEnabled = true;
        }

        Logs.LogWindowUsers LogWindowUsers;

        private void Button_confirmPwd_Click(object sender, RoutedEventArgs e)
        {
            if(passwordTextBox.Text == new_value)
            {
                dataGrid1_CellEditEnding(dataGrid1, ePasswordBox); // run CellEditEnding event to finish saving confirmed password
            }
            else
            {
                MessageBox.Show("Password incorrect, please retype!", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                passwordTextBox.Focus();
            }
        }
        // confirm password when pressing the Enter key
        private void passwordTextBox_confirmPwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { Button_confirmPwd_Click(sender, e); }
        }

        private void HidePasswordBox()
        {
            dataGrid1.CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid1_CellEditEnding);
            ePasswordBox = null;
            dataGrid1.IsReadOnly = false;
            passwordBox.Visibility = Visibility.Collapsed;
            passwordTextBox.Text = "";
        }

        private void textBlock_ShowPassword_Click(object sender, MouseButtonEventArgs e)
        {
            passwordTextBox.FontFamily = new FontFamily(familyName: "Helvetica");
            passwordTextBox.Padding = new Thickness(0, 1, 0, 0);
            textBlock_hidePassword.Visibility = Visibility.Visible;
            textBlock_showPassword.Visibility = Visibility.Collapsed;

        }
        private void textBlock_HidePassword_Click(object sender, MouseButtonEventArgs e)
        {
            passwordTextBox.FontFamily = new FontFamily(familyName: "Hololens MDL2 Assets");
            passwordTextBox.Padding = new Thickness(0, 0, 0, 4);
            textBlock_showPassword.Visibility = Visibility.Visible;
            textBlock_hidePassword.Visibility = Visibility.Collapsed;
        }

        private void Button_LogWindow_Click(object sender, RoutedEventArgs e)
        {
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(LogWindowUsers))
            {
                LogWindowUsers = new Logs.LogWindowUsers();
                if (LogWindowUsers.IsEnabled) LogWindowUsers.Show();
            }
        }

    }
}

