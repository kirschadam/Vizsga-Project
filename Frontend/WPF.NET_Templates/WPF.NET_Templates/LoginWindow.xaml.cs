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

    public partial class LoginWindow : Window
    {
        public static ServiceReference3.UserServiceClient client;
        private bool closeCompleted = false;

        public LoginWindow()
        {
            InitializeComponent();
            gifImage.StartAnimation();
          }


        private async void Button_Login_ClickAsync(object sender, RoutedEventArgs e)
        {
            client = new ServiceReference3.UserServiceClient();
            List<ServiceReference3.User> dbUsersList = new List<ServiceReference3.User>();
            // ServiceReference3.Response_User responseUser = new ServiceReference3.Response_User();
            ServiceReference3.Response_Login responseLogin = new ServiceReference3.Response_Login();

            // string uid = null;
            string username = Textbox_UserName.Text;
            string password = PasswordBox_Password.Password;

            // string query = $"WHERE Username='{userName}' AND Password='{Shared.CreateMD5(password)}'";
            // dbUsersList = User.GetUsers("", "", "", "", ""); // query the user from database

            try
            {
                responseLogin = client.LoginUser(username, Shared.CreateMD5(password));

                string errorMessage = "";
                /*
                if (responseLogin == null)
                {
                    errorMessage = $"The remote host is not accessible. Please check your Internet connection, or contact the service provider.";
                }
                */
                if (responseLogin.Message == "There was a problem with your request!")
                {
                    errorMessage = $"The remote database is not available. Please check your Internet connection, or contact the service provider.";
                }
                else if (responseLogin.Message.Contains("Welcome") == false)
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
                    MessageBox.Show("The remote host is not accessible. Please check your Internet connection, or contact the service provider.", caption: "Error message");
                    return;
                }
                else
                {
                    MessageBox.Show($"Login failed! The details of the error are the following:\n{ex.Message}", caption: "Error message");
                    return;
                }

            }

            
            /*
            client = new ServiceReference3.UserManagementClient();
            ServiceReference3.User[] userArray = new ServiceReference3.User[0];
            string query = $"WHERE Username='{userName}' AND Password='{Shared.CreateMD5(password)}'";
            try
            {
                //userArray = client.UserList(query);
                
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
             */

            // login check
            //if (Textbox_UserName.Text == "admin" && PasswordBox_Password.Password == "admin")

            // if (userArray.Length == 1)
            if (responseLogin.User != null)
            {
                Shared.loggedInUser = responseLogin.User;
                Shared.uid = responseLogin.Uid;
                Shared.loggedIn = true;
                Button_Register.Click -= Button_Register_Click; // remove event handlers
                Button_Login.Click -= Button_Login_ClickAsync;

                // Image_Login.Source = new BitmapImage(new Uri("/Resources/Images/success.png", UriKind.Relative));
                //this.WebBrowser.Source = new Uri(String.Format("file:///
                gifImage.GifSource = "/WPF.NET_Templates;component/Resources/Images/success.gif";
                gifImage.Width = 65;
                gifImage.StopAnimation();
                gifImage.StartAnimation();

                TextBlock_Login.Text = $"Hello {Textbox_UserName.Text}, you are logged in.";
                TextBlock_Login.Foreground = Brushes.LightGreen;
                Button_Login.Foreground = Brushes.LightGreen;
                Button_Login.FontWeight = FontWeight.FromOpenTypeWeight(400);
                Button_Login.FontSize = 20;
                Button_Login.Content = "O K";
                Shared.startWindow_With_PinPanels.button_login.Foreground = Brushes.Green;
                Shared.startWindow_With_PinPanels.button_login.Content = "Log out";
                Shared.startWindow_With_PinPanels.button_ManageUsersWindow.IsEnabled = true;

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

            if (!closeCompleted)
            {
                WindowFadeOut.Begin();
                e.Cancel = true;
            }
        }
        private RegisterWindow registerWindow = new RegisterWindow();
        private void Button_Register_Click(object sender, RoutedEventArgs e)
        {
            if (registerWindow.IsLoaded == false)
            {
                Button_Register.Click -= Button_Register_Click; // remove event handler
                Button_Login.Click -= Button_Login_ClickAsync;
                registerWindow = new RegisterWindow();
                registerWindow.Show();
                Close();
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
            }
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();

        }

        private void WindowFadeIn_Completed(object sender, EventArgs e)
        {
            Button_Register.IsEnabled = true;
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

