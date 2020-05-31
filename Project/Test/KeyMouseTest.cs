using System.Diagnostics;
using System.IO;
using Codeer.Friendly.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Selenium.CefSharp.Driver;
using OpenQA.Selenium.Chrome;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Test
{
    [TestClass]
    public class KeyMouseTestWinForms : KeyMouseTest
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
    public class KeyMouseTestWpf : KeyMouseTest
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
    public class KeyMouseTestWeb : KeyMouseTest
    {
        static IWebDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [TestInitialize]
        public void initialize()
        {
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

    public abstract class KeyMouseTest
    {
        public abstract IWebDriver GetDriver();

        protected string GetHtmlUrl()
        {
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            return Path.Combine(dir, @"Test\Controls.html");
        }

        [TestMethod]
        public void Click()
        {
            var buttonJs = GetDriver().FindElement(By.Id("inputJS"));
            buttonJs.Click();
            var textBoxName = GetDriver().FindElement(By.Id("textBoxName"));
            textBoxName.GetAttribute("value").Is("JS");
        }

        [TestMethod]
        public void SendKey()
        {
            var textBoxName = GetDriver().FindElement(By.Id("textBoxName"));
            textBoxName.SendKeys(Keys.Control + "a");
            textBoxName.SendKeys("abc");
            textBoxName.GetAttribute("value").Is("abc");
            textBoxName.SendKeys(Keys.Control + "a");
            textBoxName.SendKeys(Keys.Shift + "abc");
            textBoxName.GetAttribute("value").Is("ABC");
        }

        [TestMethod]
        public void SendKeyModifyKeys()
        {
            var keyTest = GetDriver().FindElement(By.Id("keyTest"));

            keyTest.SendKeys(Keys.Alt + Keys.Control + "g");

            var ret1 = PopKeyLog();
            ret1.Is(new[] 
            {
                "Alt[alt]",
                "Control[control][alt]",
                "g[control][alt]",
            });

            keyTest.SendKeys(Keys.Shift + Keys.Alt + Keys.Control + "g");
            var ret2 = PopKeyLog();

            //Allow the case that the key taken by keydown when pressing shift key is different in case.
            ret2[3] = ret2[3].ToLower();

            ret2.Is(new[]
            {
                "Shift[shift]",
                "Alt[shift][alt]" ,
                "Control[control][shift][alt]",
                "g[control][shift][alt]"  ,
            });
        }

        string[] PopKeyLog()
        {
            dynamic ret = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return window.keylog;");
            ((IJavaScriptExecutor)GetDriver()).ExecuteScript("window.keylog = [];");
            var list = new List<string>();
            foreach (var e in ret)
            {
                list.Add(e);
            }
            return list.ToArray();
        }
    }
}
