using Microsoft.VisualStudio.TestTools.UnitTesting;
using FrontendWPF.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FrontendWPF.Windows.Tests
{
    [TestClass()]
    public class ManageProductsWindowTests
    {
        // ResourceDictionary myResourceDictionary = Application.LoadComponent(new Uri("FrontendWPFTests;component/Resources/Styles/ButtonThemeModern.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;
        // ResourceDictionary myResourceDictionary = Application.LoadComponent(new Uri("FrontendWPF;component/Resources/Styles/ButtonThemeModern.xaml", UriKind.RelativeOrAbsolute)) as ResourceDictionary;
        // ResourceDictionary myResourceDictionary1 = Application.LoadComponent(new Uri("FrontendWPF;component/Resources/Fonts/fa-solid-900.ttf#Font Awesome 5 Free Solid", UriKind.RelativeOrAbsolute)) as ResourceDictionary;
        // private StockService.StockServiceClient stockClient = new StockService.StockServiceClient();
        // [TestMethod()]
        public void CompareTest()
        {
            if (Application.Current == null)
            {
                App application = new App();
                FontFamily myResourceDictionary1 = Application.LoadComponent(new Uri("FrontendWPF;component/Resources/Fonts/fa-solid-900.ttf#Font Awesome 5 Free Solid", UriKind.RelativeOrAbsolute)) as FontFamily;
                application.Resources.Add("FontAwesome", myResourceDictionary1);


            }
            // if (Application.ResourceAssembly == null)
                // Application.ResourceAssembly = typeof(MainWindow).Assembly;

            Windows.ManageProductsWindow manageProductsWindow = new Windows.ManageProductsWindow(unitTesting: true);
            bool expected = true;
            bool actual = manageProductsWindow.Compare(50, 50, "=");
            Assert.AreEqual(expected, actual);
        }
    }
}