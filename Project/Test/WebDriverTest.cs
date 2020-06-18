using OpenQA.Selenium;
using Selenium.CefSharp.Driver;
using System.IO;
using System.Linq;
using OpenQA.Selenium.Internal;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using OpenQA.Selenium.Remote;

namespace Test
{
    public abstract class WebDriverTest : CompareTestBase
    {
        public class Forms : WebDriverTest
        {
            public Forms() : base(new FormsAgent()) { }
        }

        public class Wpf : WebDriverTest
        {
            public Wpf() : base(new WpfAgent()) { }
        }

        public class Web : WebDriverTest
        {
            public Web() : base(new WebAgent()) { }

            [Ignore("")]
            public override void WebStorageLocal() { }

            [Ignore("")]
            public override void WebStorageSession() { }
        }

        protected WebDriverTest(INeed need) : base(need) { }

        [SetUp]
        public void SetUp()
            => GetDriver().Url = HtmlServer.Instance.RootUrl + "Controls.html";

        [Test]
        public void ScreenShot()
        {
            var screenShot = GetDriver<ITakesScreenshot>();
            var path = Path.GetTempFileName();
            screenShot.GetScreenshot().SaveAsFile(path, ScreenshotImageFormat.Png);
            File.Delete(path);
        }

        [Test]
        public void ShouldGetFirstElementWhenUsedFindElementById()
        {
            foreach (var element in new[] { GetDriver().FindElement(By.Id("idtest")), GetDriver<IFindsById>().FindElementById("idtest") })
            {
                var dataKey = element.GetAttribute("data-key");
                Assert.AreEqual("1", dataKey);
            }
        }

        [Test]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementById()
        {
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.Id("idtest_no")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver<IFindsById>().FindElementById("idtest_no"));
        }

        [Test]
        public void ShouldGetAllElementWhenUsedFindElementsById()
        {
            foreach (var elements in new[] { GetDriver().FindElements(By.Id("idtest")), GetDriver<IFindsById>().FindElementsById("idtest") })
            {
                Assert.AreEqual(3, elements.Count);
                Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
                Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
                Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
            }
        }

        [Test]
        public void ShouldIgnoreQuerySelectorNameWhenUsedFindElementsById()
        {
            foreach (var elements in new[] { GetDriver().FindElements(By.Id("form .term")), GetDriver<IFindsById>().FindElementsById("form .term") })
            {
                Assert.AreEqual(0, elements.Count);
            }
        }

        [Test]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsById()
        {
            foreach (var elements in new[] { GetDriver().FindElements(By.Id("idtest_no")), GetDriver<IFindsById>().FindElementsById("idtest_no") })
            {
                Assert.AreEqual(0, elements.Count);
            }
        }

        //FindElement(s)ByName

        [Test]
        public void ShouldGetFirstElementWhenUsedFindElementByName()
        {
            foreach (var element in new[] { GetDriver().FindElement(By.Name("nametest")), GetDriver<IFindsByName>().FindElementByName("nametest") })
            {
                var dataKey = element.GetAttribute("data-key");
                Assert.AreEqual("1", dataKey);
            }
        }

        [Test]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementByName()
        {
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.Name("nametest_no")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver<IFindsByName>().FindElementByName("nametest_no"));
        }

        [Test]
        public void ShouldGetAllElementWhenUsedFindElementsByName()
        {
            foreach (var elements in new[] { GetDriver().FindElements(By.Name("nametest")), GetDriver<IFindsByName>().FindElementsByName("nametest") })
            {
                Assert.AreEqual(3, elements.Count);
                Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
                Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
                Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
            }
        }

        [Test]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsByName()
        {
            foreach (var elements in new[] { GetDriver().FindElements(By.Name("nametest_no")), GetDriver<IFindsByName>().FindElementsByName("nametest_no") })
                Assert.AreEqual(0, elements.Count);
        }

        //FindElement(s)ByClassName

        [Test]
        public void ShouldGetFirstElementWhenUsedFindElementByClassName()
        {
            foreach (var element in new[] { GetDriver().FindElement(By.ClassName("classtest")), GetDriver<IFindsByClassName>().FindElementByClassName("classtest") })
            {
                var dataKey = element.GetAttribute("data-key");
                Assert.AreEqual("1", dataKey);
            }
        }

        [Test]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementByClassName()
        {
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.ClassName("classtest_no")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver<IFindsByClassName>().FindElementByClassName("classtest_no"));
        }

        [Test]
        public void ShouldGetAllElementWhenUsedFindElementsByClassName()
        {
            foreach (var elements in new[] { GetDriver().FindElements(By.ClassName("classtest")), GetDriver<IFindsByClassName>().FindElementsByClassName("classtest") })
            {
                Assert.AreEqual(3, elements.Count);
                Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
                Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
                Assert.AreEqual("3", elements[2].GetAttribute("data-key"));
            }
        }

        [Test]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsByClassName()
        {
            foreach (var elements in new[] { GetDriver().FindElements(By.ClassName("classtest_no")), GetDriver<IFindsByClassName>().FindElementsByClassName("classtest_no") })
            {
                Assert.AreEqual(0, elements.Count);
            }
        }

        //FindElement(s)ByCssSelector

        [Test]
        public void ShouldGetFirstElementWhenUsedFindElementByCssSelector()
        {
            foreach (var element in new[] { GetDriver().FindElement(By.CssSelector(".bytest > #idtest[name='nametest']")), GetDriver<IFindsByCssSelector>().FindElementByCssSelector(".bytest > #idtest[name='nametest']") })
            {
                var dataKey = element.GetAttribute("data-key");
                Assert.AreEqual("1", dataKey);
            }
        }

        [Test]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementByCssSelector()
        {
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.CssSelector(".bytest > #idtest_no[name='nametest']")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver<IFindsByCssSelector>().FindElementByCssSelector(".bytest > #idtest_no[name='nametest']"));
        }

        [Test]
        public void ShouldGetAllElementWhenUsedFindElementsByCssSelector()
        {
            foreach (var elements in new[] { GetDriver().FindElements(By.CssSelector(".bytest > #idtest[name='nametest']")), GetDriver<IFindsByCssSelector>().FindElementsByCssSelector(".bytest > #idtest[name='nametest']") })
            {
                Assert.AreEqual(2, elements.Count);
                Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
                Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
            }
        }

        [Test]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsByCssSelector()
        {
            foreach (var elements in new[] { GetDriver().FindElements(By.CssSelector(".bytest > #idtest[name='nametest_no']")), GetDriver<IFindsByCssSelector>().FindElementsByCssSelector(".bytest > #idtest[name='nametest_no']") })
            {
                Assert.AreEqual(0, elements.Count);
            }
        }

        //FindElement(s)ByTagName

        [Test]
        public void ShouldGetFirstElementWhenUsedFindElementByTagName()
        {
            foreach (var element in new[] { GetDriver().FindElement(By.TagName("tagtest")), GetDriver<IFindsByTagName>().FindElementByTagName("tagtest") })
            {
                element.GetAttribute("data-key").Is("1");
            }
        }

        [Test]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementByTagName()
        {
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.TagName("tagtest_no")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver<IFindsByTagName>().FindElementByTagName("tagtest_no"));
        }

        [Test]
        public void ShouldGetAllElementWhenUsedFindElementsByTagName()
        {
            foreach (var elements in new[] { GetDriver().FindElements(By.TagName("tagtest")), GetDriver<IFindsByTagName>().FindElementsByTagName("tagtest") })
            {
                Assert.AreEqual(2, elements.Count);
                Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
                Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
            }
        }

        [Test]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsByTagName()
        {
            foreach (var elements in new[] { GetDriver().FindElements(By.TagName("tagtest_no")), GetDriver<IFindsByTagName>().FindElementsByTagName("tagtest_no") })
            {
                Assert.AreEqual(0, elements.Count);
            }
        }

        //FindElement(s)ByXPath
        [Test]
        public void ShouldGetFirstElementWhenUsedFindElementByXPath()
        {
            foreach (var element in new[] { GetDriver().FindElement(By.XPath("/html/body/div[1]/tagtest")), GetDriver<IFindsByXPath>().FindElementByXPath("/html/body/div[1]/tagtest") })
            {
                var dataKey = element.GetAttribute("data-key");
                Assert.AreEqual("1", dataKey);
            }
        }

        [Test]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementByXPath()
        {
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.XPath("/html/body/div[1]/tagtest_no")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver<IFindsByXPath>().FindElementByXPath("/html/body/div[1]/tagtest_no"));
        }

        [Test]
        public void ShouldGetAllElementWhenUsedFindElementsByXPath()
        {
            foreach (var elements in new[] { GetDriver().FindElements(By.XPath("/html/body/div[1]/tagtest")), GetDriver<IFindsByXPath>().FindElementsByXPath("/html/body/div[1]/tagtest") })
            {
                Assert.AreEqual(2, elements.Count);
                Assert.AreEqual("1", elements[0].GetAttribute("data-key"));
                Assert.AreEqual("2", elements[1].GetAttribute("data-key"));
            }
        }

        [Test]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsByXPath()
        {
            foreach (var elements in new[] { GetDriver().FindElements(By.XPath("/html/body/div[1]/tagtest_no")), GetDriver<IFindsByXPath>().FindElementsByXPath("/html/body/div[1]/tagtest_no") })
            {
                Assert.AreEqual(0, elements.Count);
            }
        }

        //FindElement(s)ByLinkText

        [Test]
        public void ShouldGetFirstElementWhenUseFindElementByLinkText()
        {
            foreach (var element in new[] {
                GetDriver().FindElement(By.LinkText("Transfer to Frame.html")),
                GetDriver<IFindsByLinkText>().FindElementByLinkText("Transfer to Frame.html")})
            {
                element.GetAttribute("data-key").Is("1");
            }
        }

        [Test]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementByLinkText()
        {
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.LinkText("No transfer to Frame.html")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver<IFindsByLinkText>().FindElementByLinkText("No transfer to Frame.html"));
        }

        [Test]
        public void ShouldGetAllElementWhenUsedFindElementsByLinkText()
        {
            foreach (var elements in new[] {
                GetDriver().FindElements(By.LinkText("Transfer to Frame.html")),
                GetDriver<IFindsByLinkText>().FindElementsByLinkText("Transfer to Frame.html")})
            {
                elements.Count.Is(3);
                var orderd = elements.Select(e => e.GetAttribute("data-key")).OrderBy(v => v).ToList();
                orderd[0].Is("1");
                orderd[1].Is("2");
                orderd[2].Is("4");
            }
        }

        [Test]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsByLinkText()
        {
            foreach (var elements in new[] {
                GetDriver().FindElements(By.LinkText("Not transfer to Frame.html")),
                GetDriver<IFindsByLinkText>().FindElementsByLinkText("Not transfer to Frame.html")})
            {
                elements.Count.Is(0);
            }
        }

        //FindElement(s)ByPartialLinkText

        [Test]
        public void ShouldGetFirstElementWhenUseFindElementByPartialLinkText()
        {
            foreach (var element in new[] {
                GetDriver().FindElement(By.PartialLinkText("Frame")),
                GetDriver<IFindsByPartialLinkText>().FindElementByPartialLinkText("Frame")})
            {
                element.GetAttribute("data-key").Is("1");
            }
        }

        [Test]
        public void ShouldThrowExceptionWhenMissingElementUsedFindElementByPartialLinkText()
        {
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver().FindElement(By.PartialLinkText("No Frame")));
            AssertCompatible.ThrowsException<NoSuchElementException>(() => GetDriver<IFindsByPartialLinkText>().FindElementByPartialLinkText("No Frame"));
        }

        [Test]
        public void ShouldGetAllElementWhenUsedFindElementsByPartialLinkText()
        {
            foreach (var elements in new[] {
                GetDriver().FindElements(By.PartialLinkText("Transfer")),
                GetDriver<IFindsByPartialLinkText>().FindElementsByPartialLinkText("Transfer")})
            {
                elements.Count.Is(3);
                var orderd = elements.Select(e => e.GetAttribute("data-key")).OrderBy(v => v).ToList();
                orderd[0].Is("1");
                orderd[1].Is("2");
                orderd[2].Is("4");
            }
        }

        [Test]
        public void ShouldReturnEmptyWhenMissingElementsUsedByFindElementsByPartialLinkText()
        {
            foreach (var elements in new[] {
                GetDriver().FindElements(By.PartialLinkText("No Frame")),
                GetDriver<IFindsByPartialLinkText>().FindElementsByPartialLinkText("No Frame")})
            {
                elements.Count.Is(0);
            }
        }

        // Other

        [Test]
        public void ShouldThrowExceptionWhenReferenceTheRemovedElement()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            AssertCompatible.IsInstanceOfType(element, typeof(IWebElement));
            element.SendKeys("ABC");
            GetExecutor().ExecuteScript("const elem = document.querySelector('#textBoxName'); elem.parentNode.removeChild(elem);");
            AssertCompatible.ThrowsException<StaleElementReferenceException>(() => element.SendKeys("DEF"));

            GetExecutor().ExecuteScript(@"
const elem = document.createElement('input');
elem.setAttribute('id', 'textBoxName');
document.body.appendChild(elem);");

            AssertCompatible.ThrowsException<StaleElementReferenceException>(() => element.SendKeys("DEF"));

            element = GetDriver().FindElement(By.Id("textBoxName"));
            AssertCompatible.IsInstanceOfType(element, typeof(IWebElement));
            element.SendKeys("ABC");
        }

        [Test]
        public virtual void WebStorageLocal()
        {
            var driver = GetDriver() as CefSharpDriver;
            if (driver == null) return;

            driver.HasWebStorage.IsTrue();
            driver.WebStorage.LocalStorage.Clear();
            driver.WebStorage.LocalStorage.SetItem("a", "x");
            driver.WebStorage.LocalStorage.Count.Is(1);
            driver.WebStorage.LocalStorage.GetItem("a").Is("x");
            driver.WebStorage.LocalStorage.SetItem("b", "y");
            var keys = driver.WebStorage.LocalStorage.KeySet().OrderBy(e => e).ToList();
            keys.Count.Is(2);
            keys[0].Is("a");
            keys[1].Is("b");
        }

        [Test]
        public virtual void WebStorageSession()
        {
            var driver = GetDriver() as CefSharpDriver;
            if (driver == null) return;

            driver.HasWebStorage.IsTrue();
            driver.WebStorage.SessionStorage.Clear();
            driver.WebStorage.SessionStorage.SetItem("a", "x");
            driver.WebStorage.SessionStorage.Count.Is(1);
            driver.WebStorage.SessionStorage.GetItem("a").Is("x");
            driver.WebStorage.SessionStorage.SetItem("b", "y");
            var keys = driver.WebStorage.SessionStorage.KeySet().OrderBy(e => e).ToList();
            keys.Count.Is(2);
            keys[0].Is("a");
            keys[1].Is("b");
        }

        [Test]
        public void TestNavigation()
        {
            var navigate = GetDriver().Navigate();
            navigate.Back();
            GetDriver().Url.Contains("Selenium.CefSharp.Driver").IsTrue();
            navigate.Forward();
            GetDriver().Url.Contains("Controls").IsTrue();
            navigate.GoToUrl("https://github.com/Codeer-Software/Selenium.CefSharp.Driver");
            GetDriver().Url.Contains("Selenium.CefSharp.Driver").IsTrue();
            navigate.Refresh();
            GetDriver().Url.Contains("Selenium.CefSharp.Driver").IsTrue();
        }

        [Test]
        public void Alert()
        {
            GetDriver().FindElement(By.Id("alertJS")).Click();

            var wait = new WebDriverWait(GetDriver(), new System.TimeSpan(10000));

#pragma warning disable CS0618
            var alert = wait.Until(ExpectedConditions.AlertIsPresent());
#pragma warning restore CS0618
            alert.Text.Is("test");
            alert.Accept();

            GetDriver().FindElement(By.Id("confirmJS")).Click();

#pragma warning disable CS0618
            var confirm = wait.Until(ExpectedConditions.AlertIsPresent());
#pragma warning restore CS0618
            confirm.Text.Is("test");
            confirm.Dismiss();

            GetDriver().FindElement(By.Id("promptJS")).Click();

#pragma warning disable CS0618
            var prompt = wait.Until(ExpectedConditions.AlertIsPresent());
#pragma warning restore CS0618
            prompt.SendKeys("abc");
            prompt.Accept();
        }

        [Test]
        public void FileDetectorSendKeys()
        {
            var driver = GetDriver();
            ((IAllowsFileDetection)driver).FileDetector = new LocalFileDetector();
            var e = driver.FindElement(By.Id("file"));

            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            var path = Path.Combine(dir, @"Test\html\favicon.ico");

            e.SendKeys(path);
            var result = e.GetAttribute("value");
            result.Contains("favicon.ico");
        }
    }
}