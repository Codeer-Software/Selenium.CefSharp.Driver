using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using System;
using System.Threading;
using Selenium.CefSharp.Driver.InTarget;
using Codeer.Friendly.DotNetExecutor;
using System.Drawing;

namespace Selenium.CefSharp.Driver
{
    class ChromiumWebBrowserDriver :
        IAppVarOwner,
        IUIObject
    {
        readonly dynamic _webBrowserExtensions;

        public WindowsAppFriend App => (WindowsAppFriend)AppVar.App;

        public AppVar AppVar { get; }

        public Size Size
        {
            get
            {
                if (IsWPF)
                {
                    var size = this.Dynamic().RenderSize;
                    return new Size((int)(double)size.Width, (int)(double)size.Height);
                }
                return new WindowControl(AppVar).Size;
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

        internal ChromiumWebBrowserDriver(AppVar appVar)
        {
            AppVar = appVar;
            App.LoadAssembly(typeof(JSResultConverter).Assembly);
            _webBrowserExtensions = App.Type("CefSharp.WebBrowserExtensions");
        }

        public void Dispose() => AppVar.Dispose();

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
                new WindowControl(AppVar).Activate();
            }
            this.Dynamic().Focus();
        }

        internal void WaitForLoading()
        {
            while ((bool)this.Dynamic().IsLoading)
            {
                Thread.Sleep(10);
            }
        }

        internal dynamic GetMainFrame() => _webBrowserExtensions.GetMainFrame(this);

        internal void ShowDevTools() => _webBrowserExtensions.ShowDevTools(this);

        internal dynamic JavascriptObjectRepository => this.Dynamic().JavascriptObjectRepository;
    }
}
