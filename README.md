Selenium.CefSharp.Driver_β
======================

<br>
The status of this library is beta.
<br>

## Features ...
It is a library for operating ChromiumWebBrowser with the interface of Selenium.
Supports WinForms & WPF.

## Getting Started
Install Selenium.CefSharp.Driver from NuGet

    PM> Install-Package Selenium.CefSharp.Driver
https://www.nuget.org/packages/Selenium.CefSharp.Driver/

# What problem can this solve?
Chrome driver is able to manipulate apps that are configured only with CefSharp, but it can't manipulate apps that used CefSharp for only part of it.
This problem will be solved by using CefSharpDriver.

# Another process manuplation.
This library is a layer on top of Friendly, so you must learn that first. But it is very easy to learn.<br>
https://github.com/Codeer-Software/Friendly.Windows

# Sample
https://github.com/Codeer-Software/Selenium.CefSharp.Driver/tree/master/Project/Sample<br>
Sample App.<br>
 ![Sample App.](Img/SampleApp.png)

Code.<br>
```cs  
using System.Diagnostics;
using System.IO;
using Codeer.Friendly;
using Codeer.Friendly.Dynamic;
using Codeer.Friendly.Windows;
using Codeer.Friendly.Windows.Grasp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RM.Friendly.WPFStandardControls;
using Selenium.CefSharp.Driver;

namespace SampleTest
{
    [TestClass]
    public class UnitTest
    {
        WindowsAppFriend _app;
        CefSharpDriver _driver;

        [TestInitialize]
        public void TestInitialize()
        {
            //start process.
            var process = Process.Start(ProcessPath);

            //attach by friendly.
            _app = new WindowsAppFriend(process);

            //show next dialog.
            var mainWindow = _app.WaitForIdentifyFromTypeFullName("SampleApp.MainWindow");
            var button = new WPFButtonBase(mainWindow.Dynamic()._buttonNextDialog);
            button.EmulateClick(new Async());

            //get next dialog.
            var nextDialog = _app.WaitForIdentifyFromTypeFullName("SampleApp.NextDialog");

            //create driver.
            _driver = new CefSharpDriver(nextDialog.Dynamic()._browser);
        }

        [TestCleanup]
        public void TestCleanup()
            => Process.GetProcessById(_app.ProcessId).Kill();

        [TestMethod]
        public void TestMethod()
        {
            //set url.
            _driver.Url = HtmlPath;
         
            //find element by id.
            var button = _driver.FindElement(By.Id("testButtonClick"));

            //click.
            button.Click();

            //find element by name.
            var textBox = _driver.FindElement(By.Name("nameInput"));

            //sendkeys.
            textBox.SendKeys("abc");

            //find element by tag.
            var select = _driver.FindElement(By.TagName("select"));

            //selenium support.
            new SelectElement(select).SelectByText("Orange");

            //find element by xpath.
            var buttonAlt = _driver.FindElement(By.XPath("//*[@id=\"form\"]/table/tbody/tr[7]/td/input[2]"));

            //actions.
            new Actions(_driver).KeyDown(Keys.Alt).Click(buttonAlt).Build().Perform();

            //execute javascript.
            var defaultValue = (string)_driver.ExecuteScript("return arguments[0].defaultValue;", textBox);
        }

        static string TestDir
        {
            get
            {
                var dir = typeof(UnitTest).Assembly.Location;
                for (int i = 0; i < 4; i++) dir = Path.GetDirectoryName(dir);
                return dir;
            }
        }

        static string ProcessPath => Path.Combine(TestDir, @"SampleApp\bin\x86\Debug\SampleApp.exe");

        static string HtmlPath => Path.Combine(TestDir, "Controls.html");
    }
}
```



