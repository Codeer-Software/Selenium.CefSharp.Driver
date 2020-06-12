using Codeer.Friendly.Windows;
using OpenQA.Selenium;

namespace Selenium.CefSharp.Driver
{
    interface ICefFunctions
    {
        WindowsAppFriend App { get; }
        dynamic JavascriptObjectRepository { get; }
        dynamic TargetFrame { get; }
        void WaitForLoading();
        IWebElement CreateWebElement(int id);
    }
}
