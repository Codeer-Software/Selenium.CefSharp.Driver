using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Selenium.CefSharp.Driver
{
    public class CefSharpWebElement : IWebElement
    {
        private CefSharpDriver _driver;
        private int _id;

        public CefSharpWebElement(CefSharpDriver driver, int id)
        {
            _driver = driver;
            _id = id;
        }

        public string TagName => throw new System.NotImplementedException();

        public string Text => throw new System.NotImplementedException();

        public bool Enabled => throw new System.NotImplementedException();

        public bool Selected => throw new System.NotImplementedException();

        public Point Location => throw new System.NotImplementedException();

        public Size Size => throw new System.NotImplementedException();

        public bool Displayed => throw new System.NotImplementedException();

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public void Click()
            => _driver.ExecuteScript($@"
var element = window.__seleniumCefSharpDriver.elements[{_id}];
__seleniumCefSharpDriver_showAndSelectElement(element);
element.click();
");

        public IWebElement FindElement(By by)
        {
            throw new System.NotImplementedException();
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            throw new System.NotImplementedException();
        }

        public string GetAttribute(string attributeName)
        {
            throw new System.NotImplementedException();
        }

        public string GetCssValue(string propertyName)
        {
            throw new System.NotImplementedException();
        }

        public string GetProperty(string propertyName)
        {
            throw new System.NotImplementedException();
        }

        public void SendKeys(string text)
        {
            throw new System.NotImplementedException();
        }

        public void Submit()
        {
            throw new System.NotImplementedException();
        }
    }
}
