using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using OpenQA.Selenium;
using System;
using System.Threading;
using System.Drawing;
using Codeer.TestAssistant.GeneratorToolKit;

namespace Selenium.CefSharp.Driver
{
    [ControlDriver(TypeFullName = "CefSharp.Wpf.ChromiumWebBrowser|CefSharp.WinForms.ChromiumWebBrowser")]
    public class CefSharpFrameDriver : CefSharpDriverCore,
        IWebDriver,
        IAppVarOwner,
        IUIObject,
        ITakesScreenshot,
        ICefFunctions
    {
        readonly CefSharpDriver _rootDriver;
        readonly CotnrolAccessor _cotnrolAccessor;
        readonly IWebElement _frameElement;
        readonly Point _offset;

        public WindowsAppFriend App => (WindowsAppFriend)AppVar.App;

        public AppVar AppVar { get; }

        public dynamic Frame => this.Dynamic();

        public dynamic JavascriptObjectRepository => _rootDriver.Dynamic().JavascriptObjectRepository;

        public Size Size => _frameElement.Size;

        public string Url
        {
            get => (string)ExecuteScript($"return window.location.href;");
            set
            {
                //can't use local file.
                ExecuteScript($"window.location.href = '{Navigation.AdjustUrl(value)}';");
                WaitForLoading();
            }
        }

        public CefSharpFrameDriver(CefSharpDriver rootDriver, AppVar frame, IWebElement frameElement, Point offset)
        {
            _rootDriver = rootDriver;
            AppVar = frame;
            _frameElement = frameElement;
            _offset = offset;
            _cotnrolAccessor = new CotnrolAccessor(this);
            Init(this);
        }
        
        public void Dispose() => AppVar.Dispose();

        public ITargetLocator SwitchTo() => new TargetLocator(this);

        public INavigation Navigate() => new Navigation(this);

        public void WaitForLoading()
        {
            while ((bool)_rootDriver.Dynamic().IsLoading)
            {
                Thread.Sleep(10);
            }
        }

        public Point PointToScreen(Point clientPoint)
        {
            clientPoint.Offset(_offset);
            return _rootDriver.PointToScreen(clientPoint);
        }

        public void Activate() => _rootDriver.Activate();

        public IWebElement CreateWebElement(int id) => new CefSharpWebElement(this, _cotnrolAccessor, id);

        public Screenshot GetScreenshot() => _cotnrolAccessor.GetScreenShot(new Point(0, 0), Size);

        class Navigation : INavigation
        {
            CefSharpFrameDriver _this;

            public Navigation(CefSharpFrameDriver driver) => _this = driver;

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

            public void GoToUrl(string url)
            {
                //can't use local file.
                _this.ExecuteScript($"window.location.href = '{AdjustUrl(url)}';");
                _this.WaitForLoading();
            }

            internal static string AdjustUrl(string url)
            {
                if (url.ToLower().IndexOf("http") != 0)
                {
                    return "file:///" + url.Replace("\\", "/");
                }
                return url;
            }

            public void GoToUrl(Uri url) => GoToUrl(url.ToString());

            public void Refresh()
            {
                _this.ExecuteScript("window.location.reload();");
                _this.WaitForLoading();
            }
        }

        class TargetLocator : ITargetLocator
        {
            CefSharpFrameDriver _this;

            public TargetLocator(CefSharpFrameDriver driver) => _this = driver;

            public IWebDriver DefaultContent() => _this;

            public IWebElement ActiveElement() => _this.ExecuteScript("return document.activeElement;") as IWebElement;

            public IAlert Alert() => new CefSharpAlert(_this.App, _this.Url);

            //TODO
            public IWebDriver Frame(int frameIndex) => throw new NotImplementedException();

            public IWebDriver Frame(string frameName) => throw new NotImplementedException();

            public IWebDriver Frame(IWebElement frameElement) => throw new NotImplementedException();

            public IWebDriver ParentFrame() => throw new NotImplementedException();

            //don't support.
            public IWebDriver Window(string windowName) => throw new NotSupportedException();
        }
    }
}
