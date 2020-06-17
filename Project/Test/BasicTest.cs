using OpenQA.Selenium;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using System.IO;

namespace Test
{
    public abstract class BasicTest : CompareTestBase
    {
        public class Forms : BasicTest
        {
            public Forms() : base(new FormsAgent()) { }
        }
        public class Wpf : BasicTest
        {
            public Wpf() : base(new WpfAgent()) { }
        }
        public class Web : BasicTest
        {
            public Web() : base(new WebAgent()) { }
        }
        protected BasicTest(INeed need) : base(need) { }

        [SetUp]
        public void SetUp()
            => GetDriver().Url = GetHtmlUrl();// HtmlServer.Instance.RootUrl + "Controls.html";

        //TODO timing.
        string GetHtmlUrl()
        {
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            return Path.Combine(dir, @"Test\Html\Controls.html");
        }

        [Test]
        public void TestTitle()
        {
            GetDriver().Title.Is("Controls for Test");
        }

        [Test]
        public void TestPageSource()
        {
            //check only not to throw exception.
            var pageSource = GetDriver().PageSource;
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
    }
}
