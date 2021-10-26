using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using OpenQA.Selenium;
using System;
using System.Drawing;
using System.Linq;

namespace Selenium.CefSharp.Driver.Inside
{
    class CefSharpFrameDriver :
        IAppVarOwner,
        IUIObject,
        IJavaScriptExecutor
    {
        readonly JavaScriptAdaptor _javaScriptAdaptor;
        readonly Func<AppVar> _frameGetter;
        readonly CotnrolAccessor _cotnrolAccessor;

        public WindowsAppFriend App => (WindowsAppFriend)AppVar.App;

        public AppVar AppVar => _frameGetter();

        public AppVar JavascriptObjectRepository => CefSharpDriver.JavascriptObjectRepository;

        public Size Size => FrameElements.Any() ? FrameElements.Last().Size : CefSharpDriver.CurrentBrowser.Size;

        internal string Url
        {
            get => (string)ExecuteScript("return window.location.href;");
            set
            {
                this.Dynamic().LoadUrl(value);
                WaitForLoading();
            }
        }

        internal CefSharpDriver CefSharpDriver { get; }

        internal CefSharpFrameDriver ParentFrame { get; }
        
        internal IWebElement[] FrameElements { get; }

        internal string Title => (string)ExecuteScript("return document.title;");

        internal CefSharpFrameDriver(CefSharpDriver cefSharpDriver, CefSharpFrameDriver parentFrame, Func<AppVar> frameGetter, IWebElement[] frameElement)
        {
            ParentFrame = parentFrame;
            _javaScriptAdaptor = new JavaScriptAdaptor(this);
            CefSharpDriver = cefSharpDriver;
            _frameGetter = frameGetter;
            FrameElements = frameElement;
            _cotnrolAccessor = new CotnrolAccessor(this);
        }

        public object ExecuteScript(string script, params object[] args)
            => _javaScriptAdaptor.ExecuteScript(script, args);

        public object ExecuteAsyncScript(string script, params object[] args)
            => _javaScriptAdaptor.ExecuteAsyncScript(script, args);

        public void WaitForLoading()
            => CefSharpDriver.CurrentBrowser.WaitForLoading();

        public Point PointToScreen(Point clientPoint)
        {
            var offset = new Point();
            foreach (var e in FrameElements)
            {
                offset.Offset(e.Location);
            }
            clientPoint.Offset(offset);
            return CefSharpDriver.CurrentBrowser.PointToScreen(clientPoint);
        }

        public void Activate()
            => CefSharpDriver.CurrentBrowser.Activate();

        public IWebElement CreateWebElement(int id)
            => new CefSharpWebElement(this, _cotnrolAccessor, id);

        public Screenshot GetScreenshot()
            => _cotnrolAccessor.GetScreenShot(new Point(0, 0), Size);
        
        public override bool Equals(object obj)
        {
            var target = obj as CefSharpFrameDriver;
            if (target == null) return false;
            return this.Dynamic().Identifier.Equals(target.Dynamic().Identifier);
        }

        public override int GetHashCode() => this.Dynamic().Identifier;

        public object ExecuteScript(PinnedScript script, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
