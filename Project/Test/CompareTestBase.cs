using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public abstract class CompareTestBase
    {
        protected static HtmlServer server = null;

        protected static void ClassInitBase()
        {
            server = HtmlServer.CreateFromFile("Controls.html");
        }

        protected static void ClassCleanupBase()
        {
            if (server != null) server.Close();
        }

        public abstract IWebDriver GetDriver();

        public T GetDriver<T>() => (T)GetDriver();

        protected IJavaScriptExecutor GetExecutor() => (IJavaScriptExecutor)GetDriver();

        protected string GetHtmlUrl() => server?.Url;
    }
}
