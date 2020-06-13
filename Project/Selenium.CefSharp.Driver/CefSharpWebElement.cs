using OpenQA.Selenium;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;

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
        readonly IJavaScriptExecutor _javaScriptExecutor;
        readonly CotnrolAccessor _cotnrolAccessor;

        internal int Id { get; }

        internal CefSharpWebElement(IWebDriver driver, CotnrolAccessor cotnrolAccessor, int index)
        {
            WrappedDriver = driver;
            _cotnrolAccessor = cotnrolAccessor;
            _javaScriptExecutor = (IJavaScriptExecutor)WrappedDriver;
            Id = index;
        }

        public IWebDriver WrappedDriver { get; }

        public string TagName => (_javaScriptExecutor.ExecuteScript(JS.GetTagName(Id)) as string)?.ToLower();

        public string Text => _javaScriptExecutor.ExecuteScript(JS.GetInnerHTML(Id)) as string;

        public bool Enabled => !(bool)_javaScriptExecutor.ExecuteScript(JS.GetDisabled(Id));

        public bool Selected => (bool)_javaScriptExecutor.ExecuteScript(JS.GetSelected(Id));

        public Point Location
        {
            get
            {
                var x = Convert.ToInt32(_javaScriptExecutor.ExecuteScript(JS.GetBoundingClientRectX(Id)), CultureInfo.InvariantCulture);
                var y = Convert.ToInt32(_javaScriptExecutor.ExecuteScript(JS.GetBoundingClientRectY(Id)), CultureInfo.InvariantCulture);
                return new Point(x, y);
            }
        }

        public Size Size
        {
            get
            {
                var w = Convert.ToInt32(_javaScriptExecutor.ExecuteScript(JS.GetBoundingClientRectWidth(Id)), CultureInfo.InvariantCulture);
                var h = Convert.ToInt32(_javaScriptExecutor.ExecuteScript(JS.GetBoundingClientRectHeight(Id)), CultureInfo.InvariantCulture);
                return new Size(w, h);
            }
        }

        public bool Displayed => (bool)_javaScriptExecutor.ExecuteScript(JS.GetDisplayed(Id));

        public void Clear()
            => _javaScriptExecutor.ExecuteScript(JS.SetAttribute(Id, "value", string.Empty));

        private void Focus()
            => _javaScriptExecutor.ExecuteScript(JS.Focus(Id));

        public void Click()
        {
            if(TagName.Equals("OPTION", StringComparison.InvariantCultureIgnoreCase))
            {
                var parent = (CefSharpWebElement)_javaScriptExecutor.ExecuteScript(JS.GetParentElement(Id));
                //TODO: emulate mouse down / up
                //https://www.w3.org/TR/webdriver/#element-click

                parent.Focus();
                if(!parent.Enabled)
                {
                    return;
                }
                var script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({Id});
element.selected = true";

                if (parent.GetProperty("multiple").Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({Id});
element.selected = !element.selected";
                }
                _javaScriptExecutor.ExecuteScript(script);
            }
            else
            {
                _javaScriptExecutor.ExecuteScript(JS.ScrollIntoView(Id));
                var pos = Location;
                var size = Size;
                pos.Offset(size.Width / 2, size.Height / 2);
                _cotnrolAccessor.Click(pos);
            }
        }

        public string GetAttribute(string attributeName)
            => _javaScriptExecutor.ExecuteScript(JS.GetAttribute(Id, attributeName)) as string;

        public string GetCssValue(string propertyName)
            => _javaScriptExecutor.ExecuteScript(JS.GetCssValue(Id, propertyName)) as string;

        public string GetProperty(string propertyName)
            => _javaScriptExecutor.ExecuteScript(JS.GetProperty(Id, propertyName)) as string;

        public void SendKeys(string text)
        {
            _javaScriptExecutor.ExecuteScript(JS.Focus(Id));
            _cotnrolAccessor.SendKeys(text);
        }

        public void Submit()
            => _javaScriptExecutor.ExecuteScript(JS.Submit(Id));

        public IWebElement FindElement(By by) => ElementFinder.FindElementFromElement(_javaScriptExecutor, Id, by);

        public ReadOnlyCollection<IWebElement> FindElements(By by) => ElementFinder.FindElementsFromElement(_javaScriptExecutor, Id, by);

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
            _javaScriptExecutor.ExecuteScript(JS.ScrollIntoView(Id));
            return _cotnrolAccessor.GetScreenShot(Location, Size);
        }

        public Point LocationOnScreenOnceScrolledIntoView
        {
            get
            {
                var rawLocation = (Dictionary<string, object>)_javaScriptExecutor.ExecuteScript("var rect = arguments[0].getBoundingClientRect(); return {'x': rect.left, 'y': rect.top};", this);
                int x = Convert.ToInt32(rawLocation["x"], CultureInfo.InvariantCulture);
                int y = Convert.ToInt32(rawLocation["y"], CultureInfo.InvariantCulture);
                return new Point(x, y);
            }
        }

        public ICoordinates Coordinates => new CefSharpCoordinates(this);
    }
}
