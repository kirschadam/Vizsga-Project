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
using FrontendWPF.Classes;



namespace FrontendWPF
{
    /// <summary>
    /// Interaction logic for StartWindow_with_pinPanels.xaml
    /// </summary>
    public partial class StartWindow_with_pinPanels : Window
    {
        // Dummy columns for layers 0 and 1:
        ColumnDefinition colOneCopyForLayer0;
        ColumnDefinition colTwoCopyForLayer0;
        ColumnDefinition colTwoCopyForLayer1;

        public StartWindow_with_pinPanels()
        {
            InitializeComponent();

            // Initialize the dummy (grouped) columns that are created during docking:
            colOneCopyForLayer0 = new ColumnDefinition();
            colOneCopyForLayer0.SharedSizeGroup = "column1";
            colTwoCopyForLayer0 = new ColumnDefinition();
            colTwoCopyForLayer0.SharedSizeGroup = "column2";
            colTwoCopyForLayer1 = new ColumnDefinition();
            colTwoCopyForLayer1.SharedSizeGroup = "column2";

            button_login.Foreground = Brushes.Red;
            button_login.FontWeight = FontWeights.Bold;
            button_ManageUsersWindow.IsEnabled = false;

        }

        // Toggle panel 1 between docked and undocked states
        public void panel1Pin_Click(object sender, RoutedEventArgs e)
        {
            if (button_panel1.Visibility == Visibility.Collapsed)
                UndockPane(1);
            else
                DockPane(1);
        }

        // Toggle panel 2 between docked and undocked states 
        public void panel2Pin_Click(object sender, RoutedEventArgs e)
        {
            if (button_panel2.Visibility == Visibility.Collapsed)
                UndockPane(2);
            else
                DockPane(2);
        }

        // Make panel 1 visible when hovering over its button
        public void button_panel1_MouseEnter(object sender, RoutedEventArgs e)
        {
            gridlayer1.Visibility = Visibility.Visible;

            // Adjust ZIndex order to ensure the pane is on top:
            parentGrid.Children.Remove(gridlayer1);
            parentGrid.Children.Add(gridlayer1);

            // Ensure the other pane is hidden if it is undocked
            if (button_panel2.Visibility == Visibility.Visible)
                gridlayer2.Visibility = Visibility.Collapsed;
        }

        // Make panel 2 visible when hovering over its button
        public void button_panel2_MouseEnter(object sender, RoutedEventArgs e)
        {
            gridlayer2.Visibility = Visibility.Visible;

            // Adjust ZIndex order to ensure the pane is on top:
            parentGrid.Children.Remove(gridlayer2);
            parentGrid.Children.Add(gridlayer2);

            // Ensure the other pane is hidden if it is undocked
            if (button_panel1.Visibility == Visibility.Visible)
                gridlayer1.Visibility = Visibility.Collapsed;
        }

        // Hide any undocked panes when the mouse enters Layer 0
        public void layer0_MouseEnter(object sender, RoutedEventArgs e)
        {
            if (button_panel1.Visibility == Visibility.Visible)
                gridlayer1.Visibility = Visibility.Collapsed;
            if (button_panel2.Visibility == Visibility.Visible)
                gridlayer2.Visibility = Visibility.Collapsed;
        }

        // Hide the other pane if undocked when the mouse enters Panel 1
        public void panel1_MouseEnter(object sender, RoutedEventArgs e)
        {
            // Ensure the other pane is hidden if it is undocked
            if (button_panel2.Visibility == Visibility.Visible)
                gridlayer2.Visibility = Visibility.Collapsed;
        }

        // Hide the other pane if undocked when the mouse enters Panel 2
        public void panel2_MouseEnter(object sender, RoutedEventArgs e)
        {
            // Ensure the other pane is hidden if it is undocked
            if (button_panel1.Visibility == Visibility.Visible)
                gridlayer1.Visibility = Visibility.Collapsed;
        }

        // Docks a pane and hides its button
        public void DockPane(int paneNumber)
        {
            if (paneNumber == 1)
            {
                button_panel1.Visibility = Visibility.Collapsed;
                panel1PinImg.Source = new BitmapImage(new Uri("/Resources/Images/PinVer1col.png", UriKind.Relative));

                // Add the cloned column to layer 0:
                layer0.ColumnDefinitions.Add(colOneCopyForLayer0);
                // Add the cloned column to layer 1, but only if pane 2 is docked:
                if (button_panel2.Visibility == Visibility.Collapsed) gridlayer1.ColumnDefinitions.Add(colTwoCopyForLayer1);
            }
            else if (paneNumber == 2)
            {
                button_panel2.Visibility = Visibility.Collapsed;
                panel2PinImg.Source = new BitmapImage(new Uri("/Resources/Images/PinVer1col.png", UriKind.Relative));

                // Add the cloned column to layer 0:
                layer0.ColumnDefinitions.Add(colTwoCopyForLayer0);
                // Add the cloned column to layer 1, but only if pane 1 is docked:
                if (button_panel1.Visibility == Visibility.Collapsed) gridlayer1.ColumnDefinitions.Add(colTwoCopyForLayer1);
            }
        }

        // Undocks a pane, which reveals the corresponding pane button
        public void UndockPane(int panelNbr)
        {
            if (panelNbr == 1)
            {
                gridlayer1.Visibility = Visibility.Collapsed;
                button_panel1.Visibility = Visibility.Visible;
                panel1PinImg.Source = new BitmapImage(new Uri("/Resources/Images/PinHor1col.png", UriKind.Relative));

                // Remove the cloned columns from layers 0 and 1:
                layer0.ColumnDefinitions.Remove(colOneCopyForLayer0);
                // This won't always be present, but Remove silently ignores bad columns:
                gridlayer1.ColumnDefinitions.Remove(colTwoCopyForLayer1);
            }
            else if (panelNbr == 2)
            {
                gridlayer2.Visibility = Visibility.Collapsed;
                button_panel2.Visibility = Visibility.Visible;
                panel2PinImg.Source = new BitmapImage(new Uri("/Resources/Images/PinHor1col.png", UriKind.Relative));

                // Remove the cloned columns from layers 0 and 1:
                layer0.ColumnDefinitions.Remove(colTwoCopyForLayer0);
                gridlayer1.ColumnDefinitions.Remove(colTwoCopyForLayer1);
            }
        }

        private void MenuItem_exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private LoginWindow loginWindow = new LoginWindow();
        private void Button_login_Click(object sender, RoutedEventArgs e)
        {
            // open loginwindow if it logRegWindowsClosed and user not logged in    
            if (loginWindow.IsLoaded == false && button_login.Content.ToString() == "LOGIN")
            {
                loginWindow = new LoginWindow();
                loginWindow.Show();
            }
            else // LOGOUT
            {
                button_login.Content = "LOGIN";
                button_login.Foreground = Brushes.Red;
                Shared.loggedInUser = null;
                Shared.loggedIn = false;
                button_ManageUsersWindow.IsEnabled = false;

                ServiceReference3.UserServiceClient client = new ServiceReference3.UserServiceClient();
                
                try
                {
                    string responseLogout = client.LogoutUser(Shared.uid);

                    string errorMessage = "";
                    /*
                    if (responseLogout == null)
                    {
                        errorMessage = $"The remote host is not accessible. Please check your Internet connection, or contact the service provider.";
                    }
                    */
                    if (responseLogout.Contains("You have logged out!"))
                    {
                        MessageBox.Show("You have logged out.");
                    }
                    else
                    {
                        errorMessage = responseLogout;
                    }

                    if (errorMessage != "")
                    {
                        MessageBox.Show(errorMessage, caption: "Error message");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("no endpoint listening") || ex.ToString().Contains("EndpointNotFoundException"))
                    {
                        MessageBox.Show("The remote host is not accessible. Please check your Internet connection, or contact the service provider.", caption: "Error message");
                        return;
                    }
                    else
                    {
                        MessageBox.Show($"Logout failed! The details of the error are the following:\n{ex.Message}", caption: "Error message");
                        return;
                    }

                }

            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuItem_addElementProgrammatically_Click(object sender, RoutedEventArgs e)
        {
            
            Templates.Add_Element_Programmatically add_Element_Programmatically = new Templates.Add_Element_Programmatically();
            add_Element_Programmatically.Show();
        }


        private void MenuItem_dataBinding_ListOfClass_Click(object sender, RoutedEventArgs e)
        {
            Templates.DataBinding_ListOfClass dataBinding_ListOfClass = new Templates.DataBinding_ListOfClass();
            dataBinding_ListOfClass.Show();
        }

        private void MenuItem_dataBinding_ListOfUsers_Click(object sender, RoutedEventArgs e)
        {
            Templates.DataBinding_ListOfUsers dataBinding_ListOfUsers = new Templates.DataBinding_ListOfUsers();
            dataBinding_ListOfUsers.Show();
        }

        private void MenuItem_dataBinding_ObservableCollection_Click(object sender, RoutedEventArgs e)
        {
            Templates.DataBinding_ObservableCollection dataBinding_ObservableCollection = new Templates.DataBinding_ObservableCollection();
            dataBinding_ObservableCollection.Show();
        }
        
        private void MenuItem_dataBinding_ObservableCollection_Users_Click(object sender, RoutedEventArgs e)
        {
            Templates.DataBinding_ObservableCollection_Users dataBinding_ObservableCollection_Users = new Templates.DataBinding_ObservableCollection_Users();
            dataBinding_ObservableCollection_Users.Show();
        }

        private void MenuItem_image_FadeInOut_Click(object sender, RoutedEventArgs e)
        {
            Templates.Image_FadeInOut image_FadeInOut = new Templates.Image_FadeInOut();
            image_FadeInOut.Show();
        }

        private void MenuItem_layoutGridRowDefinitions_Click(object sender, RoutedEventArgs e)
        {
            Templates.Layout_GridRowDefinitions layout_GridRowDefinitions = new Templates.Layout_GridRowDefinitions();
            layout_GridRowDefinitions.Show();
        }

        private void MenuItem_layoutRotateTransform_Click(object sender, RoutedEventArgs e)
        {
            Templates.Layout_RotateTransform layout_RotateTransform = new Templates.Layout_RotateTransform();
            layout_RotateTransform.Show();
        }

        private void MenuItem_layoutScaleTransform_Click(object sender, RoutedEventArgs e)
        {
            Templates.Layout_ScaleTransform layout_ScaleTransform = new Templates.Layout_ScaleTransform();
            layout_ScaleTransform.Show();
        }

        private void MenuItem_layoutScrollViewer_ViewBox_Click(object sender, RoutedEventArgs e)
        {
            Templates.Layout_ScrollViewer_ViewBox layout_ScrollViewer_ViewBox = new Templates.Layout_ScrollViewer_ViewBox();
            layout_ScrollViewer_ViewBox.Show();
        }

        private void MenuItem_layoutSkewTransform_Click(object sender, RoutedEventArgs e)
        {
            Templates.Layout_SkewTransform layout_SkewTransform = new Templates.Layout_SkewTransform();
            layout_SkewTransform.Show();
        }

        private void MenuItem_layoutStackPanel_DockPanel_Click(object sender, RoutedEventArgs e)
        {
            Templates.Layout_StackPanel_DockPanel layout_StackPanel_DockPanel = new Templates.Layout_StackPanel_DockPanel();
            layout_StackPanel_DockPanel.Show();
        }

        private void MenuItem_PropertyElement_Click(object sender, RoutedEventArgs e)
        {
            Templates.Property_Element property_Element = new Templates.Property_Element();
            property_Element.Show();
        }

        private void MenuItem_WindowFadeInFadeOut_Click(object sender, RoutedEventArgs e)
        {
            Templates.Window_FadeIn_FadeOut window_FadeIn_FadeOut = new Templates.Window_FadeIn_FadeOut();
            window_FadeIn_FadeOut.Show();
        }

        private void MenuItem_ItemsControl_Tab_Click(object sender, RoutedEventArgs e)
        {
            Templates.ItemsControl_Tab itemsControl_Tab = new Templates.ItemsControl_Tab();
            itemsControl_Tab.Show();
        }

        private void MenuItem_ItemsControl_TreeView_Click(object sender, RoutedEventArgs e)
        {
            Templates.ItemsCotrol_TreeView itemsCotrol_TreeView = new Templates.ItemsCotrol_TreeView();
            itemsCotrol_TreeView.Show();
        }

        private void button_viewUsers_Click(object sender, RoutedEventArgs e)
        {
            ViewUsers viewUsers = new ViewUsers();
            viewUsers.Show();
        }

        private void button_GridView_Users_Click(object sender, RoutedEventArgs e)
        {
            GridView_Users gridView_Users = new GridView_Users();
            if (gridView_Users.IsEnabled) gridView_Users.Show();  // show if not closed (if set to enabled before)
        }

        private void button_DataGrid_Users_Click(object sender, RoutedEventArgs e)
        {
            DataGrid_Users dataGrid_Users = new DataGrid_Users();
            if (dataGrid_Users.IsEnabled) dataGrid_Users.Show();  // show if not closed ( if set to enabled before)
        }

        ManageUsersWindow ManageUsersWindow;
        private void button_ManageUsersWindow_Click(object sender, RoutedEventArgs e)
        {
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(ManageUsersWindow))
            {
                ManageUsersWindow = new ManageUsersWindow();
                if (ManageUsersWindow.IsEnabled) ManageUsersWindow.Show();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            bool loaded = this.IsLoaded;
            this.Close();

            WindowCollection collection = Application.Current.Windows;
            foreach (Window window in collection)
            {
                if (window.IsLoaded) window.Close();
            }
            Environment.Exit(0);
            // Application.Current.Shutdown();
        }


    }
}
