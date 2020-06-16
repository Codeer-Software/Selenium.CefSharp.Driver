using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Selenium.CefSharp.Driver.Inside
{
    static class NativeMethods
    {
        internal const int GW_CHILD = 5;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lparam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        internal delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lparam);

        internal static List<IntPtr> GetWindows(int processId)
        {
            var handles = new List<IntPtr>();
            EnumWindowsDelegate enumWindows = (IntPtr hwnd, IntPtr lparam) =>
            {
                if (!IsWindow(hwnd) || !IsWindowVisible(hwnd) || !IsWindowEnabled(hwnd)) return true;

                GetWindowThreadProcessId(hwnd, out var windowProcessId);
                if (processId != windowProcessId) return true;
                handles.Add(hwnd);
                return true;
            };

            EnumWindows(enumWindows, IntPtr.Zero);
            GC.KeepAlive(enumWindows);

            return handles;
        }
    }
}
