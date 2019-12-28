using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public abstract class SpikesBase
    {
        public abstract IWebDriver GetDriver();
        
        [TestMethod]
        public void 単純なシングルクォーテーションで囲まれた文字列()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return 'abcde'");
            Assert.AreEqual("abcde", value);
        }

        [TestMethod]
        public void 単純なダブルクォーテーションで囲まれた文字列()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return \"abcde\"");
            Assert.AreEqual("abcde", value);
        }

        [TestMethod]
        public void 単純なバッククォーテーションで囲まれた文字列()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return `abcde`");
            Assert.AreEqual("abcde", value);
        }

        [TestMethod]
        public void 単純な整数値_long()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return 12345");
            Assert.AreEqual(12345L, value);
        }

        [TestMethod]
        public void 単純な小数点()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return 12345.678");
            Assert.AreEqual(12345.678, value);
        }

        [TestMethod]
        public void 単純な真偽値()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return false");
            Assert.IsFalse((bool)value);
        }

        [TestMethod]
        public void 単純な日付()
        {
            // selenium (のChrome Driver) は ISOString の結果を返す模様
            var isoValue = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return new Date(2012,0,16,23,23,23,123).toISOString()");
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return new Date(2012,0,16,23,23,23,123)");
            Assert.AreEqual(isoValue, value);
        }

        [TestMethod]
        public void 無効な日付()
        {
            // selenium (のChrome Driver) は Invalid Date は null を返す模様
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return new Date(void 0)");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void アンデファインド()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return void 0");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ポジティブインフィニティ()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return Number.POSITIVE_INFINITY");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ネガティブインフィニティ()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return Number.NEGATIVE_INFINITY");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void ナン()
        {
            var value = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return Number.NaN");
            Assert.IsNull(value);
        }

        [TestMethod]
        public void 単純な配列()
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
        public void JavaScript側で型変換が入る配列()
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
        public void 単純なオブジェクト()
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
