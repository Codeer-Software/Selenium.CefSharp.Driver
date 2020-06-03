﻿using Codeer.Friendly.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.CefSharp.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class JavaScriptExecutorTestWinForm : JavaScriptExecutorTestBase
    {
        static WindowsAppFriend _app;
        static CefSharpDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [TestInitialize]
        public void TestInitialize()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            ClassInitBase();

            var appWithDriver = AppRunner.RunWinFormApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
            ClassCleanupBase();
        }

        [Ignore("This testcase not supported. Because CefSharp returns a null object result when execution result of the javascript is Promise.")]
        public override void ShouldReturnPromiseResultWhenExecuteReturnSuccessPromiseJavaScript()
        {
        }

        [Ignore("This testcase not supported. Because Document cannot be identified in the current processing method after the second time.")]
        public override void ShouldReturnWebElementWhenExecuteReturnDocumentScript()
        {
        }
    }

    [TestClass]
    public class JavaScriptExecutorTestWPF : JavaScriptExecutorTestBase
    {
        static WindowsAppFriend _app;
        static CefSharpDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [TestInitialize]
        public void TestInitialize()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            ClassInitBase();
            var appWithDriver = AppRunner.RunWpfApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
            ClassCleanupBase();
        }

        [Ignore("This testcase not supported. Because CefSharp returns a null object result when execution result of the javascript is Promise.")]
        public override void ShouldReturnPromiseResultWhenExecuteReturnSuccessPromiseJavaScript()
        {
        }

        [Ignore("This testcase not supported. Because Document cannot be identified in the current processing method after the second time.")]
        public override void ShouldReturnWebElementWhenExecuteReturnDocumentScript()
        {
        }
    }

    [TestClass]
    public class JavaScriptExecutorTestSelenium : JavaScriptExecutorTestBase
    {
        static IWebDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [TestInitialize]
        public void initialize()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            ClassInitBase();
            _driver = new ChromeDriver();
        }
        [ClassCleanup]
        public static void ClassCleanup()
        {
            _driver.Dispose();
            ClassCleanupBase();
        }
    }

    public abstract class JavaScriptExecutorTestBase : CompareTestBase
    {
        [TestMethod]
        public void ShouldReturnSameStringWhenExecuteReturnSingleQuotationStringJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return 'abcde'");
            Assert.AreEqual("abcde", value);
        }

        [TestMethod]
        public void ShouldReturnSameStringWhenExecuteReturnDoubleQuotationStringJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return \"abcde\"");
            Assert.AreEqual("abcde", value);
        }

        [TestMethod]
        public void ShouldReturnSameStringWhenExecuteReturnBackQuotationStringJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return `abcde`");
            Assert.AreEqual("abcde", value);
        }

        [TestMethod]
        public void ShouldReturnLongValueWhenExecuteReturnIntegerJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return 12345");
            Assert.AreEqual(12345L, value);
        }

        [TestMethod]
        public void ShouldReturnDoubleValueWhenExecuteReturnDecimalJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return 12345.678");
            Assert.AreEqual(12345.678, value);
        }

        [TestMethod]
        public void ShouldReturnBooleanValueWhenExecuteReturnBooleanJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return false");
            Assert.IsFalse((bool)value);
        }

        [TestMethod]
        public void ShouldReturnIsoDateStringWhenExecuteReturnDateJavaScript()
        {
            // selenium (のChrome Driver) は ISOString の結果を返す模様
            var isoValue = GetExecutor().ExecuteScript("return new Date(2012,0,16,23,23,23,123).toISOString()");
            var value = GetExecutor().ExecuteScript("return new Date(2012,0,16,23,23,23,123)");
            Assert.AreEqual(isoValue, value);
        }

        [TestMethod]
        public void ShouldReturnNullValueWhenExecuteReturnInvalidJavaScript()
        {
            // selenium (のChrome Driver) は Invalid Date は null を返す模様
            var value = GetExecutor().ExecuteScript("return new Date(void 0)");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldReturnNullValueWhenExecuteReturnUndefinedJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return void 0");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldReturnNullValueWhenExecuteReturnPositiveInfinitJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return Number.POSITIVE_INFINITY");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldReturnNullValueWhenExecuteReturnNegativeInfinitJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return Number.NEGATIVE_INFINITY");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldReturnNullValueWhenExecuteReturnNaNJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return Number.NaN");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldReturnReadOnlyCollectionObjectValueWhenExecuteReturnArrayJavaScript()
        {
            // selenium (のChrome Driver) は ReadOnlyCollection を返す模様
            var value = GetExecutor().ExecuteScript("return [123, 456, 789]");
            Assert.AreEqual(typeof(ReadOnlyCollection<object>), value.GetType());

            var expectValues = new[] { 123L, 456L, 789L };
            var resultValues = (ReadOnlyCollection<object>)value;

            Assert.AreEqual(expectValues.Length, resultValues.Count);
            for (var i = 0; i < expectValues.Length; i++)
            {
                Assert.AreEqual(expectValues[i], resultValues[i]);
            }
        }

        [TestMethod]
        public void ShouldReturnTypeMoldeConvertedValueOfElementsInReadOnlyCollectionObjectValueWhenExecuteReturnArrayJavaScript()
        {
            var isoValue = GetExecutor().ExecuteScript("return new Date(2012,0,16,23,23,23,123).toISOString()");
            var value = GetExecutor().ExecuteScript("return [Number.NaN, Number.POSITIVE_INFINITY, new Date(void 0), new Date(2012,0,16,23,23,23,123), 123]");
            Assert.AreEqual(typeof(ReadOnlyCollection<object>), value.GetType());

            var expectValues = new[] { null, null, null, isoValue, 123L };
            var resultValues = (ReadOnlyCollection<object>)value;

            Assert.AreEqual(expectValues.Length, resultValues.Count);
            for (var i = 0; i < expectValues.Length; i++)
            {
                Assert.AreEqual(expectValues[i], resultValues[i]);
            }
        }

        [TestMethod]
        public void ShouldReturnDictionalyStringObjectWhenExecuteReturnObjectJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return {A:'a', B:123, C: false}");
            Assert.AreEqual(typeof(Dictionary<string, object>), value.GetType());

            var resultValue = (Dictionary<string, object>)value;
            Assert.AreEqual("a", resultValue["A"]);
            Assert.AreEqual(123L, resultValue["B"]);
            Assert.AreEqual(false, resultValue["C"]);
        }

        [TestMethod]
        public virtual void ShouldReturnPromiseResultWhenExecuteReturnSuccessPromiseJavaScript()
        {
            var result = GetExecutor().ExecuteScript("return new Promise((resolve, reject) => {  setTimeout(() => resolve(123), 1000); });");
            Assert.AreEqual(123L, result);
        }

        [TestMethod]
        public void ShouldReturnLikeJavaScriptObjectResultWhenExecuteReturnFunctionJavaScript()
        {
            var value = GetExecutor().ExecuteScript(@"
const f = function() { return 123; }; 
f.A = 'a';
f.B = function() {
  return 345;
}
f.B.AA = 'aa';
f.B.BB = 345;
return f;");

            Assert.IsInstanceOfType(value, typeof(Dictionary<string, object>));

            var resultValue = (Dictionary<string, object>)value;
            Assert.AreEqual(2, resultValue.Count);
            Assert.AreEqual("a", resultValue["A"]);

            Assert.IsInstanceOfType(resultValue["B"], typeof(Dictionary<string, object>));
            var bValue = (Dictionary<string, object>)resultValue["B"];
            Assert.AreEqual("aa", bValue["AA"]);
            Assert.AreEqual(345L, bValue["BB"]);

        }

        [TestMethod]
        public void ShouldNotDefinedToGlobalScopeIfValiableDefinedJavaScript()
        {
            var funcName = "___test_define_func";

            GetExecutor().ExecuteScript($"function {funcName}() {{return 1;}}");
            Assert.ThrowsException<WebDriverException>(() => GetExecutor().ExecuteScript($"return {funcName}();"),
                $"javascript error: {funcName} is not defined");
        }

        [TestMethod]
        public void ShouldThisInstanceIsGlobalObjectInstance()
        {
            var isWindow = GetExecutor().ExecuteScript("return this === window");
            Assert.IsTrue((bool)isWindow);
        }

        [TestMethod]
        public void ShouldReturnWebElementWhenExecuteReturnElementScript()
        {
            var value = GetExecutor().ExecuteScript("return document.querySelector('#textBoxName');");
            Assert.IsInstanceOfType(value, typeof(IWebElement));
        }

        [TestMethod]
        public virtual void ShouldReturnWebElementWhenExecuteReturnDocumentScript()
        {
            var value = GetExecutor().ExecuteScript("return document;");
            Assert.IsInstanceOfType(value, typeof(IWebElement));
        }

        [TestMethod]
        public void ShouldRaiseExceptionWhenExecuteReturnWindowScript()
        {
            Assert.ThrowsException<WebDriverException>(() => GetExecutor().ExecuteScript("return window;"));
        }

        [TestMethod]
        public void ShouldReturnReadOnlyCollectionWithWebElementWhenReturnExecuteReturnNodeList()
        {
            var value = GetExecutor().ExecuteScript("return document.querySelectorAll('input');");
            Assert.AreEqual(typeof(ReadOnlyCollection<IWebElement>), value.GetType());
        }

        [TestMethod]
        public void ShouldReturnReadOnlyCollectionWithWebElementWhenReturnExecuteReturnHTMLCollection()
        {
            var value = GetExecutor().ExecuteScript("return document.getElementsByTagName('input');");
            Assert.AreEqual(typeof(ReadOnlyCollection<IWebElement>), value.GetType());
        }

        [TestMethod]
        public void ShouldReturnReadOnlyCollectionWithObjectWhenReturnExecuteReturnArrayIncludeWithVariousTypes()
        {
            var value = GetExecutor().ExecuteScript("return [123, 'AAA', true, document.querySelector('input')];");
            Assert.AreEqual(typeof(ReadOnlyCollection<object>), value.GetType());
            var results = (ReadOnlyCollection<object>)value;
            Assert.AreEqual(123L, results[0]);
            Assert.AreEqual("AAA", results[1]);
            Assert.AreEqual(true, results[2]);
            Assert.IsInstanceOfType(results[3], typeof(IWebElement));
        }

        [TestMethod]
        public void ShouldThrowExeceptionWhenReturnNonElementNode()
        {
            Assert.ThrowsException<WebDriverException>(() => GetExecutor().ExecuteScript(
                "return Array.prototype.slice.call(document.querySelector('form').childNodes)" +
                ".filter(n => n.nodeType !== Node.ELEMENT_NODE)[0]"));
        }

        private void SetupParameterCheckScript()
        {
            GetExecutor().ExecuteScript(@"window._paramcheck = function(params) {
    const args = Array.prototype.slice.call(params);
    return args.map(p => { return {type: Object.prototype.toString.call(p), value: p};});
};");
        }

        private List<(string type, object value)> ToCheckResult(object value)
        {
            var res = (ReadOnlyCollection<object>)value;
            return res.Select(o =>
            {
                var v = (Dictionary<string, object>)o;
                return (v["type"].ToString(), v["value"]);
            }).ToList();
        }

        private List<(string type, object value)> ExecuteParameterCheckString(params object[] args)
        {
            SetupParameterCheckScript();
            return ToCheckResult(GetExecutor().ExecuteScript("return window._paramcheck(arguments);", args));
        }

        [TestMethod]
        public void IntTypeParameterShouldPassedInNumberType()
        {
            var value = ExecuteParameterCheckString(123);
            Assert.AreEqual(1, value.Count);
            Assert.AreEqual("[object Number]", value[0].type);
            Assert.AreEqual(123L, value[0].value);
        }

        [TestMethod]
        public void MultiParametersShouldPassedMultiArguments()
        {
            var value = ExecuteParameterCheckString(123, true, "ABC", 456, 354.234);
            Assert.AreEqual(5, value.Count);
        }

        [TestMethod]
        public void BoolTypeParameterShouldPassedInBooleanType()
        {
            var value = ExecuteParameterCheckString(true, false);
            Assert.AreEqual(2, value.Count);
            Assert.AreEqual("[object Boolean]", value[0].type);
            Assert.AreEqual(true, value[0].value);
            Assert.AreEqual("[object Boolean]", value[1].type);
            Assert.AreEqual(false, value[1].value);
        }

        [TestMethod]
        public void NullParameterShouldPassedInNull()
        {
            var value = ExecuteParameterCheckString(new object[] { null });
            Assert.AreEqual(1, value.Count);
            Assert.AreEqual("[object Null]", value[0].type);
            Assert.IsNull(value[0].value);
        }

        [TestMethod]
        public void StringTypeParamterShouldPassedInStringType()
        {
            var paramValue = @"改行を含む文字列。
どうなるか。そのほか、エスケープも必要。
ダブルクォーテーションとか("")シングルクォーテーションとか'バックスラッシュもそう\\タブ文字とかもあるね\t!";

            var value = ExecuteParameterCheckString(paramValue);

            Assert.AreEqual(1, value.Count);
            Assert.AreEqual("[object String]", value[0].type);
            Assert.AreEqual(paramValue, value[0].value);
        }

        [TestMethod]
        public void ArrayTypeParameterShouldPassedInArrayType()
        {
            IEnumerableParameterShouldPassedInArrayType(new object[] { 1, true, "text" });
        }

        [TestMethod]
        public void ListTypeParameterShouldPassedInArrayType()
        {
            IEnumerableParameterShouldPassedInArrayType(new List<object> { 1, true, "text" });
        }

        [TestMethod]
        public void ReadOnlyCollectionParameterShouldPassedInArrayType()
        {
            IEnumerableParameterShouldPassedInArrayType(new ReadOnlyCollection<object>(new List<object> { 1, true, "text" }));
        }

        private void IEnumerableParameterShouldPassedInArrayType(IEnumerable<object> paramValue)
        {
            var value = ExecuteParameterCheckString(new object[] { paramValue });

            Assert.AreEqual(1, value.Count);
            Assert.AreEqual("[object Array]", value[0].type);

            Assert.IsInstanceOfType(value[0].value, typeof(ReadOnlyCollection<object>));

            var values = value[0].value as ReadOnlyCollection<object>;
            var param = paramValue.ToList();
            Assert.AreEqual(Convert.ToInt64(param[0]), values[0]);
            Assert.AreEqual(param[1], values[1]);
            Assert.AreEqual(param[2], values[2]);
        }

        [TestMethod]
        public void DictionalyParameterShouldPassedInObjectType()
        {
            var param = new Dictionary<string, object>()
            {
                { "key1", 123 }, { "key2", "ABCD" }, { "key3", true }
            };
            var value = ExecuteParameterCheckString(param);

            Assert.AreEqual(1, value.Count);
            Assert.AreEqual("[object Object]", value[0].type);
            var values = value[0].value as Dictionary<string, object>;

            Assert.AreEqual(param.Count, values.Count);
            Assert.AreEqual(Convert.ToInt64(param["key1"]), values["key1"]);
            Assert.AreEqual(param["key2"], values["key2"]);
            Assert.AreEqual(param["key3"], values["key3"]);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenPassedUnsupportedParameterType()
        {
            var paramValue = new Regex("[ABC]");
            Assert.ThrowsException<ArgumentException>(() => ExecuteParameterCheckString(paramValue));
        }

        [TestMethod]
        public void WebElementParameterShouldPassedInElement()
        {
            var input = GetDriver().FindElement(By.Id("textBoxName"));
            var oldvalue = input.GetProperty("value");
            var newvalue = DateTime.Now.ToString();
            GetExecutor().ExecuteScript("arguments[0].value = arguments[1];", input, newvalue);

            Assert.AreNotEqual(oldvalue, newvalue);
            Assert.AreEqual(newvalue, input.GetProperty("value"));
        }


        [TestMethod]
        public void ShouldGetAsyncCallbackResult()
        {
            // var timeout = GetDriver().Manage().Timeouts().AsynchronousJavaScript;
            var result = GetExecutor().ExecuteAsyncScript($@"
const callback = arguments[arguments.length - 1];
const param1 = arguments[0];
window.setTimeout(() => {{
    callback(param1 + "" world"");
}}, {100});
", "Hello");
            //timeout.TotalMilliseconds + 1000
            Assert.AreEqual("Hello world", result);
        }

        [TestMethod]
        public void ExecuteAsyncScript_DictionalyParameterShouldPassedInObject()
        {
            var param = new Dictionary<string, object>()
            {
                { "key1", 123 }, { "key2", "ABCD" }, { "key3", true }
            };
            var result = GetExecutor().ExecuteAsyncScript($@"
const callback = arguments[arguments.length - 1];
const param = arguments[0];
window.setTimeout(() => {{
    callback(param.key1 === 123 && param.key2 === 'ABCD' && param.key3);
}}, {100});
", param);
            result.Is(true);
        }

        [TestMethod]
        public void ExecuteAsyncScript_ShouldReturnDicitionaryWhenReturnObjectFromScript()
        {
            var value = GetExecutor().ExecuteAsyncScript($@"
const callback = arguments[arguments.length - 1];
window.setTimeout(() => {{
    callback({{A:'a', B:123, C: false}});
}}, {100});
");
            Assert.AreEqual(typeof(Dictionary<string, object>), value.GetType());
            var resultValue = (Dictionary<string, object>)value;
            Assert.AreEqual("a", resultValue["A"]);
            Assert.AreEqual(123L, resultValue["B"]);
            Assert.AreEqual(false, resultValue["C"]);
        }
    }
}