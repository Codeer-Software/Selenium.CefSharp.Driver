using Codeer.Friendly.Windows.KeyMouse;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Internal;
using Selenium.CefSharp.Driver.Inside;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Threading;

namespace Selenium.CefSharp.Driver
{
    //Not supported.
    //IWebElementReference

    public class CefSharpWebElement :
        IWebElement, 
        IWrapsDriver,
        IFindsById,
        IFindsByClassName,
        IFindsByLinkText,
        IFindsByName,
        IFindsByTagName,
        IFindsByXPath,
        IFindsByPartialLinkText,
        IFindsByCssSelector,
        ITakesScreenshot,
        ILocatable
    {
        readonly CefSharpFrameDriver _frame;

        internal IJavaScriptExecutor JavaScriptExecutor => _frame;

        internal CotnrolAccessor CotnrolAccessor { get; }
        
        internal int Id { get; }

        internal CefSharpWebElement(CefSharpFrameDriver frame, CotnrolAccessor cotnrolAccessor, int index)
        {
            _frame = frame;
            CotnrolAccessor = cotnrolAccessor;
            Id = index;
        }

        public IWebDriver WrappedDriver => _frame.CefSharpDriver;

        public string TagName => (JavaScriptExecutor.ExecuteScript(JS.GetTagName(Id)) as string)?.ToLower();

        public string Text => JavaScriptExecutor.ExecuteScript(JS.GetInnerHTML(Id)) as string;

        public bool Enabled => !(bool)JavaScriptExecutor.ExecuteScript(JS.GetDisabled(Id));

        public bool Selected => (bool)JavaScriptExecutor.ExecuteScript(JS.GetSelected(Id));

        public Point Location
        {
            get
            {
                var x = Convert.ToInt32(JavaScriptExecutor.ExecuteScript(JS.GetBoundingClientRectX(Id)), CultureInfo.InvariantCulture);
                var y = Convert.ToInt32(JavaScriptExecutor.ExecuteScript(JS.GetBoundingClientRectY(Id)), CultureInfo.InvariantCulture);
                return new Point(x, y);
            }
        }

        public Size Size
        {
            get
            {
                var w = Convert.ToInt32(JavaScriptExecutor.ExecuteScript(JS.GetBoundingClientRectWidth(Id)), CultureInfo.InvariantCulture);
                var h = Convert.ToInt32(JavaScriptExecutor.ExecuteScript(JS.GetBoundingClientRectHeight(Id)), CultureInfo.InvariantCulture);
                return new Size(w, h);
            }
        }

        public bool Displayed => (bool)JavaScriptExecutor.ExecuteScript(JS.GetDisplayed(Id));

        public void Clear()
            => JavaScriptExecutor.ExecuteScript(JS.SetAttribute(Id, "value", string.Empty));

        public void Click()
            => ClickSpeck.Click(this);

        public string GetAttribute(string attributeName)
            => JavaScriptExecutor.ExecuteScript(JS.GetAttribute(Id, attributeName)) as string;

        public string GetCssValue(string propertyName)
            => JavaScriptExecutor.ExecuteScript(JS.GetCssValue(Id, propertyName)) as string;

        public string GetProperty(string propertyName)
            => JavaScriptExecutor.ExecuteScript(JS.GetProperty(Id, propertyName)) as string;

        public void SendKeys(string text)
        {
            if (_frame.CefSharpDriver.FileDetector.IsFile(text))
            {
                var before = NativeMethods.GetWindows(_frame.App.ProcessId);
                Click();
                
                //wait for file dialog.
                while (true)
                {
                    bool hit = false;
                    foreach (var e in NativeMethods.GetWindows(_frame.App.ProcessId))
                    {
                        if (!before.Contains(e))
                        {
                            hit = true;
                            break;
                        }
                    }
                    if (hit) break;
                    Thread.Sleep(10);
                }

                _frame.App.SendKeys(text);
                _frame.App.SendKey(System.Windows.Forms.Keys.Enter);
                return;
            }
            JavaScriptExecutor.ExecuteScript(JS.Focus(Id));
            CotnrolAccessor.SendKeys(text);
        }

        public void Submit()
            => JavaScriptExecutor.ExecuteScript(JS.Submit(Id));

        public IWebElement FindElement(By by) => ElementFinder.FindElementFromElement(JavaScriptExecutor, Id, by);

        public ReadOnlyCollection<IWebElement> FindElements(By by) => ElementFinder.FindElementsFromElement(JavaScriptExecutor, Id, by);

        public IWebElement FindElementById(string id) => FindElement(By.Id(id));

        public ReadOnlyCollection<IWebElement> FindElementsById(string id) => FindElements(By.Id(id));

        public IWebElement FindElementByClassName(string className) => FindElement(By.ClassName(className));

        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className) => FindElements(By.ClassName(className));

        public IWebElement FindElementByName(string name) => FindElement(By.Name(name));

        public ReadOnlyCollection<IWebElement> FindElementsByName(string name) => FindElements(By.Name(name));

        public IWebElement FindElementByTagName(string tagName) => FindElement(By.TagName(tagName));

        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName) => FindElements(By.TagName(tagName));

        public IWebElement FindElementByXPath(string xpath) => FindElement(By.XPath(xpath));

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath) => FindElements(By.XPath(xpath));

        public IWebElement FindElementByCssSelector(string cssSelector) => FindElement(By.CssSelector(cssSelector));

        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector) => FindElements(By.CssSelector(cssSelector));

        public IWebElement FindElementByLinkText(string linkText) => FindElement(By.LinkText(linkText));
        
        public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText) => FindElements(By.LinkText(linkText));
        
        public IWebElement FindElementByPartialLinkText(string partialLinkText) => FindElement(By.PartialLinkText(partialLinkText));
       
        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText) => FindElements(By.PartialLinkText(partialLinkText));

        public Screenshot GetScreenshot()
        {
            JavaScriptExecutor.ExecuteScript(JS.ScrollIntoView(Id));
            return CotnrolAccessor.GetScreenShot(Location, Size);
        }

        public Point LocationOnScreenOnceScrolledIntoView
        {
            get
            {
                var rawLocation = (Dictionary<string, object>)JavaScriptExecutor.ExecuteScript("var rect = arguments[0].getBoundingClientRect(); return {'x': rect.left, 'y': rect.top};", this);
                int x = Convert.ToInt32(rawLocation["x"], CultureInfo.InvariantCulture);
                int y = Convert.ToInt32(rawLocation["y"], CultureInfo.InvariantCulture);
                return new Point(x, y);
            }
        }

        public ICoordinates Coordinates => new Coordinates(this);

        public override bool Equals(object obj)
        {
            var target = obj as CefSharpWebElement;
            if (target == null) return false;
            if (!target._frame.Equals(_frame)) return false;
            return Id == target.Id;
        }

        public override int GetHashCode() => WrappedDriver.GetHashCode() + Id;

        internal void Focus()
            => JavaScriptExecutor.ExecuteScript(JS.Focus(Id));
    }
}
