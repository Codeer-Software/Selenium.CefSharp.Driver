using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class SelemiumSpikes : SpikesBase
    {
        IWebDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [TestInitialize]
        public void initialize()
        {
            _driver = new ChromeDriver();
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            
            _driver.Url = Path.Combine(dir, @"Test\Controls.html");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _driver.Dispose();
        }

        
    }
}
