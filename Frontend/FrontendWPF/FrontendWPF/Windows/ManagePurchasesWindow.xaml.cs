﻿using System;
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
    public partial class ManagePurchasesWindow : Window
    {
        private StockService.StockServiceClient stockClient = new StockService.StockServiceClient();
        private bool closeCompleted = false;
        private List<StockService.SalePurchase> dbPurchasesList { get; set; }

        System.Collections.IList selectedItems;
        List<StockService.SalePurchase> filterPurchasesList { get; set; }
        List<StockService.SalePurchase> filteredPurchasesList { get; set; }
        List<StockService.SalePurchase> selectedPurchasesList { get; set; }
        List<StockService.Product> dbProductsList { get; set; }
        List<UserService.User> dbUsersList { get; set; }
        List<StockService.SalePurchase> importList { get; set; }
        List<LocationService.Store> dbLocationsList { get; set; }

        int PK_column_index = 0;
        string edit_mode;
        private List<SalePurchase> purchasesList { get; set; }
        private int[] fieldsEntered = new int[6]; // (product) Name, Quantity, TotalPrice, Date, Location, User(name)
        ScrollViewer scrollViewer;
        string lastProduct = "";
        int? lastQuantity = null;
        int? lastTotalPrice = null;
        DateTime? lastDate = null;
        string lastLocation = "";
        string lastUsername = "";
        string input = "";
        string opId = "=";
        string opQuantity = "=";
        string opTotalPrice = "=";
        string opDate = "=";
        bool pickStartDate = false;
        double windowLeft0;
        double windowTop0;
        double windowWidth0;
        double windowHeight0;

        DateTime startDate = DateTime.Now.Date.AddDays(-30); // set an initial limit of 29 days
        DateTime endDate = DateTime.Now; //

        // public double columnFontSize { get; set; }

        public ManagePurchasesWindow()
        {
            InitializeComponent();
            
            if (Shared.layout != "")
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                if (Shared.layout.Contains("Products")) // ProductsPurchases
                {
                    int overlay = Shared.layout.Contains("Overlay") ? 250 : 0;
                    Window productsWindow = Application.Current.Windows.OfType<Window>().Where(w => w.Title == "Manage products Window").FirstOrDefault();

                    window.Left = productsWindow.Width - overlay;
                    window.Top = 0;
                    window.Width = Shared.screenWidth - productsWindow.Width + overlay;
                    window.Height = 1 * Shared.screenHeight;
                }
                else if (Shared.layout.Contains("PurchasesSales"))
                {
                    
                    
                                        
                    window.Left = 0;
                    window.Top = 0;
                    window.Height = 1 * Shared.screenHeight;
                    window.Width = 0.5 * Shared.screenWidth;
                    // window.Topmost = false;
                }
            }
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

            // query purchases from database
            dbPurchasesList = SalePurchase.GetSalesPurchases(type: "purchase", id: "", product: "", qOver: "", qUnder: "", priceOver: "", priceUnder: "", before: endDate.ToString(), after:startDate.ToString(), location: "", user: "", limit: ""); ;
            if (dbPurchasesList == null) { IsEnabled = false; Close(); return; } // stop on any error
            TextBlock_message.Text = $"{dbPurchasesList.Count} purchases loaded.";

            // close window and stop if no purchase is retrieved
            /*
            if (dbPurchasesList.Count == 0)
            {
                closeCompleted = true;
                IsEnabled = false; // so that window cannot be opened
                Close();
                return;
            }
            */

            dataGrid1.ItemsSource = dbPurchasesList;

            SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);

            // if (window.IsLoaded == false) // run on the first time when window is not loaded
            if (true)
            {
                filterPurchasesList = new List<StockService.SalePurchase>();

                Dispatcher.InvokeAsync(() => {
                    double stretch = Math.Max((borderLeft.ActualWidth - 10 - 110) / (550 - 10 - 43), 0.8); // (BorderLeft width - left margin - more due to Id and Quantity column) / original borderLeft
                    dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
                    dataGrid0.Width = dataGrid1.Width;
                    stackPanel1.Height = 442 + window.ActualHeight - 500; // original window.Height

                    // stretch columns to dataGrid1 width
                    for (int i = 0; i < dataGrid1.Columns.Count; i++)
                    {
                        dataGrid1.Columns[i].Width = dataGrid1.Columns[i].MinWidth * ((stretch - 1) * (i == 0 || i == 2 ? 0.5 : 1) + 1); // resize Id and Quantity row only by 50%
                        dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
                    }
                    dataGrid1.FontSize = 14 * Math.Min(stretch, Shared.layout == "" ? 1.0664 :stretch); // reset font size to initial stretch value on large window width
                    // dataGrid1.Columns[2].Header = stretch < 1.18 ? "Quantity" : "Quant.";
                    dataGrid1.Items.Refresh();
                    ScrollDown();
                    selectedItems = dataGrid1.SelectedItems; // to make sure it is not null;
                }, DispatcherPriority.Loaded);
            }

            // create/reset purchase_filter item and add it to filter dataGrid0
            purchase_filter = new StockService.SalePurchase()
            {
                Id = null,
                Product = "", // Name of product
                Quantity = null,
                Date = null,
                Location = "",
                Username = ""
            };
            filterPurchasesList.Clear();
            filterPurchasesList.Add(purchase_filter);
            dataGrid0.ItemsSource = null; // to avoid IsEditingItem error
            dataGrid0.ItemsSource = filterPurchasesList;
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

            // in update mode  update selectedcells and purchase_edited (when SelecionUnit is Cell)
            // if (dataGrid1.SelectionUnit == DataGridSelectionUnit.Cell)
            if (edit_mode == "update")
            {
                // https://stackoverflow.com/questions/4714325/wpf-datagrid-selectionchanged-event-isnt-raised-when-selectionunit-cell
                IList<DataGridCellInfo> selectedcells = e.AddedCells;
                if (selectedcells.Count > 0) // ignore new selection when button is pressed and selection becomes 0; 
                {
                    purchase_edited = (StockService.SalePurchase)selectedcells[0].Item;
                    purchase_edited0 = purchase_edited;
                }
            }
        }

        private void Button_DeletePurchase_Click(object sender, RoutedEventArgs e)
        {
            if (edit_mode == "update")
            {
                edit_mode = "read";
                dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                DataGridCellInfo currentCell = dataGrid1.CurrentCell;

                dataGrid1.SelectedItems.Add(purchase_edited); // this triggers SelectionChanged and sets new selectedItems
            }

            if (selectedItems.Count > 0)
            {
                selectedPurchasesList = new List<StockService.SalePurchase>();
                foreach (StockService.SalePurchase purchase in selectedItems)
                {
                    selectedPurchasesList.Add(purchase);
                }
                dataGrid1.ItemsSource = selectedPurchasesList;

                // waits to render dataGrid1 and sets row background color to Salmon 
                dataGrid1.Dispatcher.InvokeAsync(() => {
                    for (int i = 0; i < selectedPurchasesList.Count; i++)
                    {
                        Shared.StyleDatagridCell(dataGrid1, row_index: i, column_index: 1, Brushes.Salmon, Brushes.White);
                    }

                    int selectedPurchases = selectedPurchasesList.Count;
                    string deleteMessage = selectedPurchases == 1 ? "Are you sure to delete the selected purchase?" : $"Are you sure to delete the selected {selectedPurchases} purchases?";

                    TextBlock_message.Text = selectedPurchases == 1 ? "Delete purchase?" : $"Delete {selectedPurchases} purchases?";
                    TextBlock_message.Foreground = Brushes.Salmon;
                    MessageBoxResult result = MessageBox.Show(deleteMessage, caption: "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        for (int i = selectedPurchases - 1; i >= 0; i--)
                        {
                            try
                            {
                                // DELETE purchase(s) from database
                                deleteMessage = stockClient.RemoveSalePurchase(Shared.uid, type: "purchase", id: selectedPurchasesList[i].Id.ToString(), location: selectedPurchasesList[i].Location.ToString());
                                if (deleteMessage == "Sale(s)/purchase(s) successfully removed!")
                                {
                                    purchase_edited = selectedPurchasesList[i]; // required to write the log
                                    Log("delete"); // write log to file
                                    dbPurchasesList.Remove(selectedPurchasesList[i]); // remove purchase also from dbPurchasesList
                                    selectedPurchasesList.RemoveAt(i);
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
                        }

                        if (selectedPurchasesList.Count == 0)
                        {
                            deleteMessage = selectedPurchases == 1 ? "The purchase has been deleted." : "The purchases have been deleted.";
                            TextBlock_message.Text = selectedPurchases == 1 ? "Purchase deleted." : "Purchases deleted.";
                        }
                        else
                        {
                            deleteMessage = selectedPurchasesList.Count == 1 ? "The purchase shown in the table could not be deleted, as reported in the error message." : "The purchases shown in the table could not be deleted, as reported in the error message.";
                        }
                        // list the purchases that could not be deleted (empty if all deleted)
                        dataGrid1.ItemsSource = null;
                        dataGrid1.ItemsSource = selectedPurchasesList;

                        checkBox_fadeInOut.IsChecked = false;
                        checkBox_fadeInOut.IsChecked = true; // show gifImage
                        gifImage.StartAnimation();
                        MessageBox.Show(deleteMessage, caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);

                    }
                    // dataGrid1.Focus();
                    dataGrid1.ItemsSource = dbPurchasesList;
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
                MessageBox.Show("Nothing is selected. Please select at least one purchase. ", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            dataGrid1.CanUserSortColumns = true;
        }


        private void Button_UpdatePurchase_Click(object sender, RoutedEventArgs e)
        {
            UpdatePurchase();
        }

        private void UpdatePurchase()
        {
            if (dataGrid1.Columns[0].SortDirection != ListSortDirection.Ascending)
            {
                SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
            }

            dataGrid1.CanUserSortColumns = false;


            if (edit_mode == "read" || edit_mode == "insert") // if read mode (or window just opened) or in inser mode, switch to update mode
            {
                dataGrid1.IsReadOnly = false; // CanUserAddRows="False" must be set in XAML
                edit_mode = "update";
                dataGrid1.SelectionMode = DataGridSelectionMode.Single;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.Cell;
                TextBlock_message.Text = "Update purchase.";
                TextBlock_message.Foreground = Brushes.White;
                ScrollDown();
            }
            else
            {
                MessageBox.Show("Please insert new data into a cell, then press Enter.", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                dataGrid1.Focus();
            }
        }


        private void Button_AddPurchase_Click(object sender, RoutedEventArgs e)
        {
            AddPurchase();
        }

        private void AddPurchase()
        {
            if (dataGrid1.Columns[0].SortDirection != ListSortDirection.Ascending)
            {
                SortDataGrid(dataGrid1, columnIndex: 0, sortDirection: ListSortDirection.Ascending);
            }

            dataGrid1.CanUserSortColumns = false;
            Array.Clear(fieldsEntered, 0, fieldsEntered.Length);

            if (edit_mode == "read" || edit_mode == "update") // if read mode (window just opened) or update mode, switch to insert mode
            {
               
                // in db select last purchase with highest Id
                int? highestId = dbPurchasesList.Count > 0 ? dbPurchasesList.Max(u => u.Id) : 0;
                purchase_edited = new StockService.SalePurchase() // create new purchase with suggested values
                {
                    Id = highestId + 1,
                    Product = lastProduct != "" ? lastProduct : "",
                    Quantity = lastQuantity != null ? lastQuantity : 1,
                    TotalPrice = lastTotalPrice != null ? lastTotalPrice : 0,
                    Date = lastDate != null ? lastDate : DateTime.Now,
                    Location = lastLocation != "" ? lastLocation : Shared.loggedInUser.Location,
                    Username = lastUsername != "" ? lastUsername : Shared.loggedInUser.Username
                };
                    
                    
            
                purchase_edited0 = purchase_edited;

                dbPurchasesList.Add(purchase_edited);
                dataGrid1.ItemsSource = null;
                dataGrid1.ItemsSource = dbPurchasesList;

                dataGrid1.IsReadOnly = false; // CanUserAddRows="False" must be set in XAML
                ScrollDown();
                row_index = dataGrid1.Items.Count - 1;
                dataGrid1.SelectedItem = dataGrid1.Items[row_index]; // select last row containing the purchase to be added

                // delay execution after dataGrid1 is re-rendered (after new itemsource binding)!
                // https://stackoverflow.com/questions/44272633/is-there-a-datagrid-rendering-complete-event
                // https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
                dataGrid1.Dispatcher.InvokeAsync(() => {
                    // style the id cell of the new purchase
                    Shared.StyleDatagridCell(dataGrid1, dataGrid1.Items.Count - 1, PK_column_index, Brushes.Salmon, Brushes.White);
                    dataGrid1.Focus();
                    row = dataGrid1.ItemContainerGenerator.ContainerFromItem(dataGrid1.Items[row_index]) as DataGridRow;
                    cell = dataGrid1.Columns[1].GetCellContent(row).Parent as DataGridCell;
                    cell.IsEditing = true;
                    cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Down));
                },
                DispatcherPriority.Background); // Background to avoid row = null error

                edit_mode = "insert";
                dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                TextBlock_message.Text = "Add purchase.";
                TextBlock_message.Foreground = Brushes.White;
            }
            else
            {
                MessageBox.Show("Please fill in all purchase data, then press Enter.", caption: "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
        int filterc_index;
        string changed_property_name;
        StockService.SalePurchase purchase_edited, purchase_edited0, purchase_filter;

        private void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // exit insert mode if 'Update purchase' is clicked
                if (Button_UpdatePurchase.IsKeyboardFocused)
                {
                    edit_mode = "read";
                    dataGrid1.SelectionMode = DataGridSelectionMode.Extended;
                    dataGrid1.SelectionUnit = DataGridSelectionUnit.FullRow;
                    dbPurchasesList.RemoveAt(dbPurchasesList.Count - 1);
                    dataGrid1.ItemsSource = null;
                    dataGrid1.ItemsSource = dbPurchasesList;
                    UpdatePurchase();
                    return;
                }
                else if (Button_ReloadData.IsKeyboardFocused) // return if 'Reload data" is clicked
                {
                    return;
                }
                else if (Button_Close.IsKeyboardFocused)
                {
                    CloseWindow();
                    return;
                }

                row = e.Row;
                row_index = row.GetIndex();
                column = e.Column;
                column_index = column.DisplayIndex;
                //purchase_edited = row.Item as StockService.SalePurchase; //  purchase_edited and purchase_edited0 are already defined in UpdatePurchase and AddPurchase (read out current (old) values from the row, because the entry is a new value)

                cell = dataGrid1.Columns[column_index].GetCellContent(row).Parent as DataGridCell;
                textBox = (TextBox)cell.Content;
                new_value = textBox.Text;

                changed_property_name = dataGrid1.Columns[column_index].Header.ToString();
                if (changed_property_name == "Product name") { changed_property_name = "Product"; }
                if (changed_property_name == "Quant.") { changed_property_name = "Quantity"; }
                if (changed_property_name == "Total price") { changed_property_name = "TotalPrice"; }
                if (changed_property_name == "User name") { changed_property_name = "Username"; }
                // get old property value of purchase by property name
                // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
                
                old_value = purchase_edited.GetType().GetProperty(changed_property_name).GetValue(purchase_edited).ToString();

                // check data correctness
                string stopMessage = "";
                if (new_value == "") // if new value is empty
                {
                    stopMessage = "New value cannot be empty!";
                }
                else if (changed_property_name == "Product") // if wrong (product) Name value is entered
                {
                    if (new_value.Length < 5)
                    {
                        stopMessage = $"Product name must be at least 5 characters!";
                    }
                    else
                    {
                        dbProductsList = Product.GetProducts("", "", "", "", "", "", "");
                        if (dbProductsList == null) { IsEnabled = false; Close(); return; } // stop on any error
                        if (dbProductsList.Any(p => p.Name == new_value) == false)
                        {
                            stopMessage = $"The product does not exist in the database. Please check product name.";
                        }
                    }
                }
                else if (changed_property_name == "Quantity") // if wrong Active value is entered
                {
                    int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                    if (int_val == null || (int_val <= 0))
                    {
                        stopMessage = $"Please enter a correct value for the Quantity!";
                    }
                    else if (int_val > 1000000)
                    {
                        stopMessage = $"Quantity cannot exceed 1,000,000!";
                    }
                }
                else if (changed_property_name == "TotalPrice") // if wrong TotalPrice value is entered
                {
                    int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                    if (int_val == null || (int_val <= 0))
                    {
                        stopMessage = $"Please enter a correct value for the Total price!";
                    }
                    else if (int_val > 1000000000)
                    {
                        stopMessage = $"Total price cannot exceed 1,000,000,000!";
                    }
                }
                else if (changed_property_name == "Date") // if wrong (product) Name value is entered
                {
                    old_value = old_value.Substring(0, old_value.Length - 3);
                    bool dateExists = DateTime.TryParse(new_value, out DateTime date_entered);
                    if (dateExists == false)
                    {
                        stopMessage = $"Please enter a correct value for the date value!";
                    }
                    else if (date_entered > DateTime.Now)
                    {
                        stopMessage = $"Date cannot be a future date!";
                    }
                    else if ((DateTime.Now - date_entered).TotalDays > 3650)
                    {
                        stopMessage = $"Date cannot be earlier than 3650 days!";
                    }
                }
                else if (changed_property_name == "Location") // if wrong Location name is entered
                {
                    dbLocationsList = Location.GetLocations("", "", "", "");
                    if (dbLocationsList == null) { IsEnabled = false; Close(); return; } // stop on any error
                    if (dbLocationsList.Any(p => p.Name == new_value) == false)
                    {
                        stopMessage = $"The location '{new_value}' does not exist, please enter the correct location!";
                    }
                }
                else if (changed_property_name == "Username" && new_value.Length < 5)
                {
                    stopMessage = $"The username must be at least 5 charachters long!";
                }
                else if (changed_property_name == "Username" && new_value != old_value)
                {
                    dbUsersList = User.GetUsers("", "", "", "", "", "", "", "");
                    if (dbUsersList.Any(p => p.Username == new_value) == false) // stop if user does not exist in database, AND if new username is different
                    {
                        stopMessage = $"The user '{new_value}' does not exist, please enter another username!";
                    }
                }


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
                        Button_AddPurchase.Focus();

                        SelectTextBox();

                        // restore event handler
                        (sender as DataGrid).CellEditEnding += new EventHandler<DataGridCellEditEndingEventArgs>(dataGrid1_CellEditEnding);
                    }, DispatcherPriority.Loaded);
                    return;
                }
                // stop in insert mode if new and old value are the same AND the field was already updated (in insert mode the suggested old values of columns Quantity, TotalPrice, Date, Location and Username can be same as old values if accepted) OR in each case in update mode; 
                else if (old_value == new_value && (fieldsEntered[column_index - 1] == 1 || edit_mode == "update")) // && column_index < 5
                {
                    MoveToNextCell();
                    return;
                }

                Dispatcher.InvokeAsync(() => {
                    SelectEditedCell(); // select edited row/cell if user selected another row/cell after data entry
                }, DispatcherPriority.Loaded);
                
                if (edit_mode == "insert") // mark edited cell with salmon in insert mode
                {
                    Shared.StyleDatagridCell(dataGrid1, row_index, column_index, Brushes.Salmon, Brushes.White); // style the updated cell
                }

                bool ProductChanged = changed_property_name == "Product" && purchase_edited.Product != "" && purchase_edited.Product != new_value ? true : false;
                bool QuantityChanged = changed_property_name == "Quantity" && purchase_edited.Quantity != null && purchase_edited.Quantity != Int32.Parse(new_value) ? true : false;

                // start saving new valid value
                fieldsEntered[column_index - 1] = 1; // register the entered property's column index

                if (column_index == 1 || column_index == 5 || column_index == 6) // // update string-type fields with new value ( (Product) Name, Location, Username )
                {
                    purchase_edited.GetType().GetProperty(changed_property_name).SetValue(purchase_edited, new_value);
                }
                else if (column_index == 4) // // update Date fields with new value
                {
                    purchase_edited.GetType().GetProperty(changed_property_name).SetValue(purchase_edited, DateTime.Parse(new_value));
                }
                else // update int?-type fields with new value (Quantity, TotalPrice)
                {
                    int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                    
                    purchase_edited.GetType().GetProperty(changed_property_name).SetValue(purchase_edited, Convert.ToInt32(new_value));
                }

                // if Name or Quantity is entered AND purchase_edited.TotalPrice is 0, OR Product changed OR Quantity changed, calculate and enter suggested TotalPrice
                if ((purchase_edited.Product != "" && purchase_edited.Quantity != null && purchase_edited.TotalPrice == 0) || ProductChanged || QuantityChanged)
                {
                    dbProductsList = Product.GetProducts("", purchase_edited.Product, "", "", "", "", "");
                    if (dbProductsList == null) { IsEnabled = false; Close(); return; } // stop on any error
                    if (dbProductsList.Count == 1)
                    {
                        int? totalPrice = purchase_edited.Quantity * dbProductsList[0].BuyUnitPrice;
                        purchase_edited.TotalPrice = totalPrice;
                    }
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
                            hostMessage = stockClient.AddSalePurchase(Shared.uid, type: "purchase", purchase_edited.Product, purchase_edited.Quantity.ToString(), purchase_edited.Location, purchase_edited.TotalPrice.ToString(), purchase_edited.Date.ToString());
                            if (hostMessage == "Unauthorized user!")
                            {
                                Shared.Logout(); // stop on unauthorized user
                                IsEnabled = false;
                                Close();
                                return;
                            }
                            else if (hostMessage.Contains("FOREIGN KEY (`productId`)"))
                            {
                                MessageBox.Show($"The product does not exist in the database. Please check product name.", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                            else if (hostMessage != "Sale/Purchase successfully added!")
                            {
                                MessageBox.Show(hostMessage, caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                // restore old value // TODO: restore cell values? (or simply reload entire list?)
                                purchase_edited = purchase_edited0;
                                return;
                            }
                        }
                        else if (edit_mode == "update")
                        {
                            hostMessage = stockClient.UpdateSalePurchase(Shared.uid, purchase_edited.Id.ToString(), type: "purchase", purchase_edited.Product, purchase_edited.Quantity.ToString(), purchase_edited.TotalPrice.ToString(), purchase_edited.Date.ToString(), purchase_edited.Location, purchase_edited.Username);
                            if (hostMessage == "Unauthorized user!")
                            {
                                Shared.Logout(); // stop on unauthorized user
                                IsEnabled = false;
                                Close();
                                return;
                            }
                            else if (hostMessage != "Sale/Purchase successfully updated!")
                            {
                                MessageBox.Show(hostMessage + " Field was not updated.", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                                // restore old value // TODO: restore cell value? 
                                purchase_edited = purchase_edited0;
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.ToString().Contains("XXXXX"))
                        {
                            MessageBox.Show("This will be a specific error.", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        else
                        {
                            MessageBox.Show("An error occured, with the following details:\n" + ex.ToString(), caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }


                    if (edit_mode == "insert")
                    {
                        lastProduct = purchase_edited.Product; // save last data to suggest them for next record
                        lastQuantity = purchase_edited.Quantity;
                        lastTotalPrice = purchase_edited.TotalPrice;
                        lastDate = purchase_edited.Date;
                        lastUsername = purchase_edited.Username;
                        lastLocation = purchase_edited.Location;
                        lastUsername = purchase_edited.Username;

                         

                        // set background color of added purchase to green
                        for (int i = 0; i < dataGrid1.Columns.Count; i++)
                        {
                            cell = dataGrid1.Columns[i].GetCellContent(row).Parent as DataGridCell;
                            cell.Background = Brushes.OliveDrab;
                        }
                        TextBlock_message.Text = $"The purchase of id '{purchase_edited.Id}' has been added.";
                        Array.Clear(fieldsEntered, 0, fieldsEntered.Length);
                        Log("insert"); // write log to file
                        edit_mode = "read";
                        dataGrid1.CanUserSortColumns = true;
                        dataGrid1.IsReadOnly = true;
                        dataGrid1.Dispatcher.InvokeAsync(() => {
                            Button_AddPurchase.Focus(); // set focus to allow repeatedly add purchase on pressing the Add purchase button
                        },
                        DispatcherPriority.Loaded);

                    }
                    else if (edit_mode == "update")
                    {
                        TextBlock_message.Text = $"The purchase of id '{purchase_edited.Id}' has been updated with {(changed_property_name == "TotalPrice"? "Total price" : changed_property_name)}.";
                        Log("update"); // write log to file
                        cell.Background = Brushes.OliveDrab;
                        // Shared.ChangeColor(cell, Colors.OliveDrab, Colors.Transparent);
                        MoveToNextCell();
                    }
                    old_value = new_value; // update old_value after successful update
                    TextBlock_message.Foreground = Brushes.LightGreen;


                    checkBox_fadeInOut.IsChecked = false;
                    checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                    gifImage.StartAnimation();
                }
                else // move to next cell
                {
                    dataGrid1.Focus();
                    dataGrid1.Dispatcher.InvokeAsync(() => {

                        cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));

                        // select next unchanged column; if last 'UserId' column is reached, return to first (product) 'Name' column
                        int column_shift = 0;
                        while (fieldsEntered[column_index + column_shift - 1] != 0)
                        {
                            column_shift = column_index + column_shift == 6 ? -column_index + 1 : column_shift + 1;
                        }
                        cell = dataGrid1.Columns[column_index + column_shift].GetCellContent(row).Parent as DataGridCell;

                        // turn off eventual editing mode caused e.g. by tab key on data entry
                        if (cell.IsEditing) { cell.IsEditing = false; }

                        // cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                        // cell.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                        Button_AddPurchase.Focus();

                        SelectTextBox();



                    },
                DispatcherPriority.Loaded); // style the id cell of the new purchase
                }
            }
            else
            {
                return;
            }
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
                    // MoveToNextCell();
                }
        }

        private void MoveToNextCell()
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

                // turn off eventual editing mode causes e.g. by tab key on data entry
                // if (cell.IsEditing) { cell.IsEditing = false; }


                // go into edit mode if in insert mode
                cell.Focus(); // set focus on cell
                if (edit_mode == "insert") // TODO: tesztelni!
                {
                    SelectTextBox();
                }

                if (edit_mode == "update") dataGrid1.SelectedCells.Clear(); // TODO: tesztelni!
                SelectEditedCell();
            },
            DispatcherPriority.Loaded);
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

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (e.Key == Key.Enter)
            {
                return;
            }
            */
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
            filteredPurchasesList = new List<StockService.SalePurchase>();

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

        private void dataGrid0_KeyUp(object sender, KeyEventArgs e)
        // private void dataGrid0_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (row == null || column == null) { return; } // stop if column or row is not selected or not in edit mode

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

            // row = e.Row;
            // column = e.Column;
            filterc_index = column.DisplayIndex;
            cell = dataGrid1.Columns[filterc_index].GetCellContent(row).Parent as DataGridCell;
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
            if (changed_property_name == "Product name") { changed_property_name = "Product"; }
            if (changed_property_name == "Quant.") { changed_property_name = "Quantity"; }
            if (changed_property_name == "Total price") { changed_property_name = "TotalPrice"; }
            if (changed_property_name == "User name") { changed_property_name = "Username"; }

            // remove operator for integer columns Id and TotalPrice
            if (changed_property_name == "Id" || changed_property_name == "Quantity" || changed_property_name == "TotalPrice" || changed_property_name == "Date")
            {
                if (op != "=" || (new_value != "" && new_value.ToString().Substring(0, 1) == "=")) { new_value = new_value.Substring(op.Length); } // remove entered operator

                switch (changed_property_name)
                {
                    case "Id": opId = op; break;
                    case "Quantity": opQuantity = op; break;
                    case "TotalPrice": opTotalPrice = op; break;
                    case "Date": opDate = op; break;
                    default: break;
                }
            }

            if (changed_property_name == "Date" && ((new_value.Length < 8 && new_value.Length > 0) || (new_value.Length > 8 && new_value.Length < 14))) { return; } // stop if date length is < 8 OR when time is edited (a value is deleted), otherwise purchase_filter.Date will be set to null

            // if any purchase_filter value is null, set it temporarily to -999 to avoid error when setting old value 
            if (changed_property_name == "Id" && purchase_filter.Id == null) purchase_filter.Id = -999;
            if (changed_property_name == "Quantity" && purchase_filter.Quantity == null) purchase_filter.Quantity = -999;
            if (changed_property_name == "TotalPrice" && purchase_filter.TotalPrice == null) purchase_filter.TotalPrice = -999;
            if (changed_property_name == "Date" && purchase_filter.Date == null) purchase_filter.Date = DateTime.Parse("01.01.01 01:01:01");

            //get old property value of purchase by property name
            // https://stackoverflow.com/questions/1196991/get-property-value-from-string-using-reflection
            old_value = purchase_filter.GetType().GetProperty(changed_property_name).GetValue(purchase_filter).ToString();
            if (changed_property_name == "Id" && purchase_filter.Id == -999) purchase_filter.Id = null;
            if (changed_property_name == "Quantity" && purchase_filter.Quantity == -999) purchase_filter.Quantity = null;
            if (changed_property_name == "TotalPrice" && purchase_filter.TotalPrice == -999) purchase_filter.TotalPrice = null;
            if (changed_property_name == "Date" && purchase_filter.Date == DateTime.Parse("01.01.01 01:01:01")) purchase_filter.Date = null;

            string stopMessage = "";
            if (old_value == "-999" || op != "=")
            {
                Dispatcher.InvokeAsync(() => {
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
            else if (new_value != "" && changed_property_name == "Quantity") // if wrong Active value is entered
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                if (int_val == null || (int_val < 0))
                {
                    stopMessage = $"Please enter a correct value for the Quantity!";
                }
                else if (int_val > 1000000)
                {
                    stopMessage = $"Quantity cannot exceed 1,000,000!";
                }
            }
            else if (new_value != "" && changed_property_name == "TotalPrice") // if wrong Active value is entered
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                if (int_val == null || (int_val < 0))
                {
                    stopMessage = $"Please enter a correct value for the Total price!";
                }
                else if (int_val > 1000000000)
                {
                    stopMessage = $"Total price cannot exceed 1,000,000,000!";
                }
            }
            else if (new_value != "" && changed_property_name == "Date") // if wrong Date value is entered
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

            if (filterc_index == 1 || filterc_index == 5 || filterc_index == 6) // // update string-type fields with new value ( Product (name),  Location, Username)
            {
                purchase_filter.GetType().GetProperty(changed_property_name).SetValue(purchase_filter, new_value);
            }
            else if (filterc_index == 4) // update Date field with new value
            {
                DateTime? int_val = DateTime.TryParse(new_value, out var tempVal) ? tempVal : (DateTime?)null;
                purchase_filter.GetType().GetProperty(changed_property_name).SetValue(purchase_filter, int_val);
            }
            else // update int?-type fields with new value (Quantity, TotalPrice)
            {
                int? int_val = Int32.TryParse(new_value, out var tempVal) ? tempVal : (int?)null;
                purchase_filter.GetType().GetProperty(changed_property_name).SetValue(purchase_filter, int_val);

            }
            
            // filter
            filteredPurchasesList.Clear();
            foreach (var purchase in dbPurchasesList)
            {
                if ((purchase_filter.Id == null || Compare(purchase.Id, purchase_filter.Id, opId)) && (purchase_filter.Product == "" || purchase.Product.ToLower().Contains(purchase_filter.Product.ToLower())) && (purchase_filter.Quantity == null || Compare(purchase.Quantity, purchase_filter.Quantity, opQuantity)) && (purchase_filter.TotalPrice == null || Compare(purchase.TotalPrice, purchase_filter.TotalPrice, opTotalPrice)) && (purchase_filter.Date == null || (minutesExist ? Compare(purchase.Date, purchase_filter.Date, opDate) : Compare(purchase.Date.Value.Date, purchase_filter.Date, opDate))) && (purchase_filter.Location == "" || purchase.Location.ToLower().Contains(purchase_filter.Location.ToLower())) && (purchase_filter.Username == "" || purchase.Username.ToLower().Contains(purchase_filter.Username.ToLower())))
                {
                    filteredPurchasesList.Add(purchase);
                    continue;
                }
            }
            // update dataGrid1 with filtered items                    
            dataGrid1.ItemsSource = filteredPurchasesList;
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
                Button_DeletePurchase.IsEnabled = false;
                Button_DeletePurchase.Foreground = Brushes.Gray;
                Button_DeletePurchase.ToolTip = "You do not have rights to delete data!";
            }
            if (Shared.loggedInUser.Permission < 3)
            {
                Button_AddPurchase.IsEnabled = false;
                Button_AddPurchase.Foreground = Brushes.Gray;
                Button_AddPurchase.ToolTip = "You do not have rights to add data!";
                Button_UpdatePurchase.IsEnabled = false;
                Button_UpdatePurchase.Foreground = Brushes.Gray;
                Button_UpdatePurchase.ToolTip = "You do not have rights to update data!";
                Button_Import.IsEnabled = false;
                Button_Import.Foreground = Brushes.Gray;
                Button_Import.ToolTip = "You do not have rights to update data!";
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
                    // endDate = endDate.ToString().Substring(10, 8) == "00:00:00" ? endDate.Date.AddDays(1).AddMinutes(-1) : endDate;
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

        private void Button_Export_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Comma separated text file (*.csv)|*.csv|Text file (*.txt)|*.txt",
                DefaultExt = ".csv",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = "dbPurchases",
                Title = "Save purchases data as:"
            };
            
            Nullable<bool> result = saveFileDialog.ShowDialog(); // show saveFileDialog
            if (result == true)
            {
                // create file content
                StreamWriter sr = new StreamWriter(saveFileDialog.FileName, append: false, encoding: Encoding.UTF8);
                // write file header line
                string header_row = "Id;Product;Quantity;TotalPrice;Date;Location;Username";
                sr.WriteLine(header_row);

                // write file rows
                string rows = "";
                StockService.SalePurchase purchase;
                int i = 0;
                for (i = 0; i < dataGrid1.Items.Count; i++)
                {
                    purchase = dataGrid1.Items[i] as StockService.SalePurchase;
                    rows += $"{purchase.Id};{purchase.Product};{purchase.Quantity};{purchase.TotalPrice};{purchase.Date.ToString().Substring(0, purchase.Date.ToString().Length -3)};{purchase.Location};{purchase.Username}\n";
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
                Title = "Open file for import to 'Purchases' table"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                if (openFileDialog.FileName.ToLower().Contains("sale"))
                {
                    MessageBoxResult result = MessageBox.Show($"Are you sure to import Purchase data from the file '{openFileDialog.FileName}'?", caption: "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    bool encrypt = result == MessageBoxResult.Yes;
                    if (result == MessageBoxResult.No) { return; }
                }

                StreamReader sr = new StreamReader(openFileDialog.FileName);
                // check header correctness
                string header_row = sr.ReadLine();
                int first_colum = header_row.Split(';').Length == 6 ? 0 : 1; // 1 if Id column is provided

                if (header_row != "Id;Product;Quantity;TotalPrice;Date;Location;Username" && header_row != "Product;Quantity;TotalPrice;Date;Location;Username")
                {
                    MessageBox.Show($"Incorrect file content! Expected header is 'Id;Product;Quantity;TotalPrice;Date;Location;Username' (Id is optional), but received '{header_row}'", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                StockService.SalePurchase purchase;
                importList = new List<StockService.SalePurchase>();
                int row_index = 0;
                string[] row;
                int purchasesAdded = 0;
                string hostMessage = "";
                string errorMessage = "";
                int? id = dbPurchasesList.Max(u => u.Id) + 1;
                dbLocationsList = Location.GetLocations("", "", "", "");
                if (dbLocationsList == null) { IsEnabled = false; Close(); return; } // stop on any error
                dbProductsList = Product.GetProducts("", "", "", "", "", "", "");
                dbUsersList = User.GetUsers("", "", "", "", "", "", "", "");
                while (sr.EndOfStream == false)
                {
                    row_index++;
                    string error = "";
                    row = sr.ReadLine().Split(';');
                    if (row.Length != 6 + first_colum) // skip row if number of columns is incorrect
                    {
                        errorMessage += $"Purchase in line {row_index}: The required {6 + first_colum} fields are not available!\n";
                        continue;
                    }

                    string product = row[first_colum];
                    string quantity = row[first_colum + 1];
                    string totalPrice = row[first_colum + 2];
                    string date = row[first_colum + 3];
                    string location = row[first_colum + 4];
                    string username = row[first_colum + 5];

                    // check data correctness
                    if (product.Length < 5) // if wrong (product) Name value is entered
                    {
                        error += $"Purchase in line {row_index}: Product name must be at least 5 characters!\n";
                    }
                    if (dbProductsList.Any(p => p.Name == product) == false)
                    {
                        error += $"Purchase in line {row_index}: product '{product}' does not exist!\n";
                    }
                    int? int_val = Int32.TryParse(quantity, out var tempVal) ? tempVal : (int?)null;
                    if (int_val == null || (int_val <= 0))
                    {
                        error += $"Purchase in line {row_index}: Quantity '{quantity}' is incorrect!\n";
                    }
                    else if (int_val > 1000000000)
                    {
                        error += $"Purchase in line {row_index}: Quantity '{quantity}' cannot exceed 1,000,000,000!\n";
                    }
                    int_val = Int32.TryParse(totalPrice, out var tempVal1) ? tempVal1 : (int?)null;
                    if (int_val == null || (int_val < 0))
                    {
                        error += $"Purchase in line {row_index}: Total price '{quantity}' is incorrect!\n";
                    }
                    else if (int_val > 1000000000)
                    {
                        error += $"Purchase in line {row_index}: Total price '{quantity}' cannot exceed 1,000,000,000!\n";
                    }
                    bool dateExists = DateTime.TryParse(date, out DateTime date_entered);
                    if (dateExists == false)
                    {
                        error += $"Purchase in line {row_index}: Date value '{date}' is incorrect!\n";
                    }
                    else if (date_entered > DateTime.Now)
                    {
                        error += $"Purchase in line {row_index}: Date '{date}' cannot be a future date!\n";
                    }
                    else if ((DateTime.Now - date_entered).TotalDays > 3650)
                    {
                        error += $"Purchase in line {row_index}: Date '{date}' cannot be earlier than 3650 days!\n";
                    }
                    if (dbLocationsList.Any(p => p.Name == location) == false) // if wrong Location name is entered
                    {
                        error += $"Purchase in line {row_index}: Location '{location}' does not exist!\n";
                    }
                    if (username.Length < 5)
                    {
                        error += $"Purchase in line {row_index}: Username '{username}' must be at least 5 charachters long!\n";
                    }
                    if (dbUsersList.Any(p => p.Username == username ) == false) // if user does not exist in database
                    {
                        error += $"Purchase in line {row_index + 1}: User '{username}' does not exist!\n";
                    }
                    
                    errorMessage += error;
                    if (error != "") { continue; } // skip on error

                    // ADD into database
                    hostMessage = stockClient.AddSalePurchase(Shared.uid, type: "purchase", product, quantity, location, totalPrice, date);
                    if (hostMessage == "Unauthorized user!")
                    {
                        Shared.Logout(); // stop on unauthorized user
                        IsEnabled = false;
                        Close();
                        return;
                    }
                    else if (hostMessage.Contains("FOREIGN KEY (`productId`)"))
                    {
                        errorMessage += $"Purchase in line'{column_index +1}': {hostMessage}\n";
                        continue;
                    }
                    
                    purchase = new StockService.SalePurchase
                    {
                        Id = id,
                        Product = product,
                        Quantity = int.Parse(quantity),
                        TotalPrice = int.Parse(totalPrice),
                        Date = DateTime.Parse(date),
                        Location = location,
                        Username = username
                    };
                    purchasesAdded++;
                    importList.Add(purchase);
                    Log("insert"); // write log to file
                    id++;
                }
                sr.Close();

                if (errorMessage != "") { MessageBox.Show($"Following error occurred during the data import:\n\n{errorMessage}", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); }
                if (importList.Count > 0)
                {
                    dataGrid1.ItemsSource = importList;

                    TextBlock_message.Text = $"{purchasesAdded} {(purchasesAdded == 1 ? "record" : "records")} added into the 'Purchases' table.";
                    TextBlock_message.Foreground = Brushes.LightGreen;
                    checkBox_fadeInOut.IsChecked = false;
                    checkBox_fadeInOut.IsChecked = true; // fade in-out gifImage, fade out TextBlock_message.Text
                    gifImage.StartAnimation();
                }
            }
        }

        private void window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double stretch = Math.Max((borderLeft.ActualWidth - 10 - 110) / (550 - 10 - 43), 0.8); // (BorderLeft width - left margin - more due to Id and Quantity column) / original borderLeft
            dataGrid1.Width = window.ActualWidth - 250 - 10; // expand dataGrid1 with to panel width (-ColumnDefinition2 width - stackPanel left margin)
            dataGrid0.Width = dataGrid1.Width;

            stackPanel1.Height = 442 + window.ActualHeight - 500 - stackPanel1.Margin.Top + 45; // original window.Heigth

            // stretch columns to dataGrid1 width
            for (int i = 0; i < dataGrid1.Columns.Count; i++)
            {
                dataGrid1.Columns[i].Width = dataGrid1.Columns[i].MinWidth * ((stretch - 1) * (i == 0 || i == 2 ? 0.5 : 1) + 1); // resize Id and Quantity row only by 50%
                dataGrid0.Columns[i].Width = dataGrid1.Columns[i].Width;
            }
            dataGrid1.FontSize = 14 * Math.Max(stretch, 0.9);
            dataGrid1.Columns[2].Header = stretch < 1.18 ? "Quantity" : "Quant.";
        }

        private void Log(string operation)
        {
            string row = "";
            // save operation into log file
            StreamWriter sr = new StreamWriter(@".\Logs\managePurchases.log", append: true, encoding: Encoding.UTF8);
            // write file header line
            // string header_row = "Date;Username;Operation;Id;Product;Quantity;TotalPrice;Date;Location;Username;";
            // sr.WriteLine(header_row);

            // write file rows
            StockService.SalePurchase purchase;
            purchase = purchase_edited;

            if (operation == "update") // in update mode add the old value in a new line
            {
                int index = column_index;
                row = $"{DateTime.Now.ToString("yy.MM.dd HH:mm:ss")};{Shared.loggedInUser.Username};{operation};{purchase.Id};{(column_index == 1 ? old_value : null)};{(column_index == 2 ? old_value : null)};{(column_index == 3 ? old_value : null)};{(column_index == 4 ? old_value.Substring(0, purchase.Date.ToString().Length - 3) : null)};{(column_index == 5 ? old_value : null)};{(column_index == 6 ? old_value : null)}\n";
            }
            DateTime date = purchase.Date == null ? DateTime.Now : (DateTime)purchase.Date;
            row += $"{DateTime.Now.ToString("yy.MM.dd HH:mm:ss")};{Shared.loggedInUser.Username};{operation};{purchase.Id};{purchase.Product};{purchase.Quantity};{purchase.TotalPrice};{(purchase.Date == null? null : date.ToString("yy.MM.dd HH:mm: ss"))};{purchase.Location};{purchase.Username}";
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

        Logs.LogWindowPurchases LogWindowPurchases;
        private void Button_LogWindow_Click(object sender, RoutedEventArgs e)
        {
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(LogWindowPurchases))
            {
                LogWindowPurchases = new Logs.LogWindowPurchases();
                if (LogWindowPurchases.IsEnabled) LogWindowPurchases.Show();
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


