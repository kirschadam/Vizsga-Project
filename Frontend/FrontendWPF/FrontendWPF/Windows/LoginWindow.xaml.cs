using System;
using System.Collections.Generic;
using System.ComponentModel;
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


namespace FrontendWPF.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>

    public partial class LoginWindow : Window
    {
        public static UserService.UserServiceClient client;
        private bool closeCompleted = false;

        public LoginWindow()
        {
            InitializeComponent();
            gifImage.StartAnimation();
        }


        private async void Button_Login_ClickAsync(object sender, RoutedEventArgs e)
        {
            client = new UserService.UserServiceClient();
            List<UserService.User> dbUsersList = new List<UserService.User>();
            // UserService.Response_User responseUser = new UserService.Response_User();
            UserService.Response_Login responseLogin = new UserService.Response_Login();

            // string uid = null;
            string username = Textbox_UserName.Text;
            string password = PasswordBox_Password.Visibility == Visibility.Visible ?  PasswordBox_Password.Password: textBox_showPassword.Text;

            // string query = $"WHERE Username='{userName}' AND Password='{Shared.CreateMD5(password)}'";
            // dbUsersList = User.GetUsers("", "", "", "", ""); // query the user from database

            try
            {
                responseLogin = client.LoginUser(username, Shared.CreateMD5(password));

                string errorMessage = "";
                if (responseLogin.Message.Contains("One or more errors occurred"))
                {
                    errorMessage = $"The remote database is not available. Please check your Internet connection, or contact the service provider.";
                }
                else if (responseLogin.Message.Contains("Username or password incorrect!") == false && responseLogin.Message.Contains("Welcome") == false)
                {
                    errorMessage = responseLogin.Message;
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
                    MessageBox.Show("The remote host is not accessible. Please check your Internet connection, or contact the service provider.", caption: "Error message", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    MessageBox.Show($"Login failed! The details of the error are the following:\n{ex.Message}", caption: "Error message");
                    return;
                }

            }

            if (responseLogin.Message.Contains("Welcome"))
            {
                Shared.loggedInUser = responseLogin.User;
                Shared.uid = responseLogin.Uid;
                Shared.loggedIn = true;
                // Button_Close.Click -= Button_Close_Click; // remove event handlers
                Button_Login.Click -= Button_Login_ClickAsync;

                // Image_Login.Source = new BitmapImage(new Uri("/Resources/Images/success.png", UriKind.Relative));
                //this.WebBrowser.Source = new Uri(String.Format("file:///
                gifImage.GifSource = "/FrontendWPF;component/Resources/Images/success.gif";
                gifImage.Width = 65;
                gifImage.StopAnimation();
                gifImage.StartAnimation();

                TextBlock_Login.Text = $"Hello {Textbox_UserName.Text}, you are logged in.";
                TextBlock_Login.Foreground = Brushes.LightGreen;
                Button_Login.Foreground = Brushes.LightGreen;
                Button_Login.FontWeight = FontWeight.FromOpenTypeWeight(400);
                Button_Login.FontSize = 20;
                Button_Login.Content = "O K";
                Shared.StartWindow.button_login.Foreground = Brushes.LightGreen;
                Shared.StartWindow.button_login.Content = "Log out";

                // 0-2: view only 3-5: +insert/update 6-8: +delete 9: +user management (admin)
                if (Shared.loggedInUser.Active == 1)
                {
                    if (Shared.loggedInUser.Permission < 9)
                    {
                        Shared.StartWindow.button_ManageUsersWindow.ToolTip = "You do not have rights to manage users!";
                        Shared.StartWindow.button_ManageLocationsWindow.ToolTip = "You do not have rights to manage locations!";
                    }
                    else
                    {
                        Shared.StartWindow.button_ManageUsersWindow.IsEnabled = true;
                        Shared.StartWindow.button_ManageUsersWindow.Foreground = Brushes.White;
                        Shared.StartWindow.button_ManageLocationsWindow.IsEnabled = true;
                        Shared.StartWindow.button_ManageLocationsWindow.Foreground = Brushes.White;
                    }
                    Shared.StartWindow.button_ManageProductsWindow.IsEnabled = true;
                    Shared.StartWindow.button_ManageProductsWindow.Foreground = Brushes.White;
                    Shared.StartWindow.button_ManagePurchasesWindow.IsEnabled = true;
                    Shared.StartWindow.button_ManagePurchasesWindow.Foreground = Brushes.White;
                    Shared.StartWindow.button_ManageSalesWindow.IsEnabled = true;
                    Shared.StartWindow.button_ManageSalesWindow.Foreground = Brushes.White;
                    Shared.StartWindow.button_ManageStocksWindow.IsEnabled = true;
                    Shared.StartWindow.button_ManageStocksWindow.Foreground = Brushes.White;
                    Shared.StartWindow.button_Layouts.IsEnabled = true;
                    Shared.StartWindow.button_Layouts.Foreground = Brushes.White;
                }
                else
                {
                    gifImage.Visibility = Visibility.Hidden;
                    TextBlock_Login.Foreground = Brushes.Salmon;
                    TextBlock_Login.Text = "Your membership is suspended, please contact the administrator!";
                    await Task.Delay(5000);
                    // MessageBox.Show("Your membership is suspended, please contact the administrator!", caption: "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                // how to run a method as an Action: https://stackoverflow.com/questions/13260322/how-to-use-net-action-to-execute-a-method-with-unknown-number-of-parameters

                // await Shared.Delay(() => CloseWindow(), 2500);
                await Task.Delay(2500); // delays below code with 2500ms
                Close();
            }
            else
            {
                TextBlock_Login.Text = "Wrong login data, please retry!";
                TextBlock_Login.Foreground = Brushes.LightSalmon;
            }
        }
        // login when pressing the Enter key
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) { Button_Login_ClickAsync(sender, e); }
        }




        // Fade-in-out animation




        private void WindowFadeOut_Completed(object sender, EventArgs e)
        {
            closeCompleted = true;

            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (!closeCompleted && (!Shared.loggedIn || !Button_Close.IsFocused)) // close window quickly if logged in and presses the Close button
            {
                WindowFadeOut.Begin();
                e.Cancel = true;
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Button_Close.Click -= Button_Close_Click; // remove event handler
            Button_Login.Click -= Button_Login_ClickAsync;
            Close();
            if (Shared.loggedIn == false)
            {
                Shared.StartWindow.pictureTimer.Start();
            }
        }

        private void Textbox_UserName_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Textbox_UserName.Text == "Username")
            {
                Textbox_UserName.Text = "";
            }
        }

        private void PasswordBox_Password_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (PasswordBox_Password.Password == "Password")
            {
                PasswordBox_Password.Password = "";
                textBox_showPassword.Text = "";
            }
        }
        private void TextBox_showPassword_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (textBox_showPassword.Text == "Password")
            {
                textBox_showPassword.Text = "";
                PasswordBox_Password.Password = "";
            }
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();

        }

        private void WindowFadeIn_Completed(object sender, EventArgs e)
        {
            Button_Close.IsEnabled = true;
        }

        private void textBlock_ShowPassword_Click(object sender, MouseButtonEventArgs e)
        {
            textBox_showPassword.Text = PasswordBox_Password.Password;
            
            PasswordBox_Password.Visibility = Visibility.Collapsed;
            textBox_showPassword.Visibility = Visibility.Visible;
            textBlock_showPassword.Visibility = Visibility.Collapsed;
            textBlock_hidePassword.Visibility = Visibility.Visible;
        }
        private void textBlock_HidePassword_Click(object sender, MouseButtonEventArgs e)
        {
            PasswordBox_Password.Password = textBox_showPassword.Text;
            textBox_showPassword.Visibility = Visibility.Collapsed;
            PasswordBox_Password.Visibility = Visibility.Visible;
            textBlock_hidePassword.Visibility = Visibility.Collapsed;
            textBlock_showPassword.Visibility = Visibility.Visible;
        }


        /*
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AlreadyFaded = false;
        }

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

        private void WindowFadeOut_Completed(object sender, EventArgs e)
        {
            Close();
        }
        */

    }
}

