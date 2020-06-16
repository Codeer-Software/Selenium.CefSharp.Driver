using Codeer.Friendly.Windows;
using OpenQA.Selenium;

namespace Selenium.CefSharp.Driver.Inside
{
    interface IJavaScriptExecutorCefFunctions
    {
        WindowsAppFriend App { get; }
        dynamic JavascriptObjectRepository { get; }
        dynamic Frame { get; }
        void WaitForLoading();
        IWebElement CreateWebElement(int id);
    }
}
