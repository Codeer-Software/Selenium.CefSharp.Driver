using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Selenium.CefSharp.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public static class AppRunner
    {
        public static AppWithDriver RunWinFormApp()
        {
            //start process.
            var dir = typeof(CefSelemiumCompareTestForCefWinForms).Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            var processPath = Path.Combine(dir, @"CefSharpWinFormsTarget\bin\x86\Debug\CefSharpWinFormsTarget.exe");
            var process = Process.Start(processPath);

            //attach by friendly.
            var _app = new WindowsAppFriend(process);
            var main = _app.WaitForIdentifyFromWindowText("MainWindow");

            //create driver.
            var _driver = new CefSharpDriver(main.Dynamic()._browser);

            return new AppWithDriver(_app, _driver);
        }

        public static AppWithDriver RunWpfApp()
        {
            //start process.
            var dir = typeof(CefSelemiumCompareTestForCefWPF).Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            var processPath = Path.Combine(dir, @"CefSharpWPFTarget\bin\x86\Debug\CefSharpWPFTarget.exe");
            var process = Process.Start(processPath);

            //attach by friendly.
            var _app = new WindowsAppFriend(process);
            var main = _app.WaitForIdentifyFromWindowText("MainWindow");

            //create driver.
            var _driver = new CefSharpDriver(main.Dynamic()._browser);

            return new AppWithDriver(_app, _driver);
        }
    }

    public class AppWithDriver
    {
        public AppWithDriver(WindowsAppFriend app, CefSharpDriver driver)
        {
            this.App = app;
            this.Driver = driver;
        }

        public WindowsAppFriend App { get; }

        public CefSharpDriver Driver { get; }
    }
}
