using OpenQA.Selenium;
using System;
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

    public abstract class CefSharpDriverCore :
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
        IHasWebStorage
    {
        IJavaScriptExecutor _javaScriptExecutor;
        IJavaScriptExecutorCefFunctions _cef;

        public string Title => (string)ExecuteScript("return document.title;");

        public bool HasApplicationCache => true;

        public IApplicationCache ApplicationCache => new CefSharpApplicationCache(_javaScriptExecutor);

        public bool HasWebStorage => true;

        public IWebStorage WebStorage => new CefSharpWebStorage(_javaScriptExecutor);

        public string PageSource => (string)ExecuteScript("return document.documentElement.outerHTML;");

        internal void Init(IJavaScriptExecutorCefFunctions cef)
        {
            _javaScriptExecutor = new CefSharpJavaScriptExecutor(cef);
            _cef = cef;
        }

        public IWebElement FindElement(By by) => ElementFinder.FindElementFromDocument(_javaScriptExecutor, by);

        public ReadOnlyCollection<IWebElement> FindElements(By by) => ElementFinder.FindElementsFromDocument(_javaScriptExecutor, by);

        public object ExecuteScript(string script, params object[] args) => _javaScriptExecutor.ExecuteScript(script, args);

        public object ExecuteAsyncScript(string script, params object[] args) => _javaScriptExecutor.ExecuteAsyncScript(script, args);

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
    }
}
