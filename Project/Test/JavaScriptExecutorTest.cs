using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace Test
{
    public abstract class JavaScriptExecutorTest : CompareTestBase
    {
        public class Forms : JavaScriptExecutorTest
        {
            public Forms() : base(new FormsAgent()) { }

            [Ignore("This testcase not supported. Because CefSharp returns a null object result when execution result of the javascript is Promise.")]
            public override void ShouldReturnPromiseResultWhenExecuteReturnSuccessPromiseJavaScript() { }

            [Ignore("This testcase not supported. Because Document cannot be identified in the current processing method after the second time.")]
            public override void ShouldReturnWebElementWhenExecuteReturnDocumentScript() { }
        }

        public class Wpf : JavaScriptExecutorTest
        {
            public Wpf() : base(new WpfAgent()) { }

            [Ignore("This testcase not supported. Because CefSharp returns a null object result when execution result of the javascript is Promise.")]
            public override void ShouldReturnPromiseResultWhenExecuteReturnSuccessPromiseJavaScript() { }

            [Ignore("This testcase not supported. Because Document cannot be identified in the current processing method after the second time.")]
            public override void ShouldReturnWebElementWhenExecuteReturnDocumentScript() { }
        }

        public class Web : JavaScriptExecutorTest
        {
            public Web() : base(new WebAgent()) { }
        }

        protected JavaScriptExecutorTest(INeed need) : base(need) { }

        [SetUp]
        public void SetUp()
            => GetDriver().Url = HtmlServer.Instance.RootUrl + "Controls.html";

        [Test]
        public void ShouldReturnSameStringWhenExecuteReturnSingleQuotationStringJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return 'abcde'");
            Assert.AreEqual("abcde", value);
        }

        [Test]
        public void ShouldReturnSameStringWhenExecuteReturnDoubleQuotationStringJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return \"abcde\"");
            Assert.AreEqual("abcde", value);
        }

        [Test]
        public void ShouldReturnSameStringWhenExecuteReturnBackQuotationStringJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return `abcde`");
            Assert.AreEqual("abcde", value);
        }

        [Test]
        public void ShouldReturnLongValueWhenExecuteReturnIntegerJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return 12345");
            Assert.AreEqual(12345L, value);
        }

        [Test]
        public void ShouldReturnDoubleValueWhenExecuteReturnDecimalJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return 12345.678");
            Assert.AreEqual(12345.678, value);
        }

        [Test]
        public void ShouldReturnBooleanValueWhenExecuteReturnBooleanJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return false");
            Assert.IsFalse((bool)value);
        }

        [Test]
        public void ShouldReturnIsoDateStringWhenExecuteReturnDateJavaScript()
        {
            // selenium return ISOString's result.
            var isoValue = GetExecutor().ExecuteScript("return new Date(2012,0,16,23,23,23,123).toISOString()");
            var value = GetExecutor().ExecuteScript("return new Date(2012,0,16,23,23,23,123)");
            Assert.AreEqual(isoValue, value);
        }

        [Test]
        public void ShouldReturnNullValueWhenExecuteReturnInvalidJavaScript()
        {
            // selenium return Invalid Date to null.
            var value = GetExecutor().ExecuteScript("return new Date(void 0)");
            Assert.IsNull(value);
        }

        [Test]
        public void ShouldReturnNullValueWhenExecuteReturnUndefinedJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return void 0");
            Assert.IsNull(value);
        }

        [Test]
        public void ShouldReturnNullValueWhenExecuteReturnPositiveInfinitJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return Number.POSITIVE_INFINITY");
            Assert.IsNull(value);
        }

        [Test]
        public void ShouldReturnNullValueWhenExecuteReturnNegativeInfinitJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return Number.NEGATIVE_INFINITY");
            Assert.IsNull(value);
        }

        [Test]
        public void ShouldReturnNullValueWhenExecuteReturnNaNJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return Number.NaN");
            Assert.IsNull(value);
        }

        [Test]
        public void ShouldReturnReadOnlyCollectionObjectValueWhenExecuteReturnArrayJavaScript()
        {
            // selenium return ReadOnlyCollection.
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

        [Test]
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

        [Test]
        public void ShouldReturnDictionalyStringObjectWhenExecuteReturnObjectJavaScript()
        {
            var value = GetExecutor().ExecuteScript("return {A:'a', B:123, C: false}");
            Assert.AreEqual(typeof(Dictionary<string, object>), value.GetType());

            var resultValue = (Dictionary<string, object>)value;
            Assert.AreEqual("a", resultValue["A"]);
            Assert.AreEqual(123L, resultValue["B"]);
            Assert.AreEqual(false, resultValue["C"]);
        }

        [Test]
        public virtual void ShouldReturnPromiseResultWhenExecuteReturnSuccessPromiseJavaScript()
        {
            var result = GetExecutor().ExecuteScript("return new Promise((resolve, reject) => {  setTimeout(() => resolve(123), 1000); });");
            Assert.AreEqual(123L, result);
        }

        [Test]
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

            AssertCompatible.IsInstanceOfType(value, typeof(Dictionary<string, object>));

            var resultValue = (Dictionary<string, object>)value;
            Assert.AreEqual(2, resultValue.Count);
            Assert.AreEqual("a", resultValue["A"]);

            AssertCompatible.IsInstanceOfType(resultValue["B"], typeof(Dictionary<string, object>));
            var bValue = (Dictionary<string, object>)resultValue["B"];
            Assert.AreEqual("aa", bValue["AA"]);
            Assert.AreEqual(345L, bValue["BB"]);

        }

        [Test]
        public void ShouldNotDefinedToGlobalScopeIfValiableDefinedJavaScript()
        {
            var funcName = "___test_define_func";

            GetExecutor().ExecuteScript($"function {funcName}() {{return 1;}}");
            AssertCompatible.ThrowsException<WebDriverException>(() => GetExecutor().ExecuteScript($"return {funcName}();"),
                $"javascript error: {funcName} is not defined");
        }

        [Test]
        public void ShouldThisInstanceIsGlobalObjectInstance()
        {
            var isWindow = GetExecutor().ExecuteScript("return this === window");
            Assert.IsTrue((bool)isWindow);
        }

        [Test]
        public void ShouldReturnWebElementWhenExecuteReturnElementScript()
        {
            var value = GetExecutor().ExecuteScript("return document.querySelector('#textBoxName');");
            AssertCompatible.IsInstanceOfType(value, typeof(IWebElement));
        }

        [Test]
        public virtual void ShouldReturnWebElementWhenExecuteReturnDocumentScript()
        {
            var value = GetExecutor().ExecuteScript("return document;");
            AssertCompatible.IsInstanceOfType(value, typeof(IWebElement));
        }

        [Test]
        public void ShouldRaiseExceptionWhenExecuteReturnWindowScript()
        {
            AssertCompatible.ThrowsException<WebDriverException>(() => GetExecutor().ExecuteScript("return window;"));
        }

        [Test]
        public void ShouldReturnReadOnlyCollectionWithWebElementWhenReturnExecuteReturnNodeList()
        {
            var value = GetExecutor().ExecuteScript("return document.querySelectorAll('input');");
            Assert.AreEqual(typeof(ReadOnlyCollection<IWebElement>), value.GetType());
        }

        [Test]
        public void ShouldReturnReadOnlyCollectionWithWebElementWhenReturnExecuteReturnHTMLCollection()
        {
            var value = GetExecutor().ExecuteScript("return document.getElementsByTagName('input');");
            Assert.AreEqual(typeof(ReadOnlyCollection<IWebElement>), value.GetType());
        }

        [Test]
        public void ShouldReturnReadOnlyCollectionWithObjectWhenReturnExecuteReturnArrayIncludeWithVariousTypes()
        {
            var value = GetExecutor().ExecuteScript("return [123, 'AAA', true, document.querySelector('input')];");
            Assert.AreEqual(typeof(ReadOnlyCollection<object>), value.GetType());
            var results = (ReadOnlyCollection<object>)value;
            Assert.AreEqual(123L, results[0]);
            Assert.AreEqual("AAA", results[1]);
            Assert.AreEqual(true, results[2]);
            AssertCompatible.IsInstanceOfType(results[3], typeof(IWebElement));
        }

        [Test]
        public void ShouldThrowExeceptionWhenReturnNonElementNode()
        {
            AssertCompatible.ThrowsException<WebDriverException>(() => GetExecutor().ExecuteScript(
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

        [Test]
        public void IntTypeParameterShouldPassedInNumberType()
        {
            var value = ExecuteParameterCheckString(123);
            Assert.AreEqual(1, value.Count);
            Assert.AreEqual("[object Number]", value[0].type);
            Assert.AreEqual(123L, value[0].value);
        }

        [Test]
        public void MultiParametersShouldPassedMultiArguments()
        {
            var value = ExecuteParameterCheckString(123, true, "ABC", 456, 354.234);
            Assert.AreEqual(5, value.Count);
        }

        [Test]
        public void BoolTypeParameterShouldPassedInBooleanType()
        {
            var value = ExecuteParameterCheckString(true, false);
            Assert.AreEqual(2, value.Count);
            Assert.AreEqual("[object Boolean]", value[0].type);
            Assert.AreEqual(true, value[0].value);
            Assert.AreEqual("[object Boolean]", value[1].type);
            Assert.AreEqual(false, value[1].value);
        }

        [Test]
        public void NullParameterShouldPassedInNull()
        {
            var value = ExecuteParameterCheckString(new object[] { null });
            Assert.AreEqual(1, value.Count);
            Assert.AreEqual("[object Null]", value[0].type);
            Assert.IsNull(value[0].value);
        }

        [Test]
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

        [Test]
        public void ArrayTypeParameterShouldPassedInArrayType()
        {
            IEnumerableParameterShouldPassedInArrayType(new object[] { 1, true, "text" });
        }

        [Test]
        public void ListTypeParameterShouldPassedInArrayType()
        {
            IEnumerableParameterShouldPassedInArrayType(new List<object> { 1, true, "text" });
        }

        [Test]
        public void ReadOnlyCollectionParameterShouldPassedInArrayType()
        {
            IEnumerableParameterShouldPassedInArrayType(new ReadOnlyCollection<object>(new List<object> { 1, true, "text" }));
        }

        private void IEnumerableParameterShouldPassedInArrayType(IEnumerable<object> paramValue)
        {
            var value = ExecuteParameterCheckString(new object[] { paramValue });

            Assert.AreEqual(1, value.Count);
            Assert.AreEqual("[object Array]", value[0].type);

            AssertCompatible.IsInstanceOfType(value[0].value, typeof(ReadOnlyCollection<object>));

            var values = value[0].value as ReadOnlyCollection<object>;
            var param = paramValue.ToList();
            Assert.AreEqual(Convert.ToInt64(param[0]), values[0]);
            Assert.AreEqual(param[1], values[1]);
            Assert.AreEqual(param[2], values[2]);
        }

        [Test]
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

        [Test]
        public void ShouldThrowExceptionWhenPassedUnsupportedParameterType()
        {
            var paramValue = new Regex("[ABC]");
            AssertCompatible.ThrowsException<ArgumentException>(() => ExecuteParameterCheckString(paramValue));
        }

        [Test]
        public void WebElementParameterShouldPassedInElement()
        {
            var input = GetDriver().FindElement(By.Id("textBoxName"));
            var oldvalue = input.GetProperty("value");
            var newvalue = DateTime.Now.ToString();
            GetExecutor().ExecuteScript("arguments[0].value = arguments[1];", input, newvalue);

            Assert.AreNotEqual(oldvalue, newvalue);
            Assert.AreEqual(newvalue, input.GetProperty("value"));
        }


        [Test]
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

        [Test]
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

        [Test]
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
