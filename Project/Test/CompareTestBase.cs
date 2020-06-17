using OpenQA.Selenium;
using System.IO;

namespace Test
{
    public abstract class CompareTestBase
    {
        protected HtmlServer _server = null;

        protected void ClassInitBase()
        {
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            var path = Path.Combine(dir, @"Test\Html\Controls.html");

            _server = HtmlServer.StartNew();
        }

        protected void ClassCleanupBase()
        {
            if (_server != null) _server.Close();
        }

        public abstract IWebDriver GetDriver();

        public T GetDriver<T>() => (T)GetDriver();

        protected IJavaScriptExecutor GetExecutor() => (IJavaScriptExecutor)GetDriver();

        protected string GetHtmlUrl() => _server.RootUrl + "Controls.html";
    }
}
