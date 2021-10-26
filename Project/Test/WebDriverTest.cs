using OpenQA.Selenium;
using Selenium.CefSharp.Driver;
using System.IO;
using System.Linq;
using OpenQA.Selenium.Internal;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using OpenQA.Selenium.Remote;
using SeleniumExtras.WaitHelpers;

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