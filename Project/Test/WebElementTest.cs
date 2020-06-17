using Codeer.Friendly.Windows;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Internal;
using Selenium.CefSharp.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;

namespace Test
{
    public class WebElementTestWinForm : WebElementTestBase
    {
        CefSharpDriver _driver;

        public override IWebDriver GetDriver()
            => _driver;

        [SetUp]
        public void SetUp()
            => _driver.Url = this.GetHtmlUrl();

        [OneTimeSetUp]
        public void ClassInit()
            => _driver = AppRunner.RunWinFormApp();

        [OneTimeTearDown]
        public void OneTimeTearDown()
            => Process.GetProcessById(_driver.App.ProcessId).Kill();
    }

    public class WebElementTestWPF : WebElementTestBase
    {
        CefSharpDriver _driver;

        public override IWebDriver GetDriver()
            => _driver;

        [SetUp]
        public void SetUp()
            => _driver.Url = this.GetHtmlUrl();

        [OneTimeSetUp]
        public void ClassInit()
            => _driver = AppRunner.RunWpfApp();

        [OneTimeTearDown]
        public void OneTimeTearDown()
            => Process.GetProcessById(_driver.App.ProcessId).Kill();
    }

    public class WebElementTestSelenium : WebElementTestBase
    {
        IWebDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [SetUp]
        public void initialize()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [OneTimeSetUp]
        public void ClassInit()
        {
            _driver = new ChromeDriver();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _driver.Dispose();
        }
    }

    public abstract class WebElementTestBase: CompareTestBase
    {
        [Test]
        public void ScreenShot()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            var screenShot = (ITakesScreenshot)element;
            var path = Path.GetTempFileName();
            screenShot.GetScreenshot().SaveAsFile(path, ScreenshotImageFormat.Png);
            File.Delete(path);
        }

        [Test]
        public void Locatable()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            var locatable = (ILocatable)element;
            var locationOnScreenOnceScrolledIntoView = locatable.LocationOnScreenOnceScrolledIntoView;
            var coordinates = locatable.Coordinates;

            var id = coordinates.AuxiliaryLocator;
            var locationInDom = coordinates.LocationInDom;
            var locationInViewport = coordinates.LocationInViewport;
            AssertCompatible.ThrowsException<Exception>(()=> coordinates.LocationOnScreen);
        }

        [Test]
        public void TagName()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            element.TagName.Is("input");
        }

        [Test]
        public void Text()
        {
            var element = GetDriver().FindElement(By.Id("labelTitle"));
            element.Text.Is("Title Controls");
        }

        [Test]
        public void Enabled()
        {
            GetDriver().FindElement(By.Id("textBoxName")).Enabled.IsTrue();
            GetDriver().FindElement(By.Id("disabletest")).Enabled.IsFalse();
        }

        [Test]
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

        [Test]
        public void Location()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            var x = element.Location;

            //Different from browser
        }

        [Test]
        public void Size()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            var size = element.Size;

            //Different from browser
        }

        [Test]
        public void Displayed()
        {
            GetDriver().FindElement(By.Id("disabletest")).Displayed.IsTrue();
            GetDriver().FindElement(By.Id("not_displayed")).Displayed.IsFalse();
        }

        [Test]
        public void Clear()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            element.GetAttribute("value").Is("ABCDE");
            element.Clear();
            element.GetAttribute("value").Is("");
        }

        [Test]
        public void GetCssValue()
        {
            var element = GetDriver().FindElement(By.Id("not_displayed"));
            element.GetCssValue("display").Is("none");
        }

        [Test]
        public void Submit()
        {
            var element = GetDriver().FindElement(By.Id("form"));
            element.Submit();
        }

        //ById

        [Test]
        public void FindElementById_ShouldReturnFirstElement()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var element in new[] { context.FindElement(By.Id("idtest")), ((IFindsById)context).FindElementById("idtest") })
            {
                var dataKey = element.GetAttribute("data-key");
                Assert.AreEqual("1", dataKey);
            }
        }

        [Test]
        public void FindElementById_ShouldThrowExceptionWhenElementNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => context.FindElement(By.Id("idtest_no")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => ((IFindsById)context).FindElementById("idtest_no"));
        }

        [Test]
        public void FindElementsById_ShouldReturnAllElements()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.Id("idtest")), ((IFindsById)context).FindElementsById("idtest") })
            {
                Assert.AreEqual(3, elements.Count);
                Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
                Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
                Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
            }
        }

        [Test]
        public void FindElementsById_ShouldReturnEmptyWhenElementsNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.Id("idtest_no")), ((IFindsById)context).FindElementsById("idtest_no") })
            {
                Assert.AreEqual(0, elements.Count);
            }
        }

        //ByName

        [Test]
        public void FindElementByName_ShouldReturnFirstElement()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var element in new[] { context.FindElement(By.Name("nametest")), ((IFindsByName)context).FindElementByName("nametest") })
            {
                var dataKey = element.GetAttribute("data-key");
                Assert.AreEqual("1", dataKey);
            }
        }

        [Test]
        public void FindElementByName_ShouldThrowExceptionWhenElementNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => context.FindElement(By.Name("nametest_no")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => ((IFindsByName)context).FindElementByName("nametest_no"));
        }

        [Test]
        public void FindElementsByName_ShouldReturnAllElements()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.Name("nametest")), ((IFindsByName)context).FindElementsByName("nametest") })
            {
                Assert.AreEqual(3, elements.Count);
                Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
                Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
                Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
            }
        }

        [Test]
        public void FindElementsByName_ShouldReturnEmptyWhenElementsNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.Name("nametest_no")), ((IFindsByName)context).FindElementsByName("nametest_no") })
            {
                Assert.AreEqual(0, elements.Count);
            }
        }

        //ByClassName

        [Test]
        public void FindElementByClassName_ShouldReturnFirstElement()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var element in new[] { context.FindElement(By.ClassName("classtest")), ((IFindsByClassName)context).FindElementByClassName("classtest") })
            {
                var dataKey = element.GetAttribute("data-key");
                Assert.AreEqual("1", dataKey);
            }
        }

        [Test]
        public void FindElementByClassName_ShouldThrowExceptionWhenElementNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => context.FindElement(By.ClassName("classtest_no")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => ((IFindsByClassName)context).FindElementByClassName("classtest_no"));
        }

        [Test]
        public void FindElementsByClassName_ShouldReturnAllElements()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.ClassName("classtest")), ((IFindsByClassName)context).FindElementsByClassName("classtest") })
            {
                Assert.AreEqual(3, elements.Count);
                Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
                Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
                Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
            }
        }

        [Test]
        public void FindElementsByClassName_ShouldReturnEmptyWhenElementsNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.ClassName("classtest_no")), ((IFindsByClassName)context).FindElementsByClassName("classtest_no") })
            {
                Assert.AreEqual(0, elements.Count);
            }
        }

        //ByTagName

        [Test]
        public void FindElementByTagName_ShouldReturnFirstElement()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var element in new[] { context.FindElement(By.TagName("tagtest")), ((IFindsByTagName)context).FindElementByTagName("tagtest") })
            {
                var dataKey = element.GetAttribute("data-key");
                Assert.AreEqual("1", dataKey);
            }
        }

        [Test]
        public void FindElementByTagName_ShouldThrowExceptionWhenElementNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => context.FindElement(By.TagName("tagtest_no")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => ((IFindsByTagName)context).FindElementByTagName("tagtest_no"));
        }

        [Test]
        public void FindElementsByTagName_ShouldReturnAllElements()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.TagName("tagtest")), ((IFindsByTagName)context).FindElementsByTagName("tagtest") })
            {
                Assert.AreEqual(2, elements.Count);
                Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
                Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
            }
        }

        [Test]
        public void FindElementsByTagName_ShouldReturnEmptyWhenElementsNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.TagName("tagtest_no")), ((IFindsByTagName)context).FindElementsByTagName("tagtest_no") })
            {
                Assert.AreEqual(0, elements.Count);
            }
        }

        //ByCssSelector

        [Test]
        public void FindElementByCssSelector_ShouldReturnFirstElement()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var element in new[] { context.FindElement(By.CssSelector("#idtest[name='nametest']")), ((IFindsByCssSelector)context).FindElementByCssSelector("#idtest[name='nametest']") })
            {
                var dataKey = element.GetAttribute("data-key");
                Assert.AreEqual("1", dataKey);
            }
        }

        [Test]
        public void FindElementByCssSelector_ShouldThrowExceptionWhenElementNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => context.FindElement(By.CssSelector("#idtest[name='nametest_no']")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => ((IFindsByCssSelector)context).FindElementByCssSelector("#idtest[name='nametest_no']"));
        }

        [Test]
        public void FindElementsByCssSelector_ShouldReturnAllElements()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.CssSelector("#idtest[name='nametest']")), ((IFindsByCssSelector)context).FindElementsByCssSelector("#idtest[name='nametest']") })
            {
                Assert.AreEqual(3, elements.Count);
                Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
                Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
                Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
            }
        }

        [Test]
        public void FindElementsByCssSelector_ShouldReturnEmptyWhenElementsNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.CssSelector("#idtest[name='nametest_no']")), ((IFindsByCssSelector)context).FindElementsByCssSelector("#idtest[name='nametest_no']") })
            {
                Assert.AreEqual(0, elements.Count);
            }
        }

        //ByXPath

        [Test]
        public void FindElementByXPath_ShouldReturnFirstElement()
        {
            var context = GetDriver().FindElement(By.XPath("/html/body/div[1]"));
            foreach (var element in new[] { context.FindElement(By.XPath("tagtest")), ((IFindsByXPath)context).FindElementByXPath("tagtest") })
            {
                var dataKey = element.GetAttribute("data-key");
                dataKey.Is("1");
            }
        }

        [Test]
        public void FindElementByXPath_ShouldThrowExceptionWhenElementNotFound()
        {
            var context = GetDriver().FindElement(By.XPath("/html/body/div[1]"));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => context.FindElement(By.XPath("tagtest1")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => ((IFindsByXPath)context).FindElementByXPath("tagtest1"));
        }

        [Test]
        public void FindElementsByXPath_ShouldReturnAllElements()
        {
            var context = GetDriver().FindElement(By.XPath("/html/body/div[1]"));
            foreach (var elements in new[] { context.FindElements(By.XPath("tagtest")), ((IFindsByXPath)context).FindElementsByXPath("tagtest") })
            {
                elements.Count.Is(2);
                elements[0].GetAttribute("data-key").Is("1");
                elements[1].GetAttribute("data-key").Is("2");
            }
        }

        [Test]
        public void FindElementsByXPath_ShouldReturnEmptyWhenElementsNotFound()
        {
            var context = GetDriver().FindElement(By.XPath("/html/body/div[1]"));
            foreach (var elements in new[] { context.FindElements(By.XPath("tagtest_no")), ((IFindsByXPath)context).FindElementsByXPath("tagtest_no") })
            {
                elements.Count.Is(0);
            }
        }

        //ByLinkText

        [Test]
        public void FindElementByLinkText_ShouldReturnFirstElement()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var element in new[] { context.FindElement(By.LinkText("Transfer to Frame.html")), 
                ((IFindsByLinkText)context).FindElementByLinkText("Transfer to Frame.html") })
            {
                element.GetAttribute("data-key").Is("1");
            }
        }

        [Test]
        public void FindElementByLinkText_ShouldThrowExceptionWhenElementNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => context.FindElement(By.LinkText("No transfer to Frame.html")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => ((IFindsByLinkText)context).FindElementByLinkText("No transfer to Frame.html"));
        }

        [Test]
        public void FindElementsByLinkText_ShouldReturnAllElements()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.LinkText("Transfer to Frame.html")), 
                ((IFindsByLinkText)context).FindElementsByLinkText("Transfer to Frame.html") })
            {
                elements.Count.Is(2);
                var orderd = elements.Select(e => e.GetAttribute("data-key")).OrderBy(v => v).ToList();
                orderd[0].Is("1");
                orderd[1].Is("2");
            }
        }

        [Test]
        public void FindElementsByLinkText_ShouldReturnEmptyWhenElementsNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.LinkText("No transfer to Frame.html")),
                ((IFindsByLinkText)context).FindElementsByLinkText("No transfer to Frame.html") })
            {
                elements.Count.Is(0);
            }
        }

        //ByPartilLinkText

        [Test]
        public void FindElementByPartialLinkText_ShouldReturnFirstElement()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var element in new[] { context.FindElement(By.PartialLinkText("Transfer")),
                ((IFindsByPartialLinkText)context).FindElementByPartialLinkText("Transfer") })
            {
                element.GetAttribute("data-key").Is("1");
            }
        }

        [Test]
        public void FindElementByPartialLinkText_ShouldThrowExceptionWhenElementNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => context.FindElement(By.PartialLinkText("Notransfer")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => ((IFindsByPartialLinkText)context).FindElementByPartialLinkText("Notransfer"));
        }

        [Test]
        public void FindElementByPartialLinkText_ShouldReturnAllElements()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.PartialLinkText("Transfer")),
                ((IFindsByPartialLinkText)context).FindElementsByPartialLinkText("Transfer") })
            {
                elements.Count.Is(2);
                var orderd = elements.Select(e => e.GetAttribute("data-key")).OrderBy(v => v).ToList();
                orderd[0].Is("1");
                orderd[1].Is("2");
            }
        }

        [Test]
        public void FindElementByPartialLinkText_ShouldReturnEmptyWhenElementsNotFound()
        {
            var context = GetDriver().FindElement(By.ClassName("bytest"));
            foreach (var elements in new[] { context.FindElements(By.PartialLinkText("NoTransfer")),
                ((IFindsByPartialLinkText)context).FindElementsByPartialLinkText("NoTransfer") })
            {
                elements.Count.Is(0);
            }
        }


        private void InitAttr()
        {
            GetExecutor().ExecuteScript(@"delete document.getElementById('attrTestInput').bar;
delete document.getElementById('attrTestInput').foo;");
        }

        [Test]
        public void GetAttribute_ShouldReturnAttributeValue()
        {
            InitAttr();
            var elem = GetDriver().FindElement(By.Id("attrTestInput"));
            var value = elem.GetAttribute("foo");
            value.Is("fooattr");
        }

        [Test]
        public void GetAttribute_ShouldReturnPropertyValueIfHasNotAttribute()
        {
            InitAttr();
            GetExecutor().ExecuteScript("document.getElementById('attrTestInput').bar = 'barprop';");
            var elem = GetDriver().FindElement(By.Id("attrTestInput"));
            var value = elem.GetAttribute("bar");
            value.Is("barprop");
        }

        [Test]
        public void GetAttribute_PorpValueShouldBeReturndByOverwritingAttrValue()
        {
            InitAttr();
            GetExecutor().ExecuteScript("document.getElementById('attrTestInput').foo = 'foodynamic';");
            var attrValue = GetExecutor().ExecuteScript("return document.getElementById('attrTestInput').getAttribute('foo');");
            var elem = GetDriver().FindElement(By.Id("attrTestInput"));
            var value = elem.GetAttribute("foo");

            // selenium は attribute と property を混同して扱っている模様。
            // property の値を先に返している。

            attrValue.Is("fooattr");
            value.Is("foodynamic");
        }

        [Test]
        public void GetAttribute_IfPropetyIsNull_ReturnAttributeValue()
        {
            InitAttr();
            GetExecutor().ExecuteScript("document.getElementById('attrTestInput').foo = null;");
            var elem = GetDriver().FindElement(By.Id("attrTestInput"));
            var value = elem.GetAttribute("foo");
            // selenium は attribute と property を混同して扱っている模様。
            // property の値がnullの場合は attribute の値を返している。
            value.Is("fooattr");
        }

        [Test]
        public void GetProperty_ShouldNotReturnAttributeValue()
        {
            InitAttr();
            var elem = GetDriver().FindElement(By.Id("attrTestInput"));
            var value = elem.GetProperty("foo");
            value.IsNull();
        }

        [Test]
        public void GetProperty_ShouldReturnPropertyValue()
        {
            InitAttr();
            GetExecutor().ExecuteScript("document.getElementById('attrTestInput').foo = 'foodynamic';");
            var elem = GetDriver().FindElement(By.Id("attrTestInput"));
            var value = elem.GetProperty("foo");
            value.Is("foodynamic");
        }
    }
}
