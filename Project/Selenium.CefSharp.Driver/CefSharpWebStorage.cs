using OpenQA.Selenium;
using OpenQA.Selenium.Html5;

namespace Selenium.CefSharp.Driver
{
    class CefSharpWebStorage : IWebStorage
    {
        public CefSharpWebStorage(IJavaScriptExecutor javaScriptExecutor)
        {
            LocalStorage = new CefSharpStorag(javaScriptExecutor, "localStorage");
            SessionStorage = new CefSharpStorag(javaScriptExecutor, "sessionStorage");
        }

        public ILocalStorage LocalStorage { get; }

        public ISessionStorage SessionStorage { get; }
    }
}
