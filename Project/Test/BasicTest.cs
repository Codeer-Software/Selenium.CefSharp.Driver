using System.Diagnostics;
using System.IO;
using System.Windows;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Selenium.CefSharp.Driver;
using Codeer.Friendly.Windows.Grasp;
using OpenQA.Selenium.Chrome;

namespace Test
{

    [TestClass]
    public class BasicTestWinForms : BasicTest
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
            var appWithDriver = AppRunner.RunWinFormApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
        }
    }

    [TestClass]
    public class BasicTestWpf : BasicTest
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
            var appWithDriver = AppRunner.RunWpfApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
        }
    }

    [TestClass]
    public class BasicTestWeb : BasicTest
    {
        static IWebDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [TestInitialize]
        public void initialize()
        {
            _driver.Url = "https://github.com/Codeer-Software/Selenium.CefSharp.Driver";
            _driver.Url = this.GetHtmlUrl();
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _driver = new ChromeDriver();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _driver.Dispose();
        }
    }


    public abstract class BasicTest
    {
        public abstract IWebDriver GetDriver();

        protected string GetHtmlUrl()
        {
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            return Path.Combine(dir, @"Test\Controls.html");
        }

        [TestMethod]
        public void TestClick()
        {
            var buttonJs = GetDriver().FindElement(By.Id("inputJS"));
            buttonJs.Click();
        }

        [TestMethod]
        public void TestSendKey()
        {
            var radio = GetDriver().FindElement(By.Id("radioMan"));
            var textBoxName = GetDriver().FindElement(By.Id("textBoxName"));
            textBoxName.SendKeys(Keys.Control + "a");
            textBoxName.SendKeys("abc");
            radio.Click();
            textBoxName.GetAttribute("value").Is("abc");
            textBoxName.SendKeys(Keys.Control + "a");
            textBoxName.SendKeys(Keys.Shift + "abc");
            radio.Click();
            textBoxName.GetAttribute("value").Is("ABC");
        }

        [TestMethod]
        public void TestTitle()
        {
            var title = GetDriver().Title;
        }

        [TestMethod]
        public void TestPageSource()
        {
            var pageSource = GetDriver().PageSource;
        }

        [TestMethod]
        public void TestNavigation()
        {
            var navigate = GetDriver().Navigate();
            navigate.Back();
            GetDriver().Url.Contains("Selenium.CefSharp.Driver").IsTrue();
            navigate.Forward();
            GetDriver().Url.Contains("Controls").IsTrue();
            navigate.GoToUrl("https://github.com/Codeer-Software/Selenium.CefSharp.Driver");
            GetDriver().Url.Contains("Selenium.CefSharp.Driver").IsTrue();
            navigate.Refresh();
            GetDriver().Url.Contains("Selenium.CefSharp.Driver").IsTrue();
        }
    }
}
