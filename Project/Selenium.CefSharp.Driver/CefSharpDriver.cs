using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using OpenQA.Selenium;
using System;
using System.Collections.ObjectModel;
using System.Threading;

namespace Selenium.CefSharp.Driver
{
    public class CefSharpDriver :
        IAppVarOwner,
        IWebDriver
    {
        WindowsAppFriend _app;

        public CefSharpDriver(AppVar appVar)
        {
            AppVar = appVar;
            _app = (WindowsAppFriend)AppVar.App;
        }

        internal dynamic ExecuteScript(string src)
        {
var init = @"
    window.__seleniumCefSharpDriver = {};
    window.__seleniumCefSharpDriver.elements = [];

    function __seleniumCefSharpDriver_showAndSelectElement(element) {
        var rect = element.getBoundingClientRect();
        var elemtop = rect.top + window.pageYOffset;
        document.documentElement.scrollTop = elemtop;
        element.focus();
    }
";
            while (!(bool)this.Dynamic().CanExecuteJavascriptInMainFrame)
            {
                Thread.Sleep(10);
            }

            var check = @"!(window.__seleniumCefSharpDriver == null)";

            if (!(bool)ExecuteScriptCore(check).Result)
            {
                ExecuteScriptCore(init);
            }
            return ExecuteScriptCore(src).Result;
        }

        internal dynamic ExecuteScriptCore(string init)
        {
            var option = new OperationTypeInfo(
                "CefSharp.WebBrowserExtensions",
                "CefSharp.IWebBrowser",
                typeof(string).FullName,
                typeof(TimeSpan?).FullName);

            var result = _app["CefSharp.WebBrowserExtensions.EvaluateScriptAsync", option](AppVar, init, null).Dynamic();

            return result.Result;
        }

        public AppVar AppVar { get; }

        public string Url 
        {
            get => this.Dynamic().Address;
            set => this.Dynamic().Address = value;
        }

        public string Title => throw new NotImplementedException();

        public string PageSource => throw new NotImplementedException();

        public string CurrentWindowHandle => throw new NotImplementedException();

        public ReadOnlyCollection<string> WindowHandles => throw new NotImplementedException();

        public void Close() => throw new NotImplementedException();

        public void Dispose() => AppVar.Dispose();

        public IWebElement FindElement(By by)
        {
            var text = by.ToString();
            if (text.Contains("By.Id:"))
            {
                var id = text.Substring("By.Id:".Length).Trim();

                var scr = $@"
var e = document.getElementById('{id}');
window.__seleniumCefSharpDriver.elements.push(e);
window.__seleniumCefSharpDriver.elements.length - 1;
";
                return new CefSharpWebElement(this, (int)ExecuteScript(scr));

            }
            return null;
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            throw new NotImplementedException();
        }

        public IOptions Manage()
        {
            throw new NotImplementedException();
        }

        public INavigation Navigate()
        {
            throw new NotImplementedException();
        }

        public void Quit()
        {
            throw new NotImplementedException();
        }

        public ITargetLocator SwitchTo()
        {
            throw new NotImplementedException();
        }
    }
}
