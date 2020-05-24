using System.Diagnostics;
using System.IO;
using System.Windows;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Selenium.CefSharp.Driver;
using Codeer.Friendly.Windows.Grasp;

namespace Test
{
    [TestClass]
    public class BasicTestWPF : BasicTest
    {
        protected override string GetExePath(string dir)
            => Path.Combine(dir, @"CefSharpWPFSample\bin\x86\Debug\CefSharpWPFSample.exe");
    }

    [TestClass]
    public class BasicTestWinForms : BasicTest
    {
        protected override string GetExePath(string dir)
            => Path.Combine(dir, @"CefSharpWinFormsSample\bin\x86\Debug\CefSharpWinFormsSample.exe");
    }



    public abstract class BasicTest
    {
        WindowsAppFriend _app;
        CefSharpDriver _driver;
        string _htmlPath;

        protected abstract string GetExePath(string dir);

        [TestInitialize]
        public void TestInitialize()
        {
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            var process = Process.Start(GetExePath(dir));

            //html
            _htmlPath = Path.Combine(dir, @"Test\Controls.html");

            //attach by friendly.
            _app = new WindowsAppFriend(process);
            var main = _app.WaitForIdentifyFromWindowText("MainWindow");

            //create driver.
            _driver = new CefSharpDriver(main.Dynamic()._browser);
        }

        [TestCleanup]
        public void TestCleanup() => Process.GetProcessById(_app.ProcessId).Kill();

        [TestMethod]
        public void TestClick()
        {
            _driver.Url = _htmlPath;
            var buttonJs = _driver.FindElement(By.Id("inputJS"));
            buttonJs.Click();
        }

        [TestMethod]
        public void TestSendKey()
        {
            _driver.Url = _htmlPath;
            var textBoxName = _driver.FindElement(By.Id("textBoxName"));
            textBoxName.SendKeys("abc");
        }

        [TestMethod]
        public void TestTitle()
        {
            var title = _driver.Title;
        }

        [TestMethod]
        public void TestPageSource()
        {
            var pageSource = _driver.PageSource;
        }

        [TestMethod]
        public void TestNavigation()
        {
            _driver.Url = _htmlPath;
            var navigate = _driver.Navigate();
            navigate.Back();
            _driver.Url.Contains("Selenium.CefSharp.Driver").IsTrue();
            navigate.Forward();
            _driver.Url.Contains("Controls").IsTrue();
            navigate.GoToUrl("https://github.com/Codeer-Software/Selenium.CefSharp.Driver");
            _driver.Url.Contains("Selenium.CefSharp.Driver").IsTrue();
            navigate.Refresh();
            _driver.Url.Contains("Selenium.CefSharp.Driver").IsTrue();
        }
    }
}
