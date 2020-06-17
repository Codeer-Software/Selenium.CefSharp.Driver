using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Selenium.CefSharp.Driver;
using System.Diagnostics;
using System.IO;

namespace Test
{
    public static class AppRunner
    {
        public static CefSharpDriver RunWinFormApp()
            => RunApp(@"CefSharpWinFormsTarget\bin\x86\Debug\CefSharpWinFormsTarget.exe");

        public static CefSharpDriver RunWpfApp()
            => RunApp(@"CefSharpWPFTarget\bin\x86\Debug\CefSharpWPFTarget.exe");

        static CefSharpDriver RunApp(string path)
        {
            var dir = typeof(AppRunner).Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);

            var processPath = Path.Combine(dir, path);
            var process = Process.Start(processPath);

            //attach by friendly.
            var _app = new WindowsAppFriend(process);
            var main = _app.WaitForIdentifyFromWindowText("MainWindow");

            //create driver.
            return new CefSharpDriver(main.Dynamic()._browser);
        }
    }
}
