using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Selenium.CefSharp.Driver.InTarget;

namespace Selenium.CefSharp.Driver
{
    public class CefSharpDriver :
        IAppVarOwner,
        IWebDriver,
        IJavaScriptExecutor
    {
        public WindowsAppFriend App => (WindowsAppFriend)AppVar.App;

        internal dynamic WebBrowserExtensions { get; }

        public AppVar AppVar { get; }

        public string Url
        {
            get => this.Dynamic().Address;
            set
            {
                this.Dynamic().Address = value;
                WaitForLoading();
            }
        }

        public string Title => this.Dynamic().Title;

        public string PageSource => WebBrowserExtensions.GetSourceAsync(this).Result;

        public CefSharpDriver(AppVar appVar)
        {
            AppVar = appVar;
            App.LoadAssembly(typeof(JSResultConverter).Assembly);
            WebBrowserExtensions = App.Type("CefSharp.WebBrowserExtensions");
            WaitForLoading();
        }

        public void Dispose() => AppVar.Dispose();

        public IWebElement FindElement(By by)
        {
            var text = by.ToString();
            if (text.Contains("By.Id:"))
            {
                var id = text.Substring("By.Id:".Length).Trim();
                var scr = JS.FindElementById(id);
                return new CefSharpWebElement(this, (int)ExecuteScriptInternal(scr));
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
                list.Add(new CefSharpWebElement(this, (int)ExecuteScriptInternal(scr)));
            }

            return new ReadOnlyCollection<IWebElement>(list);
        }

        public INavigation Navigate() => new CefSharpNavigation(this);

        public ITargetLocator SwitchTo() => new CefSharpTargetLocator(this);

        public object ExecuteScript(string script, params object[] args)
        {
            //TODO arguments & return value.
            //TODO 作者と相談
            var result = ExecuteScriptInternal(script, args);

            //この処理、本当は ExecuteScriptInternal の中じゃないとダメ。
            var rawResult = (result as DynamicAppVar)?.CodeerFriendlyAppVar?.Core;
            return ConvertExecuteScriptResult(rawResult);
        }

        public object ExecuteAsyncScript(string script, params object[] args)
        {
            //TODO arguments & return value.
            WaitForLoading();
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

        internal dynamic ExecuteScriptInternal(string script, params object[] args)
        {
            WaitForLoading();
            dynamic initializeResult = ExecuteScriptCore(JS.Initialize);
            dynamic execResult = ExecuteScriptCore(script, args);
            if(!(bool)execResult.Success)
            {
                throw new WebDriverException((string)execResult.Message);
            }
            return App.Type<JSResultConverter>().ConvertToSelializable(execResult.Result);
        }

        dynamic ExecuteScriptCore(string src, params object[] args)
            => WebBrowserExtensions.GetMainFrame(this).EvaluateScriptAsync(ConvertCefSharpScript(src, args), "about:blank", 1, null).Result;

        private string ConvertCefSharpScript(string script, object[] args)
        {
            // 1. args を分解
            // https://github.com/SeleniumHQ/selenium/blob/646b49a5acd8cc896408b8dfaaa631e71242f4b8/dotnet/src/webdriver/Remote/RemoteWebDriver.cs#L1106

            // 2. 以下のようなスクリプトを作成
            // (function() {
            //   const result = (new Function("param1", 123, true, "return document.title;"))();
            //   return toCSharpObject(result);
            // })();
            var escapedScript = script.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
            
            var result = $"(function() {{ const result = (new Function(\"{escapedScript}\"))(); {ConvertResultInJavaScriptString} }})();";

            return result;
        }

        // 日付文字列はブラウザロケーションの影響なども受ける可能性があるため、JavaScript内で変換できるものは変換しておく
        private static string ConvertResultInJavaScriptString = @"
return (function convert(val){
const toStr = Object.prototype.toString;
if(toStr.call(val) === '[object Array]') {
    return val.map(function(v) {return convert(v);});
}
if(toStr.call(val) === '[object Number]') {
    if(Number.isNaN(val)) return null;
    if(!Number.isFinite(val)) return null;
    return val;
}
if(toStr.call(val) === '[object Date]') {
    if(Number.isNaN(val.getTime())) return null;
    return val.toISOString();
}   
return val;
})(result)";

        private object ConvertExecuteScriptResult(Object value)
        {
            if(value is int) // selenium は int の範囲内でも long になる模様?
            {
                return Convert.ToInt64((int)value);
            }
            if (value is List<object> list)  // cef は配列はList<object>になる模様? ただし selenium は ReadOnlyCollection になる模様?
            {
                var result = list.Select(i => ConvertExecuteScriptResult(i)).ToList();
                return new ReadOnlyCollection<Object>(result);
            }
            if (value is Dictionary<string, object> dic)
            {
                return dic.ToDictionary(e => e.Key, e => ConvertExecuteScriptResult(e.Value));
            }
            return value;
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

        internal void WaitForLoading()
        {
            while ((bool)this.Dynamic().IsLoading)
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
