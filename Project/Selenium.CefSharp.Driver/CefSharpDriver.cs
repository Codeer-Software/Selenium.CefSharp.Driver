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
using System.Collections.ObjectModel;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Html5;

namespace Selenium.CefSharp.Driver
{
    /*
        //Not supported.
        //You can't use OpenQA.Selenium.Interactions.Actions.
        //Use Friendly.Windows.KeyMouse for complex things.
        IHasInputDevices, 
        IActionExecutor

        //Not supported. 
        IHasCapabilities,
        IHasLocationContext,
        IHasSessionId,

        //Under review.
        IAllowsFileDetection
    */

    [ControlDriver(TypeFullName = "CefSharp.Wpf.ChromiumWebBrowser|CefSharp.WinForms.ChromiumWebBrowser")]
    public class CefSharpDriver :
        IWebDriver,
        IJavaScriptExecutor,
        IFindsById,
        IFindsByClassName,
        IFindsByLinkText,
        IFindsByName,
        IFindsByTagName,
        IFindsByXPath,
        IFindsByPartialLinkText,
        IFindsByCssSelector,
        ITakesScreenshot,
        IHasApplicationCache,
        IHasWebStorage,
        IAppVarOwner,
        IUIObject,
        ICefFunctions
    {
        readonly IJavaScriptExecutor _javaScriptExecutor;
        readonly ISearchContext _searchContext;
        readonly CotnrolAccessor _cotnrolAccessor;

        public WindowsAppFriend App => (WindowsAppFriend)AppVar.App;

        internal dynamic WebBrowserExtensions { get; }

        public AppVar AppVar { get; }

        public dynamic Frame => WebBrowserExtensions.GetMainFrame(this);

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

        public string Title => (string)ExecuteScript("return document.title;");

        public string PageSource => WebBrowserExtensions.GetSourceAsync(this).Result;

        public bool HasApplicationCache => true;

        public IApplicationCache ApplicationCache { get; }

        public bool HasWebStorage => true;

        public IWebStorage WebStorage { get; }

        public CefSharpDriver(AppVar appVar)
        {
            AppVar = appVar;
            App.LoadAssembly(typeof(JSResultConverter).Assembly);
            WebBrowserExtensions = App.Type("CefSharp.WebBrowserExtensions");
            _javaScriptExecutor = new CefSharpJavaScriptExecutor(this);
            _searchContext = new DocumentElementFinder(_javaScriptExecutor);
            ApplicationCache = new CefSharpApplicationCache(_javaScriptExecutor);
            WebStorage = new CefSharpWebStorage(_javaScriptExecutor);
            _cotnrolAccessor = new CotnrolAccessor(this);
            WaitForLoading();
        }
        
        public void Dispose() => AppVar.Dispose();

        public IWebElement FindElement(By by) => _searchContext.FindElement(by);

        public ReadOnlyCollection<IWebElement> FindElements(By by) => _searchContext.FindElements(by);

        public object ExecuteScript(string script, params object[] args) => _javaScriptExecutor.ExecuteScript(script, args);

        public object ExecuteAsyncScript(string script, params object[] args) => _javaScriptExecutor.ExecuteAsyncScript(script, args);

        public Screenshot GetScreenshot() => _cotnrolAccessor.GetScreenShot(new Point(0, 0), Size);

        public IWebElement FindElementById(string id) => FindElement(By.Id(id));

        public ReadOnlyCollection<IWebElement> FindElementsById(string id) => FindElements(By.Id(id));

        public IWebElement FindElementByClassName(string className) => FindElement(By.ClassName(className));

        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className) => FindElements(By.ClassName(className));

        public IWebElement FindElementByName(string name) => FindElement(By.Name(name));

        public ReadOnlyCollection<IWebElement> FindElementsByName(string name) => FindElements(By.Name(name));

        public IWebElement FindElementByTagName(string tagName) => FindElement(By.TagName(tagName));

        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName) => FindElements(By.TagName(tagName));

        public IWebElement FindElementByXPath(string xpath) => FindElement(By.XPath(xpath));

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath) => FindElements(By.XPath(xpath));

        public IWebElement FindElementByCssSelector(string cssSelector) => FindElement(By.CssSelector(cssSelector));

        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector) => FindElements(By.CssSelector(cssSelector));

        public IWebElement FindElementByLinkText(string linkText) => FindElement(By.LinkText(linkText));
        
        public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText) => FindElements(By.LinkText(linkText));
        
        public IWebElement FindElementByPartialLinkText(string partialLinkText) => FindElement(By.PartialLinkText(partialLinkText));
        
        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText) => FindElements(By.PartialLinkText(partialLinkText));

        public INavigation Navigate() => new CefSharpNavigationWebBrowser(this);

        public ITargetLocator SwitchTo() => new CefSharpTargetLocatorWebBrowser(this);

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

        public IWebElement CreateWebElement(int id) => new CefSharpWebElement(this, id);

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

        //don't support.
        public string CurrentWindowHandle => throw new NotImplementedException();
        public ReadOnlyCollection<string> WindowHandles => throw new NotImplementedException();
        public void Close() => throw new NotImplementedException();
        public void Quit() => throw new NotImplementedException();
        public IOptions Manage() => throw new NotImplementedException();
    }
}
