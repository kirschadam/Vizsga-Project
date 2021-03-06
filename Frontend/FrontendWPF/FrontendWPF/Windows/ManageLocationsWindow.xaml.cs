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
    public partial class ManageLocationsWindow : Window
    {
        private LocationService.LocationServiceClient locationClient = new LocationService.LocationServiceClient();
        private bool closeCompleted = false;
        private List<LocationService.Store> dbLocationsList { get; set; }
        private List<LocationService.Region> dbRegionsList { get; set; }

        System.Collections.IList selectedItems;
        List<Classes.Location> filterLocationsList { get; set; }
        List<LocationService.Store> filteredLocationsList { get; set; }
        List<LocationService.Store> selectedLocationsList { get; set; }
        List<LocationService.Store> importList { get; set; }

        int PK_column_index = 0;
        string edit_mode;
        private int[] fieldsEntered = new int[2]; // Name, Region
        ScrollViewer scrollViewer;
        string input = "";
        double windowLeft0;
        double windowTop0;
        double windowWidth0;
        double windowHeight0;
        string opId = "=";





        public ManageLocationsWindow()
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
            
            // query all locations from database
            dbLocationsList = Location.GetLocations("", "", "", "");
            if (dbLocationsList == null) { IsEnabled = false; Close(); return; } // stop on any error
            TextBlock_message.Text = $"{dbLocationsList.Count} locations loaded.";

            dataGrid1.ItemsSource = dbLocationsList;
            SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);

            filterLocationsList = new List<Classes.Location>();

            dbRegionsList = Region.GetRegions("", "", "", "");
            if (dbRegionsList == null) { IsEnabled = false; Close(); return; } // stop on any error

            Dispatcher.InvokeAsync(() =>
            {
                double stretch = Math.Max((borderLeft.ActualWidth - 10 + -68) / (550 - 10 - 240), 0.8); // Border width - left margin - a bit more because first column remains unchanged
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
                dataGrid1.FontSize = 15 * Math.Min(stretch*0.8, 1); // reset font size to max 15 on large window width
                dataGrid1.Items.Refresh();
                ScrollDown();
                selectedItems = dataGrid1.SelectedItems; // to make sure it is not null;
            }, DispatcherPriority.Loaded);
            EnableButtons();

            // create/reset location_filter item and add it to filter dataGrid0
            location_filter = new Classes.Location()
            {
                Id = "",
                Name = "",
                Region = ""
            };



            filterLocationsList.Clear();
            filterLocationsList.Add(location_filter);
            dataGrid0.ItemsSource = null; // to avoid IsEditingItem error
            dataGrid0.ItemsSource = filterLocationsList;
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

            // in update mode  update selectedcells and location_edited (when SelecionUnit is Cell)
            // if (dataGrid1.SelectionUnit == DataGridSelectionUnit.Cell)
            if (edit_mode == "update")
            {
                // https://stackoverflow.com/questions/4714325/wpf-datagrid-selectionchanged-event-isnt-raised-when-selectionunit-cell
                IList<DataGridCellInfo> selectedcells = e.AddedCells;
                if (selectedcells.Count > 0) // ignore new selection when button is pressed and selection becomes 0; 
                {
                    location_edited = (LocationService.Store)selectedcells[0].Item;
                    location_edited0 = location_edited;
                }
            }
        }

        private void Button_DeleteLocation_Click(object sender, RoutedEventArgs e)
        {
            if (edit_mode == "update")
            {
                edit_mode = "read";
                dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                DataGridCellInfo currentCell = dataGrid1.CurrentCell;

                dataGrid1.SelectedItems.Add(location_edited); // this triggers SelectionChanged and sets new selectedItems
            }

            if (selectedItems.Count > 0)
            {
                selectedLocationsList = new List<LocationService.Store>();
                foreach (LocationService.Store location in selectedItems)
                {
                    selectedLocationsList.Add(location);
                }
                dataGrid1.ItemsSource = selectedLocationsList;

                // waits to render dataGrid1 and sets row background color to Salmon 
                dataGrid1.Dispatcher.InvokeAsync(() =>
                {
                    for (int i = 0; i < selectedLocationsList.Count; i++)
                    {
                        Shared.StyleDatagridCell(dataGrid1, row_index: i, column_index: 1, Brushes.Salmon, Brushes.White);
                    }

                    int selectedLocations = selectedLocationsList.Count;
                    string deleteMessage = selectedLocations == 1 ? "Are you sure to delete the selected location?" : $"Are you sure to delete the selected {selectedLocations} locations?";

                    TextBlock_message.Text = selectedLocations == 1 ? "Delete location?" : $"Delete {selectedLocations} location?";
                    TextBlock_message.Foreground = Brushes.Salmon;
                    MessageBoxResult result = MessageBox.Show(deleteMessage, caption: "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        for (int i = selectedLocations - 1; i >= 0; i--)
                        {
                            try
                            {
                                // DELETE location(s) from database
                                deleteMessage = locationClient.RemoveLocation(Shared.uid, selectedLocationsList[i].Id.ToString(), selectedLocationsList[i].Name);
                                if (deleteMessage == "Location successfully removed!")
                                {
                                    location_edited = selectedLocationsList[i]; // required to write the log
                                    Log("delete"); // write log to file
                                    dbLocationsList.Remove(selectedLocationsList[i]); // remove location also from dbLocationsList
                                    selectedLocationsList.RemoveAt(i);
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

                        if (selectedLocationsList.Count == 0)
                        {
                            deleteMessage = selectedLocations == 1 ? "The location has been deleted." : "The locations have been deleted.";
                            TextBlock_message.Text = selectedLocations == 1 ? "Location deleted." : "Locations deleted.";
                        }
                        else
                        {
                            deleteMessage = selectedLocationsList.Count == 1 ? "The location shown in the table could not be deleted, as reported in the error message." : "The locations shown in the table could not be deleted, as reported in the error message.";
                        }
                        // list the locations that could not be deleted (empty if all deleted)
                        dataGrid1.ItemsSource = null;
                        dataGrid1.ItemsSource = selectedLocationsList;

                        checkBox_fadeInOut.IsChecked = false;
                        checkBox_fadeInOut.IsChecked = true; // show gifImage
                        gifImage.StartAnimation();
                        MessageBox.Show(deleteMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    // dataGrid1.Focus();
                    dataGrid1.ItemsSource = dbLocationsList;
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
                MessageBox.Show("Nothing is selected. Please select at least one location. ", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            dataGrid1.CanUserSortColumns = true;
        }


        private void Button_UpdateLocation_Click(object sender, RoutedEventArgs e)
        {
            UpdateLocation();
        }

        private void UpdateLocation()
        {
            if (edit_mode != "update") // if not in update mode, switch to update mode
            {
                if (edit_mode == "insert") // remove incomplete added user 
                {
                    dbLocationsList.Remove(location_edited);
                    dataGrid1.ItemsSource = null;
                    dataGrid1.ItemsSource = dbLocationsList;
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
                TextBlock_message.Text = "Update location.";
                TextBlock_message.Foreground = Brushes.White;
                ScrollDown();
                
                edit_mode = "update";
            }
            else
            {
                MessageBox.Show("Please insert new data into a cell, then press Enter.", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                dataGrid1.Focus();
            }
        }

        private void Button_AddLocation_Click(object sender, RoutedEventArgs e)
        {
            AddLocation();
        }

        private void AddLocation()
        {
            if (edit_mode != "insert") // if not in insert mode, switch to insert mode
            {
                Array.Clear(fieldsEntered, 0, fieldsEntered.Length);

                // in db select last location with highest Id
                int? highestId = dbLocationsList.Count > 0 ? dbLocationsList.Max(u => u.Id) : 0;
                location_edited = new LocationService.Store() // create new location with suggested values
                {
                    Id = highestId + 1,
                    Name = "",
                    Region = ""
                };
                location_edited0 = location_edited;

                dbLocationsList.Add(location_edited);
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = dbLocationsList;

                if (dataGrid1.Columns[0].SortDirection != ListSortDirection.Ascending)
                {
                    SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
                }
                dataGrid1.CanUserSortColumns = false;

                dataGrid1.IsReadOnly = false; // CanUserAddRows="False" must be set in XAML
                ScrollDown();
                row_index = dataGrid1.Items.Count - 1;
                dataGrid1.SelectedItem = dataGrid1.Items[row_index]; // select last row containing the location to be added

                // delay execution after dataGrid1 is re-rendered (after new itemsource binding)!
                // https://stackoverflow.com/questions/44272633/is-there-a-datagrid-rendering-complete-event
                // https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
                dataGrid1.Dispatcher.InvokeAsync(() => 
                {
                    // style the id cell of the new location
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
                TextBlock_message.Text = "Add location.";
                TextBlock_message.Foreground = Brushes.White;

                Button_AddLocation.IsEnabled = false;
                Button_DeleteLocation.IsEnabled = false;
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
        LocationService.Store location_edited, location_edited0;
        Classes.Location location_filter;

        private void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                if (CellEditEnding_setup(e) == null) { return; } // setup rules: stop on null

                string stopMessage = CellEditEnding_checkInput(); // check data correctness
                if (stopMessage == "stop") { return; } // stop on database error
                if (stopMessage != "")  // warn user, and stop
                {
                    MessageBox.Show(stopMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    textBox.Text = old_value; // restore correct cell value
                    // cell.Content = old_value;

                    Dispatcher.InvokeAsync(() =>
                    {
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

                Dispatcher.InvokeAsync(() =>
                {
                    SelectEditedCell(); // select edited row/cell if user selected another row/cell after data entry
                }, DispatcherPriority.Loaded);

                if (edit_mode == "insert") // mark edited cell with salmon in insert mode
                {
                    Shared.StyleDatagridCell(dataGrid1, row_index, column_index, Brushes.Salmon, Brushes.White); // style the updated cell
                }


                // start saving new valid value
                fieldsEntered[column_index - 1] = 1; // register the entered property's column index

                if (column_index < 3) // update string-type fields with new value (Name, Region)
                {
                    location_edited.GetType().GetProperty(changed_property_name).SetValue(location_edited, new_value);
                }
                else // update int?-type fields with new value (nothing)
                {
                    int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                    location_edited.GetType().GetProperty(changed_property_name).SetValue(location_edited, Convert.ToInt32(new_value));
                }

                // check if all properties are entered, then insert into database
                if (fieldsEntered.All(n => n == 1) || edit_mode == "update") // if all fields have been updated OR update mode for one field
                {
                    string hostMessage = "";
                    try
                    {
                        if (edit_mode == "insert")
                        {
                            // ADD into database
                            hostMessage = locationClient.AddLocation(Shared.uid, location_edited.Name, location_edited.Region);
                            if (hostMessage == "Unauthorized user!")
                            {
                                Shared.Logout(); // stop on unauthorized user
                                IsEnabled = false;
                                Close();
                                return;
                            }
                            else if (hostMessage != "Location successfully added!")
                            {
                                MessageBox.Show(hostMessage, caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                // restore old value // TODO: restore cell values? (or simply reload entire list?)
                                location_edited = location_edited0;
                                return;
                            }
                        }
                        else if (edit_mode == "update")
                        {
                            hostMessage = locationClient.UpdateLocation(Shared.uid, location_edited.Id.ToString(), location_edited.Name, location_edited.Region);
                            if (hostMessage == "Unauthorized user!")
                            {
                                Shared.Logout(); // stop on unauthorized user
                                IsEnabled = false;
                                Close();
                                return;
                            }
                            else if (hostMessage != "Location successfully updated!")
                            {
                                MessageBox.Show(hostMessage + " Field was not updated.", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                // restore old value // TODO: restore cell value? 
                                location_edited = location_edited0;
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
                        // set background color of added location to green
                        for (int i = 0; i < dataGrid1.Columns.Count; i++)
                        {
                            cell = dataGrid1.Columns[i].GetCellContent(row).Parent as DataGridCell;
                            cell.Background = Brushes.OliveDrab;
                        }
                        TextBlock_message.Text = $"The location '{location_edited.Name}' has been added.";
                        Array.Clear(fieldsEntered, 0, fieldsEntered.Length);
                        Log("insert"); // write log to file
                        edit_mode = "read";
                        dataGrid1.CanUserSortColumns = true;
                        dataGrid1.IsReadOnly = true;
                        dataGrid1.Dispatcher.InvokeAsync(() =>
                        {
                            Button_AddLocation.Focus(); // set focus to allow repeatedly add location on pressing the Add location button
                        }, DispatcherPriority.Loaded);
                        
                        Button_AddLocation.IsEnabled = true;
                        Button_DeleteLocation.IsEnabled = true;
                        Button_Filter.IsEnabled = true;
                        Button_Export.IsEnabled = true;
                        Button_Import.IsEnabled = true;
                        Button_LogWindow.IsEnabled = true;
                    }
                    else if (edit_mode == "update")
                    {
                        TextBlock_message.Text = $"The location '{location_edited.Name}' has been updated with {changed_property_name}.";
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
            if (edit_mode != "update" && Button_UpdateLocation.IsKeyboardFocused) // switch to insert mode if 'Update' is clicked
            {
                edit_mode = "read";
                dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                dbLocationsList.RemoveAt(dbLocationsList.Count - 1);
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = dbLocationsList;
                EnableButtons();
                UpdateLocation();
                return null;
            }
            else if (edit_mode != "insert" && Button_AddLocation.IsKeyboardFocused) // switch to insert mode if 'Add' is clicked
            {
                edit_mode = "read";
                EnableButtons();
                AddLocation();
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
            //location_edited = row.Item as LocationService.Location; //  location_edited and location_edited0 are already defined in UpdateLocation and AddLocation (read out current (old) values from the row, because the entry is a new value)

            cell = dataGrid1.Columns[column_index].GetCellContent(row).Parent as DataGridCell;
            textBox = (TextBox)cell.Content;
            new_value = textBox.Text;

            changed_property_name = dataGrid1.Columns[column_index].Header.ToString();
            // get old property value of location by property name
            // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
            old_value = location_edited.GetType().GetProperty(changed_property_name).GetValue(location_edited).ToString();
            return "OK";
        }

        private string CellEditEnding_checkInput()
        {
            string stopMessage = "";
            if (new_value == "") // if new value is empty
            {
                stopMessage = "New value cannot be empty!";
            }
            else if (changed_property_name == "Name" && new_value.Length < 3)
            {
                stopMessage = $"The name must be at least 3 charachters long!";
            }
            else if (changed_property_name == "Name" && new_value != old_value && dbLocationsList.Any(p => p.Name == new_value)) // stop if location already exists in database, AND if new name is different
            {
                stopMessage = $"The location '{new_value}' already exists, please enter another name!";
            }
            else if (changed_property_name == "Region" && new_value.Length < 3)
            {
                stopMessage = $"The name must be at least 3 charachters long!";
            }
            else if (changed_property_name == "Region" && new_value != old_value && dbRegionsList.Any(p => p.Name == new_value) == false) // stop if region does not exist in database, AND if new name is different
            {
                stopMessage = $"The region '{new_value}' does not exist, please check in the database!";
            }
            return stopMessage;
        }

        private void CellEditEnding_nextCell_update()
        {
            dataGrid1.Dispatcher.InvokeAsync(() => {
                // select next  column; if last 'UnitPrice' column is reached, return to first 'Name' column
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

                // select next unchanged column; if last 'UnitPrice' column is reached, return to first 'Name' column
                int column_shift = 0;
                while (fieldsEntered[column_index + column_shift - 1] != 0)
                {
                    column_shift = column_index + column_shift == 2 ? -column_index + 1 : column_shift + 1;
                }
                cell = dataGrid1.Columns[column_index + column_shift].GetCellContent(row).Parent as DataGridCell;

                // turn off eventual editing mode caused e.g. by tab key on data entry
                if (cell.IsEditing) { cell.IsEditing = false; }

                Button_AddLocation.Focus();

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

        private void EnableButtons()
        {
            Button_AddLocation.IsEnabled = true;
            Button_DeleteLocation.IsEnabled = true;
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

            // in insert mode, do not allow user to edit a different row, and restore selection, focus and editing
            if (edit_mode == "insert" && e.Row.GetIndex() != row_index)
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

        private void dataGrid1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            DataGrid dataGrid1 = sender as DataGrid;

            // https://stackoverflow.com/questions/3248023/code-to-check-if-a-cell-of-a-datagrid-is-currently-edited
            IEditableCollectionView itemsView = dataGrid1.Items;

            // prevent dataGrid to select lower cell on Enter if not editing (otherwise entire editing would stop
            if (e.Key == Key.Enter && (itemsView.IsAddingNew || itemsView.IsEditingItem) == false && edit_mode != "insert")
                if (e.Key == Key.Enter && itemsView.IsEditingItem == false && edit_mode != "insert")
                {
                    // e.Handled = true;
                    // CellEditEnding_nextCell_update();
                }
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
            filteredLocationsList = new List<LocationService.Store>();

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
            row = e.Row;
            column = e.Column;
        }

        private void dataGrid0_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            input = e.Text; // get character entered
        }

        int? location_filterId = null;
        private void dataGrid0_KeyUp(object sender, KeyEventArgs e)
        //private void dataGrid0_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

            if (Button_ReloadData.IsKeyboardFocused) // return if 'Reload data" is clicked
            {
                return;
            }
            else if (Button_Close.IsKeyboardFocused)
            {
                CloseWindow();
                return;
            }

            if (row == null || column == null) { return; } // stop if column or row is not selected or not in edit mode


            // check whether the pressed key is digit or number, otherwise stop
            if (input.Length == 0)
            {
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
            if (new_value != "" && new_value.Length == 1 && (textBox.Text == "<" || textBox.Text == ">" || textBox.Text == "=") && e.Key != Key.Back && e.Key != Key.Delete) { return; } // stop if < or > in empty cell (but continue to recalculate if last key was Key.Back or Key.Delete)
            if (new_value != "" && new_value.Length < 3 && (textBox.Text.Substring(1) == "=")) { return; } // stop if '=' when there are no more characters       

            string firstChar = new_value != "" ? new_value.ToString().Substring(0, 1) : "";
            string op = firstChar != ">" && firstChar != "<" ? "=" : firstChar;
            if (new_value.Length > 1 && new_value.ToString().Substring(1, 1) == "=")
            {
                op = op == ">" ? ">=" : op == "<" ? "<=" : op; // setting >= or <= operator values
            }

            changed_property_name = dataGrid1.Columns[filterc_index].Header.ToString();

            // set operator value for specific column
            if (changed_property_name == "Id")
            {
                switch (changed_property_name)
                {
                    case "Id": opId = op; break;
                    default: break;
                }
            }
            else { op = ""; } // clear operator for string columns

            //get old property value of location by property name
            // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
            old_value = location_filter.GetType().GetProperty(changed_property_name).GetValue(location_filter).ToString();

            // check data correctness
            string stopMessage = "";
            if (new_value != "")
            {
                if (changed_property_name == "Id")
                {
                    string location_filterId0 = new_value.Replace(">", "").Replace("<", "").Replace("=", "");
                    location_filterId = int.TryParse(location_filterId0, out var tempVal1) ? tempVal1 : (int?)null;
                    if ((location_filterId0 != "" && location_filterId == null) || (location_filterId < 0 || location_filterId > 10000000))
                    {
                        stopMessage = $"The Id '{location_filterId0}' does not exist, please enter a correct value for the Id!";
                    }
                }
            }

            if (stopMessage != "")  // warn user, and stop
            {
                MessageBox.Show(stopMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                textBox.Text = old_value; // restore correct cell value if old value is not null
                Shared.SendKey(Key.End);
                return;
            }

            // update filter fields
            location_filter.GetType().GetProperty(changed_property_name).SetValue(location_filter, new_value);

            // filter
            filteredLocationsList.Clear();
            foreach (var location in dbLocationsList)
            {

                if ((location_filterId == null || Compare(location.Id, location_filterId, opId)) && (location_filter.Name == "" || location.Name.ToLower().Contains(location_filter.Name.ToLower())) && (location_filter.Region == "" || location.Region.ToLower().Contains(location_filter.Region.ToLower())))
                {
                    filteredLocationsList.Add(location);
                }
            }
            // update dataGrid1 with filtered items                    
            dataGrid1.ItemsSource = filteredLocationsList;
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
                Button_DeleteLocation.IsEnabled = false;
                Button_DeleteLocation.Foreground = Brushes.Gray;
                Button_DeleteLocation.ToolTip = "You do not have rights to delete data!";
            }
            if (Shared.loggedInUser.Permission < 3)
            {
                Button_AddLocation.IsEnabled = false;
                Button_AddLocation.Foreground = Brushes.Gray;
                Button_AddLocation.ToolTip = "You do not have rights to add data!";
                Button_UpdateLocation.IsEnabled = false;
                Button_UpdateLocation.Foreground = Brushes.Gray;
                Button_UpdateLocation.ToolTip = "You do not have rights to update data!";
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
                FileName = "dbLocations",
                Title = "Save locations data as:"
            };

            Nullable<bool> result = saveFileDialog.ShowDialog(); // show saveFileDialog
            if (result == true)
            {
                // create file content
                StreamWriter sr = new StreamWriter(saveFileDialog.FileName, append: false, encoding: Encoding.UTF8);
                // write file header line
                string header_row = "Id;Name;Region";
                sr.WriteLine(header_row);

                // write file rows
                string rows = "";
                LocationService.Store location;
                int i;
                for (i = 0; i < dataGrid1.Items.Count; i++)
                {
                    location = dataGrid1.Items[i] as LocationService.Store;
                    rows += $"{location.Id};{location.Name};{location.Region}\n";
                }
                sr.Write(rows);
                sr.Close();

                TextBlock_message.Text = $"Database content ({i} records) printed to '{saveFileDialog.FileName}' file.";
                TextBlock_message.Foreground = Brushes.LightGreen;
                checkBox_fadeInOut.IsChecked = false;
                checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                gifImage.StartAnimation();
            }
        }

        private void Button_Import_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Comma separated file (*.csv) |*.csv|Text file (*.txt)|*.txt",
                DefaultExt = ".csv",
                Title = "Open file for import to 'Locations' table"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                StreamReader sr = new StreamReader(openFileDialog.FileName);
                // check header correctness
                string header_row = sr.ReadLine();
                int first_colum = header_row.Split(';').Length == 2 ? 0 : 1; // 1 if Id column is provided

                if (header_row != "Id;Name;Region" && header_row != "Name;Region")
                {
                    MessageBox.Show($"Incorrect file content! Expected header is 'Id;Name;Region' (Id is optional), but received '{header_row}'");
                    return;
                }

                LocationService.Store location;
                importList = new List<LocationService.Store>();
                int row_index = 0;
                string[] row;
                int locationsAdded = 0;
                string hostMessage = "";
                string errorMessage = "";
                int? id = dbLocationsList.Max(u => u.Id) + 1;
                while (sr.EndOfStream == false)
                {
                    string error = "";
                    row = sr.ReadLine().Split(';');
                    if (row.Length != 2 + first_colum) // skip row if number of columns is incorrect
                    {
                        continue;
                    }

                    string name = row[first_colum];
                    string region = row[first_colum + 1];

                    // check data correctness
                    if (name.Length < 3)
                    {
                        error += $"Name must be et least 3 characters long!\n";
                    }
                    if (dbLocationsList.Any(p => p.Name == name)) // if location already exists in database
                    {
                        error += $"The location '{name}' already exists, please enter another name!\n";
                    }
                    if (region.Length < 3)
                    {
                        error += $"Name must be et least 3 characters long!\n";
                    }
                    else if (dbRegionsList.Any(p => p.Name == region) == false) // if region does not exist in database
                    {
                        error += $"The region '{region}' does not exist!\n";
                    }

                    errorMessage += error;
                    if (error != "") { continue; } // continue on error

                    // ADD into database
                    hostMessage = locationClient.AddLocation(Shared.uid, name, region);
                    if (hostMessage == "Unauthorized user!")
                    {
                        Shared.Logout(); // stop on unauthorized user
                        IsEnabled = false;
                        Close();
                        return;
                    }
                    else if (hostMessage != "Location successfully added!")
                    {
                        errorMessage += $"'{name}': {hostMessage}\n";
                        continue;
                    }

                    location = new LocationService.Store
                    {
                        Id = id,
                        Name = name,
                        Region = region
                    };

                    locationsAdded++;
                    importList.Add(location);
                    Log("insert"); // write log to file
                    id++;
                    row_index++;
                }
                sr.Close();

                if (errorMessage != "") { MessageBox.Show($"Following error occurred during the data import:\n\n{errorMessage}", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
                if (importList.Count > 0)
                {
                    dataGrid1.ItemsSource = importList;

                    TextBlock_message.Text = $"{locationsAdded} {(locationsAdded == 1 ? "record" : "records")} added into the 'Locations' table.";
                    TextBlock_message.Foreground = Brushes.LightGreen;
                    checkBox_fadeInOut.IsChecked = false;
                    checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                    gifImage.StartAnimation();
                }
            }
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double stretch = Math.Max((borderLeft.ActualWidth - 10 + -68) / (550 - 10 - 240), 0.8); // Border width - left margin - a bit more because first column remains unchanged
            dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
            dataGrid0.Width = dataGrid1.Width;
            dataGrid0.Columns[0].Width = dataGrid1.Columns[0].ActualWidth;

            stackPanel1.Height = 442 + window.ActualHeight - 500 - stackPanel1.Margin.Top + 45; // original window.Height

            // stretch columns to dataGrid1 width
            for (int i = 1; i < dataGrid1.Columns.Count; i++)
            {
                dataGrid1.Columns[i].Width = dataGrid1.Columns[i].MinWidth * stretch;
                dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
                // dataGrid0.Columns[i].MaxWidth = dataGrid1.Columns[i].ActualWidth * stretch;
            }
            dataGrid1.FontSize = 15 * Math.Max(stretch * 0.65, 0.9);
        }

        private void Log(string operation)
        {
            string row = "";
            // save operation into log file
            StreamWriter sr = new StreamWriter(@".\Logs\manageLocations.log", append: true, encoding: Encoding.UTF8);
            // write file header line
            // string header_row = "LogDate;LogUsername;LogOperation;Id;Name;BuyUnitPrice;SellUnitPrice";
            // sr.WriteLine(header_row);

            // write file rows
            LocationService.Store location;
            location = location_edited;

            if (operation == "update") // in update mode add the old value in a new line
            {
                int index = column_index;
                row = $"{DateTime.Now.ToString("yy.MM.dd HH:mm:ss")};{Shared.loggedInUser.Username};{operation};{location.Id};{(column_index == 1 ? old_value : null)};{(column_index == 2 ? old_value : null)}\n";
            }

            row += $"{DateTime.Now.ToString("yy.MM.dd HH:mm:ss")};{Shared.loggedInUser.Username};{operation};{location.Id};{location.Name};{location.Region}";
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

        Logs.LogWindowLocations LogWindowLocations;
        private void Button_LogWindow_Click(object sender, RoutedEventArgs e)
        {
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(LogWindowLocations))
            {
                LogWindowLocations = new Logs.LogWindowLocations();
                if (LogWindowLocations.IsEnabled) LogWindowLocations.Show();
            }
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid1.Dispatcher.InvokeAsync(async () => {
                await Task.Delay(2000);
                // fals is needed to display colored rows properly
                dataGrid1.EnableRowVirtualization = false; // must be delayed, otherwise animation does not work properly
            }, DispatcherPriority.SystemIdle);
        }

    }
}


