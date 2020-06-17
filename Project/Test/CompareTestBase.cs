using OpenQA.Selenium;
using System.IO;

namespace Test
{
    public abstract class CompareTestBase
    {
        protected void ClassInitBase()
        {
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            var path = Path.Combine(dir, @"Test\Html\Controls.html");
        }

        protected void ClassCleanupBase() { }

        public abstract IWebDriver GetDriver();

        public T GetDriver<T>() => (T)GetDriver();

        protected IJavaScriptExecutor GetExecutor() => (IJavaScriptExecutor)GetDriver();

        protected string GetHtmlUrl() => HtmlServer.Instance.RootUrl + "Controls.html";
    }
}
