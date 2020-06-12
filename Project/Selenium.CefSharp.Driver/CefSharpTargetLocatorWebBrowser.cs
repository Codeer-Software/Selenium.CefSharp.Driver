using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Codeer.Friendly.Windows.KeyMouse;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace Selenium.CefSharp.Driver
{
    class CefSharpTargetLocatorWebBrowser : ITargetLocator
    {
        CefSharpDriver _driver;

        public CefSharpTargetLocatorWebBrowser(CefSharpDriver driver) => _driver = driver;

        public IWebDriver DefaultContent() => _driver;

        public IWebElement ActiveElement() => _driver.ExecuteScript("return document.activeElement;") as IWebElement;

        public IAlert Alert() => new CefSharpAlert(_driver);

        //TODO
        public IWebDriver Frame(int frameIndex) => throw new NotImplementedException();

        public IWebDriver Frame(string frameName) => throw new NotImplementedException();

        public IWebDriver Frame(IWebElement frameElement) => throw new NotImplementedException();

        public IWebDriver ParentFrame() => throw new NotImplementedException();

        //don't support.
        public IWebDriver Window(string windowName) => throw new NotSupportedException();
    }
}
