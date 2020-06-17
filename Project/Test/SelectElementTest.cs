﻿using Codeer.Friendly.Windows;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Selenium.CefSharp.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Test
{
    public class SelectElementTestWinForm : SelectElementTestBase
    {
        CefSharpDriver _driver;

        public override IWebDriver GetDriver()
            => _driver;

        [SetUp]
        public void SetUp()
            => _driver.Url = this.GetHtmlUrl();

        [OneTimeSetUp]
        public void ClassInit()
            => _driver = AppRunner.RunWinFormApp();

        [OneTimeTearDown]
        public void OneTimeTearDown()
            => Process.GetProcessById(_driver.App.ProcessId).Kill();

        //[Ignore("clickの挙動を作り込んだうえで対応")]
        //public override void SelectByIndex()
        //{
        //    throw new NotImplementedException();
        //}

        //[Ignore("clickの挙動を作り込んだうえで対応")]
        //public override void SelectByText()
        //{
        //    throw new NotImplementedException();
        //}

        //[Ignore("clickの挙動を作り込んだうえで対応")]
        //public override void SelectByValue()
        //{
        //    throw new NotImplementedException();
        //}
    }

    public class SelectElementTestWPF : SelectElementTestBase
    {
        CefSharpDriver _driver;

        public override IWebDriver GetDriver()
            => _driver;

        [SetUp]
        public void SetUp()
            => _driver.Url = this.GetHtmlUrl();

        [OneTimeSetUp]
        public void ClassInit()
            => _driver = AppRunner.RunWpfApp();

        [OneTimeTearDown]
        public void OneTimeTearDown()
            => Process.GetProcessById(_driver.App.ProcessId).Kill();

        //[Ignore("clickの挙動を作り込んだうえで対応")]
        //public override void SelectByIndex()
        //{
        //    throw new NotImplementedException();
        //}

        //[Ignore("clickの挙動を作り込んだうえで対応")]
        //public override void SelectByText()
        //{
        //    throw new NotImplementedException();
        //}

        //[Ignore("clickの挙動を作り込んだうえで対応")]
        //public override void SelectByValue()
        //{
        //    throw new NotImplementedException();
        //}
    }

    public class SelectElementTestSelenium : SelectElementTestBase
    {
        IWebDriver _driver;

        public override IWebDriver GetDriver() => _driver;

        [SetUp]
        public void initialize()
        {
            _driver.Url = this.GetHtmlUrl();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [OneTimeSetUp]
        public void ClassInit()
        {
            _driver = new ChromeDriver();
        }
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _driver.Dispose();
        }
    }

    public abstract class SelectElementTestBase : CompareTestBase
    {
        public void InitializeSelect()
        {
            GetExecutor().ExecuteScript("document.getElementById('singleSelect').selectedIndex = -1");
            GetExecutor().ExecuteScript("document.getElementById('multipleSelect').selectedIndex = -1");
        }

        [Test]
        public void Options()
        {
            InitializeSelect();
            
            var select = GetDriver().FindElement(By.Id("singleSelect"));
            var elem = new SelectElement(select);
            var texts = elem.Options.Select(o => o.Text).ToList(); ;
            texts.Count.Is(6);
            texts[0].Is("ABCDE");
            texts[1].Is("FGHIJ");
            texts[2].Is("KLMNO");
            texts[3].Is("PQRST");
            texts[4].Is("UVWXY");
            texts[5].Is("Z");
        }

        [Test]
        public void IsMultiple_ShouldReturnFalseWhenSingleSelect()
        {
            InitializeSelect();
            
            var select = GetDriver().FindElement(By.Id("singleSelect"));
            var elem = new SelectElement(select);
            elem.IsMultiple.IsFalse();
        }

        [Test]
        public void IsMultiple_ShouldReturnTrueWhenMultipleSelect()
        {
            InitializeSelect();
            
            var select = GetDriver().FindElement(By.Id("multipleSelect"));
            var elem = new SelectElement(select);
            elem.IsMultiple.IsTrue();
        }

        [Test]
        public void SelectedOption_ShouldReturnSelectedOption()
        {
            InitializeSelect();
            
            var select = GetDriver().FindElement(By.Id("singleSelect"));
            var elem = new SelectElement(select);
            GetExecutor().ExecuteScript("document.getElementById('singleSelect').selectedIndex = 1");
            var option = elem.SelectedOption;

            option.Text.Is("FGHIJ");
        }

        [Test]
        public void SelectedOption_ShouldThrowExeceptionWhenHasNoSelected()
        {
            InitializeSelect();
            
            var select = GetDriver().FindElement(By.Id("singleSelect"));
            var elem = new SelectElement(select);
            AssertCompatible.ThrowsException<NoSuchElementException>(() => elem.SelectedOption);
        }

        [Test]
        public void AllSelectedOptions_ShouldReturnEmptyWhenHasNoSelected()
        {
            InitializeSelect();

            var select = GetDriver().FindElement(By.Id("singleSelect"));
            var elem = new SelectElement(select);
            var selects = elem.AllSelectedOptions;
            selects.Count.Is(0);
        }

        [Test]
        public void AllSelectedOptions_ShouldReturnSelectedOption_single()
        {
            InitializeSelect();

            var select = GetDriver().FindElement(By.Id("singleSelect"));
            var elem = new SelectElement(select);
            GetExecutor().ExecuteScript("document.getElementById('singleSelect').selectedIndex = 1");
            var selects = elem.AllSelectedOptions;

            selects.Count.Is(1);
            selects[0].Text.Is("FGHIJ");
        }

        [Test]
        public void AllSelectedOptions_ShouldReturnSelectedOption_multiple()
        {
            InitializeSelect();

            var select = GetDriver().FindElement(By.Id("multipleSelect"));
            var elem = new SelectElement(select);
            GetExecutor().ExecuteScript("document.getElementById('multipleSelect').options[1].selected = true");
            GetExecutor().ExecuteScript("document.getElementById('multipleSelect').options[3].selected = true");
            GetExecutor().ExecuteScript("document.getElementById('multipleSelect').options[5].selected = true");
            var selects = elem.AllSelectedOptions;

            selects.Count.Is(3);
            selects[0].Text.Is("FGHIJ");
            selects[1].Text.Is("PQRST");
            selects[2].Text.Is("Z");
        }

        [Test]
        public void WrappedElement()
        {
            InitializeSelect();

            var select = GetDriver().FindElement(By.Id("multipleSelect"));
            var elem = new SelectElement(select);

            elem.WrappedElement.IsSameReferenceAs(select);
        }

        [Test]
        public virtual void SelectByText()
        {
            InitializeSelect();

            var select = GetDriver().FindElement(By.Id("singleSelect"));
            var elem = new SelectElement(select);
            elem.SelectByText("ABCDE");
            var selectedValue = GetExecutor().ExecuteScript("return document.getElementById('singleSelect').value");
            selectedValue.Is("1");
        }

        [Test]
        public virtual void SelectByIndex()
        {
            InitializeSelect();

            var select = GetDriver().FindElement(By.Id("singleSelect"));
            var elem = new SelectElement(select);
            elem.SelectByIndex(4);
            var selectedValue = GetExecutor().ExecuteScript("return document.getElementById('singleSelect').value");
            selectedValue.Is("5");
        }

        [Test]
        public virtual void SelectByValue()
        {
            InitializeSelect();

            var select = GetDriver().FindElement(By.Id("singleSelect"));
            var elem = new SelectElement(select);
            elem.SelectByValue("2");
            var selectedIndex = GetExecutor().ExecuteScript("return document.getElementById('singleSelect').selectedIndex");
            selectedIndex.Is(1L);
        }


        [Test]
        public virtual void SelectEvent()
        {
            InitializeSelect();

            var select = GetDriver().FindElement(By.Id("dropDownFruit"));
            var elem = new SelectElement(select);
            elem.SelectByText("Banana");
            GetDriver().FindElement(By.Id("textBoxName")).GetAttribute("value").Is("select");
        }
    }
}
