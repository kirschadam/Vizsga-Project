using Microsoft.VisualStudio.TestTools.UnitTesting;
using FrontendWPF.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontendWPF.Windows.Tests
{
    [TestClass()]
    public class ManageProductsWindowTests
    {
        private StockService.StockServiceClient stockClient = new StockService.StockServiceClient();
        [TestMethod()]
        public void CompareTest()
        {
            Windows.ManageProductsWindow manageProductsWindow = new Windows.ManageProductsWindow();
            bool expected = true;
            bool actual = manageProductsWindow.Compare(50, 50, "=");
            Assert.AreEqual(expected, actual);
        }
    }
}