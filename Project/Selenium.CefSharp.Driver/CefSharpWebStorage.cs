using OpenQA.Selenium;
using OpenQA.Selenium.Html5;

namespace Selenium.CefSharp.Driver
{
    class CefSharpWebStorage : IWebStorage
    {
        public CefSharpWebStorage(IJavaScriptExecutor jsExecutor)
        {
            LocalStorage = new CefSharpStorag(jsExecutor, "localStorage");
            SessionStorage = new CefSharpStorag(jsExecutor, "sessionStorage");
        }

        public ILocalStorage LocalStorage { get; }

        public ISessionStorage SessionStorage { get; }
    }
}
