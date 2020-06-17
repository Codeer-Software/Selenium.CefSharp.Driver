using OpenQA.Selenium;
using NUnit.Framework;
using System.Threading;

namespace Test
{
    //TODO
    public abstract class FrameTest : CompareTestBase
    {
        public class Forms : FrameTest
        {
            public Forms() : base(new FormsAgent()) { }
        }
        public class Wpf : FrameTest
        {
            public Wpf() : base(new WpfAgent()) { }
        }
        public class Web : FrameTest
        {
            public Web() : base(new WebAgent()) { }
        }
        protected FrameTest(INeed need) : base(need) { }

        [SetUp]
        public void SetUp()
            => GetDriver().Url = HtmlServer.Instance.RootUrl + "Frame.html";

        [Test]
        public void TestFrame()
        {
            Thread.Sleep(3000);

            var driver = GetDriver();

            driver.SwitchTo().Frame(0);
            driver.FindElement(By.Id("textBoxName")).SendKeys("abc");
            driver.SwitchTo().DefaultContent();

            driver.SwitchTo().Frame(1);
            driver.SwitchTo().Frame(0);
            driver.FindElement(By.Id("inputJS")).Click();
            driver.SwitchTo().DefaultContent();

            driver.SwitchTo().Frame(2);
            var url = driver.Url;

            driver.Navigate().GoToUrl("https://github.com/Codeer-Software/Selenium.CefSharp.Driver");
            driver.Url = HtmlServer.Instance.RootUrl + "Frame.html";

            var y = driver.FindElement(By.Id("frameInput1"));
            /*
            driver.SwitchTo().ParentFrame();
            driver.FindElement(By.Id("frameInput1")).SendKeys("abc");
            */
            //TODO
            //url is browser's url. not iframe url.
            //check navigate too.
        }
    }
}
