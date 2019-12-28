using System.Diagnostics;
using System.IO;
using System.Windows;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Selenium.CefSharp.Driver;

namespace Test
{
    [TestClass]
    public class CefSpikes : SpikesBase
    {
        WindowsAppFriend _app;
        CefSharpDriver _driver;
        string _htmlPath;

        [TestInitialize]
        public void TestInitialize()
        {
            //start process.
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            var processPath = Path.Combine(dir, @"CefSharpWPFSample\bin\x86\Debug\CefSharpWPFSample.exe");
            var process = Process.Start(processPath);

            //html
            _htmlPath = Path.Combine(dir, @"Test\Controls.html");

            //attach by friendly.
            _app = new WindowsAppFriend(process);
            var main = _app.Type<Application>().Current.MainWindow;

            //create driver.
            _driver = new CefSharpDriver(main._browser);
            _driver.Url = _htmlPath;
        }

        [TestCleanup]
        public void TestCleanup() => Process.GetProcessById(_app.ProcessId).Kill();

        public override IWebDriver GetDriver() => _driver;
    }
}
