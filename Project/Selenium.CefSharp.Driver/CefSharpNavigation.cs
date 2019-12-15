using Codeer.Friendly.Dynamic;
using OpenQA.Selenium;
using System;

namespace Selenium.CefSharp.Driver
{
    public class CefSharpNavigation : INavigation
    {
        CefSharpDriver _driver;

        public CefSharpNavigation(CefSharpDriver driver) => _driver = driver;

        public void Back()
        {
            _driver.WebBrowserExtensions.Back(_driver);
            _driver.WaitForLoading();
        }

        public void Forward()
        {
            _driver.WebBrowserExtensions.Forward(_driver);
            _driver.WaitForLoading();
        }

        public void GoToUrl(string url) => _driver.Url = url;

        public void GoToUrl(Uri url) => _driver.Url = url.ToString();

        public void Refresh()
        {
            _driver.WebBrowserExtensions.Reload(_driver);
            _driver.WaitForLoading();
        }
    }
}
