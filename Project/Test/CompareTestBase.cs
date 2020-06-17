using OpenQA.Selenium;
using System.IO;

namespace Test
{
    public abstract class CompareTestBase
    {
        public abstract IWebDriver GetDriver();

        public T GetDriver<T>() => (T)GetDriver();

        protected IJavaScriptExecutor GetExecutor() => (IJavaScriptExecutor)GetDriver();

        protected string GetHtmlUrl() => HtmlServer.Instance.RootUrl + "Controls.html";
    }
}
