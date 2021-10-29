using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.IO;
using System.Linq;

namespace Test
{
    public abstract class WebElementTest: CompareTestBase
    {
        public class Forms : WebElementTest
        {
            public Forms() : base(new FormsAgent()) { }
        }
        public class Wpf : WebElementTest
        {
            public Wpf() : base(new WpfAgent()) { }
        }
        public class Web : WebElementTest
        {
            public Web() : base(new WebAgent()) { }
        }
        protected WebElementTest(INeed need) : base(need) { }

        [SetUp]
        public void SetUp()
            => GetDriver().Url = HtmlServer.Instance.RootUrl + "Controls.html";

        [Test]
        public void ScreenShot()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            var screenShot = (ITakesScreenshot)element;
            var path = Path.GetTempFileName();
            screenShot.GetScreenshot().SaveAsFile(path, ScreenshotImageFormat.Png);
            File.Delete(path);
        }

        [Test]
        public void Locatable()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            var locatable = (ILocatable)element;
            var locationOnScreenOnceScrolledIntoView = locatable.LocationOnScreenOnceScrolledIntoView;
            var coordinates = locatable.Coordinates;

            var id = coordinates.AuxiliaryLocator;
            var locationInDom = coordinates.LocationInDom;
            var locationInViewport = coordinates.LocationInViewport;
            AssertCompatible.ThrowsException<Exception>(()=> coordinates.LocationOnScreen);
        }

        [Test]
        public void TagName()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            element.TagName.Is("input");
        }

        [Test]
        public void Text()
        {
            var element = GetDriver().FindElement(By.Id("labelTitle"));
            element.Text.Is("Title Controls");
        }

        [Test]
        public void Enabled()
        {
            GetDriver().FindElement(By.Id("textBoxName")).Enabled.IsTrue();
            GetDriver().FindElement(By.Id("disabletest")).Enabled.IsFalse();
        }

        [Test]
        public void Selected()
        {
            var checkBox = GetDriver().FindElement(By.Id("checkBoxCellPhone"));
            checkBox.Selected.IsFalse();
            checkBox.Click();
            checkBox.Selected.IsTrue();

            GetDriver().FindElement(By.Id("opt0")).Selected.IsTrue();
            GetDriver().FindElement(By.Id("opt1")).Selected.IsFalse();

            GetDriver().FindElement(By.Id("radioMan")).Selected.IsTrue();
            GetDriver().FindElement(By.Id("radioWoman")).Selected.IsFalse();
        }

        [Test]
        public void Location()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            var x = element.Location;

            //Different from browser
        }

        [Test]
        public void Size()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            var size = element.Size;

            //Different from browser
        }

        [Test]
        public void Displayed()
        {
            GetDriver().FindElement(By.Id("disabletest")).Displayed.IsTrue();
            GetDriver().FindElement(By.Id("not_displayed")).Displayed.IsFalse();
        }

        [Test]
        public void Clear()
        {
            var element = GetDriver().FindElement(By.Id("textBoxName"));
            element.GetAttribute("value").Is("ABCDE");
            element.Clear();
            element.GetAttribute("value").Is("");
        }

        [Test]
        public void GetCssValue()
        {
            var element = GetDriver().FindElement(By.Id("not_displayed"));
            element.GetCssValue("display").Is("none");
        }

        [Test]
        public void Submit()
        {
            var element = GetDriver().FindElement(By.Id("form"));
            element.Submit();
        }

        void InitAttr()
        {
            GetExecutor().ExecuteScript(@"delete document.getElementById('attrTestInput').bar;
delete document.getElementById('attrTestInput').foo;");
        }

        [Test]
        public void GetAttribute_ShouldReturnAttributeValue()
        {
            InitAttr();
            var elem = GetDriver().FindElement(By.Id("attrTestInput"));
            var value = elem.GetAttribute("foo");
            value.Is("fooattr");
        }

        [Test]
        public void GetAttribute_ShouldReturnPropertyValueIfHasNotAttribute()
        {
            InitAttr();
            GetExecutor().ExecuteScript("document.getElementById('attrTestInput').bar = 'barprop';");
            var elem = GetDriver().FindElement(By.Id("attrTestInput"));
            var value = elem.GetAttribute("bar");
            value.Is("barprop");
        }

        [Test]
        public void GetAttribute_PorpValueShouldBeReturndByOverwritingAttrValue()
        {
            InitAttr();
            GetExecutor().ExecuteScript("document.getElementById('attrTestInput').foo = 'foodynamic';");
            var attrValue = GetExecutor().ExecuteScript("return document.getElementById('attrTestInput').getAttribute('foo');");
            var elem = GetDriver().FindElement(By.Id("attrTestInput"));
            var value = elem.GetAttribute("foo");

            // selenium treats attribute and property as same.
            // The value of property is returned with priority.
            attrValue.Is("fooattr");
            value.Is("foodynamic");
        }

        [Test]
        public void GetAttribute_IfPropetyIsNull_ReturnAttributeValue()
        {
            InitAttr();
            GetExecutor().ExecuteScript("document.getElementById('attrTestInput').foo = null;");
            var elem = GetDriver().FindElement(By.Id("attrTestInput"));
            var value = elem.GetAttribute("foo");
            var value2 = elem.GetDomAttribute("foo");
            // selenium treats attribute and property as same.
            // If the value of property is null, the value of attribute is returned.
            value.Is("fooattr");
            value2.Is("fooattr");
        }

        [Test]
        public void GetProperty_ShouldNotReturnAttributeValue()
        {
            InitAttr();
            var elem = GetDriver().FindElement(By.Id("attrTestInput"));
            var value = elem.GetProperty("foo");
            var value2 = elem.GetDomProperty("foo");
            value.IsNull();
            value2.IsNull();
        }

        [Test]
        public void GetProperty_ShouldReturnPropertyValue()
        {
            InitAttr();
            GetExecutor().ExecuteScript("document.getElementById('attrTestInput').foo = 'foodynamic';");
            var elem = GetDriver().FindElement(By.Id("attrTestInput"));
            var value = elem.GetProperty("foo");
            var value2 = elem.GetDomProperty("foo");
            value.Is("foodynamic");
            value2.Is("foodynamic");
        }

        [Test]
        public void GetShadowRoot()
        {
            var elem = GetDriver().FindElement(By.Id("labelShadowRootTest"));
            try
            {
                elem.GetShadowRoot();
            }
            catch (System.NotImplementedException e)
            {
                // ChromeDriver 95.0.4638.1700 時点で未実装
                Console.WriteLine(e.Message);
                return;
            }
            var height = GetExecutor().ExecuteScript("return arguments[0].getBoundingClientRect().height;", elem);
            // 要素が表示されている事の確認(表示中は高さが設定されている)
            height.IsNot(0);
            var shadowRoot = elem.GetShadowRoot();
            // ShadowRoot未指定なのでNULL
            shadowRoot.IsNull();
            // ShadowRootを指定
            GetExecutor().ExecuteScript("arguments[0].attachShadow({ mode: 'open' });", elem);
            shadowRoot = elem.GetShadowRoot();
            // 取得されることを確認
            shadowRoot.IsNotNull();
            height = GetExecutor().ExecuteScript("return arguments[0].getBoundingClientRect().height;", elem);
            // ShadowRoot指定された要素は表示されないはずなので高さが0になる
            height.Is(0);
        }
    }
}
