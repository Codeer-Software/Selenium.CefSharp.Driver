using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Selenium.CefSharp.Driver;
using System.Diagnostics;

namespace Test
{
    public abstract class CompareTestBase
    {
        protected interface INeed
        {
            IWebDriver Driver { get; }
            void OneTimeSetUp();
            void OneTimeTearDown();
        }

        INeed _need;

        protected IWebDriver GetDriver() => _need.Driver;

        protected T GetDriver<T>() => (T)GetDriver();

        protected IJavaScriptExecutor GetExecutor() => (IJavaScriptExecutor)GetDriver();

        protected CompareTestBase(INeed need) => _need = need;

        [OneTimeSetUp]
        public void OneTimeSetUp() => _need.OneTimeSetUp();

        [OneTimeTearDown]
        public void OneTimeTearDown() => _need.OneTimeTearDown();

        protected class FormsAgent : INeed
        {
            CefSharpDriver _driver;

            public IWebDriver Driver => _driver;

            public void OneTimeSetUp()
                => _driver = AppRunner.RunWinFormApp();

            public void OneTimeTearDown()
                => Process.GetProcessById(_driver.App.ProcessId).Kill();
        }

        protected class WpfAgent : INeed
        {
            CefSharpDriver _driver;

            public IWebDriver Driver => _driver;

            public void OneTimeSetUp()
                => _driver = AppRunner.RunWpfApp();

            public void OneTimeTearDown()
                => Process.GetProcessById(_driver.App.ProcessId).Kill();
        }

        protected class WebAgent : INeed
        {
            ChromeDriver _driver;

            public IWebDriver Driver => _driver;

            public void OneTimeSetUp()
            {
                _driver = new ChromeDriver();
                _driver.Url = "https://github.com/Codeer-Software/Selenium.CefSharp.Driver";
            }

            public void OneTimeTearDown()
                => _driver.Dispose();
        }
    }
}
