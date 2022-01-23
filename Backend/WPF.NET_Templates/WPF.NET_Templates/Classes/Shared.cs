using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF.NET_Templates.Classes
{
    public class Shared
    {
        public static StartWindow_with_pinPanels startWindow_With_PinPanels;
        public static List<string> locationsList = new List<string> { "Miskolc", "Budapest" };
        public static List<string> permissionList = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        public static ServiceReference3.User loggedInUser = null;
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
            int itemsCount = dataGrid.Items.Count;
            // cannot recognizes rows (items) that are not visible (and results in error)!!
            DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromItem(dataGrid.Items[row_index]) as DataGridRow;
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


    }

}
