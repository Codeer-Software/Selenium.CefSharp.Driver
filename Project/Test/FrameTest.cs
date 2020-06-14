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
            Server.Start();
            _driver.Url = this.GetHtmlUrl();
        }

        [OneTimeSetUp]
        public void ClassInit()
        {
            var appWithDriver = AppRunner.RunWinFormApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
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
            Server.Start();
            _driver.Url = this.GetHtmlUrl();
        }

        [OneTimeSetUp]
        public void ClassInit()
        {
            var appWithDriver = AppRunner.RunWpfApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
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
            Server.Start();
            _driver = new ChromeDriver();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _driver.Dispose();
            Server.Dispose();
        }
    }

    public abstract class FrameTest
    {
        protected TestDirHtmlServer Server = new TestDirHtmlServer();

        public abstract IWebDriver GetDriver();

        protected string GetHtmlUrl()
        {
            return Server.RootUrl +"Frame.html";
        }

        [Test]
        public void TestFrame()
        {
        }
    }
}
