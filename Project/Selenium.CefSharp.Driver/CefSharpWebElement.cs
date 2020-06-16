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

        public IWebDriver WrappedDriver => _frame.CefSharpDriver;

        public string TagName => Execute<string>(JsGetTagName())?.ToLower();

        public string Text => Execute<string>(JsGetInnerHTML());

        public bool Enabled => !Execute<bool>(JsGetDisabled());

        public bool Selected => Execute<bool>(JsGetSelected());

        public Point Location
        {
            get
            {
                var rawLocation = Execute<Dictionary<string, object>>("var rect = arguments[0].getBoundingClientRect(); return {'x': rect.left, 'y': rect.top};");
                int x = Convert.ToInt32(rawLocation["x"], CultureInfo.InvariantCulture);
                int y = Convert.ToInt32(rawLocation["y"], CultureInfo.InvariantCulture);
                return new Point(x, y);
            }
        }

        public Size Size
        {
            get
            {
                var rawLocation = Execute<Dictionary<string, object>>("var rect = arguments[0].getBoundingClientRect(); return {'w': rect.width, 'h': rect.height};");
                int w = Convert.ToInt32(rawLocation["w"], CultureInfo.InvariantCulture);
                int h = Convert.ToInt32(rawLocation["h"], CultureInfo.InvariantCulture);
                return new Size(w, h);
            }
        }

        public bool Displayed => Execute<bool>(JsGetDisplayed());

        public Point LocationOnScreenOnceScrolledIntoView
        {
            get
            {
                ScrollIntoView();
                return Location;
            }
        }

        public ICoordinates Coordinates => new Coordinates(this);

        internal IJavaScriptExecutor JavaScriptExecutor => _frame;

        internal CotnrolAccessor CotnrolAccessor { get; }
        
        internal int Id { get; }

        internal CefSharpWebElement(CefSharpFrameDriver frame, CotnrolAccessor cotnrolAccessor, int index)
        {
            _frame = frame;
            CotnrolAccessor = cotnrolAccessor;
            Id = index;
        }

        public void Clear()
            => Execute(JsSetAttribute("value", string.Empty));

        public void Click()
            => ClickSpeck.Click(this);

        public string GetAttribute(string attributeName)
            => Execute<string>(JsGetAttribute(attributeName));

        public string GetCssValue(string propertyName)
            => Execute<string>(JsGetCssValue(propertyName));

        public string GetProperty(string propertyName)
            => Execute<string>(JsGetProperty(propertyName));

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
            Execute(JsFocus());
            CotnrolAccessor.SendKeys(text);
        }

        public void Submit()
            => Execute(JsSubmit());

        public IWebElement FindElement(By by)
            => ElementFinder.FindElementFromElement(JavaScriptExecutor, Id, by);

        public ReadOnlyCollection<IWebElement> FindElements(By by)
            => ElementFinder.FindElementsFromElement(JavaScriptExecutor, Id, by);

        public IWebElement FindElementById(string id)
            => FindElement(By.Id(id));

        public ReadOnlyCollection<IWebElement> FindElementsById(string id)
            => FindElements(By.Id(id));

        public IWebElement FindElementByClassName(string className)
            => FindElement(By.ClassName(className));

        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
            => FindElements(By.ClassName(className));

        public IWebElement FindElementByName(string name)
            => FindElement(By.Name(name));

        public ReadOnlyCollection<IWebElement> FindElementsByName(string name)
            => FindElements(By.Name(name));

        public IWebElement FindElementByTagName(string tagName)
            => FindElement(By.TagName(tagName));

        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
            => FindElements(By.TagName(tagName));

        public IWebElement FindElementByXPath(string xpath)
            => FindElement(By.XPath(xpath));

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
            => FindElements(By.XPath(xpath));

        public IWebElement FindElementByCssSelector(string cssSelector)
            => FindElement(By.CssSelector(cssSelector));

        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector)
            => FindElements(By.CssSelector(cssSelector));

        public IWebElement FindElementByLinkText(string linkText)
            => FindElement(By.LinkText(linkText));
        
        public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText)
            => FindElements(By.LinkText(linkText));
        
        public IWebElement FindElementByPartialLinkText(string partialLinkText)
            => FindElement(By.PartialLinkText(partialLinkText));
       
        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText)
            => FindElements(By.PartialLinkText(partialLinkText));

        public Screenshot GetScreenshot()
        {
            ScrollIntoView();
            return CotnrolAccessor.GetScreenShot(Location, Size);
        }

        public override bool Equals(object obj)
        {
            var target = obj as CefSharpWebElement;
            if (target == null) return false;
            if (!target._frame.Equals(_frame)) return false;
            return Id == target.Id;
        }

        public override int GetHashCode()
            => WrappedDriver.GetHashCode() + Id;

        internal CefSharpWebElement GetParentElement()
            => Execute<CefSharpWebElement>(JsGetParentElement());

        internal void ScrollIntoView()
            => Execute(JsScrollIntoView());

        internal void Focus()
            => Execute(JsFocus());

        object Execute(string js)
            => JavaScriptExecutor.ExecuteScript(js, this);

        T Execute<T>(string js)
            => (T)JavaScriptExecutor.ExecuteScript(js, this);

        static string JsGetAttribute(string attrName) => $@"
const elem = arguments[0];
const value = elem['{attrName}'];
if(window.__seleniumCefSharpDriver.isUndefOrNull(value)) {{
    return elem.getAttribute('{attrName}');
}}
return value + '';";

        static string JsSetAttribute(string attrName, string value) => $@"
const elem = arguments[0];
return elem.setAttribute('{attrName}', '{value}');";

        static string JsGetProperty(string propName) => $@"
const elem = arguments[0];
const value = elem['{propName}'];
if(window.__seleniumCefSharpDriver.isUndefOrNull(value)) {{
    return;
}}
return value + '';";

        static string JsFocus()
    => $@"
const element = arguments[0];
element.scrollIntoView(true);
element.focus();
";

        static string JsScrollIntoView() => $@"
const element = arguments[0];
element.scrollIntoView(true);
";

        static string JsGetTagName()
    => $@"
const element = arguments[0];
return element.tagName;
";

        static string JsGetInnerHTML()
    => $@"
const element = arguments[0];
return element.innerHTML;
";

        static string JsGetDisabled()
    => $@"
const element = arguments[0];
return element.disabled;
";
        static string JsGetSelected()
    => $@"
const element = arguments[0];
if ('selected' in element) return element.selected;
if ('checked' in element) return element.checked;
return false;
";

        static string JsGetDisplayed()
    => $@"
const element = arguments[0];
" + @"
if (element.offsetParent === null) {
    return false;
}

let target = element;
do {
const style = getComputedStyle(target);

if (style.display === 'none'
    || style.visibility !== 'visible'
    || parseFloat(style.opacity || '') <= 0.0
    || parseInt(style.height || '', 10) <= 0
    || parseInt(style.width || '', 10) <= 0
) {
    return false;
}

target = target.parentElement;
} while (target !== null)

return true;
";

        static string JsGetCssValue(string propertyName)
    => $@"
const element = arguments[0];
const style = getComputedStyle(element);
return style['{propertyName}'];
";

        static string JsSubmit()
            => $@"
const element = arguments[0];
element.submit();
";

        static string JsGetParentElement()
            => $@"
const element = arguments[0];
return element.parentElement;
";
    }
}
