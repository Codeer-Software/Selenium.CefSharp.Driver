using OpenQA.Selenium;
using System;

namespace Selenium.CefSharp.Driver
{
    class CefSharpNavigation : INavigation
    {
        readonly IJavaScriptExecutor _javaScriptExecutor;
        readonly Action _waitForLoading;

        public CefSharpNavigation(IJavaScriptExecutor javaScriptExecutor, Action waitForLoading)
        {
            _javaScriptExecutor = javaScriptExecutor;
            _waitForLoading = waitForLoading;
        }

        public void Back()
        {
            _javaScriptExecutor.ExecuteScript("window.history.back();");
            _waitForLoading();
        }

        public void Forward()
        {
            _javaScriptExecutor.ExecuteScript("window.history.forward();");
            _waitForLoading();
        }

        public void GoToUrl(string url)
        {

            _javaScriptExecutor.ExecuteScript($"window.location.href = '{AdjustUrl(url)}';");
            _waitForLoading();
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
            _javaScriptExecutor.ExecuteScript("window.location.reload();");
            _waitForLoading();
        }
    }
}
