using OpenQA.Selenium;
using OpenQA.Selenium.Html5;

namespace Selenium.CefSharp.Driver.Inside
{
    class WebStorage : IWebStorage
    {
        public ILocalStorage LocalStorage { get; }

        public ISessionStorage SessionStorage { get; }

        public WebStorage(IJavaScriptExecutor javaScriptExecutor)
        {
            LocalStorage = new Storag(javaScriptExecutor, "localStorage");
            SessionStorage = new Storag(javaScriptExecutor, "sessionStorage");
        }
    }
}
