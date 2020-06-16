using Codeer.Friendly;
using Codeer.Friendly.Windows.Grasp;
using System;

namespace Selenium.CefSharp.Driver.Inside
{
    interface ICefSharpBrowser : IUIObject
    {
        CefSharpFrameDriver MainFrame { get; }
        CefSharpFrameDriver CurrentFrame { get; set; }
        AppVar BrowserCore { get; }
        IntPtr WindowHandle { get; }
        void WaitForLoading();
        void Close();
    }
}
