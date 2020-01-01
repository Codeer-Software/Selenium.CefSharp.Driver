using Codeer.Friendly.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Selenium.CefSharp.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Codeer.Friendly.Dynamic;
using OpenQA.Selenium.Chrome;
using System.Net.Sockets;
using System.Net;

namespace Test
{
    [TestClass]
    public class CefSelemiumCompareTestForCef : CefSeleniumCompareTestBase
    {
        WindowsAppFriend _app;
        CefSharpDriver _driver;
        string _htmlPath;

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
            var dir = typeof(CefSeleniumCompareTestBase).Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);

            var file = Path.Combine(dir, @"Test\Controls.html");
            server = HtmlServer.Create(File.ReadAllText(file));
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
    }
}
