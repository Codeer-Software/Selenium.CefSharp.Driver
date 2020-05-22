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

        public string TagName => (_driver.ExecuteScript(JS.GetTagName(this._index)) as string)?.ToLower();

        public string Text => _driver.ExecuteScript(JS.GetInnerHTML(this._index)) as string;

        public bool Enabled => !(bool)_driver.ExecuteScript(JS.GetDisabled(this._index));

        public bool Selected => (bool)_driver.ExecuteScript(JS.GetSelected(this._index));

        public Point Location
        {
            get
            {
                var x = (long)_driver.ExecuteScript(JS.GetBoundingClientRectX(this._index));
                var y = (long)_driver.ExecuteScript(JS.GetBoundingClientRectY(this._index));
                return new Point((int)x, (int)y);
            }
        }

        public Size Size
        {
            get
            {
                var w = (long)_driver.ExecuteScript(JS.GetBoundingClientRectWidth(this._index));
                var h = (long)_driver.ExecuteScript(JS.GetBoundingClientRectHeight(this._index));
                return new Size((int)w, (int)h);
            }
        }

        public bool Displayed => throw new System.NotImplementedException();

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public void Click()
            => _driver.ExecuteScript(JS.Click(_index));

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
            return _driver.ExecuteScript(JS.GetAttribute(this._index, attributeName)) as string;
        }

        public string GetCssValue(string propertyName)
        {
            throw new System.NotImplementedException();
        }

        public string GetProperty(string propertyName)
        {
            return _driver.ExecuteScript(JS.GetProperty(this._index, propertyName)) as string;
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

        internal int Id
        {
            get { return this._index; }
        }
    }
}
