using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
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
        public WindowsAppFriend App => (WindowsAppFriend)AppVar.App;

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

        public CefSharpDriver(AppVar appVar) => AppVar = appVar;

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
        
        public void Activate()
        {
            //TODO WinForms
            var source = App.Type("System.Windows.Interop.HwndSource").FromVisual(this);
            new WindowControl(App, (IntPtr)source.Handle).Activate();
            this.Dynamic().Focus();
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

            var result = App["CefSharp.WebBrowserExtensions.EvaluateScriptAsync", option](AppVar, src, null).Dynamic();

            return result.Result;
        }

        void ExecuteScriptAsyncCore(string src)
        {
            var option = new OperationTypeInfo(
                "CefSharp.WebBrowserExtensions",
                "CefSharp.IWebBrowser",
                typeof(string).FullName,
                typeof(TimeSpan?).FullName);

            App["CefSharp.WebBrowserExtensions.EvaluateScriptAsync", option](AppVar, src, null);
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
