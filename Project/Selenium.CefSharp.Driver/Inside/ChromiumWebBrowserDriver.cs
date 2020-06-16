using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using System;
using System.Threading;
using Selenium.CefSharp.Driver.InTarget;
using Codeer.Friendly.DotNetExecutor;
using System.Drawing;
using OpenQA.Selenium;

namespace Selenium.CefSharp.Driver.Inside
{
    class ChromiumWebBrowserDriver :
        IAppVarOwner,
        ICefSharpBrowser
    {
        readonly dynamic _webBrowserExtensions;

        public WindowsAppFriend App => (WindowsAppFriend)AppVar.App;

        public AppVar AppVar { get; }

        public CefSharpFrameDriver MainFrame { get; }

        public CefSharpFrameDriver CurrentFrame { get; set; }

        public Size Size
        {
            get
            {
                if (IsWPF)
                {
                    var size = this.Dynamic().RenderSize;
                    return new Size((int)(double)size.Width, (int)(double)size.Height);
                }
                return new WindowControl(this.AppVar).Size;
            }
        }

        bool IsWPF
        {
            get
            {
                var finder = App.Type<TypeFinder>()();
                var wpfType = (AppVar)finder.GetType("CefSharp.Wpf.ChromiumWebBrowser");
                var t = this.Dynamic().GetType();
                var isWPF = !wpfType.IsNull && (bool)wpfType["IsAssignableFrom", new OperationTypeInfo(typeof(Type).FullName, typeof(Type).FullName)]((AppVar)t).Core;
                return isWPF;
            }
        }

        public AppVar BrowserCore => this.Dynamic().GetBrowser();

        public Point PointToScreen(Point clientPoint)
        {
            if (IsWPF)
            {
                var pos = this.Dynamic().PointToScreen(App.Type("System.Windows.Point")((double)clientPoint.X, (double)clientPoint.Y));
                return new System.Drawing.Point((int)(double)pos.X, (int)(double)pos.Y);
            }
            return new WindowControl(AppVar).PointToScreen(clientPoint);
        }

        public void Activate()
        {
            if (IsWPF)
            {
                //WPF
                var source = App.Type("System.Windows.Interop.HwndSource").FromVisual(this);
                new WindowControl(App, (IntPtr)source.Handle).Activate();
            }
            else
            {
                //WinForms
                new WindowControl(this.AppVar).Activate();
            }
            this.Dynamic().Focus();
        }

        public void WaitForLoading()
        {
            while ((bool)this.Dynamic().IsLoading)
            {
                Thread.Sleep(10);
            }
        }

        internal ChromiumWebBrowserDriver(CefSharpDriver driver)
        {
            AppVar = driver.AppVar;
            App.LoadAssembly(typeof(JSResultConverter).Assembly);
            _webBrowserExtensions = App.Type("CefSharp.WebBrowserExtensions");

            MainFrame = CurrentFrame = new CefSharpFrameDriver(driver, null, () => (AppVar)GetMainFrame(), new IWebElement[0]);

        }

        internal dynamic GetMainFrame() => _webBrowserExtensions.GetMainFrame(this);

        internal void ShowDevTools() => _webBrowserExtensions.ShowDevTools(this);

        public void Close()
        {
            WindowControl window = null;
            if (IsWPF)
            {
                IntPtr handle = App.Type("System.Windows.Interop.HwndSource").FromVisual(this).Handle;
                new WindowControl(App, handle).Close();
            }
            else
            {
                window = new WindowControl(AppVar);
                while (window.ParentWindow != null) window = window.ParentWindow;
            }
            window.Close();

        }

        internal dynamic JavascriptObjectRepository => this.Dynamic().JavascriptObjectRepository;

        public IntPtr WindowHandle => IntPtr.Zero;
    }
}
