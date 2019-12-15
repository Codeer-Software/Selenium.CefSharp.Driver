using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class ResarchChromeDriver
    {
        ChromeDriver _driver;

        [TestInitialize]
        public void TestInitialize()
        {
            _driver = new ChromeDriver();
        }

        [TestCleanup]
        public void TestCleanup() => _driver.Dispose();

        [TestMethod]
        public void Test()
        {
            _driver.Url = "https://github.com/Codeer-Software/Selenium.CefSharp.Driver";
            _driver.Quit();

        }
    }
}
