using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using FrontendWPF.Classes;
using Microsoft.Win32;

namespace FrontendWPF
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        // Dummy columns for layers 0 and 1:
        ColumnDefinition colOneCopyForLayer0;
        ColumnDefinition colTwoCopyForLayer0;
        ColumnDefinition colTwoCopyForLayer1;
        private bool closeCompleted = false;

        private List<BitmapImage> imagesList = new List<BitmapImage>();
        private int imageNumber = 0;
        public DispatcherTimer pictureTimer = new DispatcherTimer();

        public StartWindow()
        {
            InitializeComponent();

            Shared.screenWidth = System.Windows.SystemParameters.WorkArea.Width;
            Shared.screenHeight = System.Windows.SystemParameters.WorkArea.Height;

            // Initialize the dummy (grouped) columns that are created during docking:
            colOneCopyForLayer0 = new ColumnDefinition();
            colOneCopyForLayer0.SharedSizeGroup = "column1";
            colTwoCopyForLayer0 = new ColumnDefinition();
            colTwoCopyForLayer0.SharedSizeGroup = "column2";
            colTwoCopyForLayer1 = new ColumnDefinition();
            colTwoCopyForLayer1.SharedSizeGroup = "column2";
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


        private LoginWindow loginWindow = new LoginWindow();
        private void Button_login_Click(object sender, RoutedEventArgs e)
        {
            // open loginwindow if it logRegWindowsClosed and user not logged in    
            if (loginWindow.IsLoaded == false && button_login.Content.ToString() == "LOGIN")
            {
                loginWindow = new LoginWindow();
                loginWindow.Show();
                pictureTimer.Stop();
            }
            else // LOGOUT
            {
                button_login.Content = "LOGIN";
                button_login.Foreground = Brushes.LightSalmon;
                Shared.loggedInUser = null;
                Shared.loggedIn = false;
                button_ManageUsersWindow.IsEnabled = false;
                button_ManageUsersWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF9D9D9D");
                button_ManageProductsWindow.IsEnabled = false;
                button_ManageProductsWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF989898");
                button_ManagePurchasesWindow.IsEnabled = false;
                button_ManagePurchasesWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF989898");
                button_ManageSalesWindow.IsEnabled = false;
                button_ManageSalesWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF8C8C8C");
                button_ManageStocksWindow.IsEnabled = false;
                button_ManageStocksWindow.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF868686");
                button_ManageLocationsWindow.IsEnabled = false;
                button_ManageLocationsWindow.Foreground = Brushes.Gray;
                button_Layouts.IsEnabled = false;
                button_Layouts.Foreground = Brushes.Gray;

                if (gridLayout.Visibility == Visibility.Visible)
                {
                    gridLayout.Visibility = Visibility.Hidden;
                    button_Layouts.Content = "Show layouts";
                    imageBackground.Visibility = Visibility.Visible;
                }

                pictureTimer.Start();


                UserService.UserServiceClient client = new UserService.UserServiceClient();
                
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
                        MessageBox.Show("You have logged out.", caption: "Information", button: MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void MenuItem_ViewUsers_Click(object sender, RoutedEventArgs e)
        {
            Templates.ViewUsers viewUsers = new Templates.ViewUsers();
            viewUsers.Show();
        }

        private void MenuItem_GridView_Users_Click(object sender, RoutedEventArgs e)
        {
            Templates.GridView_Users gridView_Users = new Templates.GridView_Users();
            if (gridView_Users.IsEnabled) gridView_Users.Show();  // show if not closed (if set to enabled before)
        }

        private void MenuItem_DataGrid_Users_Click(object sender, RoutedEventArgs e)
        {
            Templates.DataGrid_Users dataGrid_Users = new Templates.DataGrid_Users();
            if (dataGrid_Users.IsEnabled) dataGrid_Users.Show();  // show if not closed (if set to enabled before)
        }




        ManageUsersWindow ManageUsersWindow;
        private void button_ManageUsersWindow_Click(object sender, RoutedEventArgs e)
        {
            Shared.layout = "";
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(ManageUsersWindow))
            {
                ManageUsersWindow = new ManageUsersWindow();
                if (ManageUsersWindow.IsEnabled) ManageUsersWindow.Show();
            }
        }

        ManageProductsWindow ManageProductsWindow;
        private void button_ManageProductsWindow_Click(object sender, RoutedEventArgs e)
        {
            Shared.layout = "";
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(ManageProductsWindow))
            {
                ManageProductsWindow = new ManageProductsWindow();
                if (ManageProductsWindow.IsEnabled) ManageProductsWindow.Show();
            }
        }

        ManagePurchasesWindow ManagePurchasesWindow;
        private void button_ManagePurchasesWindow_Click(object sender, RoutedEventArgs e)
        {
            Shared.layout = "";
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(ManagePurchasesWindow))
            {
                ManagePurchasesWindow = new ManagePurchasesWindow();
                if (ManagePurchasesWindow.IsEnabled) ManagePurchasesWindow.Show();
            }
        }

        ManageSalesWindow ManageSalesWindow;
        private void button_ManageSalesWindow_Click(object sender, RoutedEventArgs e)
        {
            Shared.layout = "";
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(ManageSalesWindow))
            {
                ManageSalesWindow = new ManageSalesWindow();
                if (ManageSalesWindow.IsEnabled) ManageSalesWindow.Show();
            }
        }

        ManageStocksWindow ManageStocksWindow;
        private void button_ManageStocksWindow_Click(object sender, RoutedEventArgs e)
        {
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(ManageStocksWindow))
            {
                ManageStocksWindow = new ManageStocksWindow();
                if (ManageStocksWindow.IsEnabled) ManageStocksWindow.Show();
            }
        }

        ManageLocationsWindow ManageLocationsWindow;
        private void button_ManageLocationsWindow_Click(object sender, RoutedEventArgs e)
        {
            Shared.layout = "";
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(ManageLocationsWindow))
            {
                ManageLocationsWindow = new ManageLocationsWindow();
                if (ManageLocationsWindow.IsEnabled) ManageLocationsWindow.Show();
            }
        }

        private void MenuItem_exit_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void button_Exit_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }


        private void CloseWindow()
        {
            button_Exit.IsEnabled = false;
            button_login.Content = "Exiting...";
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

        private void Window_Closed(object sender, EventArgs e)
        {
            bool loaded = this.IsLoaded;
            // this.Close();

            WindowCollection collection = Application.Current.Windows;
            foreach (Window window in collection)
            {
                if (window.IsLoaded) window.Close();
            }
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        private void Button_layoutProductsPurchases_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            Shared.layout = buttonName;
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(ManageProductsWindow))
            {
                ManageProductsWindow = new ManageProductsWindow();
                if (ManageProductsWindow.IsEnabled) ManageProductsWindow.Show();
            }
            if(buttonName.Contains("Purchases") && !Application.Current.Windows.OfType<Window>().Contains(ManagePurchasesWindow))
            {
                ManagePurchasesWindow = new ManagePurchasesWindow();
                if (ManagePurchasesWindow.IsEnabled) ManagePurchasesWindow.Show();
            }
            else if (buttonName.Contains("Sales") && !Application.Current.Windows.OfType<Window>().Contains(ManageSalesWindow))
            {
                ManageSalesWindow = new ManageSalesWindow();
                if (ManageSalesWindow.IsEnabled) ManageSalesWindow.Show();
            }
        }

        private void Button_layoutPurchasesSales_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            Shared.layout = buttonName;
            // show only if not open already (to avoid multiple instances)
            if (!Application.Current.Windows.OfType<Window>().Contains(ManagePurchasesWindow))
            {
                ManagePurchasesWindow = new ManagePurchasesWindow();
                if (ManagePurchasesWindow.IsEnabled) ManagePurchasesWindow.Show();
            }
            if (!Application.Current.Windows.OfType<Window>().Contains(ManageSalesWindow))
            {
                ManageSalesWindow = new ManageSalesWindow();
                if (ManageSalesWindow.IsEnabled) ManageSalesWindow.Show();
            }
        }

        private void button_Layouts_Click(object sender, RoutedEventArgs e)
        {
            if (gridLayout.Visibility == Visibility.Hidden)
            {
                gridLayout.Visibility = Visibility.Visible;
                button_Layouts.Content = "Hide layouts";
                imageBackground.Visibility = Visibility.Hidden;
            }
            else
            {
                gridLayout.Visibility = Visibility.Hidden;
                button_Layouts.Content = "Show layouts";
                imageBackground.Visibility = Visibility.Visible;
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // create list of images for slideshow
            // http://csharphelper.com/blog/2020/04/use-code-to-make-a-slideshow-in-c-and-wpf/
            DirectoryInfo dir_info = new DirectoryInfo(Environment.CurrentDirectory + "\\Resources\\Images\\Slide");
            foreach (FileInfo file_info in dir_info.GetFiles())
            {
                if ((file_info.Extension.ToLower() == ".jpg") ||
                    (file_info.Extension.ToLower() == ".png"))
                {
                    imagesList.Add(new BitmapImage(new Uri(file_info.FullName)));
                }
            }

            // Display the first image.
            imageBackground.Source = imagesList[0];

            // Install a timer to show each image.
            pictureTimer.Interval = TimeSpan.FromSeconds(5);
            pictureTimer.Tick += Tick;
            pictureTimer.Start();
        }

        // Display the next image.
        private void Tick(object sender, System.EventArgs e)
        {
            imageNumber = (imageNumber + 1) % imagesList.Count;
            ShowNextImage(imageBackground);
        }

        private void ShowNextImage(Image img)
        {
            const double transition_time = 0.9;
            Storyboard sb = new Storyboard();

            // ***************************
            // Animate Opacity 1.0 --> 0.0
            // ***************************
            DoubleAnimation fade_out = new DoubleAnimation(1.0, 0.0,
                TimeSpan.FromSeconds(transition_time));
            fade_out.BeginTime = TimeSpan.FromSeconds(0);

            // Use the Storyboard to set the target property.
            Storyboard.SetTarget(fade_out, img);
            Storyboard.SetTargetProperty(fade_out,
                new PropertyPath(Image.OpacityProperty));

            // Add the animation to the StoryBoard.
            sb.Children.Add(fade_out);


            // *********************************
            // Animate displaying the new image.
            // *********************************
            ObjectAnimationUsingKeyFrames new_image_animation =
                new ObjectAnimationUsingKeyFrames();
            // Start after the first animation has finisheed.
            new_image_animation.BeginTime = TimeSpan.FromSeconds(transition_time);

            // Add a key frame to the animation.
            // It should be at time 0 after the animation begins.
            DiscreteObjectKeyFrame new_image_frame =
                new DiscreteObjectKeyFrame(imagesList[imageNumber], TimeSpan.Zero);
            new_image_animation.KeyFrames.Add(new_image_frame);

            // Use the Storyboard to set the target property.
            Storyboard.SetTarget(new_image_animation, img);
            Storyboard.SetTargetProperty(new_image_animation,
                new PropertyPath(Image.SourceProperty));

            // Add the animation to the StoryBoard.
            sb.Children.Add(new_image_animation);


            // ***************************
            // Animate Opacity 0.0 --> 1.0
            // ***************************
            // Start when the first animation ends.
            DoubleAnimation fade_in = new DoubleAnimation(0.0, 1.0,
                TimeSpan.FromSeconds(transition_time));
            fade_in.BeginTime = TimeSpan.FromSeconds(transition_time);

            // Use the Storyboard to set the target property.
            Storyboard.SetTarget(fade_in, img);
            Storyboard.SetTargetProperty(fade_in,
                new PropertyPath(Image.OpacityProperty));

            // Add the animation to the StoryBoard.
            sb.Children.Add(fade_in);

            // Start the storyboard on the img control.
            sb.Begin(img);
        }

        private void button_OpenNotes_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Text file (*.txt)|*.txt|Rich text file (*.rtf) |*.rtf",
                DefaultExt = ".txt",
                Title = "Open notes file:"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                int filter = openFileDialog.FilterIndex;
                // https://docs.microsoft.com/en-us/dotnet/desktop/wpf/controls/how-to-save-load-and-print-richtextbox-content?view=netframeworkdesktop-4.8
                TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                FileStream fStream = new FileStream(openFileDialog.FileName, FileMode.OpenOrCreate);
                range.Load(fStream, (filter == 1 ? DataFormats.Text : DataFormats.Rtf));
                fStream.Close();
                /*
                StreamReader sr = new StreamReader(openFileDialog.FileName, Encoding.UTF8);
                string fileContent = sr.ReadToEnd();

                */
            }
        }
        private void button_SaveNotes_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = "Text file (*.txt)|*.txt|Rich text file (*.rtf) |*.rtf",
                DefaultExt = "*.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = "Notes",
                Title = "Save notes as:"
            };
            Nullable<bool> result = saveFileDialog.ShowDialog(); // show saveFileDialog

            if (result == true)
            {
                /*using ( FileStream myStream = new FileStream( myDlg.FileName, FileMode.OpenOrCreate, FileAccess.Write ) ) {
                    TextRange myRange = new TextRange( rtbTraffic.Document.ContentStart, rtbTraffic.Document.ContentEnd );
                    myRange.Save( myStream, DataFormats.Rtf );
                    myStream.Close();
                }*/

                int filter = saveFileDialog.FilterIndex;
                richTextBox.SelectAll();
                richTextBox.Selection.Save(new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write), (filter == 1 ? DataFormats.Text : DataFormats.Rtf));
                button_SaveNotes.Content = "File saved";
                button_SaveNotes.IsEnabled = false;
                richTextBox.TextChanged += new TextChangedEventHandler(richTextBox_TextChanged);
            }
        }

        private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (button_SaveNotes.Content.ToString() == "File saved")
            {
                button_SaveNotes.Content = "Save notes";
                button_SaveNotes.IsEnabled = true;
                richTextBox.TextChanged -= new TextChangedEventHandler(richTextBox_TextChanged);
            }
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (IsLoaded)
            {
                // https://stackoverflow.com/questions/11874800/change-style-of-selected-text-in-richtextbox
                TextSelection ts = richTextBox.Selection;
                if(ts != null)
                ts.ApplyPropertyValue(ForegroundProperty, new SolidColorBrush((Color)colorPickerFont.SelectedColor));
            }
        }
    }
}
