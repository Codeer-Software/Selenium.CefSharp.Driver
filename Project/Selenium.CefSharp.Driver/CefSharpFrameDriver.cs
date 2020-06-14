using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using OpenQA.Selenium;
using System.Drawing;
using System.Linq;

namespace Selenium.CefSharp.Driver
{
    class CefSharpFrameDriver :
        IAppVarOwner,
        IUIObject,
        IJavaScriptExecutor,
        IJavaScriptExecutorCefFunctions
    {
        IJavaScriptExecutor _javaScriptExecutor;
        internal CefSharpDriver CefSharpDriver { get; }
        readonly CotnrolAccessor _cotnrolAccessor;
        readonly IWebElement[] _frameElements;

        public WindowsAppFriend App => (WindowsAppFriend)AppVar.App;

        public AppVar AppVar { get; }

        public dynamic Frame => this.Dynamic();

        public dynamic JavascriptObjectRepository => CefSharpDriver.ChromiumWebBrowser.JavascriptObjectRepository;

        public Size Size => _frameElements.Any() ? _frameElements.Last().Size : CefSharpDriver.ChromiumWebBrowser.Size;

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
            _javaScriptExecutor = new CefSharpJavaScriptExecutor(this);
            CefSharpDriver = rootDriver;
            AppVar = frame;
            _frameElements = frameElement;
            _cotnrolAccessor = new CotnrolAccessor(this);
        }

        internal string Title => (string)ExecuteScript("return document.title;");

        public object ExecuteScript(string script, params object[] args) => _javaScriptExecutor.ExecuteScript(script, args);

        public object ExecuteAsyncScript(string script, params object[] args) => _javaScriptExecutor.ExecuteAsyncScript(script, args);

        public void WaitForLoading() => CefSharpDriver.ChromiumWebBrowser.WaitForLoading();

        public Point PointToScreen(Point clientPoint)
        {
            var offset = new Point();
            foreach (var e in _frameElements)
            {
                offset.Offset(e.Location);
            }
            clientPoint.Offset(offset);
            return CefSharpDriver.ChromiumWebBrowser.PointToScreen(clientPoint);
        }

        public void Activate() => CefSharpDriver.ChromiumWebBrowser.Activate();

        public IWebElement CreateWebElement(int id) => new CefSharpWebElement(this, _cotnrolAccessor, id);

        public Screenshot GetScreenshot() => _cotnrolAccessor.GetScreenShot(new Point(0, 0), Size);
        
        public override bool Equals(object obj)
        {
            var target = obj as CefSharpFrameDriver;
            if (target == null) return false;
            return this.Dynamic().Equals(target);
        }

        public override int GetHashCode() => this.Dynamic().GetHashCode();
    }
}
