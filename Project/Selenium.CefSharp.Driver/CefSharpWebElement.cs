using Codeer.Friendly.Windows.KeyMouse;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Drawing;

namespace Selenium.CefSharp.Driver
{
    public class CefSharpWebElement : IWebElement
    {
        private CefSharpDriver _driver;
        internal int Id { get; }

        public CefSharpWebElement(CefSharpDriver driver, int index)
        {
            _driver = driver;
            Id = index;
        }

        public string TagName => (_driver.ExecuteScript(JS.GetTagName(this.Id)) as string)?.ToLower();

        public string Text => _driver.ExecuteScript(JS.GetInnerHTML(this.Id)) as string;

        public bool Enabled => !(bool)_driver.ExecuteScript(JS.GetDisabled(this.Id));

        public bool Selected => (bool)_driver.ExecuteScript(JS.GetSelected(this.Id));

        public Point Location
        {
            get
            {
                var x = (long)_driver.ExecuteScript(JS.GetBoundingClientRectX(this.Id));
                var y = (long)_driver.ExecuteScript(JS.GetBoundingClientRectY(this.Id));
                return new Point((int)x, (int)y);
            }
        }

        public Size Size
        {
            get
            {
                var w = (long)_driver.ExecuteScript(JS.GetBoundingClientRectWidth(this.Id));
                var h = (long)_driver.ExecuteScript(JS.GetBoundingClientRectHeight(this.Id));
                return new Size((int)w, (int)h);
            }
        }

        public bool Displayed => (bool)_driver.ExecuteScript(JS.GetDisplayed(this.Id));

        public void Clear()
            => _driver.ExecuteScript(JS.SetAttribute(this.Id, "value", string.Empty));

        public void Click()
            => _driver.ExecuteScript(JS.Click(Id));

        public string GetAttribute(string attributeName)
            => _driver.ExecuteScript(JS.GetAttribute(this.Id, attributeName)) as string;

        public string GetCssValue(string propertyName)
            => _driver.ExecuteScript(JS.GetCssValue(this.Id, propertyName)) as string;

        public string GetProperty(string propertyName)
            => _driver.ExecuteScript(JS.GetProperty(this.Id, propertyName)) as string;

        public void SendKeys(string text)
        {
            _driver.ExecuteScriptInternal(JS.Focus(Id));

            //TODO adjust text spec.

            _driver.Activate();
            _driver.App.SendKeys(text);
        }

        public void Submit()
            => _driver.ExecuteScript(JS.Submit(this.Id));

        public IWebElement FindElement(By by)
        {
            throw new System.NotImplementedException();
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            throw new System.NotImplementedException();
        }
    }
}
