using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows.KeyMouse;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Selenium.CefSharp.Driver
{
    public class CefSharpWebElement : IWebElement
    {
        private CefSharpDriver _driver;
        private int _index;

        public CefSharpWebElement(CefSharpDriver driver, int index)
        {
            _driver = driver;
            _index = index;
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
            => _driver.ExecuteScriptInternal(JS.Click(_index));

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
            _driver.ExecuteScriptInternal(JS.Focus(_index));

            //TODO adjust text spec.

            _driver.Activate();
            _driver.App.SendKeys(text);
        }

        public void Submit()
        {
            throw new System.NotImplementedException();
        }
    }
}
