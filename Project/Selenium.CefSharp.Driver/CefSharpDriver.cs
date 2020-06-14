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
        IWebDriver,
        IAppVarOwner,
        IUIObject,
        ITakesScreenshot,
        IJavaScriptExecutorCefFunctions
    {
        readonly CotnrolAccessor _cotnrolAccessor;
        readonly dynamic _webBrowserExtensions;

        public WindowsAppFriend App => (WindowsAppFriend)AppVar.App;

        public AppVar AppVar { get; }

        public dynamic Frame => _webBrowserExtensions.GetMainFrame(this);

        public dynamic JavascriptObjectRepository => this.Dynamic().JavascriptObjectRepository;

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

        public string Url
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

        public CefSharpDriver(AppVar appVar)
        {
            AppVar = appVar;
            App.LoadAssembly(typeof(JSResultConverter).Assembly);
            _webBrowserExtensions = App.Type("CefSharp.WebBrowserExtensions");
            _cotnrolAccessor = new CotnrolAccessor(this);
            WaitForLoading();
            Init(this);
        }
        
        public void Dispose() => AppVar.Dispose();

        public ITargetLocator SwitchTo() => new TargetLocator(this);

        public INavigation Navigate() => new Navigation(this);

        public void WaitForLoading()
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
            => _webBrowserExtensions.ShowDevTools(this);

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

        public IWebElement CreateWebElement(int id) => new CefSharpWebElement(this, _cotnrolAccessor, id);

        public Screenshot GetScreenshot() => _cotnrolAccessor.GetScreenShot(new Point(0, 0), Size);

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

        class Navigation : INavigation
        {
            CefSharpDriver _this;

            public Navigation(CefSharpDriver driver) => _this = driver;

            public void Back()
            {
                _this.ExecuteScript("window.history.back();");
                _this.WaitForLoading();
            }

            public void Forward()
            {
                _this.ExecuteScript("window.history.forward();");
                _this.WaitForLoading();
            }

            public void GoToUrl(string url) => _this.Url = url;

            public void GoToUrl(Uri url) => _this.Url = url.ToString();

            public void Refresh()
            {
                _this.ExecuteScript("window.location.reload();");
                _this.WaitForLoading();
            }
        }

        class TargetLocator : ITargetLocator
        {
            CefSharpDriver _this;

            public TargetLocator(CefSharpDriver driver) => _this = driver;

            public IWebDriver DefaultContent() => _this;

            public IWebElement ActiveElement() => _this.ExecuteScript("return document.activeElement;") as IWebElement;

            public IAlert Alert() => new CefSharpAlert(_this.App, _this.Url);

            //TODO
            public IWebDriver Frame(int frameIndex)
            {
                var cefFrameIndex = frameIndex + 1;
                var element = _this.FindElementsByTagName("iframe")[frameIndex];

                return new CefSharpFrameDriver(_this,
                    _this.Dynamic().GetBrowser().GetFrame(_this.Dynamic().GetBrowser().GetFrameIdentifiers()[cefFrameIndex]),
                        new[] { element });
            }

            public IWebDriver Frame(string frameName) => throw new NotImplementedException();

            public IWebDriver Frame(IWebElement frameElement) => throw new NotImplementedException();

            public IWebDriver ParentFrame() => null;

            //don't support.
            public IWebDriver Window(string windowName) => throw new NotSupportedException();
        }
    }
}
