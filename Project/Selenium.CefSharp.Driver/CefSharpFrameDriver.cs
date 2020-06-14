using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using OpenQA.Selenium;
using System;
using System.Threading;
using System.Drawing;
using Codeer.TestAssistant.GeneratorToolKit;
using System.Linq;

namespace Selenium.CefSharp.Driver
{
    [ControlDriver(TypeFullName = "CefSharp.Wpf.ChromiumWebBrowser|CefSharp.WinForms.ChromiumWebBrowser")]
    public class CefSharpFrameDriver : CefSharpDriverCore,
        IWebDriver,
        IAppVarOwner,
        IUIObject,
        ITakesScreenshot,
        IJavaScriptExecutorCefFunctions
    {
        readonly CefSharpDriver _rootDriver;
        readonly CotnrolAccessor _cotnrolAccessor;
        readonly IWebElement[] _frameElements;

        public WindowsAppFriend App => (WindowsAppFriend)AppVar.App;

        public AppVar AppVar { get; }

        public dynamic Frame => this.Dynamic();

        public dynamic JavascriptObjectRepository => _rootDriver.Dynamic().JavascriptObjectRepository;

        public Size Size => _frameElements.Last().Size;

        public string Url
        {
            get => this.Dynamic().Url;
            set
            {
                this.Dynamic().LoadUrl(value);
                WaitForLoading();
            }
        }

        internal CefSharpFrameDriver(CefSharpDriver rootDriver, AppVar frame, IWebElement[] frameElement)
        {
            _rootDriver = rootDriver;
            AppVar = frame;
            _frameElements = frameElement;
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
            var offset = new Point();
            foreach (var e in _frameElements)
            {
                offset.Offset(e.Location);
            }
            clientPoint.Offset(offset);
            return _rootDriver.PointToScreen(clientPoint);
        }

        public void Activate() => _rootDriver.Activate();

        public IWebElement CreateWebElement(int id) => new CefSharpWebElement(this, _cotnrolAccessor, id);

        public Screenshot GetScreenshot() => _cotnrolAccessor.GetScreenShot(new Point(0, 0), Size);
        
        public override bool Equals(object obj)
        {
            var target = obj as CefSharpFrameDriver;
            if (target == null) return false;
            return this.Dynamic().Equals(target);
        }

        public override int GetHashCode() => this.Dynamic().GetHashCode();

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

            public void GoToUrl(string url) => _this.Url = url;

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
