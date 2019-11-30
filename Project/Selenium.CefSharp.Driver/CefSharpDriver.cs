using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Codeer.Friendly;
using OpenQA.Selenium;
using OpenQA.Selenium.Html5;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;

namespace Selenium.CefSharp.Driver
{
    public class CefSharpDriver : IWebDriver,
                                    ISearchContext,
                                    IDisposable,
                                    IJavaScriptExecutor,
                                    IFindsById,
                                    IFindsByClassName,
                                    IFindsByLinkText,
                                    IFindsByName,
                                    IFindsByTagName,
                                    IFindsByXPath,
                                    IFindsByPartialLinkText,
                                    IFindsByCssSelector,
                                    ITakesScreenshot,
                                    IHasCapabilities,
                                    IHasWebStorage,
                                    IHasLocationContext,
                                    IHasApplicationCache,
                                    IAllowsFileDetection,
                                    IHasSessionId,
                                    IActionExecutor,
                                    IAppVarOwner
    {
        public AppVar AppVar { get; }

        public string Url { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        public string Title => throw new NotImplementedException();

        public string PageSource => throw new NotImplementedException();

        public string CurrentWindowHandle => throw new NotImplementedException();

        public ReadOnlyCollection<string> WindowHandles => throw new NotImplementedException();

        public ICapabilities Capabilities {get; }

        public bool HasWebStorage => this.WebStorage != null;

        public IWebStorage WebStorage {get; }

        public bool HasLocationContext => this.LocationContext != null;

        public ILocationContext LocationContext { get; }

        public bool HasApplicationCache => this.ApplicationCache != null;

        public IApplicationCache ApplicationCache { get; }

        public IFileDetector FileDetector { get ; set; }

        public SessionId SessionId { get; }

        public bool IsActionExecutor { get { return true; } }

        public ICommandExecutor CommandExecutor { get; }


        public CefSharpDriver(AppVar appVar)
        {
            AppVar = appVar;
        }

        public void Close()
        {
            AppVar.Dispose();
        }

        public void Quit()
        {
            AppVar.Dispose();
        }

        public IOptions Manage()
        {
            return new CefSharpOption(this);
        }

        public INavigation Navigate()
        {
            return null;
        }

        public ITargetLocator SwitchTo()
        {
            return null;
        }

        public IWebElement FindElement(By by)
        {
            return by.FindElement(this);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return by.FindElements(this);
        }

        public void Dispose()
        {
            AppVar.Dispose();
        }

        public object ExecuteScript(string script, params object[] args)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("script", script);

            parameters.Add("args", args);

            Response commandResponse = this.Execute(script, parameters);
            return commandResponse.Value;
        }

        public object ExecuteAsyncScript(string script, params object[] args)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("script", script);

            parameters.Add("args", args);

            Response commandResponse = this.Execute(script, parameters);
            return commandResponse.Value;
        }

        public IWebElement FindElementById(string id)
        {
            return this.FindElement(By.Id(id));
        }

        public ReadOnlyCollection<IWebElement> FindElementsById(string id)
        {
            return this.FindElements(By.Id(id));
        }

        public IWebElement FindElementByClassName(string className)
        {
            return this.FindElement(By.ClassName(className));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
        {
            return this.FindElements(By.ClassName(className));
        }

        public IWebElement FindElementByLinkText(string linkText)
        {
            return this.FindElement(By.LinkText(linkText));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText)
        {
            return this.FindElements(By.LinkText(linkText));
        }

        public IWebElement FindElementByName(string name)
        {
            return this.FindElement(By.Name(name));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByName(string name)
        {
            return this.FindElements(By.Name(name));
        }

        public IWebElement FindElementByTagName(string tagName)
        {
            return this.FindElement(By.TagName(tagName));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
        {
            return this.FindElements(By.TagName(tagName));
        }

        public IWebElement FindElementByXPath(string xpath)
        {
            return this.FindElement(By.XPath(xpath));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
        {
            return this.FindElements(By.XPath(xpath));
        }

        public IWebElement FindElementByPartialLinkText(string partialLinkText)
        {
            return this.FindElement(By.PartialLinkText(partialLinkText));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText)
        {
            return this.FindElements(By.PartialLinkText(partialLinkText));
        }

        public IWebElement FindElementByCssSelector(string cssSelector)
        {
            return this.FindElement(By.CssSelector(cssSelector));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector)
        {
            return this.FindElements(By.CssSelector(cssSelector));
        }

        public Screenshot GetScreenshot()
        {
            Response screenshotResponse = this.Execute(DriverCommand.Screenshot, null);
            string base64 = screenshotResponse.Value.ToString();

            return new Screenshot(base64);
        }

        public void PerformActions(IList<ActionSequence> actionSequenceList)
        {
            List<object> objectList = new List<object>();
            foreach (ActionSequence sequence in actionSequenceList)
            {
                objectList.Add(sequence.ToDictionary());
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters["actions"] = objectList;
            this.Execute(DriverCommand.Actions, parameters);
        }

        public void ResetInputState()
        {
            this.Execute(DriverCommand.CancelActions, null);
        }

        internal Response InternalExecute(string driverCommandToExecute, Dictionary<string, object> parameters)
        {
            return this.Execute(driverCommandToExecute, parameters);
        }

        protected virtual Response Execute(string driverCommandToExecute, Dictionary<string, object> parameters)
        {
            Command commandToExecute = new Command(this.SessionId, driverCommandToExecute, parameters);

            Response commandResponse = new Response();

            try
            {
                commandResponse = this.CommandExecutor.Execute(commandToExecute);
            }
            catch (System.Net.WebException e)
            {
                commandResponse.Status = WebDriverResult.UnhandledError;
                commandResponse.Value = e;
            }

            if (commandResponse.Status != WebDriverResult.Success)
            {
                Dictionary<string, object> errorAsDictionary = commandResponse.Value as Dictionary<string, object>;

                ErrorResponse errorResponseObject = new ErrorResponse(errorAsDictionary);
                string errorMessage = errorResponseObject.Message;

                throw new NotImplementedException(errorMessage);
            }

            return commandResponse;
        }

        protected IWebElement FindElement(string mechanism, string value)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("using", mechanism);
            parameters.Add("value", value);
            Response commandResponse = this.Execute(DriverCommand.FindElement, parameters);
            return this.GetElementFromResponse(commandResponse);
        }

        internal IWebElement GetElementFromResponse(Response response)
        {
            if (response == null)
            {
                throw new NoSuchElementException();
            }

            IWebElement element = null;
            Dictionary<string, object> elementDictionary = response.Value as Dictionary<string, object>;
            if (elementDictionary != null)
            {
                element = new CefSharpWebElement(elementDictionary);
            }

            return element;
        }
    }
}
