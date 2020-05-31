using Codeer.Friendly.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Selenium.CefSharp.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class WinFormSelectElementTest : SelectElementTestBase
    {
        static WindowsAppFriend _app;
        static CefSharpDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [TestInitialize]
        public void TestInitialize()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            ClassInitBase();

            var appWithDriver = AppRunner.RunWinFormApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
            ClassCleanupBase();
        }
    }

    [TestClass]
    public class WpfSelectElementTest : SelectElementTestBase
    {
        static WindowsAppFriend _app;
        static CefSharpDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [TestInitialize]
        public void TestInitialize()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            ClassInitBase();
            var appWithDriver = AppRunner.RunWpfApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
            ClassCleanupBase();
        }
    }

    [TestClass]
    public class SeleniumSelectElementTest : SelectElementTestBase
    {
        static IWebDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [TestInitialize]
        public void initialize()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            ClassInitBase();
            _driver = new ChromeDriver();
        }
        [ClassCleanup]
        public static void ClassCleanup()
        {
            _driver.Dispose();
            ClassCleanupBase();
        }
    }

    public abstract class SelectElementTestBase : CompareTestBase
    {
        [TestMethod]
        public void SelectByText()
        {
            var select = GetDriver().FindElement(By.Id("singleSelect"));
            var elem = new SelectElement(select);
            elem.SelectByText("ABCDE");
            var selectedValue = select.GetProperty("value");
            selectedValue.Is("1");
        }
    }
}
