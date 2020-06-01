using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.KeyMouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Selenium.CefSharp.Driver
{
    static class KeySpec
    {
        enum KeyAction
        { 
            Down,
            Up,
            DownUp
        }

        static Dictionary<Keys, string> WinFormsSendKeysModifyText = new Dictionary<Keys, string>
        {
            { Keys.Menu, "%"},
            { Keys.ControlKey, "^"},
            { Keys.ShiftKey, "+"},
        };

        static Dictionary<string, Action<KeyAction, WindowsAppFriend, List<Keys>>> SpecialKeys = new Dictionary<string, Action<KeyAction, WindowsAppFriend, List<Keys>>>();

        static KeySpec()
        {
            foreach (var e in typeof(OpenQA.Selenium.Keys).GetFields().Where(e => e.FieldType == typeof(string)))
            {
                if (Enum.TryParse<Keys>(e.Name, out var result))
                {
                    SpecialKeys[(string)e.GetValue(null)] = (keyAction, app, modifyKeys) => SendSpecialKey(keyAction, app, result, modifyKeys);
                }
            }

            SpecialKeys[OpenQA.Selenium.Keys.Backspace] = (keyAction, app, modifyKeys) => SendSpecialKey(keyAction, app, Keys.Back, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.Semicolon] = (keyAction, app, modifyKeys) => SendSpecialKey(keyAction, app, Keys.OemSemicolon, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad0] = (keyAction, app, modifyKeys) => SendSpecialKey(keyAction, app, Keys.NumPad0, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad1] = (keyAction, app, modifyKeys) => SendSpecialKey(keyAction, app, Keys.NumPad1, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad2] = (keyAction, app, modifyKeys) => SendSpecialKey(keyAction, app, Keys.NumPad2, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad3] = (keyAction, app, modifyKeys) => SendSpecialKey(keyAction, app, Keys.NumPad3, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad4] = (keyAction, app, modifyKeys) => SendSpecialKey(keyAction, app, Keys.NumPad4, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad5] = (keyAction, app, modifyKeys) => SendSpecialKey(keyAction, app, Keys.NumPad5, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad6] = (keyAction, app, modifyKeys) => SendSpecialKey(keyAction, app, Keys.NumPad6, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad7] = (keyAction, app, modifyKeys) => SendSpecialKey(keyAction, app, Keys.NumPad7, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad8] = (keyAction, app, modifyKeys) => SendSpecialKey(keyAction, app, Keys.NumPad8, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.NumberPad9] = (keyAction, app, modifyKeys) => SendSpecialKey(keyAction, app, Keys.NumPad9, modifyKeys);
            SpecialKeys[OpenQA.Selenium.Keys.Shift] = (keyAction, app, modifyKeys) => SendModifyKey(keyAction, app, modifyKeys, Keys.ShiftKey);
            SpecialKeys[OpenQA.Selenium.Keys.LeftShift] = (keyAction, app, modifyKeys) => SendModifyKey(keyAction, app, modifyKeys, Keys.ShiftKey);
            SpecialKeys[OpenQA.Selenium.Keys.Control] = (keyAction, app, modifyKeys) => SendModifyKey(keyAction, app, modifyKeys, Keys.ControlKey);
            SpecialKeys[OpenQA.Selenium.Keys.LeftControl] = (keyAction, app, modifyKeys) => SendModifyKey(keyAction, app, modifyKeys, Keys.ControlKey);
            SpecialKeys[OpenQA.Selenium.Keys.Alt] = (keyAction, app, modifyKeys) => SendModifyKey(keyAction, app, modifyKeys, Keys.Menu);
            SpecialKeys[OpenQA.Selenium.Keys.LeftAlt] = (keyAction, app, modifyKeys) => SendModifyKey(keyAction, app, modifyKeys, Keys.Menu);
            SpecialKeys[OpenQA.Selenium.Keys.Null] = (_, __, ___) => { };
            SpecialKeys[OpenQA.Selenium.Keys.Equal] = (_, __, ___) => throw new NotSupportedException("Keys.Equal is not supprted. Please use \"=\".");
            SpecialKeys[OpenQA.Selenium.Keys.Meta] = (_, __, ___) => throw new NotSupportedException("Keys.Meta is not supprted. Please don't use it.");
            SpecialKeys[OpenQA.Selenium.Keys.Command] = (_, __, ___) => throw new NotSupportedException("Keys.Command is not supprted. Please don't use it.");
        }

        internal static void SendKeys(WindowsAppFriend app, string keys)
        {
            if (IsSimpleModify(keys))
            {
                //For alphanumeric characters and modifier keys only, make the key up / down order closer to human operation
                SendSimpleModifyKeys(app, keys);
            }
            else
            {
                //If the modifier key is not included, it works the same as selenium. However, if the modifier keys and non-alphanumeric characters are mixed, they will not be exactly the same.
                keys = SendKeysCore(app, keys);
            }
        }

        static void SendSimpleModifyKeys(WindowsAppFriend app, string keys)
        {
            var modifyKeyUp = new List<Action>();
            foreach (var e in keys)
            {
                if (SpecialKeys.TryGetValue(e.ToString(), out var sendSpecialKey))
                {
                    sendSpecialKey(KeyAction.Down, app, new List<Keys>());
                    modifyKeyUp.Add(() => sendSpecialKey(KeyAction.Up, app, new List<Keys>()));
                }
                else
                {
                    var keyCode = (Keys)e.ToString().ToUpper()[0];
                    app.KeyDown(keyCode);
                    app.KeyUp(keyCode);
                }
            }
            modifyKeyUp.Reverse();
            modifyKeyUp.ForEach(e => e());
        }

        static void SendModifyKey(KeyAction keyAction, WindowsAppFriend app, List<Keys> modifyKeys, Keys key)
        {
            switch (keyAction)
            {
                case KeyAction.Down:
                    app.KeyDown(key);
                    break;
                case KeyAction.Up:
                    app.KeyUp(key);
                    break;
                case KeyAction.DownUp:
                    modifyKeys.Add(key);
                    break;
            }
        }

        static string SendKeysCore(WindowsAppFriend app, string keys)
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
                    ModifyAndAdjustSendKeys(app, before, modifyed);
                }

                special.execute(KeyAction.DownUp, app, modifyed);

                keys = keys.Substring(special.index + special.Key.Length);
            }

            return keys;
        }

        static void SendSpecialKey(KeyAction keyAction, WindowsAppFriend app, Keys key, List<Keys> modifyKeys)
        {
            switch (keyAction)
            {
                case KeyAction.Down:
                    app.KeyDown(key);
                    break;
                case KeyAction.Up:
                    app.KeyUp(key);
                    break;
                case KeyAction.DownUp:
                    var modifyKeysDistinct = modifyKeys.Distinct().ToList();
                    modifyKeysDistinct.ForEach(e => app.KeyDown(e));
                    app.SendKey(key);
                    modifyKeysDistinct.Reverse();
                    modifyKeysDistinct.ForEach(e => app.KeyUp(e));
                    break;
            }
        }

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
            var modify = string.Join(string.Empty, modifyKeys.Select(e => WinFormsSendKeysModifyText[e]));
            if (!string.IsNullOrEmpty(modify)) keys = modify + "(" + keys + ")";

            app.SendKeys(keys);
        }

        public static bool IsSimpleModify(string target)
        {
            foreach (var e in SpecialKeys)
            {
                target = target.Replace(e.Key, string.Empty);
            }
            return !Regex.IsMatch(target, @"[^a-zA-z0-9]");
        }
    }
}
