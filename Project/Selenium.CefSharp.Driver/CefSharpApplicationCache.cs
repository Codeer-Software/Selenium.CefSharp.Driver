using OpenQA.Selenium.Html5;
using System;
using System.Globalization;

namespace Selenium.CefSharp.Driver
{
    class CefSharpApplicationCache : IApplicationCache
    {
        CefSharpDriver _driver;
        internal CefSharpApplicationCache(CefSharpDriver driver) => _driver = driver;
        public AppCacheStatus Status => (AppCacheStatus)Convert.ToInt32(_driver.ExecuteScript("window.applicationCache.status"), CultureInfo.InvariantCulture);
    }
}
