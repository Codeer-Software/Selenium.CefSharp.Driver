using OpenQA.Selenium;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using SeleniumExtras.WaitHelpers;

namespace Test
{
    //TODO add many window test.
    public abstract class WindowTest : CompareTestBase
    {
        public class Forms : WindowTest
        {
            public Forms() : base(new FormsAgent()) { }
        }
        public class Wpf : WindowTest
        {
            public Wpf() : base(new WpfAgent()) { }
        }
        public class Web : WindowTest
        {
            public Web() : base(new WebAgent()) { }
        }
        protected WindowTest(INeed need) : base(need) { }

        [SetUp]
        public void initialize()
        {
            GetDriver().Url = HtmlServer.Instance.RootUrl + "Window.html";
        }

        [Test]
        public void TestWindow()
        {
            var driver = GetDriver();
            driver.FindElement(By.Id("window")).Click();

            Thread.Sleep(3000);

            driver.SwitchTo().Window( driver.WindowHandles[1]);
            driver.FindElement(By.Id("inputJS")).Click();

            driver.FindElement(By.Id("alertJS")).Click();

            var wait = new WebDriverWait(driver, new System.TimeSpan(10000));

#pragma warning disable CS0618
            var alert = wait.Until(ExpectedConditions.AlertIsPresent());
#pragma warning restore CS0618
            alert.Text.Is("test");
            alert.Accept();

            driver.FindElement(By.Id("confirmJS")).Click();

#pragma warning disable CS0618
            var confirm = wait.Until(ExpectedConditions.AlertIsPresent());
#pragma warning restore CS0618
            confirm.Text.Is("test");
            confirm.Dismiss();

            driver.FindElement(By.Id("promptJS")).Click();

#pragma warning disable CS0618
            var prompt = wait.Until(ExpectedConditions.AlertIsPresent());
#pragma warning restore CS0618
            prompt.SendKeys("abc");
            prompt.Accept();
        }
    }
}
