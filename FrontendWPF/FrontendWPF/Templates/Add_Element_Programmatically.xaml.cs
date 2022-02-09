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

namespace FrontendWPF.Templates
{
    /// <summary>
    /// Interaction logic for Add_Element_Programmatically.xaml
    /// </summary>
    public partial class Add_Element_Programmatically : Window
    {
        public Add_Element_Programmatically()
        {
            InitializeComponent();

            // add a label programmatically
            System.Windows.Controls.Label label2 = new Label
            {
                Margin = new Thickness(0, 250, 0, 0),
                Content = "This is label2 from the code-behind, and the below button2 as well",
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            _ = grid.Children.Add(label2);


            // add a button programmatically
            Button button2 = new Button
            {
                Width = 70,
                Height = 40,
                Margin = new Thickness(0, 150, 0, 0)
            };
            Ellipse ellipse = new Ellipse
            {
                Width = 60,
                Height = 30,
                Fill = Brushes.Red
            };
            button2.Content = ellipse;
            _ = grid.Children.Add(button2);


        }
    }
}
