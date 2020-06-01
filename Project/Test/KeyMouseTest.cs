using System.Diagnostics;
using System.IO;
using Codeer.Friendly.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Selenium.CefSharp.Driver;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using Codeer.Friendly.Windows.KeyMouse;

namespace Test
{
    [TestClass]
    public class KeyMouseTestWinForms : KeyMouseTest
    {
        static WindowsAppFriend _app;
        static CefSharpDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [TestInitialize]
        public void TestInitialize()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var appWithDriver = AppRunner.RunWinFormApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
        }
    }

    [TestClass]
    public class KeyMouseTestWpf : KeyMouseTest
    {
        static WindowsAppFriend _app;
        static CefSharpDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [TestInitialize]
        public void TestInitialize()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            var appWithDriver = AppRunner.RunWpfApp();
            _app = appWithDriver.App;
            _driver = appWithDriver.Driver;
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Process.GetProcessById(_app.ProcessId).Kill();
        }
    }

    [TestClass]
    public class KeyMouseTestWeb : KeyMouseTest
    {
        static IWebDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [TestInitialize]
        public void initialize()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _driver = new ChromeDriver();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _driver.Dispose();
        }
    }

    public abstract class KeyMouseTest
    {
        public abstract IWebDriver GetDriver();

        protected string GetHtmlUrl()
        {
            var dir = GetType().Assembly.Location;
            for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
            return Path.Combine(dir, @"Test\Controls.html");
        }

        [TestMethod]
        public void Click()
        {
            var buttonJs = GetDriver().FindElement(By.Id("inputJS"));
            buttonJs.Click();
            var textBoxName = GetDriver().FindElement(By.Id("textBoxName"));
            textBoxName.GetAttribute("value").Is("JS");
        }

        [TestMethod]
        public void SendKey()
        {
            var textBoxName = GetDriver().FindElement(By.Id("textBoxName"));
            textBoxName.SendKeys(Keys.Control + "a");
            textBoxName.SendKeys("abc");
            textBoxName.GetAttribute("value").Is("abc");
            textBoxName.SendKeys(Keys.Control + "a");
            textBoxName.SendKeys(Keys.Shift + "abc");
            textBoxName.GetAttribute("value").Is("ABC");
        }

        [TestMethod]
        public void SendKeySeleniumSpecialKey_BackSpace()
        {
            var textBoxName = GetDriver().FindElement(By.Id("textBoxName"));
            textBoxName.SendKeys(Keys.Control + "a");
            textBoxName.SendKeys("abc");
            textBoxName.SendKeys(Keys.Backspace);
            textBoxName.GetAttribute("value").Is("ab");
        }

        [TestMethod]
        public void SendKeySeleniumSpecialKeyNull()
        {
            var keyTest = GetDriver().FindElement(By.Id("keyTest"));
            keyTest.SendKeys(Keys.Null);
            var ret = PopKeyLog();
            ret.Length.Is(0);
        
        }

        //TODO F10 doesn't work well with .net apps. 
        //[TestMethod]
        //public void SendKeySeleniumSpecialKeyF10()
        //{
        //    var keyTest = GetDriver().FindElement(By.Id("keyTest"));
        //    keyTest.SendKeys(Keys.F10);
        //    var ret = PopKeyLog();
        //    ret.Length.Is(1);
        //    ret[0].Is("F10");

        //    //The F10 key will affect later testing.
        //    var cef = GetDriver() as CefSharpDriver;
        //    if (cef != null)
        //    {
        //        cef.App.SendKey(System.Windows.Forms.Keys.Escape);
        //    }
        //}

        [TestMethod]
        public void SendKeySeleniumSpecialKey()
        {
            var keyTest = GetDriver().FindElement(By.Id("keyTest"));
            keyTest.SendKeys(Keys.NumberPad0);
            keyTest.SendKeys(Keys.NumberPad1);
            keyTest.SendKeys(Keys.NumberPad2);
            keyTest.SendKeys(Keys.NumberPad3);
            keyTest.SendKeys(Keys.NumberPad4);
            keyTest.SendKeys(Keys.NumberPad5);
            keyTest.SendKeys(Keys.NumberPad6);
            keyTest.SendKeys(Keys.NumberPad7);
            keyTest.SendKeys(Keys.NumberPad8);
            keyTest.SendKeys(Keys.NumberPad9);
            keyTest.SendKeys(Keys.Multiply);
            keyTest.SendKeys(Keys.Add);
            keyTest.SendKeys(Keys.Subtract);
            keyTest.SendKeys(Keys.Divide);
            keyTest.SendKeys(Keys.F1);
            keyTest.SendKeys(Keys.F2);
            keyTest.SendKeys(Keys.F3);
            keyTest.SendKeys(Keys.F4);
            keyTest.SendKeys(Keys.F5);
            keyTest.SendKeys(Keys.F6);
            keyTest.SendKeys(Keys.F7);
            keyTest.SendKeys(Keys.F8);
            keyTest.SendKeys(Keys.F9);
            keyTest.SendKeys(Keys.F11);
            keyTest.SendKeys(Keys.F12);
            keyTest.SendKeys(Keys.Decimal);
            keyTest.SendKeys(Keys.Insert);
            keyTest.SendKeys(Keys.Cancel);
            keyTest.SendKeys(Keys.Help);
            keyTest.SendKeys(Keys.Backspace);
            keyTest.SendKeys(Keys.Tab);
            keyTest.SendKeys(Keys.Clear);
            keyTest.SendKeys(Keys.Return);
            keyTest.SendKeys(Keys.Enter);
            keyTest.SendKeys(Keys.Delete);
            keyTest.SendKeys(Keys.Pause);
            keyTest.SendKeys(Keys.Space);
            keyTest.SendKeys(Keys.PageUp);
            keyTest.SendKeys(Keys.PageDown);
            keyTest.SendKeys(Keys.End);
            keyTest.SendKeys(Keys.Home);
            keyTest.SendKeys(Keys.Left);
            keyTest.SendKeys(Keys.ArrowLeft);
            keyTest.SendKeys(Keys.Up);
            keyTest.SendKeys(Keys.ArrowUp);
            keyTest.SendKeys(Keys.Right);
            keyTest.SendKeys(Keys.ArrowRight);
            keyTest.SendKeys(Keys.Down);
            keyTest.SendKeys(Keys.ArrowDown);
            keyTest.SendKeys(Keys.Escape);

            //TODO Depends on keyboard.
            //keyTest.SendKeys(Keys.Separator);
            //keyTest.SendKeys(Keys.Semicolon);
            //System.Windows.Forms.Keys.Oemplus
            //System.Windows.Forms.Keys.Oemcomma

            var ret = PopKeyLog();

            ret.Length.Is(50);
            var i = 0;
            ret[i++].Is("0");
            ret[i++].Is("1");
            ret[i++].Is("2");
            ret[i++].Is("3");
            ret[i++].Is("4");
            ret[i++].Is("5");
            ret[i++].Is("6");
            ret[i++].Is("7");
            ret[i++].Is("8");
            ret[i++].Is("9");
            ret[i++].Is("*");
            ret[i++].Is("+");
            ret[i++].Is("-");
            ret[i++].Is("/");
            ret[i++].Is("F1");
            ret[i++].Is("F2");
            ret[i++].Is("F3");
            ret[i++].Is("F4");
            ret[i++].Is("F5");
            ret[i++].Is("F6");
            ret[i++].Is("F7");
            ret[i++].Is("F8");
            ret[i++].Is("F9");
            ret[i++].Is("F11");
            ret[i++].Is("F12");
            ret[i++].Is(".");
            ret[i++].Is("Insert");
            ret[i++].Is("Cancel");
            ret[i++].Is("Help");
            ret[i++].Is("Backspace");
            ret[i++].Is("Tab");
            ret[i++].Is("Clear");
            ret[i++].Is("Enter");
            ret[i++].Is("Enter");
            ret[i++].Is("Delete");
            ret[i++].Is("Pause");
            ret[i++].Is(" ");
            ret[i++].Is("PageUp");
            ret[i++].Is("PageDown");
            ret[i++].Is("End");
            ret[i++].Is("Home");
            ret[i++].Is("ArrowLeft");
            ret[i++].Is("ArrowLeft");
            ret[i++].Is("ArrowUp");
            ret[i++].Is("ArrowUp");
            ret[i++].Is("ArrowRight");
            ret[i++].Is("ArrowRight");
            ret[i++].Is("ArrowDown");
            ret[i++].Is("ArrowDown");
            ret[i++].Is("Escape");
        }

        [TestMethod]
        public void SendKeyWinFormsSpecialKey()
        {
            var textBoxName = GetDriver().FindElement(By.Id("textBoxName"));
            textBoxName.SendKeys(Keys.Control + "a");
            textBoxName.SendKeys("+%{}");
            textBoxName.GetAttribute("value").Is("+%{}");

            //TODO {^} -> & why? 
        }

        [TestMethod]
        public void SendKeyNotModifiedAndSpecialKeyAndText()
        {
            var textBoxName = GetDriver().FindElement(By.Id("textBoxName"));
            textBoxName.SendKeys(Keys.Control + "a");
            textBoxName.SendKeys("abcdefg" + Keys.Left + Keys.Backspace + "xyz");
            textBoxName.GetAttribute("value").Is("abcdexyzg");
        }

        [TestMethod]
        public void SendKeyModifiedAndSpecialKeyAndText全角()
        {
            var textBoxName = GetDriver().FindElement(By.Id("textBoxName"));
            textBoxName.SendKeys(Keys.Control + "a");
            textBoxName.SendKeys(Keys.Shift + "あbcdefg" + Keys.Left + Keys.Backspace + "xyz");
            textBoxName.GetAttribute("value").Is("あBCDEFXYZ");
        }

        [TestMethod]
        public void SendKeyModifyKeys()
        {
            var keyTest = GetDriver().FindElement(By.Id("keyTest"));

            keyTest.SendKeys(Keys.Alt + Keys.Control + "g");

            var ret1 = PopKeyLog();
            ret1.Is(new[]
            {
                "Alt[alt]",
                "Control[control][alt]",
                "g[control][alt]",
            });

            keyTest.SendKeys(Keys.Shift + Keys.Alt + Keys.Control + "g");
            var ret2 = PopKeyLog();

            //Allow the case that the key taken by keydown when pressing shift key is different in case.
            ret2[3] = ret2[3].ToLower();

            ret2.Is(new[]
            {
                "Shift[shift]",
                "Alt[shift][alt]" ,
                "Control[control][shift][alt]",
                "g[control][shift][alt]"  ,
            });
        }

        [TestMethod]
        public void SendKeyModifyKeysLeft()
        {
            var keyTest = GetDriver().FindElement(By.Id("keyTest"));

            keyTest.SendKeys(Keys.LeftAlt + Keys.LeftControl + "g");

            var ret1 = PopKeyLog();
            ret1.Is(new[]
            {
                "Alt[alt]",
                "Control[control][alt]",
                "g[control][alt]",
            });

            keyTest.SendKeys(Keys.LeftShift + Keys.LeftAlt + Keys.LeftControl + "g");
            var ret2 = PopKeyLog();

            //Allow the case that the key taken by keydown when pressing shift key is different in case.
            ret2[3] = ret2[3].ToLower();

            ret2.Is(new[]
            {
                "Shift[shift]",
                "Alt[shift][alt]" ,
                "Control[control][shift][alt]",
                "g[control][shift][alt]"  ,
            });
        }

        string[] PopKeyLog()
        {
            dynamic ret = ((IJavaScriptExecutor)GetDriver()).ExecuteScript("return window.keylog;");
            if (ret == null) return new string[0];
            ((IJavaScriptExecutor)GetDriver()).ExecuteScript("window.keylog = [];");
            var list = new List<string>();
            foreach (var e in ret)
            {
                list.Add(e);
            }
            return list.ToArray();
        }
    }
}
