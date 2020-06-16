using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Selenium.CefSharp.Driver
{
    public class CefSharpWindowManager : ILifeSpanHandler
    {
        public class BrowserInfo
        {
            public IntPtr WindowHandle;
            public IBrowser Browser;
        }

        public List<BrowserInfo> Browsers = new List<BrowserInfo>();
        List<IntPtr> _beforeOpen = null;

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl,
            string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures,
            IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            _beforeOpen = GetWindows();

            newBrowser = null;
            return false;
        }

        static IntPtr GetNewWindow(List<IntPtr> before, List<IntPtr> after)
        {
            foreach (var e in after)
            {
                if (!before.Contains(e)) return e;
            }
            return IntPtr.Zero;
        }

        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            if (_beforeOpen == null) return;

            var after = GetWindows();
            var handle = GetNewWindow(_beforeOpen, after);
            _beforeOpen = null;
            if (handle == IntPtr.Zero) return;
            Browsers.Add(new BrowserInfo
            {
                WindowHandle = handle,
                Browser = browser
            });
        }

        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            return false;
        }

        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            BrowserInfo info = null;
            foreach (var e in Browsers)
            {
                if (e.Browser.Identifier == browser.Identifier)
                {
                    info = e;
                    break;
                }
            }
            Browsers.Remove(info);
        }

        public List<IntPtr> GetWindowHandles()
        {
            var list = new List<IntPtr>();
            foreach (var e in Browsers)
            {
                list.Add(e.WindowHandle);
            }
            return list;
        }

        public IBrowser GetWindowHandles(IntPtr windowHandle)
        {
            foreach (var e in Browsers)
            {
                if (e.WindowHandle == windowHandle) return e.Browser;
            }
            return null;
        }

        static List<IntPtr> GetWindows()
        {
            int processId = Process.GetCurrentProcess().Id;

            var handles = new List<IntPtr>();
            EnumWindowsDelegate enumWindows = (IntPtr hwnd, IntPtr lparam) =>
            {
                if (!IsWindow(hwnd) || !IsWindowVisible(hwnd) || !IsWindowEnabled(hwnd)) return true;

                int windowProcessId = 0;
                GetWindowThreadProcessId(hwnd, out windowProcessId);
                if (processId != windowProcessId) return true;
                handles.Add(hwnd);
                return true;
            };

            EnumWindows(enumWindows, IntPtr.Zero);
            GC.KeepAlive(enumWindows);

            return handles;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lparam);

        delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lparam);
    }
}
