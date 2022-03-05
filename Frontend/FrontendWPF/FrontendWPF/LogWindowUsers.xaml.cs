using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using FrontendWPF.Classes;


namespace FrontendWPF
{
    public partial class LogWindowUsers : Window
    {
        private StockService.StockServiceClient stockClient = new StockService.StockServiceClient();
        private bool closeCompleted = false;
        private List<UserLog> logList { get; set; }

        System.Collections.IList selectedItems;
        List<UserLog> filterUsersList { get; set; }
        List<UserLog> filteredUsersList { get; set; }

        string edit_mode;
        private List<User> UsersList { get; set; }
        private int[] fieldsEntered = new int[9]; // LogDate, LogUser, LogOperation, (product) Name, Quantity, TotalPrice, Date, Location, User(name)
        ScrollViewer scrollViewer;
        string input = "";
        string opLogDate = "=";
        string opId = "=";
        string opPermission = "=";
        string opActive = "=";
        bool pickStartDate = false;
        double windowLeft0;
        double windowTop0;
        double windowWidth0;
        double windowHeight0;

        DateTime startDate = DateTime.Now.Date.AddDays(-30); // set an initial limit of 29 days
        DateTime endDate = DateTime.Now; //
        // public double columnFontSize { get; set; }

        public LogWindowUsers()
        {
            InitializeComponent();
            logList = new List<UserLog>();
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

            //  https://www.codeproject.com/Questions/155935/how-to-add-rows-to-WPF-datagrids
            /*
            DataGridTextColumn col1 = new DataGridTextColumn();
            dataGrid1.Columns.Add(col1);
            col1.Binding = new Binding("id");
            col1.Header = "ID";
            */

            // get log file content
            logList = UserLog.GetUsersLog(startDate, endDate);
            if (logList == null) { IsEnabled = false; Close(); return; } // stop on any error
            dataGrid1.ItemsSource = logList;
            SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
            TextBlock_message.Text = $"{logList.Count} records loaded.";

            // close window and stop if no User is retrieved
            /*
            if (logList.Count == 0)
            {
                closeCompleted = true;
                IsEnabled = false; // so that window cannot be opened
                Close();
                return;
            }
            */

            // if (window.IsLoaded == false) // run on the first time when window is not loaded
            if (true)
            {
                filterUsersList = new List<UserLog>();

                Dispatcher.InvokeAsync(() =>
                {
                    double stretch = Math.Max((borderLeft.ActualWidth - 10 - 130) / (750 - 10 - 155), 0.8); // (BorderLeft width - left margin - more due to Id and Quantity column) / original borderLeft
                    dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
                    dataGrid0.Width = dataGrid1.Width;

                    stackPanel1.Height = 442 + window.ActualHeight - 500; // original window.Height

                    // stretch columns to dataGrid1 width
                    for (int i = 0; i < dataGrid1.Columns.Count; i++)
                    {
                        dataGrid1.Columns[i].Width = dataGrid1.Columns[i].MinWidth * ((stretch - 1) * (i == 3 || i == 7 || i == 8 ? 0.4 : 1) + 1); // resize Quantity row only by 50%
                        dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
                    }
                    dataGrid1.FontSize = 12 * Math.Min(stretch, 1.45); // reset font size to initial stretch value on large window width
                    // dataGrid1.Columns[2].Header = stretch < 1.18 ? "Quantity" : "Quant.";
                    dataGrid1.Items.Refresh();
                    ScrollDown();
                    selectedItems = dataGrid1.SelectedItems; // to make sure it is not null;
                }, DispatcherPriority.Loaded);
            }
            ScrollDown();

            // create/reset user_filter item and add it to filter dataGrid0
            user_filter = new UserLog()
            {
                LogDate = null,
                LogUsername = "",
                LogOperation = "",
                Id = null,
                Username = "",
                Password = "",
                Location = "",
                Permission = null,
                Active = null
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

        private void Button_DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            TextBlock_message.Text = "Delete log file's content?";
            TextBlock_message.Foreground = Brushes.Salmon;
            MessageBoxResult result = MessageBox.Show("Are you sure to delete all records in the the log file?", caption: "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // DELETE log file's content

                    StreamWriter sw = new StreamWriter("manageUsers.log", append: false, encoding: Encoding.UTF8);
                    sw.WriteLine("LogDate;LogUsername;LogOperation;Id;Username;Password;Location;Permission;Active");
                    sw.Close();
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("XXXXX"))
                    {
                        MessageBox.Show($"This will be a specific error. Details:\n{ex.Message}", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("An error occurred, with the following details:\n" + ex.Message, caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                TextBlock_message.Text = "Log file's content deleted.";

                checkBox_fadeInOut.IsChecked = false;
                checkBox_fadeInOut.IsChecked = true; // show gifImage
                gifImage.StartAnimation();
                MessageBox.Show("Log file's content deleted.", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                ReloadData();
            }
            else
            {
                TextBlock_message.Text = "";
                TextBlock_message.Foreground = Brushes.White;
            }
        }


        DataGridRow row0;
        DataGridColumn column0;
        DataGridCell cell;
        TextBox textBox;
        string old_value;
        string new_value = "";
        int filterc_index;
        string changed_property_name;
        UserLog user_filter;


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



        private void dataGrid1_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

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
            filteredUsersList = new List<UserLog>();

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


        private void dataGrid0_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            row0 = e.Row;
            column0 = e.Column;
        }

        private void dataGrid0_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            input = e.Text; // get character entered
        }

        private void dataGrid0_KeyUp(object sender, KeyEventArgs e)
        // private void dataGrid0_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (row0 == null || column0 == null) { return; } // stop if column or row is not selected or not in edit mode

            // check whether the pressed key is digit or number, otherwise stop
            if (input.Length == 0)
            {
                return; // should not happen
            }
            int ASCII = (int)input[0];
            input = " "; // reset input to empty to avoid false value, becasuse KeyUp event may run on function keys as well
            if ((ASCII > 31 && ASCII < 256) == false) { return; } // stop if not number or digit
            // if (ASCII == 43 || ASCII == 60 || ASCII == 61 || ASCII == 62) { return; } // stop if +, <, =, >
            bool key = e.Key == Key.Back;

            // stop on most function keys
            if (e.Key != Key.Back && e.Key != Key.Delete && e.Key != Key.Oem102 && e.Key != Key.Subtract && e.Key != Key.OemPeriod && ((e.Key >= Key.A && e.Key <= Key.Z) || (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)) == false)
            {
                return;
            }

            filterc_index = column0.DisplayIndex;
            cell = dataGrid1.Columns[filterc_index].GetCellContent(row0).Parent as DataGridCell;
            if (cell.IsEditing == false) { return; } // stop if cell is not editing
            textBox = (TextBox)cell.Content;
            new_value = textBox.Text;
            if (new_value != "" && new_value.Length == 1 && (textBox.Text == "<" || textBox.Text == ">" || textBox.Text == "=") && e.Key != Key.Back && e.Key != Key.Delete) { return; } // stop if < or > in empty cell (but continue to recalculate if last key was Key.Back or Key.Delete
            if (new_value != "" && new_value.Length < 3 && (textBox.Text.Substring(1) == "=")) { return; } // stop if '=' when there are no more characters       

            string firstChar = new_value != "" ? new_value.ToString().Substring(0, 1) : "";
            string op = firstChar != ">" && firstChar != "<" ? "=" : firstChar;
            if (new_value.Length > 1 && new_value.ToString().Substring(1, 1) == "=")
            {
                op = op == ">" ? ">=" : op == "<" ? "<=" : op; // setting >= or <= operator values
            }

            changed_property_name = dataGrid1.Columns[filterc_index].Header.ToString();
            if (changed_property_name == "date") { changed_property_name = "LogDate"; }
            if (changed_property_name == "user name") { changed_property_name = "LogUsername"; }
            if (changed_property_name == "operation") { changed_property_name = "LogOperation"; }

            // remove operator for integer columns Id, Permission and Active
            if (changed_property_name == "LogDate" || changed_property_name == "Id" || changed_property_name == "Permission" || changed_property_name == "Active")
            {
                if (op != "=" || (new_value != "" && new_value.ToString().Substring(0, 1) == "=")) { new_value = new_value.Substring(op.Length); } // remove entered operator

                switch (changed_property_name)
                {
                    case "LogDate": opLogDate = op; break;
                    case "Id": opId = op; break;
                    case "Permission": opPermission = op; break;
                    case "Active": opActive = op; break;
                    default: break;
                }
            }

            if (changed_property_name == "LogDate" && ((new_value.Length < 8 && new_value.Length > 0) || (new_value.Length > 8 && new_value.Length < 14))) { return; } // stop if date length is < 8 OR when time is edited (a character is deleted), otherwise user_filter.Date will be set to null

            // if any user_filter value is null, set it temporarily to -999 to avoid error when setting old value 
            if (changed_property_name == "LogDate" && user_filter.LogDate == null) user_filter.LogDate = DateTime.Parse("01.01.01 01:01:01");
            if (changed_property_name == "Id" && user_filter.Id == null) user_filter.Id = -999;
            if (changed_property_name == "Permission" && user_filter.Permission == null) user_filter.Permission = -999;
            if (changed_property_name == "Active" && user_filter.Active == null) user_filter.Active = -999;

            //get old property value of user by property name
            // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
            old_value = user_filter.GetType().GetProperty(changed_property_name).GetValue(user_filter).ToString();
            if (changed_property_name == "LogDate" && user_filter.LogDate == DateTime.Parse("01.01.01 01:01:01")) user_filter.LogDate = null;
            if (changed_property_name == "Id" && user_filter.Id == -999) user_filter.Id = null;
            if (changed_property_name == "Permission" && user_filter.Permission == -999) user_filter.Permission = null;
            if (changed_property_name == "Active" && user_filter.Active == -999) user_filter.Active = null;

            string stopMessage = "";
            if (old_value == "-999" || op != "=")
            {
                Dispatcher.InvokeAsync(() =>
                {
                    // for some reason, cursor goes to the front of the cell when inputting into empty integer-type cell; therefore, set cursor to the end; skip if an operator is entered into cell

                    if (op != "=" && stopMessage == "") { textBox.Text = op + new_value; } // restore operator into cell, only if there is no error message (because it restores the old value);

                    Shared.SendKey(Key.End);
                }, DispatcherPriority.Input);
            }

            // check data correctness
            bool minutesExist = false;
            if (changed_property_name == "Id")
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                if ((new_value != "" && int_val == null) || (int_val < 0 || int_val > 10000000))
                {
                    stopMessage = $"The Id '{new_value}' does not exist, please enter a correct value for the Id!";
                }
            }
            else if (changed_property_name == "Permission" && new_value != "" && Shared.permissionList.Any(p => p == new_value) == false) // if wrong Permission value is entered
            {
                stopMessage = $"The Permission value '{new_value}' does not exist, please enter the correct value (between 0-9)!";
            }
            else if (changed_property_name == "Active" && (new_value != "" && (new_value == "0" || new_value == "1") == false)) // if wrong Active value is entered
            {
                stopMessage = $"The Active value '{new_value}' does not exist, please enter the correct value (1 or 0)!";
            }
            else if (new_value != "" && (changed_property_name == "LogDate" || changed_property_name == "Date")) // if wrong Date value is entered
            {
                old_value = old_value.Substring(0, old_value.Length - 3);
                bool dateExists = DateTime.TryParse(new_value, out DateTime date_entered);
                if (dateExists == false)
                {
                    stopMessage = $"Please enter a correct value for the date value!";
                }
                else
                {
                    // checks if minutes are entered; if not, minutues in the record will be ignored
                    minutesExist = date_entered.Minute > 0;
                }
            }

            if (stopMessage != "")  // warn user, and stop
            {
                MessageBox.Show(stopMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                if (old_value != "-999" && old_value != "01.01.01 01:01")
                {
                    textBox.Text = op == "=" ? old_value : op + old_value; // restore correct cell value if old value is not null, plus the operator if any
                    Shared.SendKey(Key.End);
                }
                return;
            }

            if (filterc_index == 1 || filterc_index == 2 || filterc_index == 4 || filterc_index == 5 || filterc_index == 6) // // update string-type fields with new value ( LogUsername, LogOperation, Username, Password, Location )
            {
                user_filter.GetType().GetProperty(changed_property_name).SetValue(user_filter, new_value);
            }
            else if (filterc_index == 0) // // update LogDate with new value
            {
                DateTime? int_val = DateTime.TryParse(new_value, out var tempVal) ? tempVal : (DateTime?)null;
                user_filter.GetType().GetProperty(changed_property_name).SetValue(user_filter, int_val);
            }
            else // update int?-type fields with new value (Id, Permission, Active)
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                user_filter.GetType().GetProperty(changed_property_name).SetValue(user_filter, int_val);

            }

            string password = ""; // encrypt only as a separate variable, leaving user_password unchanged
            if (new_value != "" && new_value != "-1" && changed_property_name == "Password" && new_value != old_value)
            {
                password = Shared.CreateMD5(new_value); // encypt new password (the old password is already encrypted
            }

            // filter
            filteredUsersList.Clear();
            foreach (var user in logList)
            {
                if ((user_filter.LogDate == null || (minutesExist ? Compare(user.LogDate, user_filter.LogDate, opLogDate) : Compare(user.LogDate.Value.Date, user_filter.LogDate, opLogDate))) && (user_filter.LogUsername == "" || user.LogUsername.ToLower().Contains(user_filter.LogUsername.ToLower())) && (user_filter.LogOperation == "" || user.LogOperation.ToLower().Contains(user_filter.LogOperation.ToLower())) && (user_filter.Id == null || Compare(user.Id, user_filter.Id, opId)) && (user_filter.Username == "" || user.Username.ToLower().Contains(user_filter.Username.ToLower())) && (password == "" || user.Password == password) && (user_filter.Location == "" || user.Location.ToLower().Contains(user_filter.Location.ToLower())) && (user_filter.Permission == null || Compare(user.Permission, user_filter.Permission, opPermission)) && (user_filter.Active == null || Compare(user.Active, user_filter.Active, opActive)))
                {
                    filteredUsersList.Add(user);
                    continue;
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

        private bool Compare(DateTime? a, DateTime? b, string op)
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

        }

        private void Button_DatePicker_Click(object sender, RoutedEventArgs e)
        {
            pickStartDate = !pickStartDate;
            Button_DatePicker.Foreground = pickStartDate ? Brushes.LightSkyBlue : Brushes.CornflowerBlue;
            TextBlock_datePicker.Foreground = pickStartDate ? Brushes.White : Brushes.LightSkyBlue;
            TextBlock_datePicker.Text = pickStartDate ? "Pick start date" : "Pick end date";

            Button_Restore.Visibility = Visibility.Hidden;
            Button_Maximize.Visibility = Visibility.Hidden;
            TextBlock_datePicker.Visibility = Visibility.Visible;

            // remove/add event handler 
            datePicker.SelectedDateChanged -= new EventHandler<SelectionChangedEventArgs>(datePicker_SelectedDateChanged);
            datePicker.SelectedDate = pickStartDate ? startDate : endDate;
            datePicker.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(datePicker_SelectedDateChanged);

            datePicker.Visibility = Visibility.Visible;
            datePicker.IsDropDownOpen = true;
        }

        private void datePicker_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker != null)
            {
                System.Windows.Controls.Primitives.DatePickerTextBox datePickerTextBox = FindVisualChild<System.Windows.Controls.Primitives.DatePickerTextBox>(datePicker);
                if (datePickerTextBox != null)
                {

                    ContentControl watermark = datePickerTextBox.Template.FindName("PART_Watermark", datePickerTextBox) as ContentControl;
                    if (watermark != null)
                    {
                        watermark.Content = startDate.ToShortDateString(); // "Pick start date"; // string.Empty;
                    }
                }
            }
        }
        private T FindVisualChild<T>(DependencyObject depencencyObject) where T : DependencyObject
        {
            if (depencencyObject != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depencencyObject); ++i)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depencencyObject, i);
                    T result = (child as T) ?? FindVisualChild<T>(child);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (startDate != (DateTime)datePicker.SelectedDate && endDate != (DateTime)datePicker.SelectedDate)  // to avoid set change two times due to false double selection change from datePicker databox editing
            {


                if (pickStartDate)
                {
                    startDate = (DateTime)datePicker.SelectedDate;
                }
                else
                {
                    endDate = (DateTime)datePicker.SelectedDate;
                    endDate = endDate.Hour == 0 && endDate.Minute == 0 && endDate.Second == 0 ? endDate.Date.AddDays(1).AddMinutes(-1) : endDate;
                    //endDate = endDate.ToString().Substring(10, 8) == "00:00:00" ? endDate.Date.AddDays(1).AddMinutes(-1) : endDate;
                }

                ReloadData();

                string startOrEnd = pickStartDate ? "Start" : "End";
                // int trimZeros = datePicker.SelectedDate.ToString().Substring(10, 8) == "00:00:00" ? 10 : 3;
                TextBlock_message.Text = $"{startOrEnd} date changed to {(pickStartDate ? startDate : endDate).ToString().Substring(0, (pickStartDate ? startDate : endDate).ToString().Length - 3)}.";

                pickStartDate = !pickStartDate; // toggle 
                checkBox_fadeInOut.IsChecked = false;
                checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                gifImage.StartAnimation();
            }
        }

        private void datePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            datePicker.Visibility = Visibility.Hidden;
            TextBlock_datePicker.Text = "";
            Button_Restore.Visibility = Visibility.Visible;
            Button_Maximize.Visibility = Visibility.Visible;
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double stretch = Math.Max((borderLeft.ActualWidth - 10 - 130) / (750 - 10 - 155), 0.8); // (BorderLeft width - left margin - more due to Id and Quantity column) / original borderLeft
            dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
            dataGrid0.Width = dataGrid1.Width;
            // dataGrid0.Columns[0].Width = dataGrid1.Columns[0].ActualWidth;

            stackPanel1.Height = 442 + window.ActualHeight - 500; // original window.Heigth

            // stretch columns to dataGrid1 width
            for (int i = 0; i < dataGrid1.Columns.Count; i++)
            {
                dataGrid1.Columns[i].Width = dataGrid1.Columns[i].MinWidth * ((stretch - 1) * (i == 3 || i == 7 || i == 8 ? 0.4 : 1) + 1); // resize Id and Quantity row only by 50%
                dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
            }
            dataGrid1.FontSize = 12 * Math.Max(stretch, 1);
            dataGrid1.Columns[7].Header = stretch < 1.15 ? "Permission" : "Perm.";
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

        UserLog userlog;
        private void dataGrid1_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow row = e.Row;
            userlog = dataGrid1.Items[row.GetIndex()] as UserLog;
            if (userlog.Permission == null || userlog.Active == null)
                {
                    e.Row.Foreground = new SolidColorBrush(Colors.Gray);
                }
            else if (userlog.LogOperation == "insert")
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Green);
            }
            else if (userlog.LogOperation == "delete")
            {
                e.Row.Foreground = new SolidColorBrush(Colors.Salmon);
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.Dispatcher.InvokeAsync(async () => {
                await Task.Delay(2000);
                // false is needed to display colored rows properly
                dataGrid1.EnableRowVirtualization = false; // must be delayed, otherwise animation does not work properly
            }, DispatcherPriority.SystemIdle);
        }

   
    }
}


