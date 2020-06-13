using Codeer.Friendly.Dynamic;
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
    class CefSharpJavaScriptExecutor : IJavaScriptExecutor
    {
        IJavaScriptExecutorCefFunctions _cef;

        internal CefSharpJavaScriptExecutor(IJavaScriptExecutorCefFunctions cef)
            => _cef = cef;

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

        dynamic ExecuteScriptInternal(string script, params object[] args)
        {
            _cef.WaitForLoading();
            dynamic initializeResult = ExecuteScriptCore(JS.Initialize);

            dynamic execResult = ExecuteScriptCore(script, args);
            if (!(bool)execResult.Success)
            {
                var errorMessage = (string)execResult.Message;
                // TODO: Somehow make a pattern.
                var formattedErrorMessage = errorMessage.Split('\n')[0].Substring("Uncaught".Length).Trim();
                if (formattedErrorMessage == "EntriedElementNotFound")
                {
                    throw new StaleElementReferenceException(
                        "stale element reference: element is not attached to the page document");
                }
                throw new WebDriverException(errorMessage);
            }
            return _cef.App.Type<JSResultConverter>().ConvertToSelializable(execResult.Result);
        }

        dynamic ExecuteScriptCore(string src, params object[] args)
            => _cef.Frame.EvaluateScriptAsync(ConvertCefSharpScript(src, args), "about:blank", 1, null).Result;

        dynamic ExecuteScriptAsyncInternal(string script, params object[] args)
        {
            _cef.WaitForLoading();
            ExecuteScriptCore(JS.Initialize);

            var callbackObj = _cef.App.Type<AsyncResultBoundObject>()();
            var scriptId = $"_cefsharp_script_{Guid.NewGuid():N}";
            var BindingOptions = _cef.App.Type("CefSharp.BindingOptions");

            _cef.JavascriptObjectRepository.Register(scriptId, callbackObj, true, BindingOptions.DefaultBinder);
            ExecuteScriptAsyncCore(scriptId, script, args);
            while (!(bool)callbackObj.IsCompleted)
            {
                Thread.Sleep(10);
            }

            _cef.JavascriptObjectRepository.UnRegister(scriptId);
            return callbackObj.Value;
        }

        dynamic ExecuteScriptAsyncCore(string scriptId, string src, params object[] args)
            => _cef.Frame.EvaluateScriptAsync(ConvertCefSharpAsyncScript(scriptId, src, args), "about:blank", 1, null).Result;

        string ConvertCefSharpScript(string script, object[] args)
            => $"(function() {{ const result = (function() {{ {script} }})({ConvertScriptParameters(args)}); \r\n return {ConvertResultInJavaScriptString} }})();";

        string ConvertCefSharpAsyncScript(string scriptId, string script, object[] args)
            => $@"
(function() {{
    CefSharp.BindObjectAsync('{scriptId}').then(() => {{
        (function() {{ {script} }})({(args != null && args.Length > 0 ? (ConvertScriptParameters(args) + ",") : "")} (result) => {{
            {scriptId}.complete({ConvertResultInJavaScriptString});
        }});
    }});
}})()";

        const string HtmlElementEntryIdStringPrefix = "$$_selemniumCefSharpDriverEntryId:";
        const string HtmlElementEntryIdListStringPrefix = "$$_selemniumCefSharpDriverEntryIdList:";
        // 日付文字列はブラウザロケーションの影響なども受ける可能性があるため、JavaScript内で変換できるものは変換しておく
        const string ConvertResultInJavaScriptString = @"(function convert(val){
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

        object ConvertExecuteScriptResult(object value)
        {
            if (value is int) // selenium は int の範囲内でも long になる模様?
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
                if (stringValue.StartsWith(HtmlElementEntryIdStringPrefix))
                {
                    if (int.TryParse(stringValue.Substring(HtmlElementEntryIdStringPrefix.Length), out var val))
                    {
                        return _cef.CreateWebElement(val);
                    }
                }
                if (stringValue.StartsWith(HtmlElementEntryIdListStringPrefix))
                {
                    var ids = stringValue.Substring(HtmlElementEntryIdListStringPrefix.Length).Split(',');
                    if (ids.Select(id => id.Trim()).All(id => int.TryParse(id, out _)))
                    {
                        return new ReadOnlyCollection<IWebElement>(
                            ids.Select(id => int.Parse(id)).Select(id => (IWebElement)_cef.CreateWebElement(id)).ToList());
                    }
                }
            }
            return value;
        }

        static string ConvertScriptParameters(object[] args)
        {
            if (args == null) return string.Empty;
            return string.Join(",", args.Select(v => ConvertScriptParameter(v)));
        }

        static string ConvertScriptParameter(object v)
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

        static bool IsNumericType(object o)
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
    }
}
