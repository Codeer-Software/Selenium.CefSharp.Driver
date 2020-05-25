using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.KeyMouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Selenium.CefSharp.Driver
{
    static class KeySpec
    {
        static Dictionary<string, Action<WindowsAppFriend, List<System.Windows.Forms.Keys>>> SpecialKeys = new Dictionary<string, Action<WindowsAppFriend, List<System.Windows.Forms.Keys>>>();

        static KeySpec()
        {
            foreach (var e in typeof(OpenQA.Selenium.Keys).GetFields().Where(e => e.FieldType == typeof(string)))
            {
                if (Enum.TryParse<System.Windows.Forms.Keys>(e.Name, out var result))
                {
                    SpecialKeys[(string)e.GetValue(null)] = (app, _) => app.SendKey(result);
                }
            }
            SpecialKeys[OpenQA.Selenium.Keys.Backspace] = (app, _) => app.SendKey(System.Windows.Forms.Keys.Back);
            SpecialKeys[OpenQA.Selenium.Keys.Semicolon] = (app, _) => app.SendKey(System.Windows.Forms.Keys.OemSemicolon);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad0] = (app, _) => app.SendKey(System.Windows.Forms.Keys.NumPad0);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad1] = (app, _) => app.SendKey(System.Windows.Forms.Keys.NumPad1);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad2] = (app, _) => app.SendKey(System.Windows.Forms.Keys.NumPad2);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad3] = (app, _) => app.SendKey(System.Windows.Forms.Keys.NumPad3);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad4] = (app, _) => app.SendKey(System.Windows.Forms.Keys.NumPad4);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad5] = (app, _) => app.SendKey(System.Windows.Forms.Keys.NumPad5);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad6] = (app, _) => app.SendKey(System.Windows.Forms.Keys.NumPad6);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad7] = (app, _) => app.SendKey(System.Windows.Forms.Keys.NumPad7);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad8] = (app, _) => app.SendKey(System.Windows.Forms.Keys.NumPad8);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad9] = (app, _) => app.SendKey(System.Windows.Forms.Keys.NumPad9);
            SpecialKeys[OpenQA.Selenium.Keys.LeftShift] = (app, modified) => KeyDownModify(app, System.Windows.Forms.Keys.Shift, modified);
            SpecialKeys[OpenQA.Selenium.Keys.LeftControl] = (app, modified) => KeyDownModify(app, System.Windows.Forms.Keys.Control, modified);
            SpecialKeys[OpenQA.Selenium.Keys.LeftAlt] = (app, modified) => KeyDownModify(app, System.Windows.Forms.Keys.Menu, modified);
            SpecialKeys[OpenQA.Selenium.Keys.Null] = (_, __) => { };
            SpecialKeys[OpenQA.Selenium.Keys.Equal] = (_, __) => { };
            SpecialKeys[OpenQA.Selenium.Keys.Meta] = (_, __) => { };
            SpecialKeys[OpenQA.Selenium.Keys.Command] = (_, __) => { };
        }

        internal static void SendKeys(WindowsAppFriend app, string keys)
        {
            var modifyed = new List<System.Windows.Forms.Keys>();
            int f(int n) => n >= 1 ? n * f(n - 1) : 1;

            while (!string.IsNullOrEmpty(keys))
            {
                var special = SpecialKeys.Select(e => new { e.Key, index = keys.IndexOf(e.Key), execute = e.Value }).Where(e => e.index != -1).OrderBy(e => e.index).FirstOrDefault();
                if (special == null)
                {
                    app.SendKeys(keys);
                    break;
                }

                var before = keys.Substring(0, special.index);
                if (!string.IsNullOrEmpty(before))
                {
                    app.SendKeys(before);
                }

                special.execute(app, modifyed);

                keys = keys.Substring(special.index + special.Key.Length);
            }

            modifyed.ForEach(e => app.KeyUp(e));
        }

        static void KeyDownModify(WindowsAppFriend app, Keys key, List<Keys> modifyed)
        {
            if (modifyed.IndexOf(key) != -1) return;
            app.KeyDown(key);
            modifyed.Add(key);
        }

    }
}
