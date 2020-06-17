﻿using System.Diagnostics;
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
    public class WindowTestForms : WindowTest
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

    public class WindowTestWpf : WindowTest
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

    public class WindowTestWeb : WindowTest
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
    public abstract class WindowTest
    {
        protected HtmlServer Server { get; set; }

        public abstract IWebDriver GetDriver();

        protected string GetHtmlUrl()
        {
            return Server.RootUrl +"Window.html";
        }

        [Test]
        public void TestWindow()
        {
            var driver = GetDriver();
            driver.FindElement(By.Id("window")).Click();

            Thread.Sleep(3000);

            driver.SwitchTo().Window( driver.WindowHandles[1]);
            driver.FindElement(By.Id("inputJS")).Click();

            driver.FindElement(By.Id("alertJS")).Click();

            var wait = new WebDriverWait(driver, new System.TimeSpan(10000));

#pragma warning disable CS0618
            var alert = wait.Until(ExpectedConditions.AlertIsPresent());
#pragma warning restore CS0618
            alert.Text.Is("test");
            alert.Accept();

            driver.FindElement(By.Id("confirmJS")).Click();

#pragma warning disable CS0618
            var confirm = wait.Until(ExpectedConditions.AlertIsPresent());
#pragma warning restore CS0618
            confirm.Text.Is("test");
            confirm.Dismiss();

            driver.FindElement(By.Id("promptJS")).Click();

#pragma warning disable CS0618
            var prompt = wait.Until(ExpectedConditions.AlertIsPresent());
#pragma warning restore CS0618
            prompt.SendKeys("abc");
            prompt.Accept();
        }
    }
}
