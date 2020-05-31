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
using Codeer.Friendly.DotNetExecutor;
using System.Drawing;
using Codeer.TestAssistant.GeneratorToolKit;

namespace Selenium.CefSharp.Driver
{
    [ControlDriver(TypeFullName = "CefSharp.Wpf.ChromiumWebBrowser|CefSharp.WinForms.ChromiumWebBrowser")]
    public class CefSharpDriver :
        IAppVarOwner,
        IWebDriver,
        IJavaScriptExecutor,
        IUIObject
    {
        public WindowsAppFriend App => (WindowsAppFriend)AppVar.App;

        internal dynamic WebBrowserExtensions { get; }

        public AppVar AppVar { get; }

        public Size Size
        {
            get
            {
                if (IsWPF)
                {
                    var size = this.Dynamic().RenderSize;
                    return new Size((int)(double)size.Width, (int)(double)size.Height);
                }
                return new WindowControl(AppVar).Size;
            }
        }
    
        public Point PointToScreen(Point clientPoint)
        {
            if (IsWPF)
            {
                var pos = this.Dynamic().PointToScreen(App.Type("System.Windows.Point")((double)clientPoint.X, (double)clientPoint.Y));
                return new System.Drawing.Point((int)(double)pos.X, (int)(double)pos.Y);
            }
            return new WindowControl(AppVar).PointToScreen(clientPoint);
        }

        public string Url
        {
            get => this.Dynamic().Address;
            set
            {
                if (IsWPF)
                {
                    this.Dynamic().Address = value;
                }
                else
                {
                    this.Dynamic().Load(value);
                }
                WaitForLoading();
            }
        }

        public string Title => (string)ExecuteScript("return document.title;");

        public string PageSource => WebBrowserExtensions.GetSourceAsync(this).Result;

        public CefSharpDriver(AppVar appVar)
        {
            AppVar = appVar;
            App.LoadAssembly(typeof(JSResultConverter).Assembly);
            WebBrowserExtensions = App.Type("CefSharp.WebBrowserExtensions");
            WaitForLoading();
        }

        public void Dispose() => AppVar.Dispose();

        public void ShowDevTools()
            => WebBrowserExtensions.ShowDevTools(this);

        public IWebElement FindElement(By by)
        {
            var text = by.ToString();
            var script = "";
            if(text.StartsWith("By.Id:"))
            {
                script = $"return document.getElementById('{text.Substring("By.Id:".Length).Trim()}');";
            }
            if (text.StartsWith("By.Name:"))
            {
                script = $"return document.getElementsByName('{text.Substring("By.Name:".Length).Trim()}')[0];";
            }
            if (text.StartsWith("By.ClassName[Contains]:"))
            {
                script = $"return document.getElementsByClassName('{text.Substring("By.ClassName[Contains]:".Length).Trim()}')[0];";
            }
            if (text.StartsWith("By.CssSelector:"))
            {
                script = $"return document.querySelector(\"{text.Substring("By.CssSelector:".Length).Trim()}\");";
            }
            if (text.StartsWith("By.TagName:"))
            {
                script = $"return document.getElementsByTagName('{text.Substring("By.TagName:".Length).Trim()}')[0];";
            }
            if (text.StartsWith("By.XPath:"))
            {
                script = $"return window.__seleniumCefSharpDriver.getElementsByXPath('{text.Substring("By.XPath:".Length).Trim()}')[0];";
            }
            if (!(ExecuteScript(script) is CefSharpWebElement result))
            {
                throw new NoSuchElementException($"Element not found: {text}");
            }
            return result;
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            var text = by.ToString();
            var script = "";
            if (text.StartsWith("By.Id:"))
            {
                script = $"return document.querySelectorAll('[id=\"{text.Substring("By.Id:".Length).Trim()}\"]');";
            }
            if (text.StartsWith("By.Name:"))
            {
                script = $"return document.getElementsByName('{text.Substring("By.Name:".Length).Trim()}');";
            }
            if (text.StartsWith("By.ClassName[Contains]:"))
            {
                script = $"return document.getElementsByClassName('{text.Substring("By.ClassName[Contains]:".Length).Trim()}');";
            }
            if (text.StartsWith("By.CssSelector:"))
            {
                script = $"return document.querySelectorAll(\"{text.Substring("By.CssSelector:".Length).Trim()}\");";
            }
            if (text.StartsWith("By.TagName:"))
            {
                script = $"return document.getElementsByTagName('{text.Substring("By.TagName:".Length).Trim()}');";
            }
            if (text.StartsWith("By.XPath:"))
            {
                script = $"return window.__seleniumCefSharpDriver.getElementsByXPath('{text.Substring("By.XPath:".Length).Trim()}');";
            }
            if (!(ExecuteScript(script) is ReadOnlyCollection<IWebElement> result)) {
                return new ReadOnlyCollection<IWebElement>(new List<IWebElement>());
            }
            return result;
        }

        public INavigation Navigate() => new CefSharpNavigation(this);

        public ITargetLocator SwitchTo() => new CefSharpTargetLocator(this);

        public object ExecuteScript(string script, params object[] args)
        {
            var result = ExecuteScriptInternal(script, args);

            var rawResult = (result as DynamicAppVar)?.CodeerFriendlyAppVar?.Core;
            return ConvertExecuteScriptResult(rawResult);
        }

        public object ExecuteAsyncScript(string script, params object[] args)
        {
            var result = ExecuteScriptAsyncInternal(script, args);
            var rawResult = (result as DynamicAppVar)?.CodeerFriendlyAppVar?.Core;
            return ConvertExecuteScriptResult(rawResult);
        }
        
        public void Activate()
        {
            if (IsWPF)
            {
                //WPF
                var source = App.Type("System.Windows.Interop.HwndSource").FromVisual(this);
                new WindowControl(App, (IntPtr)source.Handle).Activate();
            }
            else
            {
                //WinForms
                new WindowControl(AppVar).Activate();
            }
            this.Dynamic().Focus();
        }

        internal dynamic ExecuteScriptInternal(string script, params object[] args)
        {
            WaitForLoading();
            dynamic initializeResult = ExecuteScriptCore(JS.Initialize);
            
            dynamic execResult = ExecuteScriptCore(script, args);
            if(!(bool)execResult.Success)
            {
                var errorMessage = (string)execResult.Message;
                // TODO: Somehow make a pattern.
                var formattedErrorMessage = errorMessage.Split('\n')[0].Substring("Uncaught".Length).Trim();
                if(formattedErrorMessage == "EntriedElementNotFound")
                {
                    throw new StaleElementReferenceException(
                        "stale element reference: element is not attached to the page document");
                } 
                throw new WebDriverException(errorMessage);
            }
            return App.Type<JSResultConverter>().ConvertToSelializable(execResult.Result);
        }

        dynamic ExecuteScriptCore(string src, params object[] args)
            => WebBrowserExtensions.GetMainFrame(this).EvaluateScriptAsync(ConvertCefSharpScript(src, args), "about:blank", 1, null).Result;

        internal dynamic ExecuteScriptAsyncInternal(string script, params object[] args)
        {
            WaitForLoading();
            ExecuteScriptCore(JS.Initialize);

            var callbackObj = App.Type<AsyncResultBoundObject>()();
            var scriptId = $"_cefsharp_script_{Guid.NewGuid():N}";
            var BindingOptions = App.Type("CefSharp.BindingOptions");

            this.Dynamic().JavascriptObjectRepository.Register(scriptId, callbackObj, true, BindingOptions.DefaultBinder);
            ExecuteScriptAsyncCore(scriptId, script, args);
            while (!(bool)callbackObj.IsCompleted)
            {
                Thread.Sleep(10);
            }

            this.Dynamic().JavascriptObjectRepository.UnRegister(scriptId);
            return callbackObj.Value;
        }

        dynamic ExecuteScriptAsyncCore(string scriptId, string src, params object[] args)
            => WebBrowserExtensions.GetMainFrame(this).EvaluateScriptAsync(ConvertCefSharpAsyncScript(scriptId, src, args), "about:blank", 1, null).Result;

        private string ConvertCefSharpScript(string script, object[] args)
            => $"(function() {{ const result = (function() {{ {script} }})({ConvertScriptParameters(args)}); \r\n return {ConvertResultInJavaScriptString} }})();";

        private string ConvertCefSharpAsyncScript(string scriptId, string script, object[] args)
            => $@"
(function() {{
    CefSharp.BindObjectAsync('{scriptId}').then(() => {{
        (function() {{ {script} }})({(args != null && args.Length > 0 ? (ConvertScriptParameters(args) + ",") : "")} (result) => {{
            {scriptId}.complete({ConvertResultInJavaScriptString});
        }});
    }});
}})()";

        private const string HtmlElementEntryIdStringPrefix = "$$_selemniumCefSharpDriverEntryId:";
        private const string HtmlElementEntryIdListStringPrefix = "$$_selemniumCefSharpDriverEntryIdList:";
        // 日付文字列はブラウザロケーションの影響なども受ける可能性があるため、JavaScript内で変換できるものは変換しておく
        private const string ConvertResultInJavaScriptString = @"(function convert(val){
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
if(toStr.call(val) === '[object Function]' || toStr.call(val) === '[object Object]') {
    return Object.entries(val).reduce(function(v, kv) {
        v[kv[0]] = convert(kv[1]);
        return v;
    }, {});
}
if(val === window) {
    throw 'ExpectReturnWindowReference\nCannot return window object';
}
if(val instanceof HTMLElement || val instanceof Node) { // Document type not support
    if(val.nodeType !== Node.ELEMENT_NODE) {
        throw 'ExpectReturnNonElementReference\nCannot return non element node';
    }
    const entryId = window.__seleniumCefSharpDriver.entryElement(val);
    return `" + HtmlElementEntryIdStringPrefix + @"${entryId}`;
}
if(val instanceof HTMLCollection || val instanceof NodeList) {
    const entryIds = Array.prototype.slice.call(val).map(elem => {
        if(elem.nodeType !== Node.ELEMENT_NODE) {
            throw 'ExpectReturnNonElementReference\nCannot return non element node';
        }
        return window.__seleniumCefSharpDriver.entryElement(elem);
    }).join(',');
    return `" + HtmlElementEntryIdListStringPrefix + @"${entryIds}`;
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

                //TODO
                var webElements = result.OfType<IWebElement>().ToList();
                if (0 < webElements.Count && webElements.Count == result.Count)
                {
                    return new ReadOnlyCollection<IWebElement>(webElements);
                }

                return new ReadOnlyCollection<Object>(result);
            }
            if (value is Dictionary<string, object> dic)
            {
                return dic.ToDictionary(e => e.Key, e => ConvertExecuteScriptResult(e.Value));
            }
            if (value is string stringValue)
            {
                if(stringValue.StartsWith(HtmlElementEntryIdStringPrefix))
                {
                    if(int.TryParse(stringValue.Substring(HtmlElementEntryIdStringPrefix.Length), out var val)) 
                    {
                        return new CefSharpWebElement(this, val);
                    }
                }
                if (stringValue.StartsWith(HtmlElementEntryIdListStringPrefix))
                {
                    var ids = stringValue.Substring(HtmlElementEntryIdListStringPrefix.Length).Split(',');
                    if(ids.Select(id => id.Trim()).All(id => int.TryParse(id, out _)))
                    {
                        return new ReadOnlyCollection<IWebElement>(
                            ids.Select(id => int.Parse(id)).Select(id => (IWebElement)new CefSharpWebElement(this, id)).ToList());
                    }
                }
            }
            return value;
        }

        private string ConvertScriptParameters(object[] args)
        {
            if (args == null) return string.Empty;
            return string.Join(",", args.Select(v => ConvertScriptParameter(v)));
        }

        private string ConvertScriptParameter(object v)
        {
            if (v == null) return "null";
            if (IsNumericType(v)) return v.ToString();
            if (v is bool) return ((bool)v).ToString().ToLower();
            if (v is string) return $"\"{JsUtils.ToJsEscapedString(v.ToString())}\"";
            if (v is CefSharpWebElement)
            {
                return JS.FindElementByEntryIdScriptBody(((CefSharpWebElement)v).Id);
            }
            if (v is IDictionary)
            {
                var dic = (IDictionary)v;
                return "{" + string.Join(",", dic.Keys.OfType<object>()
                    .Select(key => $"\"{key.ToString()}\":{ConvertScriptParameter(dic[key])}"))
                    + "}";
            }
            if (v is IEnumerable)
            {
                return 
                    $"[{string.Join(",", ((IEnumerable)v).OfType<object>().Select(v1 => ConvertScriptParameter(v1)))}]";
            }
            throw new ArgumentException($"Argument is of an illegal type[${v}]");
        }

        private bool IsNumericType(object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
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

        bool IsWPF
        {
            get
            {
                var finder = App.Type<TypeFinder>()();
                var wpfType = (AppVar)finder.GetType("CefSharp.Wpf.ChromiumWebBrowser");
                var t = this.Dynamic().GetType();
                var isWPF = !wpfType.IsNull && (bool)wpfType["IsAssignableFrom", new OperationTypeInfo(typeof(Type).FullName, typeof(Type).FullName)]((AppVar)t).Core;
                return isWPF;
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
