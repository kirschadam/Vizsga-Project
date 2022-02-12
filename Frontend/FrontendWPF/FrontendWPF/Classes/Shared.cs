using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FrontendWPF.Classes
{
    public class Shared
    {
        public static StartWindow StartWindow;
        public static List<string> locationsList = new List<string> { "Budapest", "Debrecen", "Szeged", "Miskolc", "Pécs", "Győr", "Nyíregyháza", "Kecskemét", "Székesfehérvár", "Szombathely","Érd", "Szolnok", "Tatabánya", "Sopron", "Kaposvár", "Veszprém", "Békéscsaba", "Zalaegerszeg", "Eger", "Nagykanizsa" };
        /*
        public static List<string> regionsList = new List<string> { "Pest", "Hajdú-Bihar", "Csongrád-Csanád", "Borsod-Abaúj", "Pécs", "Győr", "Nyíregyháza", "Kecskemét", "Székesfehérvár", "Szombathely", "Érd", "Szolnok", "Tatabánya", "Sopron", "Kaposvár", "Veszprém", "Békéscsaba", "Zalaegerszeg", "Eger", "Nagykanizsa" };
        */
        public static List<string> permissionList = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        public static UserService.User loggedInUser = null;
        public static string uid;
        public static bool loggedIn = false;

        // delay execution: https://stackoverflow.com/questions/5904636/best-way-to-create-a-run-once-time-delayed-function-in-c-sharp
        public static async Task Delay(Action action, int timeoutInMilliseconds)
        {
            await Task.Delay(timeoutInMilliseconds);
            action();
        }
        // how to run a method as an Action: https://stackoverflow.com/questions/13260322/how-to-use-net-action-to-execute-a-method-with-unknown-number-of-parameters
        // envoke with this: await Shared.Delay(() => MethodName(), 2500);

        /*
        //  https://stackoverflow.com/questions/24697495/shortest-way-of-checking-if-double-is-nan
        // extension method to check NaN or Infinity - MUST BE DEFINED IN A NON-GENERIC STATIC CLASS!
        public static bool IsNanOrInfinity(this double value)
        {
            return !Double.IsNaN(value) && !Double.IsInfinity(value);
        }
        */

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

        // style a DataGrid cell
        public static void StyleDatagridCell(DataGrid dataGrid, int row_index, int column_index, System.Windows.Media.SolidColorBrush backgroundColor, System.Windows.Media.SolidColorBrush foregroundColor)
        {
            // int itemsCount = dataGrid.Items.Count;
            // cannot recognizes rows (items) that are not visible (and results in error)!!
            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.Items[row_index]) as DataGridRow;
            if (row == null)
            {
                return;
            }
            
            DataGridCell cell = dataGrid.Columns[column_index].GetCellContent(row).Parent as DataGridCell;
            //set style
            cell.Background = backgroundColor;
            cell.Foreground = foregroundColor;
            return;
        }

        public static ScrollViewer GetScrollViewer(DataGrid dataGrid)
        {
            if (VisualTreeHelper.GetChildrenCount(dataGrid) == 0) return null;
            var x = VisualTreeHelper.GetChild(dataGrid, 0);
            if (x == null) return null;
            if (VisualTreeHelper.GetChildrenCount(x) == 0) return null;
            return VisualTreeHelper.GetChild(x, 0) as ScrollViewer;
        }

        // https://docs.microsoft.com/hu-hu/dotnet/desktop/wpf/graphics-multimedia/how-to-animate-a-property-without-using-a-storyboard?view=netframeworkdesktop-4.8
        public static void ChangeColor(DataGridCell cell, Color fromColor, Color toColor)
        {
            SolidColorBrush myBrush = new SolidColorBrush
            {
                Color = fromColor
            };

            ColorAnimation animation = new ColorAnimation
            {
                To = toColor,
                BeginTime = TimeSpan.FromSeconds(3),
                Duration = TimeSpan.FromSeconds(2),
                FillBehavior = FillBehavior.Stop,
                AutoReverse = false,
                // RepeatBehavior = RepeatBehavior.Forever
            };
            // due to FillBehavior.Stop the color reverts to starting color, so it has to be reset to ending color
            animation.Completed += (s, a) =>
            {
                cell.Background = new SolidColorBrush(toColor); // convert Color to SolidColorBrush
                ;
            };

            // Apply the animation to the brush's Color property.
            myBrush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

            cell.Background = myBrush;
            // cell.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        // https://stackoverflow.com/questions/11572411/sendkeys-send-method-in-wpf-application
        ///   Sends the specified key.
        /// <param name="key">The key.</param>
        public static void SendKey(Key key)
        {
            if (Keyboard.PrimaryDevice != null)
            {
                if (Keyboard.PrimaryDevice.ActiveSource != null)
                {
                    var e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
                    {
                        RoutedEvent = Keyboard.KeyDownEvent
                    };
                    InputManager.Current.ProcessInput(e);

                    // Note: Based on your requirements you may also need to fire events for:
                    // RoutedEvent = Keyboard.PreviewKeyDownEvent
                    // RoutedEvent = Keyboard.KeyUpEvent
                    // RoutedEvent = Keyboard.PreviewKeyUpEvent
                }
            }
        }

    }
}
