using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using OpenQA.Selenium;
using System;
using System.Threading;
using Selenium.CefSharp.Driver.InTarget;
using Codeer.Friendly.DotNetExecutor;
using System.Drawing;
using Codeer.TestAssistant.GeneratorToolKit;

namespace Selenium.CefSharp.Driver
{
    [ControlDriver(TypeFullName = "CefSharp.Wpf.ChromiumWebBrowser|CefSharp.WinForms.ChromiumWebBrowser")]
    public class CefSharpDriver : CefSharpDriverCore,
        IAppVarOwner,
        IUIObject
    {
        public override WindowsAppFriend App => (WindowsAppFriend)AppVar.App;

        internal dynamic WebBrowserExtensions { get; }

        public AppVar AppVar { get; }

        public override Size Size
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

        public override string Url
        {
            get => this.Dynamic().Address;
            set
            {
                if (IsWPF)
                {
                    this.Dynamic().Address = value;
                }
                else
                {
                    this.Dynamic().Load(value);
                }
                WaitForLoading();
            }
        }

        public override string PageSource => WebBrowserExtensions.GetSourceAsync(this).Result;

        protected override dynamic TargetFrame => WebBrowserExtensions.GetMainFrame(this);

        protected override dynamic JavascriptObjectRepository => this.Dynamic().JavascriptObjectRepository;

        public override INavigation Navigate() => new CefSharpNavigationWebBrowser(this);

        public override ITargetLocator SwitchTo() => new CefSharpTargetLocatorWebBrowser(this);

        public CefSharpDriver(AppVar appVar)
        {
            AppVar = appVar;
            App.LoadAssembly(typeof(JSResultConverter).Assembly);
            WebBrowserExtensions = App.Type("CefSharp.WebBrowserExtensions");
            WaitForLoading();
        }

        public override void Dispose() => AppVar.Dispose();

        public override void WaitForLoading()
        {
            while ((bool)this.Dynamic().IsLoading)
            {
                Thread.Sleep(10);
            }
        }

        public Point PointToScreen(Point clientPoint)
        {
            if (IsWPF)
            {
                var pos = this.Dynamic().PointToScreen(App.Type("System.Windows.Point")((double)clientPoint.X, (double)clientPoint.Y));
                return new System.Drawing.Point((int)(double)pos.X, (int)(double)pos.Y);
            }
            return new WindowControl(AppVar).PointToScreen(clientPoint);
        }

        public void ShowDevTools()
            => WebBrowserExtensions.ShowDevTools(this);

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
    }
}
