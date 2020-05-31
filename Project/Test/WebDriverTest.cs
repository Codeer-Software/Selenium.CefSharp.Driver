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
using System.Linq;
using System.Text.RegularExpressions;
using Codeer.Friendly.Windows.Grasp;

namespace Test
{
    [TestClass]
    public class WebDriverTestWinForm : WebDriverTestBase
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
    public class WebDriverTestWPF : WebDriverTestBase
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
    public class WebDriverTestSelenium : WebDriverTestBase
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

    public abstract class WebDriverTestBase: CompareTestBase
    {
        
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
            Assert.AreEqual(3, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
            Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
        }

        [TestMethod]
        public void ShouldIgnoreQuerySelectorNameWhenUsedFindElementsById()
        {
            var elements = GetDriver().FindElements(By.Id("form .term"));
            Assert.AreEqual(0, elements.Count);
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
            Assert.AreEqual(3, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
            Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
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
            Assert.AreEqual(3, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
            Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
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

        //FindElement(s)ByXPath
        [TestMethod]
        public void ShouldGetFirstElementWhenUsedFindElementByXPath()
        {
            var element = GetDriver().FindElement(By.XPath("/html/body/div[1]/tagtest"));
            var dataKey = element.GetAttribute("data-key");
            Assert.AreEqual("1", dataKey);
        }

        [TestMethod]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementByXPath()
        {
            Assert.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.XPath("/html/body/div[1]/tagtest_no")));
        }

        [TestMethod]
        public void ShouldGetAllElementWhenUsedFindElementsByXPath()
        {
            var elements = GetDriver().FindElements(By.XPath("/html/body/div[1]/tagtest"));
            Assert.AreEqual(2, elements.Count);
            Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
            Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
        }

        [TestMethod]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsByXPath()
        {
            var elements = GetDriver().FindElements(By.XPath("/html/body/div[1]/tagtest_no"));
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
    }
}