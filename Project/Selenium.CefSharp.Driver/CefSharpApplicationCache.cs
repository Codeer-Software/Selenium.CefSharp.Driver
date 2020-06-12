using OpenQA.Selenium;
using OpenQA.Selenium.Html5;
using System;
using System.Globalization;

namespace Selenium.CefSharp.Driver
{
    class CefSharpApplicationCache : IApplicationCache
    {
        IJavaScriptExecutor _jsExecutor;
        internal CefSharpApplicationCache(IJavaScriptExecutor jsExecutor) => _jsExecutor = jsExecutor;
        public AppCacheStatus Status => (AppCacheStatus)Convert.ToInt32(_jsExecutor.ExecuteScript("window.applicationCache.status"), CultureInfo.InvariantCulture);
    }
}
