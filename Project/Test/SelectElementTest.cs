using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace Test
{
    public abstract class SelectElementTest : CompareTestBase
    {
        public class Forms : SelectElementTest
        {
            public Forms() : base(new FormsAgent()) { }
        }
        public class Wpf : SelectElementTest
        {
            public Wpf() : base(new WpfAgent()) { }
        }
        public class Web : SelectElementTest
        {
            public Web() : base(new WebAgent()) { }
        }
        protected SelectElementTest(INeed need) : base(need) { }

        [SetUp]
        public void SetUp()
            => GetDriver().Url = HtmlServer.Instance.RootUrl + "Controls.html";

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
