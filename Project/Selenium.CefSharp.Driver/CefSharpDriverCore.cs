using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using Selenium.CefSharp.Driver.InTarget;
using System.Drawing;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Html5;

namespace Selenium.CefSharp.Driver
{
    /*
        //Not supported.
        //You can't use OpenQA.Selenium.Interactions.Actions.
        //Use Friendly.Windows.KeyMouse for complex things.
        IHasInputDevices, 
        IActionExecutor

        //Not supported. 
        IHasCapabilities,
        IHasLocationContext,
        IHasSessionId,

        //Under review.
        IAllowsFileDetection
    */

    public abstract class CefSharpDriverCore :
        IWebDriver,
        IJavaScriptExecutor,
        IFindsById, 
        IFindsByClassName, 
        IFindsByLinkText,
        IFindsByName, 
        IFindsByTagName, 
        IFindsByXPath, 
        IFindsByPartialLinkText, 
        IFindsByCssSelector,
        ITakesScreenshot,
        IHasApplicationCache,
        IHasWebStorage
    {
        readonly CotnrolAccessor _cotnrolAccessor;

        public abstract Size Size { get; }

        public abstract WindowsAppFriend App { get; }

        public abstract string Url { get; set; }

        public string Title => (string)ExecuteScript("return document.title;");

        public abstract string PageSource { get; }

        public bool HasApplicationCache => true;

        public IApplicationCache ApplicationCache { get; }

        public bool HasWebStorage => true;

        public IWebStorage WebStorage { get; }

        public abstract void WaitForLoading();

        protected abstract dynamic JavascriptObjectRepository { get; }

        protected abstract dynamic TargetFrame { get; }

        protected CefSharpDriverCore()
        {
            ApplicationCache = new CefSharpApplicationCache(this);
            WebStorage = new CefSharpWebStorage(this);
            _cotnrolAccessor = new CotnrolAccessor(this);
        }

        public abstract void Dispose();

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

        public abstract INavigation Navigate();

        public abstract ITargetLocator SwitchTo();

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
            => TargetFrame.EvaluateScriptAsync(ConvertCefSharpScript(src, args), "about:blank", 1, null).Result;

        internal dynamic ExecuteScriptAsyncInternal(string script, params object[] args)
        {
            WaitForLoading();
            ExecuteScriptCore(JS.Initialize);

            var callbackObj = App.Type<AsyncResultBoundObject>()();
            var scriptId = $"_cefsharp_script_{Guid.NewGuid():N}";
            var BindingOptions = App.Type("CefSharp.BindingOptions");

            JavascriptObjectRepository.Register(scriptId, callbackObj, true, BindingOptions.DefaultBinder);
            ExecuteScriptAsyncCore(scriptId, script, args);
            while (!(bool)callbackObj.IsCompleted)
            {
                Thread.Sleep(10);
            }

            JavascriptObjectRepository.UnRegister(scriptId);
            return callbackObj.Value;
        }

        dynamic ExecuteScriptAsyncCore(string scriptId, string src, params object[] args)
            => TargetFrame.EvaluateScriptAsync(ConvertCefSharpAsyncScript(scriptId, src, args), "about:blank", 1, null).Result;

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

        public Screenshot GetScreenshot() => _cotnrolAccessor.GetScreenShot(new Point(0, 0), Size);

        public IWebElement FindElementById(string id) => FindElement(By.Id(id));

        public ReadOnlyCollection<IWebElement> FindElementsById(string id) => FindElements(By.Id(id));

        public IWebElement FindElementByClassName(string className) => FindElement(By.ClassName(className));

        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className) => FindElements(By.ClassName(className));

        public IWebElement FindElementByName(string name) => FindElement(By.Name(name));

        public ReadOnlyCollection<IWebElement> FindElementsByName(string name) => FindElements(By.Name(name));

        public IWebElement FindElementByTagName(string tagName) => FindElement(By.TagName(tagName));

        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName) => FindElements(By.TagName(tagName));

        public IWebElement FindElementByXPath(string xpath) => FindElement(By.XPath(xpath));

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath) => FindElements(By.XPath(xpath));

        public IWebElement FindElementByCssSelector(string cssSelector) => FindElement(By.CssSelector(cssSelector));

        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector) => FindElements(By.CssSelector(cssSelector));

        //TODO
        public IWebElement FindElementByLinkText(string linkText) => FindElement(By.LinkText(linkText));
        public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText) => FindElements(By.LinkText(linkText));
        public IWebElement FindElementByPartialLinkText(string partialLinkText) => FindElement(By.PartialLinkText(partialLinkText));
        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText) => FindElements(By.PartialLinkText(partialLinkText));

        //don't support.
        public string CurrentWindowHandle => throw new NotImplementedException();
        public ReadOnlyCollection<string> WindowHandles => throw new NotImplementedException();
        public void Close() => throw new NotImplementedException();
        public void Quit() => throw new NotImplementedException();
        public IOptions Manage() => throw new NotImplementedException();
    }
}
