using Codeer.Friendly;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using OpenQA.Selenium;
using System;
using System.Drawing;
using Codeer.Friendly.Dynamic;
using Selenium.CefSharp.Driver.InTarget;
using System.Threading;
using static Selenium.CefSharp.Driver.Inside.NativeMethods;

namespace Selenium.CefSharp.Driver.Inside
{
    class CefSharpWindowBrowser : ICefSharpBrowser
    {
        WindowControl _core;

        public IntPtr WindowHandle { get; }

        public WindowsAppFriend App => _core.App;

        public Size Size => _core.Size;

        public CefSharpFrameDriver MainFrame { get; }

        public CefSharpFrameDriver CurrentFrame { get; set; }

        public AppVar BrowserCore { get; }

        public CefSharpWindowBrowser(CefSharpDriver driver, IntPtr window, AppVar browser)
        {
            WindowHandle = window;
            _core = new WindowControl((WindowsAppFriend)browser.App, GetWindow(window, GW_CHILD));
            BrowserCore = browser;
            MainFrame = CurrentFrame = new CefSharpFrameDriver(driver, null, () => (AppVar)App.Type<FrameFinder>().GetMainFrame(browser), new IWebElement[0]);
        }

        public void Activate()
            => _core.Activate();

        public Point PointToScreen(Point clientPoint)
            => _core.PointToScreen(clientPoint);

        public void WaitForLoading()
        {
            while ((bool)BrowserCore.Dynamic().IsLoading)
            {
                Thread.Sleep(10);
            }
        }

        public void Close()
            => new WindowControl(App, WindowHandle).Close();
    }
}
