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

namespace WPF.NET_Templates
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>

    public partial class RegisterWindow : Window
    {
        public static ServiceReference3.UserServiceClient client;
        public RegisterWindow()
        {
            InitializeComponent();
            gifImage.StartAnimation();
        }

        
        private async void Button_Register_ClickAsync(object sender, RoutedEventArgs e)
        {
            client = new ServiceReference3.UserServiceClient();
            // string uid = null;
            string userName = Textbox_UserName.Text;
            string password = PasswordBox_Password.Password;
            string location = Textbox_Location.Text;
            string permission = Textbox_Permission.Text;


            // Register check

            string errorMessage = "";
            if (Textbox_UserName.Text.Length >= 4 && PasswordBox_Password.Password.Length < 4)
            {
                errorMessage = "Username and password must be at least 4 characters!";
                TextBlock_Register.Foreground = Brushes.LightSalmon;
            }
            else if (!Shared.locationsList.Contains(location))
            {
               errorMessage = "Please enter a valid location.";
            }
            else if (Textbox_Permission.Text.Length != 1)
            {
                errorMessage = "Please enter a valid Persmission value (between 0-9).";
            }

            if (errorMessage != "")
            {
                MessageBox.Show(errorMessage, caption: "Warning");
                return;
            }
            
            try
            {
                ServiceReference3.User[] userArray = client.ListUser(Shared.uid, "", userName, "", "", "").Users;
                //user = client.UserList(query);

                if (userArray == null)
                {
                    MessageBox.Show("The remote database is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                    return;
                }
                else if (userArray.Length == 1)
                {
                    TextBlock_Register.Text = "Username is already taken, please choose another name!";
                    TextBlock_Register.Foreground = Brushes.LightSalmon;
                    return;
                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("Unable to connect to the remote server") || ex.ToString().Contains("EndpointNotFoundException"))
                {
                    MessageBox.Show("The remote server is not accessible. Please make sure you have Internet access and the application is allowed by the firewall.", caption: "Error message");
                }
                else
                {
                    MessageBox.Show("An error occurred, the details are the following:\n" + ex.ToString(), caption: "Error message");
                }
            }

            try
            {
                // REGISTER into database

                string registerMessage = client.RegisterUser(Shared.uid, userName, Shared.CreateMD5(password), location, permission);
                //registerMessage = client.Register(userName, CreateMD5(password));
                if (registerMessage == "There was a problem with your registration!")
                {
                    MessageBox.Show(registerMessage, caption: "Error message");
                    return;
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
                    MessageBox.Show("Hiba történt, amelynek a részletei a következők:\n" + ex.ToString(), caption: "Error message");
                    return;
                }
            }

            Button_Register.Click -= Button_Register_ClickAsync; // remove event handlers
            Button_Login.Click -= Button_Login_Click;

            // Image_Register.Source = new BitmapImage(new Uri("/Resources/Images/success.png", UriKind.Relative));
            gifImage.GifSource = "/WPF.NET_Templates;component/Resources/Images/success.gif";
            gifImage.Width = 65;
            gifImage.StopAnimation();
            gifImage.StartAnimation();

            TextBlock_Register.Text = $"Hello {Textbox_UserName.Text}, you are registered. Now you can log in.";
            TextBlock_Register.Foreground = Brushes.LightGreen;
            Button_Register.Foreground = Brushes.LightGreen;
            Button_Register.FontWeight = FontWeight.FromOpenTypeWeight(400);
            Button_Register.FontSize = 20;
            Button_Register.Content = "O K";

            // await Shared.Delay(() => StopAnimation(), 3000);
            // await Shared.Delay(() => CloseWindow(), 4500);
            await Task.Delay(3000); // delays below code with 3500ms
            gifImage.StopAnimation();
                
            await Task.Delay(1500); // delays below code with 1500ms
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
            

        }
        // register when pressing the Enter key
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) { Button_Register_ClickAsync(sender, e); }
        }

        private void CloseWindow()
        {
            LoginWindow loginWindow  = new LoginWindow();
            loginWindow.Show();
            Close();
        }
        private void StopAnimation()
        {
            gifImage.StopAnimation();
        }

        // Fade-in-out animation

        private bool closeCompleted = false;


        private void WindowFadeOut_Completed(object sender, EventArgs e)
        {
            // close window only after fade out completed
            closeCompleted = true;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // when closing the window, start fade out, and do not close window;
            if (!closeCompleted)
            {
                WindowFadeOut.Begin();
                e.Cancel = true;
            }
        }

        
        private void Button_Login_Click(object sender, RoutedEventArgs e)
        {
            Button_Login.Click -= Button_Login_Click; // remove event handlers
            Button_Register.Click -= Button_Register_ClickAsync;
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
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


        private void Textbox_Location_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Textbox_Location.Text == "Location")
            {
                Textbox_Location.Text = "";
            }
        }

        private void Textbox_Permission_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Textbox_Permission.Text == "Permission (0-9)")
            {
                Textbox_Permission.Text = "";
            }
        }


        private void WindowFadeIn_Completed(object sender, EventArgs e)
        {
            Button_Login.IsEnabled = true;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) // enable dragging window
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();

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

