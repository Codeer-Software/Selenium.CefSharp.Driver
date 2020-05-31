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
        static Dictionary<string, Action<WindowsAppFriend, List<Keys>>> SpecialKeys = new Dictionary<string, Action<WindowsAppFriend, List<Keys>>>();

        static KeySpec()
        {
            foreach (var e in typeof(OpenQA.Selenium.Keys).GetFields().Where(e => e.FieldType == typeof(string)))
            {
                if (Enum.TryParse<Keys>(e.Name, out var result))
                {
                    SpecialKeys[(string)e.GetValue(null)] = (app, modifyKeys) => SendModifyAndKey(app, result, modifyKeys);
                }
            }

            SpecialKeys[OpenQA.Selenium.Keys.Backspace] = (app, modifyKeys) => SendModifyAndKey(app, Keys.Back, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.Semicolon] = (app, modifyKeys) => SendModifyAndKey(app, Keys.OemSemicolon, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad0] = (app, modifyKeys) => SendModifyAndKey(app, Keys.NumPad0, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad1] = (app, modifyKeys) => SendModifyAndKey(app, Keys.NumPad1, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad2] = (app, modifyKeys) => SendModifyAndKey(app, Keys.NumPad2, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad3] = (app, modifyKeys) => SendModifyAndKey(app, Keys.NumPad3, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad4] = (app, modifyKeys) => SendModifyAndKey(app, Keys.NumPad4, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad5] = (app, modifyKeys) => SendModifyAndKey(app, Keys.NumPad5, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad6] = (app, modifyKeys) => SendModifyAndKey(app, Keys.NumPad6, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad7] = (app, modifyKeys) => SendModifyAndKey(app, Keys.NumPad7, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad8] = (app, modifyKeys) => SendModifyAndKey(app, Keys.NumPad8, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad9] = (app, modifyKeys) => SendModifyAndKey(app, Keys.NumPad9, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.LeftShift] = (app, modifyKeys) => modifyKeys.Add(Keys.Shift);
            SpecialKeys[OpenQA.Selenium.Keys.LeftControl] = (app, modifyKeys) => modifyKeys.Add(Keys.Control);
            SpecialKeys[OpenQA.Selenium.Keys.LeftAlt] = (app, modifyKeys) => modifyKeys.Add(Keys.Alt);
            SpecialKeys[OpenQA.Selenium.Keys.Null] = (_, __) => { };
            SpecialKeys[OpenQA.Selenium.Keys.Equal] = (_, __) => { };
            SpecialKeys[OpenQA.Selenium.Keys.Meta] = (_, __) => { };
            SpecialKeys[OpenQA.Selenium.Keys.Command] = (_, __) => { };
        }

        internal static void SendKeys(WindowsAppFriend app, string keys)
        {
            var modifyed = new List<Keys>();
            int f(int n) => n >= 1 ? n * f(n - 1) : 1;

            while (!string.IsNullOrEmpty(keys))
            {
                var special = SpecialKeys.Select(e => new { e.Key, index = keys.IndexOf(e.Key), execute = e.Value }).Where(e => e.index != -1).OrderBy(e => e.index).FirstOrDefault();
                if (special == null)
                {
                    ModifyAndAdjustSendKeys(app, keys, modifyed);
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

        static void SendModifyAndKey(WindowsAppFriend app, Keys key, List<Keys> modifyKeys)
            => app.SendModifyAndKey(modifyKeys.Contains(Keys.Control), modifyKeys.Contains(Keys.Shift), modifyKeys.Contains(Keys.Alt), key);

        static void ModifyAndAdjustSendKeys(WindowsAppFriend app, string keys, List<Keys> modifyKeys)
        {
            //adjust winforms sendkeys spec.
            keys = keys.Replace("{", OpenQA.Selenium.Keys.Null);
            keys = keys.Replace("}", "{}}");
            keys = keys.Replace(OpenQA.Selenium.Keys.Null, "{{}");
            keys = keys.Replace("%", "{%}");
            keys = keys.Replace("+", "{+}");
            keys = keys.Replace("^", "{^}");

            //modify
            var modify = string.Empty;
            if (modifyKeys.Contains(Keys.Alt)) modify += "%";
            if (modifyKeys.Contains(Keys.Control)) modify += "^";
            if (modifyKeys.Contains(Keys.Shift)) modify += "+";
            if (!string.IsNullOrEmpty(modify)) keys = modify + "(" + keys + ")";

            app.SendKeys(keys);
        }
    }
}
