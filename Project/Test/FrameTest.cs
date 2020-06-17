using System.Diagnostics;
using System.IO;
using Codeer.Friendly.Windows;
using OpenQA.Selenium;
using Selenium.CefSharp.Driver;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace Test
{
    public class FrameTestWinForms : FrameTest
    {
        WindowsAppFriend _app;
        CefSharpDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [SetUp]
        public void SetUp()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [OneTimeSetUp]
        public void ClassInit()
        {
            Server = HtmlServer.StartNew();
            var appWithDriver = AppRunner.RunWinFormApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
            Server.Dispose();
        }
    }

    public class FrameTestWpf : FrameTest
    {
        WindowsAppFriend _app;
        CefSharpDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [SetUp]
        public void SetUp()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [OneTimeSetUp]
        public void ClassInit()
        {
            Server = HtmlServer.StartNew();
            var appWithDriver = AppRunner.RunWpfApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
            Server.Dispose();
        }
    }

    public class FrameTestWeb : FrameTest
    {
        IWebDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [SetUp]
        public void initialize()
        {
            _driver.Url = "https://github.com/Codeer-Software/Selenium.CefSharp.Driver";
            _driver.Url = this.GetHtmlUrl();
        }

        [OneTimeSetUp]
        public void ClassInit()
        {
            Server = HtmlServer.StartNew();
            _driver = new ChromeDriver();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _driver.Dispose();
            Server.Dispose();
        }
    }

    //TODO
    public abstract class FrameTest
    {
        protected HtmlServer Server { get; set; }

        public abstract IWebDriver GetDriver();

        protected string GetHtmlUrl()
        {
            return Server.RootUrl +"Frame.html";
        }

        [Test]
        public void TestFrame()
        {
            Thread.Sleep(3000);

            var driver = GetDriver();

            driver.SwitchTo().Frame(0);
            driver.FindElement(By.Id("textBoxName")).SendKeys("abc");
            driver.SwitchTo().DefaultContent();

            driver.SwitchTo().Frame(1);
            driver.SwitchTo().Frame(0);
            driver.FindElement(By.Id("inputJS")).Click();
            driver.SwitchTo().DefaultContent();

            driver.SwitchTo().Frame(2);
            var url = driver.Url;

            driver.Navigate().GoToUrl("https://github.com/Codeer-Software/Selenium.CefSharp.Driver");
            driver.Url = this.GetHtmlUrl();

            var y = driver.FindElement(By.Id("frameInput1"));
            /*
            driver.SwitchTo().ParentFrame();
            driver.FindElement(By.Id("frameInput1")).SendKeys("abc");
            */
            //TODO
            //url is browser's url. not iframe url.
            //check navigate too.
        }
    }
}
