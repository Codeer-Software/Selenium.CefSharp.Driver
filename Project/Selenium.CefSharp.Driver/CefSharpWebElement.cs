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
        readonly CotnrolAccessor _cotnrolAccessor;

        internal int Id { get; }

        IJavaScriptExecutor JsExecutor => (IJavaScriptExecutor)WrappedDriver;

        internal CefSharpWebElement(IWebDriver driver, int index)
        {
            WrappedDriver = driver;
            _cotnrolAccessor = new CotnrolAccessor(driver);
            Id = index;
        }

        public IWebDriver WrappedDriver { get; }

        public string TagName => (JsExecutor.ExecuteScript(JS.GetTagName(this.Id)) as string)?.ToLower();

        public string Text => JsExecutor.ExecuteScript(JS.GetInnerHTML(this.Id)) as string;

        public bool Enabled => !(bool)JsExecutor.ExecuteScript(JS.GetDisabled(this.Id));

        public bool Selected => (bool)JsExecutor.ExecuteScript(JS.GetSelected(this.Id));

        public Point Location
        {
            get
            {
                var x = Convert.ToInt32(JsExecutor.ExecuteScript(JS.GetBoundingClientRectX(this.Id)), CultureInfo.InvariantCulture);
                var y = Convert.ToInt32(JsExecutor.ExecuteScript(JS.GetBoundingClientRectY(this.Id)), CultureInfo.InvariantCulture);
                return new Point(x, y);
            }
        }

        public Size Size
        {
            get
            {
                var w = Convert.ToInt32(JsExecutor.ExecuteScript(JS.GetBoundingClientRectWidth(this.Id)), CultureInfo.InvariantCulture);
                var h = Convert.ToInt32(JsExecutor.ExecuteScript(JS.GetBoundingClientRectHeight(this.Id)), CultureInfo.InvariantCulture);
                return new Size(w, h);
            }
        }

        public bool Displayed => (bool)JsExecutor.ExecuteScript(JS.GetDisplayed(this.Id));

        public void Clear()
            => JsExecutor.ExecuteScript(JS.SetAttribute(this.Id, "value", string.Empty));

        private void Focus()
            => JsExecutor.ExecuteScript(JS.Focus(this.Id));

        public void Click()
        {
            if(this.TagName.Equals("OPTION", StringComparison.InvariantCultureIgnoreCase))
            {
                var parent = (CefSharpWebElement)JsExecutor.ExecuteScript(JS.GetParentElement(this.Id));
                //TODO: emulate mouse down / up
                //https://www.w3.org/TR/webdriver/#element-click

                parent.Focus();
                if(!parent.Enabled)
                {
                    return;
                }
                var script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
element.selected = true";

                if (parent.GetProperty("multiple").Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
element.selected = !element.selected";
                }
                JsExecutor.ExecuteScript(script);
            }
            else
            {
                JsExecutor.ExecuteScript(JS.ScrollIntoView(Id));
                var pos = Location;
                var size = Size;
                pos.Offset(size.Width / 2, size.Height / 2);
                _cotnrolAccessor.Click(pos);
            }
        }

        public string GetAttribute(string attributeName)
            => JsExecutor.ExecuteScript(JS.GetAttribute(this.Id, attributeName)) as string;

        public string GetCssValue(string propertyName)
            => JsExecutor.ExecuteScript(JS.GetCssValue(this.Id, propertyName)) as string;

        public string GetProperty(string propertyName)
            => JsExecutor.ExecuteScript(JS.GetProperty(this.Id, propertyName)) as string;

        public void SendKeys(string text)
        {
            JsExecutor.ExecuteScript(JS.Focus(Id));
            _cotnrolAccessor.SendKeys(text);
        }

        public void Submit()
            => JsExecutor.ExecuteScript(JS.Submit(this.Id));

        public IWebElement FindElement(By by)
        {
            var text = by.ToString();
            var script = "";
            if (text.StartsWith("By.Id:"))
            {
                script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
return element.querySelector('[id=""{text.Substring("By.Id:".Length).Trim()}""]');";
            }
            if (text.StartsWith("By.Name:"))
            {
                script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
return element.querySelector('[name=""{text.Substring("By.Name:".Length).Trim()}""]');";
            }
            if (text.StartsWith("By.ClassName[Contains]:"))
            {
                script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
return element.getElementsByClassName('{text.Substring("By.ClassName[Contains]:".Length).Trim()}')[0];";
            }
            if (text.StartsWith("By.CssSelector:"))
            {
                script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
return element.querySelector(""{text.Substring("By.CssSelector:".Length).Trim()}"");";
            }
            if (text.StartsWith("By.TagName:"))
            {
                script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
return element.getElementsByTagName('{text.Substring("By.TagName:".Length).Trim()}')[0];";
            }
            if (text.StartsWith("By.XPath:"))
            {
                script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
return window.__seleniumCefSharpDriver.getElementsByXPath('{text.Substring("By.XPath:".Length).Trim()}', element)[0];";
            }
            if (!(JsExecutor.ExecuteScript(script) is CefSharpWebElement result))
            {
                throw new NoSuchElementException($"Element not found: {text}");
            }
            return result;
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            var text = by.ToString();
            var script = "";
            if (text.StartsWith("By.Id:"))
            {
                script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
return element.querySelectorAll('[id=""{text.Substring("By.Id:".Length).Trim()}""]');";
            }
            if (text.StartsWith("By.Name:"))
            {
                script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
return element.querySelectorAll('[name=""{text.Substring("By.Name:".Length).Trim()}""]');";
            }
            if (text.StartsWith("By.ClassName[Contains]:"))
            {
                script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
return element.getElementsByClassName('{text.Substring("By.ClassName[Contains]:".Length).Trim()}');";
            }
            if (text.StartsWith("By.CssSelector:"))
            {
                script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
return element.querySelectorAll(""{text.Substring("By.CssSelector:".Length).Trim()}"");";
            }
            if (text.StartsWith("By.TagName:"))
            {
                script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
return element.getElementsByTagName('{text.Substring("By.TagName:".Length).Trim()}');";
            }
            if (text.StartsWith("By.XPath:"))
            {
                script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({this.Id});
return window.__seleniumCefSharpDriver.getElementsByXPath('{text.Substring("By.XPath:".Length).Trim()}', element);";
            }
            if (!(JsExecutor.ExecuteScript(script) is ReadOnlyCollection<IWebElement> result))
            {
                return new ReadOnlyCollection<IWebElement>(new List<IWebElement>());
            }
            return result;
        }

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

        //TODO 
        public IWebElement FindElementByLinkText(string linkText) => FindElement(By.LinkText(linkText));
        public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText) => FindElements(By.LinkText(linkText));
        public IWebElement FindElementByPartialLinkText(string partialLinkText) => FindElement(By.PartialLinkText(partialLinkText));
        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText) => FindElements(By.PartialLinkText(partialLinkText));

        public Screenshot GetScreenshot()
        {
            JsExecutor.ExecuteScript(JS.ScrollIntoView(Id));
            return _cotnrolAccessor.GetScreenShot(Location, Size);
        }

        public Point LocationOnScreenOnceScrolledIntoView
        {
            get
            {
                var rawLocation = (Dictionary<string, object>)JsExecutor.ExecuteScript("var rect = arguments[0].getBoundingClientRect(); return {'x': rect.left, 'y': rect.top};", this);
                int x = Convert.ToInt32(rawLocation["x"], CultureInfo.InvariantCulture);
                int y = Convert.ToInt32(rawLocation["y"], CultureInfo.InvariantCulture);
                return new Point(x, y);
            }
        }

        public ICoordinates Coordinates => new CefSharpCoordinates(this);
    }
}
