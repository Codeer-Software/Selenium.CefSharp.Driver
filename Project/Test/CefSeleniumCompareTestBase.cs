using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.CefSharp.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Linq;
using System.Text.RegularExpressions;

namespace Test
{
    [TestClass]
    public class CefSelemiumCompareTestForCef : CefSeleniumCompareTestBase
    {
        WindowsAppFriend _app;
        CefSharpDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [TestInitialize]
        public void TestInitialize()
        {
            //start process.
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            var processPath = Path.Combine(dir, @"CefSharpWPFSample\bin\x86\Debug\CefSharpWPFSample.exe");
            var process = Process.Start(processPath);

            //attach by friendly.
            _app = new WindowsAppFriend(process);
            var main = _app.Type<Application>().Current.MainWindow;

            //create driver.
            _driver = new CefSharpDriver(main._browser);
            _driver.Url = this.GetHtmlUrl();
        }

        [TestCleanup]
        public void TestCleanup() => Process.GetProcessById(_app.ProcessId).Kill();

        [ClassInitialize]
        public static void ClassInit(TestContext context) => ClassInitBase();

        [ClassCleanup]
        public static void ClassCleanup() => ClassCleanupBase();


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
    public class CefSelemiumCompareTestForSelenium : CefSeleniumCompareTestBase
    {
        IWebDriver _driver;

        public override IWebDriver GetDriver() => _driver;


        [TestInitialize]
        public void initialize()
        {
            _driver = new ChromeDriver();
            _driver.Url = this.GetHtmlUrl();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _driver.Dispose();
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context) => ClassInitBase();

        [ClassCleanup]
        public static void ClassCleanup() => ClassCleanupBase();
    }

    public abstract class CefSeleniumCompareTestBase
    {
        protected static HtmlServer server = null;

        protected static void ClassInitBase()
        {
            server = HtmlServer.CreateFromFile("Controls.html");
        }

        protected static void ClassCleanupBase()
        {
            if (server != null) server.Close();
        }

        public abstract IWebDriver GetDriver();

        private IJavaScriptExecutor GetExecutor() => (IJavaScriptExecutor)GetDriver();

        protected string GetHtmlUrl() => server?.Url;

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

        //FindElement(s)ById

        [TestMethod]
        public void ShouldGetFirstElementWhenUsedFindElementById()
        {
            var element = GetDriver().FindElement(By.Id("idtest"));
            var dataKey = element.GetAttribute("data-key");
            Assert.AreEqual("1", dataKey);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementById()
        {
            Assert.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.Id("idtest_no")));
        }

        [TestMethod]
        public void ShouldGetAllElementWhenUsedFindElementsById()
        {
            var elements = GetDriver().FindElements(By.Id("idtest"));
            Assert.AreEqual(2, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
        }

        [TestMethod]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsById()
        {
            var elements = GetDriver().FindElements(By.Id("idtest_no"));
            Assert.AreEqual(0, elements.Count);
        }

        //FindElement(s)ByName

        [TestMethod]
        public void ShouldGetFirstElementWhenUsedFindElementByName()
        {
            var element = GetDriver().FindElement(By.Name("nametest"));
            var dataKey = element.GetAttribute("data-key");
            Assert.AreEqual("1", dataKey);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementByName()
        {
            Assert.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.Name("nametest_no")));
        }

        [TestMethod]
        public void ShouldGetAllElementWhenUsedFindElementsByName()
        {
            var elements = GetDriver().FindElements(By.Name("nametest"));
            Assert.AreEqual(2, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
        }

        [TestMethod]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsByName()
        {
            var elements = GetDriver().FindElements(By.Name("nametest_no"));
            Assert.AreEqual(0, elements.Count);
        }

        //FindElement(s)ByClassName

        [TestMethod]
        public void ShouldGetFirstElementWhenUsedFindElementByClassName()
        {
            var element = GetDriver().FindElement(By.ClassName("classtest"));
            var dataKey = element.GetAttribute("data-key");
            Assert.AreEqual("1", dataKey);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementByClassName()
        {
            Assert.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.ClassName("classtest_no")));
        }

        [TestMethod]
        public void ShouldGetAllElementWhenUsedFindElementsByClassName()
        {
            var elements = GetDriver().FindElements(By.ClassName("classtest"));
            Assert.AreEqual(2, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
        }

        [TestMethod]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsByClassName()
        {
            var elements = GetDriver().FindElements(By.ClassName("classtest_no"));
            Assert.AreEqual(0, elements.Count);
        }

        //FindElement(s)ByCssSelector

        [TestMethod]
        public void ShouldGetFirstElementWhenUsedFindElementByCssSelector()
        {
            var element = GetDriver().FindElement(By.CssSelector(".bytest > #idtest[name='nametest']"));
            var dataKey = element.GetAttribute("data-key");
            Assert.AreEqual("1", dataKey);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementByCssSelector()
        {
            Assert.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.CssSelector(".bytest > #idtest_no[name='nametest']")));
        }

        [TestMethod]
        public void ShouldGetAllElementWhenUsedFindElementsByCssSelector()
        {
            var elements = GetDriver().FindElements(By.CssSelector(".bytest > #idtest[name='nametest']"));
            Assert.AreEqual(2, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
        }

        [TestMethod]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsByCssSelector()
        {
            var elements = GetDriver().FindElements(By.CssSelector(".bytest > #idtest[name='nametest_no']"));
            Assert.AreEqual(0, elements.Count);
        }

        //FindElement(s)ByTagName

        [TestMethod]
        public void ShouldGetFirstElementWhenUsedFindElementByTagName()
        {
            var element = GetDriver().FindElement(By.TagName("tagtest"));
            var dataKey = element.GetAttribute("data-key");
            Assert.AreEqual("1", dataKey);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementByTagName()
        {
            Assert.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.TagName("tagtest_no")));
        }

        [TestMethod]
        public void ShouldGetAllElementWhenUsedFindElementsByTagName()
        {
            var elements = GetDriver().FindElements(By.TagName("tagtest"));
            Assert.AreEqual(2, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
        }

        [TestMethod]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsByTagName()
        {
            var elements = GetDriver().FindElements(By.TagName("tagtest_no"));
            Assert.AreEqual(0, elements.Count);
        }

        // Other

        [TestMethod]
        public void ShouldThrowExceptionWhenReferenceTheRemovedElement()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            Assert.IsInstanceOfType(element, typeof(IWebElement));
            element.SendKeys("ABC");
            GetExecutor().ExecuteScript("const elem = document.querySelector('#textBoxName'); elem.parentNode.removeChild(elem);");
            Assert.ThrowsException<StaleElementReferenceException>(() => element.SendKeys("DEF"));

            GetExecutor().ExecuteScript(@"
const elem = document.createElement('input');
elem.setAttribute('id', 'textBoxName');
document.body.appendChild(elem);");

            Assert.ThrowsException<StaleElementReferenceException>(() => element.SendKeys("DEF"));

            element = GetDriver().FindElement(By.Id("textBoxName"));
            Assert.IsInstanceOfType(element, typeof(IWebElement));
            element.SendKeys("ABC");
        }

        [TestMethod]
        public void TagName()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            element.TagName.Is("input");
        }

        [TestMethod]
        public void Text()
        {
            var element = GetDriver().FindElement(By.Id("labelTitle"));
            element.Text.Is("Title Controls");
        }

        [TestMethod]
        public void Enabled()
        {
            GetDriver().FindElement(By.Id("textBoxName")).Enabled.IsTrue();
            GetDriver().FindElement(By.Id("disabletest")).Enabled.IsFalse();
        }

        [TestMethod]
        public void Selected()
        {
            var checkBox = GetDriver().FindElement(By.Id("checkBoxCellPhone"));
            checkBox.Selected.IsFalse();
            checkBox.Click();
            checkBox.Selected.IsTrue();

            GetDriver().FindElement(By.Id("opt0")).Selected.IsTrue();
            GetDriver().FindElement(By.Id("opt1")).Selected.IsFalse();

            GetDriver().FindElement(By.Id("radioMan")).Selected.IsTrue();
            GetDriver().FindElement(By.Id("radioWoman")).Selected.IsFalse();
        }

        [TestMethod]
        public void Location()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            var x = element.Location;

            //Different from browser
        }

        [TestMethod]
        public void Size()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            element.Size.Is(new System.Drawing.Size(173, 21));
        }

        [TestMethod]
        public void Displayed()
        {
            var element = GetDriver().FindElement(By.Id("disabletest"));
            var x = element.Displayed;

        }
        // Parameter

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
    }
}