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
    /// <summary>
    /// Provides a mechanism to get elements off the page for test
    /// </summary>
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

        /// <summary>
        /// Gets the <see cref="IWebDriver"/> used to find this element.
        /// </summary>
        public IWebDriver WrappedDriver => _frame.CefSharpDriver;

        /// <summary>
        /// Gets the tag name of this element.
        /// </summary>
        /// <remarks>
        /// The <see cref="TagName"/> property returns the tag name of the
        /// element, not the value of the name attribute. For example, it will return
        /// "input" for an element specified by the HTML markup &lt;input name="foo" /&gt;.
        /// </remarks>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
        public string TagName => Execute<string>(JsGetTagName())?.ToLower();

        /// <summary>
        /// Gets the innerText of this element, without any leading or trailing whitespace,
        /// and with other whitespace collapsed.
        /// </summary>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
        public string Text => Execute<string>(JsGetInnerHTML());

        /// <summary>
        /// Gets a value indicating whether or not this element is enabled.
        /// </summary>
        /// <remarks>The <see cref="Enabled"/> property will generally
        /// return <see langword="true"/> for everything except explicitly disabled input elements.</remarks>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
        public bool Enabled => !Execute<bool>(JsGetDisabled());

        /// <summary>
        /// Gets a value indicating whether or not this element is selected.
        /// </summary>
        /// <remarks>This operation only applies to input elements such as checkboxes,
        /// options in a select element and radio buttons.</remarks>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
        public bool Selected => Execute<bool>(JsGetSelected());

        /// <summary>
        /// Gets a <see cref="Point"/> object containing the coordinates of the upper-left corner
        /// of this element relative to the upper-left corner of the page.
        /// </summary>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
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

        /// <summary>
        /// Gets a <see cref="Size"/> object containing the height and width of this element.
        /// </summary>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
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

        /// <summary>
        /// Gets a value indicating whether or not this element is displayed.
        /// </summary>
        /// <remarks>The <see cref="Displayed"/> property avoids the problem
        /// of having to parse an element's "style" attribute to determine
        /// visibility of an element.</remarks>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
        public bool Displayed => Execute<bool>(JsGetDisplayed());

        /// <summary>
        /// Gets the point where the element would be when scrolled into view.
        /// </summary>
        public Point LocationOnScreenOnceScrolledIntoView
        {
            get
            {
                ScrollIntoView();
                return Location;
            }
        }

        /// <summary>
        /// Gets the coordinates identifying the location of this element using
        /// various frames of reference.
        /// </summary>
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

        /// <summary>
        /// Clears the content of this element.
        /// </summary>
        /// <remarks>If this element is a text entry element, the <see cref="Clear"/>
        /// method will clear the value. It has no effect on other elements. Text entry elements
        /// are defined as elements with INPUT or TEXTAREA tags.</remarks>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
        public void Clear()
            => Execute(JsSetAttribute("value", string.Empty));

        /// <summary>
        /// Clicks this element.
        /// </summary>
        /// <remarks>
        /// Click this element. If the click causes a new page to load, the <see cref="Click"/>
        /// method will attempt to block until the page has loaded. After calling the
        /// <see cref="Click"/> method, you should discard all references to this
        /// element unless you know that the element and the page will still be present.
        /// Otherwise, any further operations performed on this element will have an undefined
        /// behavior.
        /// </remarks>
        /// <exception cref="InvalidElementStateException">Thrown when the target element is not enabled.</exception>
        /// <exception cref="ElementNotVisibleException">Thrown when the target element is not visible.</exception>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
        public void Click()
            => ClickSpeck.Click(this);

        /// <summary>
        /// Gets the value of the specified attribute for this element.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>The attribute's current value. Returns a <see langword="null"/> if the
        /// value is not set.</returns>
        /// <remarks>The <see cref="GetAttribute"/> method will return the current value
        /// of the attribute, even if the value has been modified after the page has been
        /// loaded. Note that the value of the following attributes will be returned even if
        /// there is no explicit attribute on the element:
        /// <list type="table">
        /// <listheader>
        /// <term>Attribute name</term>
        /// <term>Value returned if not explicitly specified</term>
        /// <term>Valid element types</term>
        /// </listheader>
        /// <item>
        /// <description>checked</description>
        /// <description>checked</description>
        /// <description>Check Box</description>
        /// </item>
        /// <item>
        /// <description>selected</description>
        /// <description>selected</description>
        /// <description>Options in Select elements</description>
        /// </item>
        /// <item>
        /// <description>disabled</description>
        /// <description>disabled</description>
        /// <description>Input and other UI elements</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
        public string GetAttribute(string attributeName)
            => Execute<string>(JsGetAttribute(attributeName));

        /// <summary>
        /// Gets the value of a CSS property of this element.
        /// </summary>
        /// <param name="propertyName">The name of the CSS property to get the value of.</param>
        /// <returns>The value of the specified CSS property.</returns>
        /// <remarks>The value returned by the <see cref="GetCssValue"/>
        /// method is likely to be unpredictable in a cross-browser environment.
        /// Color values should be returned as hex strings. For example, a
        /// "background-color" property set as "green" in the HTML source, will
        /// return "#008000" for its value.</remarks>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
        public string GetCssValue(string propertyName)
            => Execute<string>(JsGetCssValue(propertyName));

        /// <summary>
        /// Gets the value of a JavaScript property of this element.
        /// </summary>
        /// <param name="propertyName">The name JavaScript the JavaScript property to get the value of.</param>
        /// <returns>The JavaScript property's current value. Returns a <see langword="null"/> if the
        /// value is not set or the property does not exist.</returns>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
        public string GetProperty(string propertyName)
            => Execute<string>(JsGetProperty(propertyName));

        /// <summary>
        /// Simulates typing text into the element.
        /// </summary>
        /// <param name="text">The text to type into the element.</param>
        /// <remarks>The text to be typed may include special characters like arrow keys,
        /// backspaces, function keys, and so on. Valid special keys are defined in
        /// <see cref="Keys"/>.</remarks>
        /// <seealso cref="Keys"/>
        /// <exception cref="InvalidElementStateException">Thrown when the target element is not enabled.</exception>
        /// <exception cref="ElementNotVisibleException">Thrown when the target element is not visible.</exception>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
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

        /// <summary>
        /// Submits this element to the web server.
        /// </summary>
        /// <remarks>If this current element is a form, or an element within a form,
        /// then this will be submitted to the web server. If this causes the current
        /// page to change, then this method will attempt to block until the new page
        /// is loaded.</remarks>
        /// <exception cref="StaleElementReferenceException">Thrown when the target element is no longer valid in the document DOM.</exception>
        public void Submit()
            => Execute(JsSubmit());

        /// <summary>
        /// Finds the first <see cref="IWebElement"/> using the given method.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>The first matching <see cref="IWebElement"/> on the current context.</returns>
        /// <exception cref="NoSuchElementException">If no element matches the criteria.</exception>
        public IWebElement FindElement(By by)
            => ElementFinder.FindElementFromElement(JavaScriptExecutor, Id, by);

        /// <summary>
        /// Finds all <see cref="IWebElement">IWebElements</see> within the current context
        /// using the given mechanism.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> of all <see cref="IWebElement">WebElements</see>
        /// matching the current criteria, or an empty list if nothing matches.</returns>
        public ReadOnlyCollection<IWebElement> FindElements(By by)
            => ElementFinder.FindElementsFromElement(JavaScriptExecutor, Id, by);

        /// <summary>
        /// Finds the first element matching the specified id.
        /// </summary>
        /// <param name="id">The id to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        public IWebElement FindElementById(string id)
            => FindElement(By.Id(id));

        /// <summary>
        /// Finds all elements matching the specified id.
        /// </summary>
        /// <param name="id">The id to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        public ReadOnlyCollection<IWebElement> FindElementsById(string id)
            => FindElements(By.Id(id));

        /// <summary>
        /// Finds the first element matching the specified CSS class.
        /// </summary>
        /// <param name="className">The CSS class to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        public IWebElement FindElementByClassName(string className)
            => FindElement(By.ClassName(className));

        /// <summary>
        /// Finds all elements matching the specified CSS class.
        /// </summary>
        /// <param name="className">The CSS class to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
            => FindElements(By.ClassName(className));

        /// <summary>
        /// Finds the first element matching the specified name.
        /// </summary>
        /// <param name="name">The name to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        public IWebElement FindElementByName(string name)
            => FindElement(By.Name(name));

        /// <summary>
        /// Finds all elements matching the specified name.
        /// </summary>
        /// <param name="name">The name to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        public ReadOnlyCollection<IWebElement> FindElementsByName(string name)
            => FindElements(By.Name(name));

        /// <summary>
        /// Finds the first element matching the specified tag name.
        /// </summary>
        /// <param name="tagName">The tag name to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        public IWebElement FindElementByTagName(string tagName)
            => FindElement(By.TagName(tagName));

        /// <summary>
        /// Finds all elements matching the specified tag name.
        /// </summary>
        /// <param name="tagName">The tag name to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
            => FindElements(By.TagName(tagName));

        /// <summary>
        /// Finds the first element matching the specified XPath query.
        /// </summary>
        /// <param name="xpath">The XPath query to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        public IWebElement FindElementByXPath(string xpath)
            => FindElement(By.XPath(xpath));

        /// <summary>
        /// Finds all elements matching the specified XPath query.
        /// </summary>
        /// <param name="xpath">The XPath query to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
            => FindElements(By.XPath(xpath));
        /// <summary>
        /// Finds the first element matching the specified CSS selector.
        /// </summary>
        /// <param name="cssSelector">The id to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        public IWebElement FindElementByCssSelector(string cssSelector)
            => FindElement(By.CssSelector(cssSelector));

        /// <summary>
        /// Finds all elements matching the specified CSS selector.
        /// </summary>
        /// <param name="cssSelector">The CSS selector to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector)
            => FindElements(By.CssSelector(cssSelector));

        /// <summary>
        /// Finds the first element matching the specified link text.
        /// </summary>
        /// <param name="linkText">The link text to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        public IWebElement FindElementByLinkText(string linkText)
            => FindElement(By.LinkText(linkText));

        /// <summary>
        /// Finds all elements matching the specified link text.
        /// </summary>
        /// <param name="linkText">The link text to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText)
            => FindElements(By.LinkText(linkText));

        /// <summary>
        /// Finds the first element matching the specified partial link text.
        /// </summary>
        /// <param name="partialLinkText">The partial link text to match.</param>
        /// <returns>The first <see cref="IWebElement"/> matching the criteria.</returns>
        public IWebElement FindElementByPartialLinkText(string partialLinkText)
            => FindElement(By.PartialLinkText(partialLinkText));

        /// <summary>
        /// Finds all elements matching the specified partial link text.
        /// </summary>
        /// <param name="partialLinkText">The partial link text to match.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> containing all
        /// <see cref="IWebElement">IWebElements</see> matching the criteria.</returns>
        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText)
            => FindElements(By.PartialLinkText(partialLinkText));

        /// <summary>
        /// Gets a <see cref="Screenshot"/> object representing the image of this element on the screen.
        /// </summary>
        /// <returns>A <see cref="Screenshot"/> object containing the image.</returns>
        public Screenshot GetScreenshot()
        {
            ScrollIntoView();
            return CotnrolAccessor.GetScreenShot(Location, Size);
        }

        /// <summary>
        /// Compares if two elements are equal
        /// </summary>
        /// <param name="obj">Object to compare against</param>
        /// <returns>A boolean if it is equal or not</returns>
        public override bool Equals(object obj)
        {
            var target = obj as CefSharpWebElement;
            if (target == null) return false;
            if (!target._frame.Equals(_frame)) return false;
            return Id == target.Id;
        }

        /// <summary>
        /// Method to get the hash code of the element
        /// </summary>
        /// <returns>Integer of the hash code for the element</returns>
        public override int GetHashCode()
            => WrappedDriver.GetHashCode() << 8 + Id;

        internal CefSharpWebElement GetParentElement()
            => Execute<CefSharpWebElement>(JsGetParentElement());

        internal void ScrollIntoView()
            => Execute(JsScrollIntoView());

        internal void Focus()
            => Execute(JsFocus());

        internal bool HitTestCenter()
            => Execute<bool>(@"
var element = arguments[0];
var rc = element.getBoundingClientRect();
var hitElement = document.elementFromPoint(rc.x + rc.width / 2, rc.y + rc.height / 2);

while (!!hitElement)
{
    if (hitElement === element) return true;
    hitElement = hitElement.parentElement;
}
return false;
");

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
