using System.Diagnostics;
using System.IO;
using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using RM.Friendly.WPFStandardControls;
using Selenium.CefSharp.Driver;

namespace SampleTest
{
    [TestClass]
    public class UnitTest
    {
        WindowsAppFriend _app;
        WindowControl _nextDialog;
        string _htmlPath;

        [TestInitialize]
        public void TestInitialize()
        {
            //start process.
            var dir = typeof(UnitTest).Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            var processPath = Path.Combine(dir, @"SampleApp\bin\x86\Debug\SampleApp.exe");
            var process = Process.Start(processPath);

            //attach by friendly.
            _app = new WindowsAppFriend(process);

            //show next dialog.
            var mainWindow = _app.WaitForIdentifyFromTypeFullName("SampleApp.MainWindow");
            var button = new WPFButtonBase(mainWindow.Dynamic()._buttonNextDialog);
            button.EmulateClick(new Async());

            //get next dialog.
            _nextDialog = _app.WaitForIdentifyFromTypeFullName("SampleApp.NextDialog");

            //test html
            _htmlPath = Path.Combine(dir, @"Controls.html");
        }

        [TestCleanup]
        public void TestCleanup()
            => Process.GetProcessById(_app.ProcessId).Kill();

        [TestMethod]
        public void TestMethod()
        {
            //create driver.
            var driver = new CefSharpDriver(_nextDialog.Dynamic()._browser);

            //set url.
            driver.Url = _htmlPath;
         
            //find element.
            var buttonJs = driver.FindElement(By.Id("testButton"));

            //click.
            buttonJs.Click();

            //find element.
            var textBoxName = driver.FindElement(By.Id("textBoxName"));

            //sendkeys.
            textBoxName.SendKeys("abc");
        }
    }
}
