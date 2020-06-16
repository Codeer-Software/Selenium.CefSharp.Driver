using System;

namespace Selenium.CefSharp.Driver.Inside
{
    static class ClickSpeck
    {
        internal static void Click(CefSharpWebElement element)
        {
            if (element.TagName.Equals("OPTION", StringComparison.InvariantCultureIgnoreCase))
            {
                var parent = (CefSharpWebElement)element.JavaScriptExecutor.ExecuteScript(JS.GetParentElement(element.Id));
                //TODO: emulate mouse down / up
                //https://www.w3.org/TR/webdriver/#element-click

                parent.Focus();
                if (!parent.Enabled)
                {
                    return;
                }
                var script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({element.Id});
element.selected = true";

                if (parent.GetProperty("multiple").Equals("true", StringComparison.InvariantCultureIgnoreCase))
                {
                    script = $@"
const element = window.__seleniumCefSharpDriver.getElementByEntryId({element.Id});
element.selected = !element.selected";
                }
                element.JavaScriptExecutor.ExecuteScript(script);
                element.JavaScriptExecutor.ExecuteScript("arguments[0].dispatchEvent(new Event('change', {bubbles: true, composed: true}))", parent);
            }
            else
            {
                element.JavaScriptExecutor.ExecuteScript(JS.ScrollIntoView(element.Id));
                var pos = element.Location;
                var size = element.Size;
                pos.Offset(size.Width / 2, size.Height / 2);
                element.CotnrolAccessor.Click(pos);
            }
        }

    }
}
