﻿using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Codeer.Friendly.Windows.KeyMouse;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Selenium.CefSharp.Driver.Inside.NativeMethods;

namespace Selenium.CefSharp.Driver.Inside
{
    class Alert : IAlert
    {
        WindowsAppFriend _app;
        WindowControl _editor;
        WindowControl _ok;
        WindowControl _cancel;
        WindowControl _message;

        public string Text => _message == null ? string.Empty : _message.GetWindowText();

        public Alert(WindowsAppFriend driverApp)
        {
            int processId = driverApp.ProcessId;
            int currentThreadId = driverApp.Type(typeof(Codeer.Friendly.Windows.Grasp.Inside.NativeMethods)).GetCurrentThreadId();

            var handles = new List<IntPtr>();
            EnumWindowsDelegate enumWindows = (IntPtr hwnd, IntPtr lparam) =>
            {
                if (!IsWindow(hwnd) || !IsWindowVisible(hwnd) || !IsWindowEnabled(hwnd)) return true;

                GetWindowThreadProcessId(hwnd, out var windowProcessId);
                if (processId != windowProcessId) return true;

                var sb = new StringBuilder(1024);
                GetWindowText(hwnd, sb, 1024);
                if (sb.ToString().IndexOf("JavaScript") == 0)
                {
                    handles.Add(hwnd);
                }
                return true;
            };

            EnumWindows(enumWindows, IntPtr.Zero);
            GC.KeepAlive(enumWindows);

            if (0 < handles.Count)
            {
                _app = new WindowsAppFriend(handles[0]);
                var win = new WindowControl(_app, handles[0]);
                _editor = win.GetFromWindowClass("Edit").FirstOrDefault();
                _ok = win.GetFromWindowClass("Button").Where(e => e.GetWindowText() == "OK").FirstOrDefault();
                _cancel = win.GetFromWindowClass("Button").Where(e => e.GetWindowText() == "Cancel").FirstOrDefault();
                _message = win.GetFromWindowClass("Static").FirstOrDefault();
            }
        }

        public void Accept()
        {
            if (_ok == null) return;

            while (_ok.IsWindow())
            {
                _ok.Activate();
                _ok.Click();
            }
        }

        public void Dismiss()
        {
            if (_cancel == null) return;

            while (_cancel.IsWindow())
            {
                _cancel.Activate();
                _cancel.Click();
            }
        }

        public void SendKeys(string keysToSend)
        {
            if (_editor == null) return;
            _editor.Activate();
            KeySpec.SendKeys(_editor.App, keysToSend);
        }

        public void SetAuthenticationCredentials(string userName, string password)
        {
            //TODO not test.
            _app.SendKeys(userName);
            _app.SendKey(System.Windows.Forms.Keys.Tab);
            _app.SendKeys(password);
            _app.SendKey(System.Windows.Forms.Keys.Enter);
        }
    }
}
