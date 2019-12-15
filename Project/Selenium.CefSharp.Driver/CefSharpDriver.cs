using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace Selenium.CefSharp.Driver
{
    public class CefSharpDriver :
        IAppVarOwner,
        IWebDriver,
        IJavaScriptExecutor
    {
        internal dynamic WebBrowserExtensions { get; }

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

        public string Title => this.Dynamic().Title;

        public string PageSource => WebBrowserExtensions.GetSourceAsync(this).Result;

        public CefSharpDriver(AppVar appVar)
        {
            AppVar = appVar;
            WebBrowserExtensions = App.Type("CefSharp.WebBrowserExtensions");
            WaitForJavaScriptUsable();
        }

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
            var list = new List<IWebElement>();

            var text = by.ToString();
            if (text.Contains("By.Id:"))
            {
                //id is only one in the html.
                var id = text.Substring("By.Id:".Length).Trim();
                var scr = JS.FindElementById(id);
                list.Add(new CefSharpWebElement(this, (int)ExecuteScript(scr)));
            }

            return new ReadOnlyCollection<IWebElement>(list);
        }

        public INavigation Navigate() => new CefSharpNavigation(this);

        public ITargetLocator SwitchTo() => new CefSharpTargetLocator(this);

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
            => ExecuteScriptAsyncCore(src).Result;

        dynamic ExecuteScriptAsyncCore(string src)
        {
            var option = new OperationTypeInfo(
                "CefSharp.WebBrowserExtensions",
                "CefSharp.IWebBrowser",
                typeof(string).FullName,
                typeof(TimeSpan?).FullName);

            return App["CefSharp.WebBrowserExtensions.EvaluateScriptAsync", option](AppVar, src, null).Dynamic();
        }

        internal void WaitForJavaScriptUsable()
        {
            while (!(bool)this.Dynamic().CanExecuteJavascriptInMainFrame)
            {
                Thread.Sleep(10);
            }
        }

        //don't support.
        public string CurrentWindowHandle => throw new NotImplementedException();
        public ReadOnlyCollection<string> WindowHandles => throw new NotImplementedException();
        public void Close() => throw new NotImplementedException();
        public void Quit() => throw new NotImplementedException();
        public IOptions Manage() => throw new NotImplementedException();
    }
}
