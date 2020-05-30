using Codeer.Friendly.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.CefSharp.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class WinFormWebElementTest : WebElementTestBase
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
    }

    [TestClass]
    public class WpfWebElementTest : WebElementTestBase
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
    }

    [TestClass]
    public class SeleniumWebElementTest : WebElementTestBase
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

    public abstract class WebElementTestBase: CompareTestBase
    {
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
            var size = element.Size;

            //Different from browser
        }

        [TestMethod]
        public void Displayed()
        {
            GetDriver().FindElement(By.Id("disabletest")).Displayed.IsTrue();
            GetDriver().FindElement(By.Id("not_displayed")).Displayed.IsFalse();
        }

        [TestMethod]
        public void Clear()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            element.GetAttribute("value").Is("ABCDE");
            element.Clear();
            element.GetAttribute("value").Is("");
        }

        [TestMethod]
        public void GetCssValue()
        {
            var element = GetDriver().FindElement(By.Id("not_displayed"));
            element.GetCssValue("display").Is("none");
        }

        [TestMethod]
        public void Submit()
        {
            var element = GetDriver().FindElement(By.Id("form"));
            element.Submit();
        }

        [TestMethod]
        public void FindElementById_ShouldReturnFirstElement()
        {
            var element = GetDriver().FindElement(By.ClassName("bytest")).FindElement(By.Id("idtest"));
            var dataKey = element.GetAttribute("data-key");
            Assert.AreEqual("1", dataKey);
        }


        [TestMethod]
        public void FindElementByName_ShouldReturnFirstElement()
        {
            var element = GetDriver().FindElement(By.ClassName("bytest")).FindElement(By.Name("nametest"));
            var dataKey = element.GetAttribute("data-key");
            Assert.AreEqual("1", dataKey);
        }

        [TestMethod]
        public void FindElementByClassName_ShouldReturnFirstElement()
        {
            var element = GetDriver().FindElement(By.ClassName("bytest")).FindElement(By.ClassName("classtest"));
            var dataKey = element.GetAttribute("data-key");
            Assert.AreEqual("1", dataKey);
        }

        [TestMethod]
        public void FindElementByTagName_ShouldReturnFirstElement()
        {
            var element = GetDriver().FindElement(By.ClassName("bytest")).FindElement(By.TagName("tagtest"));
            var dataKey = element.GetAttribute("data-key");
            Assert.AreEqual("1", dataKey);
        }

        [TestMethod]
        public void FindElementByCssSelector_ShouldReturnFirstElement()
        {
            var element = GetDriver().FindElement(By.ClassName("bytest")).FindElement(By.CssSelector("#idtest[name='nametest']"));
            var dataKey = element.GetAttribute("data-key");
            Assert.AreEqual("1", dataKey);
        }

        [TestMethod]
        public void FindElementsById_ShouldReturnAllElements()
        {
            var elements = GetDriver().FindElement(By.ClassName("bytest")).FindElements(By.Id("idtest"));
            Assert.AreEqual(3, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
            Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
        }

        [TestMethod]
        public void FindElementsById_ShouldReturnEmptyWhenElementsNotFound()
        {
            var elements = GetDriver().FindElement(By.ClassName("bytest")).FindElements(By.Id("idtest_no"));
            Assert.AreEqual(0, elements.Count);
        }

        [TestMethod]
        public void FindElementsByName_ShouldReturnAllElements()
        {
            var elements = GetDriver().FindElement(By.ClassName("bytest")).FindElements(By.Name("nametest"));
            Assert.AreEqual(3, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
            Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
        }

        [TestMethod]
        public void FindElementsByName_ShouldReturnEmptyWhenElementsNotFound()
        {
            var elements = GetDriver().FindElement(By.ClassName("bytest")).FindElements(By.Name("nametest_no"));
            Assert.AreEqual(0, elements.Count);
        }

        [TestMethod]
        public void FindElementsByClassName_ShouldReturnAllElements()
        {
            var elements = GetDriver().FindElement(By.ClassName("bytest")).FindElements(By.ClassName("classtest"));
            Assert.AreEqual(3, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
            Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
        }

        [TestMethod]
        public void FindElementsByClassName_ShouldReturnEmptyWhenElementsNotFound()
        {
            var elements = GetDriver().FindElement(By.ClassName("bytest")).FindElements(By.ClassName("classtest_no"));
            Assert.AreEqual(0, elements.Count);
        }

        [TestMethod]
        public void FindElementsByCssSelector_ShouldReturnAllElements()
        {
            var elements = GetDriver().FindElement(By.ClassName("bytest")).FindElements(By.CssSelector("#idtest[name='nametest']"));
            Assert.AreEqual(3, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
            Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
        }

        [TestMethod]
        public void FindElementsByCssSelector_ShouldReturnEmptyWhenElementsNotFound()
        {
            var elements = GetDriver().FindElement(By.ClassName("bytest")).FindElements(By.CssSelector("#idtest[name='nametest_no']"));
            Assert.AreEqual(0, elements.Count);
        }

        [TestMethod]
        public void FindElementsByTagName_ShouldReturnAllElements()
        {
            var elements = GetDriver().FindElement(By.ClassName("bytest")).FindElements(By.TagName("tagtest"));

            Assert.AreEqual(2, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
        }

        [TestMethod]
        public void FindElementsByTagName_ShouldReturnEmptyWhenElementsNotFound()
        {
            var elements = GetDriver().FindElement(By.ClassName("bytest")).FindElements(By.TagName("tagtest_no"));
            Assert.AreEqual(0, elements.Count);
        }

        [TestMethod]
        public void FindElementByXPath_ShouldReturnFirstElement()
        {
            var context = GetDriver().FindElement(By.XPath("/html/body/div[1]"));
            var element = context.FindElement(By.XPath("tagtest"));
            var dataKey = element.GetAttribute("data-key");
            dataKey.Is("1");
        }

        [TestMethod]
        public void FindElementByXPath_ShouldThrowExceptionWhenElementNotFound()
        {
            var context = GetDriver().FindElement(By.XPath("/html/body/div[1]"));
            Assert.ThrowsException<NoSuchElementException>(() => context.FindElement(By.XPath("tagtest1")));
        }

        [TestMethod]
        public void FindElementsByXPath_ShouldReturnAllElements()
        {
            var context = GetDriver().FindElement(By.XPath("/html/body/div[1]"));
            var elements = context.FindElements(By.XPath("tagtest"));
            elements.Count.Is(2);
            elements[0].GetAttribute("data-key").Is("1");
            elements[1].GetAttribute("data-key").Is("2");
        }

        [TestMethod]
        public void FindElementsByXPath_ShouldReturnEmptyWhenElementsNotFound()
        {
            var context = GetDriver().FindElement(By.XPath("/html/body/div[1]"));
            var elements = context.FindElements(By.XPath("tagtest_no"));
            elements.Count.Is(0);
        }

    }
}
