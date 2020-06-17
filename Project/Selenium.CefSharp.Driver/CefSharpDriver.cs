using Codeer.Friendly;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using OpenQA.Selenium;
using System;
using System.Drawing;
using Codeer.TestAssistant.GeneratorToolKit;
using OpenQA.Selenium.Html5;
using OpenQA.Selenium.Internal;
using System.Collections.ObjectModel;
using Codeer.Friendly.Dynamic;
using System.Linq;
using Selenium.CefSharp.Driver.InTarget;
using OpenQA.Selenium.Interactions.Internal;
using System.Collections.Generic;
using OpenQA.Selenium.Interactions;
using System.Diagnostics;
using Selenium.CefSharp.Driver.Inside;

namespace Selenium.CefSharp.Driver
{
    /// <summary>
    /// Provides a mechanism to write tests against ChromiumWebBrowser
    /// </summary>
    /// <example>
    /// <code>
    /// [TestFixture]
    /// public class Testing
    /// {
    ///     private IWebDriver driver;
    ///     <para></para>
    ///     [SetUp]
    ///     public void SetUp()
    ///     {
    ///         //get process.
    ///         var process = Process.GetProcessByName("Target")[0];
    ///         
    ///         //using Friendly.
    ///         //https://github.com/Codeer-Software/Friendly
    ///         var app = new WindowsAppFriend(process);
    ///         var window = app.&lt;Application&gt;.().Current.MainWindow;
    ///         
    ///         //create driver.
    ///         driver = new CefSharpDriver(window._browser);
    ///     }
    ///     <para></para>
    ///     [Test]
    ///     public void TestGoogle()
    ///     {
    ///         driver.Navigate().GoToUrl("https://github.com/Codeer-Software/Selenium.CefSharp.Driver");
    ///         /*
    ///         *   Rest of the test
    ///         */
    ///     }
    ///     <para></para>
    ///     [TearDown]
    ///     public void TearDown()
    ///     {
    ///         app.Dispose();
    ///     }
    /// }
    /// </code>
    /// </example>
    [ControlDriver(TypeFullName = "CefSharp.Wpf.ChromiumWebBrowser|CefSharp.WinForms.ChromiumWebBrowser")]
    public class CefSharpDriver :
        IWebDriver,
        IJavaScriptExecutor,
        IFindsById,
        IFindsByClassName,
        IFindsByLinkText,
        IFindsByName,
        IFindsByTagName,
        IFindsByXPath,
        IFindsByPartialLinkText,
        IFindsByCssSelector,
        IHasApplicationCache,
        IHasWebStorage,
        ITakesScreenshot,
#pragma warning disable CS0618
        IHasInputDevices,
#pragma warning restore CS0618
        IAllowsFileDetection,
        IActionExecutor,
        IAppVarOwner,
        IUIObject
    {
        ChromiumWebBrowserDriver _chromiumWebBrowser;
        dynamic _windowManager;

        /// <summary>
        /// Returns the associated application manipulation object.
        /// </summary>
        public WindowsAppFriend App { get; }

        /// <summary>
        /// Returns a variable that manipulates a ChromiumWebBrowser object.
        /// </summary>
        public AppVar AppVar { get; }

        /// <summary>
        /// Returns the size of IUIObject.
        /// </summary>
        public Size Size => CurrentBrowser.Size;

        /// <summary>
        /// Gets an OpenQA.Selenium.IKeyboard object for sending keystrokes to the browser.
        /// </summary>
#pragma warning disable CS0618
        public IKeyboard Keyboard => new ObsoleteKeyboard();
#pragma warning restore CS0618

        /// <summary>
        /// Gets an OpenQA.Selenium.IMouse object for sending mouse commands to the browser.
        /// </summary>
#pragma warning disable CS0618
        public IMouse Mouse => new ObsoleteMouse();
#pragma warning restore CS0618

        /// <summary>
        /// Gets a value indicating whether this object is a valid action executor.
        /// </summary>
        public bool IsActionExecutor => true;

        /// <summary>
        /// Gets or sets the OpenQA.Selenium.IFileDetector responsible for detecting sequences
        /// of keystrokes representing file paths and names.
        /// </summary>
        public IFileDetector FileDetector { get; set; } = new DefaultFileDetector();

        /// <summary>
        /// Gets or sets the URL the browser is currently displaying.
        /// </summary>
        public string Url
        {
            get => CurrentBrowser.MainFrame.Url;
            set
            {
                CurrentBrowser.MainFrame.Url = value;
                CurrentBrowser.CurrentFrame = CurrentBrowser.MainFrame;
            }
        }

        /// <summary>
        /// Gets the title of the current browser window.
        /// </summary>
        public string Title => CurrentBrowser.CurrentFrame.Title;

        /// <summary>
        /// Gets a value indicating whether web storage is supported for this driver.
        /// </summary>
        public bool HasWebStorage => true;

        /// <summary>
        /// Gets an <see cref="IWebStorage"/> object for managing web storage.
        /// </summary>
        public IWebStorage WebStorage => new WebStorage(this);

        /// <summary>
        /// Gets a value indicating whether manipulating the application cache is supported for this driver.
        /// </summary>
        public bool HasApplicationCache => true;

        /// <summary>
        /// Gets an <see cref="IApplicationCache"/> object for managing application cache.
        /// </summary>
        public IApplicationCache ApplicationCache => new ApplicationCache(this);

        /// <summary>
        /// Gets the source of the page last loaded by the browser.
        /// </summary>
        public string PageSource => (string)ExecuteScript("return document.documentElement.outerHTML;");

        /// <summary>
        /// Gets the current window handle, which is an opaque handle to this window that
        /// uniquely identifies it within this driver instance.
        /// </summary>
        public string CurrentWindowHandle => CurrentBrowser.WindowHandle.ToString();

        /// <summary>
        /// Gets the window handles of open browser windows.
        /// </summary>
        public ReadOnlyCollection<string> WindowHandles
        {
            get
            {
                List<IntPtr> list = new List<IntPtr>();
                list.Add(IntPtr.Zero);
                List<IntPtr> otherWindows = _windowManager.GetWindowHandles();
                list.AddRange(otherWindows);
                return new ReadOnlyCollection<string>(list.Select(e => e.ToInt64().ToString()).ToArray());
            }
        }

        /// <summary>
        /// Gets a value indicating whether manipulating geolocation is supported for this driver.
        /// </summary>
        public bool HasLocationContext => true;

        internal ICefSharpBrowser CurrentBrowser { get; set; }

        internal AppVar JavascriptObjectRepository => _chromiumWebBrowser.JavascriptObjectRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="appVar">AppVar of ChromiumWebBrowser.</param>
        public CefSharpDriver(AppVar appVar)
        {
            AppVar = appVar;
            App = (WindowsAppFriend)appVar.App;
            _chromiumWebBrowser = new ChromiumWebBrowserDriver(this);
            CurrentBrowser = _chromiumWebBrowser;

            AppVar mgr = appVar.App.Type<CefSharpWindowManagerFactory>().InstallCefSharpWindowManager(this);
            if (!mgr.IsNull) _windowManager = mgr.Dynamic();

            CurrentBrowser.WaitForLoading();
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
            => AppVar.Dispose();

        /// <summary>
        /// Close the current window, quitting the browser if it is the last window currently
        /// open.
        /// </summary>
        public void Close()
            => CurrentBrowser.Close();

        /// <summary>
        /// Quits this driver, closing every associated window.
        /// </summary>
        public void Quit() => Process.GetProcessById(App.ProcessId).Kill();

        /// <summary>
        /// Instructs the driver to send future commands to a different frame or window.
        /// </summary>
        /// <returns>An OpenQA.Selenium.ITargetLocator object which can be used to select a frame or window.</returns>
        public ITargetLocator SwitchTo()
            => new TargetLocator(this);

        /// <summary>
        /// Instructs the driver to navigate the browser to another location.
        /// </summary>
        /// <returns>An OpenQA.Selenium.INavigation object allowing the user to access the browser's history and to navigate to a given URL.</returns>
        public INavigation Navigate()
            => new Navigation(this);

        /// <summary>
        /// Convert IUIObject's client coordinates to screen coordinates.
        /// </summary>
        /// <param name="clientPoint">client coordinates.</param>
        /// <returns>screen coordinates.</returns>
        public Point PointToScreen(Point clientPoint)
            => CurrentBrowser.PointToScreen(clientPoint);

        /// <summary>
        /// Make it active.
        /// </summary>
        public void Activate()
            => CurrentBrowser.Activate();

        /// <summary>
        /// Gets a <see cref="Screenshot"/> object representing the image of the page on the screen.
        /// </summary>
        /// <returns>A <see cref="Screenshot"/> object containing the image.</returns>
        public Screenshot GetScreenshot()
            => CurrentBrowser.CurrentFrame.GetScreenshot();

        /// <summary>
        /// Performs the specified list of actions with this action executor.
        /// </summary>
        /// <param name="actionSequenceList">The list of action sequences to perform.</param>
        public void PerformActions(IList<ActionSequence> actionSequenceList)
            => ActionsAnalyzer.PerformActions(this, actionSequenceList);

        /// <summary>
        /// Resets the input state of the action executor.
        /// </summary>
        public void ResetInputState() { }

        /// <summary>
        /// Finds the first <see cref="IWebElement"/> using the given method.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>The first matching <see cref="IWebElement"/> on the current context.</returns>
        /// <exception cref="NoSuchElementException">If no element matches the criteria.</exception>
        public IWebElement FindElement(By by)
            => ElementFinder.FindElementFromDocument(this, by);

        /// <summary>
        /// Finds all <see cref="IWebElement">IWebElements</see> within the current context
        /// using the given mechanism.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> of all <see cref="IWebElement">WebElements</see>
        /// matching the current criteria, or an empty list if nothing matches.</returns>
        public ReadOnlyCollection<IWebElement> FindElements(By by)
            => ElementFinder.FindElementsFromDocument(this, by);

        /// <summary>
        /// Executes JavaScript in the context of the currently selected frame or window.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="args">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        /// <remarks>
        /// <para>
        /// The <see cref="ExecuteScript"/>method executes JavaScript in the context of
        /// the currently selected frame or window. This means that "document" will refer
        /// to the current document. If the script has a return value, then the following
        /// steps will be taken:
        /// </para>
        /// <para>
        /// <list type="bullet">
        /// <item><description>For an HTML element, this method returns a <see cref="IWebElement"/></description></item>
        /// <item><description>For a number, a <see cref="long"/> is returned</description></item>
        /// <item><description>For a boolean, a <see cref="bool"/> is returned</description></item>
        /// <item><description>For all other cases a <see cref="string"/> is returned.</description></item>
        /// <item><description>For an array,we check the first element, and attempt to return a
        /// <see cref="List{T}"/> of that type, following the rules above. Nested lists are not
        /// supported.</description></item>
        /// <item><description>If the value is null or there is no return value,
        /// <see langword="null"/> is returned.</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// Arguments must be a number (which will be converted to a <see cref="long"/>),
        /// a <see cref="bool"/>, a <see cref="string"/> or a <see cref="IWebElement"/>.
        /// An exception will be thrown if the arguments do not meet these criteria.
        /// The arguments will be made available to the JavaScript via the "arguments" magic
        /// variable, as if the function were called via "Function.apply"
        /// </para>
        /// </remarks>
        public object ExecuteScript(string script, params object[] args)
            => CurrentBrowser.CurrentFrame.ExecuteScript(script, args);

        /// <summary>
        /// Executes JavaScript asynchronously in the context of the currently selected frame or window.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="args">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        public object ExecuteAsyncScript(string script, params object[] args)
            => CurrentBrowser.CurrentFrame.ExecuteAsyncScript(script, args);

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

        class Navigation : INavigation
        {
            CefSharpDriver _this;

            public Navigation(CefSharpDriver driver)
                => _this = driver;

            public void Back()
            {
                _this.CurrentBrowser.MainFrame.ExecuteScript("window.history.back();");
                _this.CurrentBrowser.CurrentFrame = _this.CurrentBrowser.MainFrame;
                _this.CurrentBrowser.WaitForLoading();
            }

            public void Forward()
            {
                _this.CurrentBrowser.MainFrame.ExecuteScript("window.history.forward();");
                _this.CurrentBrowser.CurrentFrame = _this.CurrentBrowser.MainFrame;
                _this.CurrentBrowser.WaitForLoading();
            }

            public void GoToUrl(string url)
                => _this.Url = url;

            public void GoToUrl(Uri url)
                => _this.Url = url.ToString();

            public void Refresh()
            {
                _this.CurrentBrowser.MainFrame.ExecuteScript("window.location.reload();");
                _this.CurrentBrowser.CurrentFrame = _this.CurrentBrowser.MainFrame;
                _this.CurrentBrowser.WaitForLoading();
            }
        }

        class TargetLocator : ITargetLocator
        {
            CefSharpDriver _this;

            public TargetLocator(CefSharpDriver driver)
                => _this = driver;

            public IWebDriver DefaultContent()
            {
                _this.CurrentBrowser = _this._chromiumWebBrowser;
                _this.CurrentBrowser.CurrentFrame = _this.CurrentBrowser.MainFrame;
                return _this;
            }

            public IWebElement ActiveElement()
                => _this.ExecuteScript("return document.activeElement;") as IWebElement;

            public IAlert Alert()
                => new Alert(_this.App, _this.Url);

            public IWebDriver Frame(int frameIndex)
            {
                var frameElements = _this.FindElementsByTagName("iframe");
                var frameNames = frameElements.Select(e => e.GetAttribute("name")).ToList();
                var frameElement = frameElements[frameIndex];
                var frame = _this.App.Type<FrameFinder>().FindFrame(_this.CurrentBrowser.BrowserCore, _this.CurrentBrowser.CurrentFrame, frameNames, frameIndex);
                if (((AppVar)frame).IsNull)
                {
                    throw new NotFoundException("Frame was not found.");
                }
                _this.CurrentBrowser.CurrentFrame = new CefSharpFrameDriver(_this, _this.CurrentBrowser.CurrentFrame,
                    () => (AppVar)frame,
                    _this.CurrentBrowser.CurrentFrame.FrameElements.Concat(new[] { frameElement }).ToArray());
                return _this;
            }

            public IWebDriver Frame(string frameName)
            {
                var frameElements = _this.FindElementsByTagName("iframe");
                for (int i = 0; i < frameElements.Count; i++)
                {
                    var e = frameElements[i];
                    if (e.GetAttribute("id") == frameName || e.GetAttribute("name") == frameName)
                    {
                        return Frame(i);
                    }
                }
                throw new NotFoundException("Frame was not found.");
            }

            public IWebDriver Frame(IWebElement frameElementSrc)
            {
                var frameElement = (CefSharpWebElement)frameElementSrc;
                var frameElements = _this.FindElementsByTagName("iframe").Cast<CefSharpWebElement>().ToList();
                for (int i = 0; i < frameElements.Count; i++)
                {
                    var e = frameElements[i];
                    if (e.Id == frameElement.Id)
                    {
                        return Frame(i);
                    }
                }
                throw new NotFoundException("Frame was not found.");
            }

            public IWebDriver ParentFrame()
            {
                if (_this.CurrentBrowser.CurrentFrame.ParentFrame == null) throw new NotFoundException("Frame was not found.");
                _this.CurrentBrowser.CurrentFrame = _this.CurrentBrowser.CurrentFrame.ParentFrame;
                return _this;
            }

            public IWebDriver Window(string windowName)
            {
                if (windowName == IntPtr.Zero.ToString()) return DefaultContent();

                if (!long.TryParse(windowName, out var value)) throw new NotSupportedException("Invalid name.");

                var handle = new IntPtr(value);
                _this.CurrentBrowser = new CefSharpWindowBrowser(_this, handle, _this._windowManager.GetBrowser(handle));

                return _this;
            }
        }

#pragma warning disable CS0618
        class ObsoleteKeyboard : IKeyboard
#pragma warning restore CS0618
        {
            public void PressKey(string keyToPress) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void ReleaseKey(string keyToRelease) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void SendKeys(string keySequence) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
        }

#pragma warning disable CS0618
        class ObsoleteMouse : IMouse
#pragma warning restore CS0618
        {
            public void Click(ICoordinates where) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void ContextClick(ICoordinates where) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void DoubleClick(ICoordinates where) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void MouseDown(ICoordinates where) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void MouseMove(ICoordinates where) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void MouseMove(ICoordinates where, int offsetX, int offsetY) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
            public void MouseUp(ICoordinates where) => throw new NotSupportedException("Obsolete! Use the Actions or ActionBuilder class to simulate keyboard input.");
        }

        /// <summary>
        /// don't support.
        /// </summary>
        /// <returns>nothing.</returns>
        public IOptions Manage() => throw new NotSupportedException();
    }
}
