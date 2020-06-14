using Codeer.Friendly;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using OpenQA.Selenium;
using System;
using System.Drawing;
using Codeer.TestAssistant.GeneratorToolKit;
using OpenQA.Selenium.Html5;
using OpenQA.Selenium.Internal;
using System.Collections.ObjectModel;

namespace Selenium.CefSharp.Driver
{
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
        IHasApplicationCache,
        IHasWebStorage,
        ITakesScreenshot,
        IAppVarOwner,
        IUIObject
    {
        CefSharpFrameDriver _currentFrame;
        internal ChromiumWebBrowserDriver ChromiumWebBrowser { get; }

        public WindowsAppFriend App => ChromiumWebBrowser.App;

        public AppVar AppVar => ChromiumWebBrowser.AppVar;

        public Size Size => ChromiumWebBrowser.Size;

        public string Url
        {
            get => _currentFrame.Url;
            set => _currentFrame.Url = value;
        }

        public CefSharpDriver(AppVar appVar)
        {
            ChromiumWebBrowser = new ChromiumWebBrowserDriver(appVar);
            _currentFrame = new CefSharpFrameDriver(this, ChromiumWebBrowser.GetMainFrame(), new IWebElement[0]);
            ChromiumWebBrowser.WaitForLoading();
        }

        public void Dispose() => AppVar.Dispose();

        public ITargetLocator SwitchTo() => new TargetLocator(this);

        public INavigation Navigate() => new Navigation(this);

        public Point PointToScreen(Point clientPoint) => _currentFrame.PointToScreen(clientPoint);

        public void ShowDevTools()
            => ChromiumWebBrowser.ShowDevTools();

        public void Activate() => _currentFrame.Activate();

        public IWebElement CreateWebElement(int id) => _currentFrame.CreateWebElement(id);

        public Screenshot GetScreenshot() => _currentFrame.GetScreenshot();

        public string Title => _currentFrame.Title;

        public bool HasApplicationCache => true;

        public IApplicationCache ApplicationCache => new CefSharpApplicationCache(this);

        public bool HasWebStorage => true;

        public IWebStorage WebStorage => new CefSharpWebStorage(this);

        public string PageSource => (string)ExecuteScript("return document.documentElement.outerHTML;");

        public IWebElement FindElement(By by) => ElementFinder.FindElementFromDocument(this, by);

        public ReadOnlyCollection<IWebElement> FindElements(By by) => ElementFinder.FindElementsFromDocument(this, by);

        public object ExecuteScript(string script, params object[] args) => _currentFrame.ExecuteScript(script, args);

        public object ExecuteAsyncScript(string script, params object[] args) => _currentFrame.ExecuteAsyncScript(script, args);

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

        //don't support.
        public string CurrentWindowHandle => throw new NotImplementedException();
        public ReadOnlyCollection<string> WindowHandles => throw new NotImplementedException();
        public void Close() => throw new NotImplementedException();
        public void Quit() => throw new NotImplementedException();
        public IOptions Manage() => throw new NotImplementedException();

        class Navigation : INavigation
        {
            CefSharpDriver _this;

            public Navigation(CefSharpDriver driver) => _this = driver;

            public void Back()
            {
                _this.ExecuteScript("window.history.back();");
                _this.ChromiumWebBrowser.WaitForLoading();
            }

            public void Forward()
            {
                _this.ExecuteScript("window.history.forward();");
                _this.ChromiumWebBrowser.WaitForLoading();
            }

            public void GoToUrl(string url) => _this.Url = url;

            public void GoToUrl(Uri url) => _this.Url = url.ToString();

            public void Refresh()
            {
                _this.ExecuteScript("window.location.reload();");
                _this.ChromiumWebBrowser.WaitForLoading();
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
                throw new NotImplementedException();
                /*
                var cefFrameIndex = frameIndex + 1;
                var element = _this.FindElementsByTagName("iframe")[frameIndex];

                return new CefSharpFrameDriver(_this,
                    _this.Dynamic().GetBrowser().GetFrame(_this.Dynamic().GetBrowser().GetFrameIdentifiers()[cefFrameIndex]),
                        new[] { element });*/
            }

            public IWebDriver Frame(string frameName) => throw new NotImplementedException();

            public IWebDriver Frame(IWebElement frameElement) => throw new NotImplementedException();

            public IWebDriver ParentFrame() => null;

            //don't support.
            public IWebDriver Window(string windowName) => throw new NotSupportedException();
        }
    }
}
