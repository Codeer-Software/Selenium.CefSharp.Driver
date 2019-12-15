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
        IWebDriver,
        IJavaScriptExecutor
    {
        WindowsAppFriend _app;

        public AppVar AppVar { get; }

        public string Url
        {
            get => this.Dynamic().Address;
            set
            {
                this.Dynamic().Address = value;
                WaitForJavaScriptUsable();
            }
        }

        public string Title => throw new NotImplementedException();

        public string PageSource => throw new NotImplementedException();

        public string CurrentWindowHandle => throw new NotImplementedException();

        public ReadOnlyCollection<string> WindowHandles => throw new NotImplementedException();

        public CefSharpDriver(AppVar appVar)
        {
            AppVar = appVar;
            _app = (WindowsAppFriend)AppVar.App;
        }

        public void Close() => throw new NotImplementedException();

        public void Dispose() => AppVar.Dispose();

        public IWebElement FindElement(By by)
        {
            var text = by.ToString();
            if (text.Contains("By.Id:"))
            {
                var id = text.Substring("By.Id:".Length).Trim();
                var scr = JS.FindElementById(id);
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

        public object ExecuteScript(string script, params object[] args)
        {
            //TODO arguments & return value.
            return ExecuteScript(script);
        }

        public object ExecuteAsyncScript(string script, params object[] args)
        {
            //TODO arguments & return value.
            WaitForJavaScriptUsable();
            ExecuteScriptAsyncCore(JS.Initialize);
            ExecuteScriptAsyncCore(script);
            return null;
        }

        internal dynamic ExecuteScript(string script)
        {
            WaitForJavaScriptUsable();
            ExecuteScriptCore(JS.Initialize);
            return ExecuteScriptCore(script).Result;
        }

        dynamic ExecuteScriptCore(string src)
        {
            var option = new OperationTypeInfo(
                "CefSharp.WebBrowserExtensions",
                "CefSharp.IWebBrowser",
                typeof(string).FullName,
                typeof(TimeSpan?).FullName);

            var result = _app["CefSharp.WebBrowserExtensions.EvaluateScriptAsync", option](AppVar, src, null).Dynamic();

            return result.Result;
        }

        void ExecuteScriptAsyncCore(string src)
        {
            var option = new OperationTypeInfo(
                "CefSharp.WebBrowserExtensions",
                "CefSharp.IWebBrowser",
                typeof(string).FullName,
                typeof(TimeSpan?).FullName);

            _app["CefSharp.WebBrowserExtensions.EvaluateScriptAsync", option](AppVar, src, null);
        }

        void WaitForJavaScriptUsable()
        {
            while (!(bool)this.Dynamic().CanExecuteJavascriptInMainFrame)
            {
                Thread.Sleep(10);
            }
        }
    }
}
