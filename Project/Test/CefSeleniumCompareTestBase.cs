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

namespace Test
{
    [TestClass]
    public class CefSelemiumCompareTestForCef : CefSeleniumCompareTestBase
    {
        WindowsAppFriend _app;
        CefSharpDriver _driver;
        string _htmlPath;

        [TestInitialize]
        public void TestInitialize()
        {
            //start process.
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            var processPath = Path.Combine(dir, @"CefSharpWPFSample\bin\x86\Debug\CefSharpWPFSample.exe");
            var process = Process.Start(processPath);

            //html
            _htmlPath = Path.Combine(dir, @"Test\Controls.html");

            //attach by friendly.
            _app = new WindowsAppFriend(process);
            var main = _app.Type<Application>().Current.MainWindow;

            //create driver.
            _driver = new CefSharpDriver(main._browser);
            _driver.Url = _htmlPath;
        }

        [TestCleanup]
        public void TestCleanup() => Process.GetProcessById(_app.ProcessId).Kill();

        public override IWebDriver GetDriver() => _driver;
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
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);

            _driver.Url = Path.Combine(dir, @"Test\Controls.html");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _driver.Dispose();
        }
    }

    public abstract class CefSeleniumCompareTestBase
    {
        public abstract IWebDriver GetDriver();
        
        [TestMethod]
        public void ShouldReturnSameStringWhenExecuteReturnSingleQuotationStringJavaScript()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return 'abcde'");
            Assert.AreEqual("abcde", value);
        }

        [TestMethod]
        public void ShouldReturnSameStringWhenExecuteReturnDoubleQuotationStringJavaScript()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return \"abcde\"");
            Assert.AreEqual("abcde", value);
        }

        [TestMethod]
        public void ShouldReturnSameStringWhenExecuteReturnBackQuotationStringJavaScript()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return `abcde`");
            Assert.AreEqual("abcde", value);
        }

        [TestMethod]
        public void ShouldReturnLongValueWhenExecuteReturnIntegerJavaScript()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return 12345");
            Assert.AreEqual(12345L, value);
        }

        [TestMethod]
        public void ShouldReturnDoubleValueWhenExecuteReturnDecimalJavaScript()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return 12345.678");
            Assert.AreEqual(12345.678, value);
        }

        [TestMethod]
        public void ShouldReturnBooleanValueWhenExecuteReturnBooleanJavaScript()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return false");
            Assert.IsFalse((bool)value);
        }

        [TestMethod]
        public void ShouldReturnIsoDateStringWhenExecuteReturnDateJavaScript()
        {
            // selenium (のChrome Driver) は ISOString の結果を返す模様
            var isoValue = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return new Date(2012,0,16,23,23,23,123).toISOString()");
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return new Date(2012,0,16,23,23,23,123)");
            Assert.AreEqual(isoValue, value);
        }

        [TestMethod]
        public void ShouldReturnNullValueWhenExecuteReturnInvalidJavaScript()
        {
            // selenium (のChrome Driver) は Invalid Date は null を返す模様
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return new Date(void 0)");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldReturnNullValueWhenExecuteReturnUndefinedJavaScript()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return void 0");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldReturnNullValueWhenExecuteReturnPositiveInfinitJavaScript()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return Number.POSITIVE_INFINITY");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldReturnNullValueWhenExecuteReturnNegativeInfinitJavaScript()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return Number.NEGATIVE_INFINITY");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldReturnNullValueWhenExecuteReturnNaNJavaScript()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return Number.NaN");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ShouldReturnReadOnlyCollectionObjectValueWhenExecuteReturnArrayJavaScript()
        {
            // selenium (のChrome Driver) は ReadOnlyCollection を返す模様
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return [123, 456, 789]");
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
            var isoValue = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return new Date(2012,0,16,23,23,23,123).toISOString()");
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return [Number.NaN, Number.POSITIVE_INFINITY, new Date(void 0), new Date(2012,0,16,23,23,23,123), 123]");
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
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return {A:'a', B:123, C: false}");
            Assert.AreEqual(typeof(Dictionary<string, object>), value.GetType());

            var resultValue = (Dictionary<string, object>)value;
            Assert.AreEqual("a", resultValue["A"]);
            Assert.AreEqual(123L, resultValue["B"]);
            Assert.AreEqual(false, resultValue["C"]);
        }
    }
}
