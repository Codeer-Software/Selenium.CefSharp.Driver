using OpenQA.Selenium;
using OpenQA.Selenium.Html5;
using System;
using System.Globalization;

namespace Selenium.CefSharp.Driver.Inside
{
    class ApplicationCache : IApplicationCache
    {
        IJavaScriptExecutor _javaScriptExecutor;
        internal ApplicationCache(IJavaScriptExecutor javaScriptExecutor) => _javaScriptExecutor = javaScriptExecutor;
        public AppCacheStatus Status => (AppCacheStatus)Convert.ToInt32(_javaScriptExecutor.ExecuteScript("window.applicationCache.status"), CultureInfo.InvariantCulture);
    }
}
