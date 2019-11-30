using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Selenium.CefSharp.Driver
{
    public class CefSharpWebElement : IWebElement
    {

        private CefSharpDriver driver;

        private string elementId;
        private Dictionary<string, object> elementDictionary;

        public CefSharpWebElement(Dictionary<string, object> elementDictionary)
        {
            this.elementDictionary = elementDictionary;
        }

        public CefSharpWebElement(CefSharpDriver driver, string id) {
            this.driver = driver;
            this.elementId = id;
        }

        /// <summary>
        /// Get Tag of element
        /// </summary>
        public string TagName
        {
            get
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("id", this.elementId);
                Response commandResponse = this.Execute(DriverCommand.GetElementTagName, parameters);
                return commandResponse.Value.ToString();
            }
        }

        /// <summary>
        /// Get Text of element
        /// </summary>
        public string Text
        {
            get
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("id", this.elementId);
                Response commandResponse = this.Execute(DriverCommand.GetElementText, parameters);
                return commandResponse.Value.ToString();
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this element is enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("id", this.elementId);
                Response commandResponse = this.Execute(DriverCommand.IsElementEnabled, parameters);
                return (bool)commandResponse.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this element is selected.
        /// </summary>
        public bool Selected
        {
            get
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("id", this.elementId);
                Response commandResponse = this.Execute(DriverCommand.IsElementSelected, parameters);
                return (bool)commandResponse.Value;
            }
        }

        /// <summary>
        /// Gets a <see cref="Point"/> object containing the coordinates of the upper-left corner
        /// </summary>
        public Point Location
        {
            get
            {
                string getLocationCommand = DriverCommand.GetElementRect;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("id", this.elementId);
                Response commandResponse = this.Execute(getLocationCommand, parameters);
                Dictionary<string, object> rawPoint = (Dictionary<string, object>)commandResponse.Value;
                int x = Convert.ToInt32(rawPoint["x"], CultureInfo.InvariantCulture);
                int y = Convert.ToInt32(rawPoint["y"], CultureInfo.InvariantCulture);
                return new Point(x, y);
            }
        }

        /// <summary>
        /// Gets a <see cref="Size"/> object containing the height and width of this element.
        /// </summary>
        public Size Size
        {
            get
            {
                string getSizeCommand = DriverCommand.GetElementRect;
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                parameters.Add("id", this.elementId);
                Response commandResponse = this.Execute(getSizeCommand, parameters);
                Dictionary<string, object> rawSize = (Dictionary<string, object>)commandResponse.Value;
                int width = Convert.ToInt32(rawSize["width"], CultureInfo.InvariantCulture);
                int height = Convert.ToInt32(rawSize["height"], CultureInfo.InvariantCulture);
                return new Size(width, height);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this element is displayed.
        /// </summary>
        public bool Displayed
        {
            get
            {
                Response commandResponse = null;
                Dictionary<string, object> parameters = new Dictionary<string, object>();

                commandResponse = this.Execute(DriverCommand.ExecuteScript, parameters);

                return (bool)commandResponse.Value;
            }
        }

        public void Clear()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("id", this.elementId);
            this.Execute(DriverCommand.ClearElement, parameters);
        }

        public void Click()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("id", this.elementId);
            this.Execute(DriverCommand.ClickElement, parameters);
        }

        public IWebElement FindElement(By by)
        {
            return by.FindElement(this);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            throw new NotImplementedException();
        }

        public string GetAttribute(string attributeName)
        {
            Response commandResponse = null;
            string attributeValue = string.Empty;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            
            commandResponse = this.Execute(DriverCommand.ExecuteScript, parameters);

            if (commandResponse.Value == null)
            {
                attributeValue = null;
            }
            else
            {
                attributeValue = commandResponse.Value.ToString();

                // Normalize string values of boolean results as lowercase.
                if (commandResponse.Value is bool)
                {
                    attributeValue = attributeValue.ToLowerInvariant();
                }
            }

            return attributeValue;
        }

        public string GetCssValue(string propertyName)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("id", this.Id);
            parameters.Add("name", propertyName);

            Response commandResponse = this.Execute(DriverCommand.GetElementValueOfCssProperty, parameters);
            return commandResponse.Value.ToString();
        }

        public string GetProperty(string propertyName)
        {
            string propertyValue = string.Empty;
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("id", this.elementId);
            parameters.Add("name", propertyName);

            Response commandResponse = this.Execute(DriverCommand.GetElementProperty, parameters);
            if (commandResponse.Value == null)
            {
                propertyValue = null;
            }
            else
            {
                propertyValue = commandResponse.Value.ToString();
            }

            return propertyValue;
        }

        public void SendKeys(string text)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("id", this.elementId);
            parameters.Add("text", text);
            parameters.Add("value", text.ToCharArray());

            this.Execute(DriverCommand.SendKeysToElement, parameters);
        }

        public void Submit()
        {
            string elementType = this.GetAttribute("type");
            if (elementType != null && elementType == "submit")
            {
                this.Click();
            }
            else
            {
                IWebElement form = this.FindElement(By.XPath("./ancestor-or-self::form"));
                this.driver.ExecuteScript(
                    "var e = arguments[0].ownerDocument.createEvent('Event');" +
                    "e.initEvent('submit', true, true);" +
                    "if (arguments[0].dispatchEvent(e)) { arguments[0].submit(); }", form);
            }
        }

        /// <summary>
        /// Executes a command on this element using the specified parameters.
        /// </summary>
        /// <param name="commandToExecute">The <see cref="DriverCommand"/> to execute against this element.</param>
        /// <param name="parameters">A <see cref="Dictionary{K, V}"/> containing names and values of the parameters for the command.</param>
        /// <returns>The <see cref="Response"/> object containing the result of the command execution.</returns>
        protected virtual Response Execute(string commandToExecute, Dictionary<string, object> parameters)
        {
            return this.driver.InternalExecute(commandToExecute, parameters);
        }
    }
}
