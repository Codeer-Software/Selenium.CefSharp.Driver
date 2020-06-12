using OpenQA.Selenium;
using OpenQA.Selenium.Html5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Selenium.CefSharp.Driver
{
    class CefSharpStorag : ILocalStorage, ISessionStorage
    {
        readonly IJavaScriptExecutor _jsExecutor;
        readonly string _storageName;

        public CefSharpStorag(IJavaScriptExecutor jsExecutor, string storageName)
        {
            _jsExecutor = jsExecutor;
            _storageName = storageName;
        }

        public int Count => Convert.ToInt32(_jsExecutor.ExecuteScript($"return window.{_storageName}.length;"), CultureInfo.InvariantCulture);

        public void Clear() => _jsExecutor.ExecuteScript($"window.{_storageName}.clear();");

        public string GetItem(string key) => (string)_jsExecutor.ExecuteScript($"return window.{_storageName}.getItem('{key}');");

        public ReadOnlyCollection<string> KeySet()
        {
            var list = new List<string>();
            foreach (var e in (IEnumerable)_jsExecutor.ExecuteScript($"return Object.keys(window.{_storageName});"))
            {
                list.Add(e?.ToString());
            }
            return new ReadOnlyCollection<string>(list);
        }

        public string RemoveItem(string key)
        {
            var value = GetItem(key);
            _jsExecutor.ExecuteScript($"window.{_storageName}.removeItem('{key}');");
            return value;
        }

        public void SetItem(string key, string value) => _jsExecutor.ExecuteScript($"window.{_storageName}.setItem('{key}', '{value}');");
    }
}
