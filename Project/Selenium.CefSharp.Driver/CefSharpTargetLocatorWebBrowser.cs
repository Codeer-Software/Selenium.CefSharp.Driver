using OpenQA.Selenium;
using System;

namespace Selenium.CefSharp.Driver
{
    class CefSharpTargetLocatorWebBrowser : ITargetLocator
    {
        CefSharpDriver _driver;

        public CefSharpTargetLocatorWebBrowser(CefSharpDriver driver) => _driver = driver;

        public IWebDriver DefaultContent() => _driver;

        //TODO 
        public IWebElement ActiveElement() => throw new NotImplementedException();

        //TODO Possible?
        public IAlert Alert() => throw new NotImplementedException();

        public IWebDriver Frame(int frameIndex) => throw new NotImplementedException();

        public IWebDriver Frame(string frameName) => throw new NotImplementedException();

        public IWebDriver Frame(IWebElement frameElement) => throw new NotImplementedException();

        public IWebDriver ParentFrame() => throw new NotImplementedException();

        //don't support.
        public IWebDriver Window(string windowName) => throw new NotSupportedException();
    }
}
