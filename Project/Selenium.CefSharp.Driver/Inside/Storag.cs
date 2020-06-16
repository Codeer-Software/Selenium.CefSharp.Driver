using OpenQA.Selenium;
using OpenQA.Selenium.Html5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Selenium.CefSharp.Driver.Inside
{
    class Storag : ILocalStorage, ISessionStorage
    {
        readonly IJavaScriptExecutor _javaScriptExecutor;
        readonly string _storageName;

        public int Count => Convert.ToInt32(_javaScriptExecutor.ExecuteScript($"return window.{_storageName}.length;"), CultureInfo.InvariantCulture);

        public Storag(IJavaScriptExecutor javaScriptExecutor, string storageName)
        {
            _javaScriptExecutor = javaScriptExecutor;
            _storageName = storageName;
        }

        public void Clear()
            => _javaScriptExecutor.ExecuteScript($"window.{_storageName}.clear();");

        public string GetItem(string key)
            => (string)_javaScriptExecutor.ExecuteScript($"return window.{_storageName}.getItem('{key}');");

        public ReadOnlyCollection<string> KeySet()
        {
            var list = new List<string>();
            foreach (var e in (IEnumerable)_javaScriptExecutor.ExecuteScript($"return Object.keys(window.{_storageName});"))
            {
                list.Add(e?.ToString());
            }
            return new ReadOnlyCollection<string>(list);
        }

        public string RemoveItem(string key)
        {
            var value = GetItem(key);
            _javaScriptExecutor.ExecuteScript($"window.{_storageName}.removeItem('{key}');");
            return value;
        }

        public void SetItem(string key, string value)
            => _javaScriptExecutor.ExecuteScript($"window.{_storageName}.setItem('{key}', '{value}');");
    }
}
