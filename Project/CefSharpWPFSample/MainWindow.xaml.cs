using System.Windows;
using System.Windows.Input;
using CefSharp;
using CefSharp.Wpf;

namespace CefSharpWPFSample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            _browser.Address = @"C:\GitHub\Selenium.CefSharp.Driver\Project\Test\Controls.html";
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
        }

        bool inited = false;


        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (Keyboard.Modifiers != ModifierKeys.Control) return;

            //test
            if (e.Key == Key.U)
            {
                if (!inited)
                {
                    //
                    //_browser.loa

                    var check = @"!(window.__seleniumCefSharpDriver == null)";

                    var retx = CefSharp.WebBrowserExtensions.EvaluateScriptAsync(_browser, check).Result;

                    var init = @"
if (!(window.__seleniumCefSharpDriver == null)) return;

    window.__seleniumCefSharpDriver = {};
    window.__seleniumCefSharpDriver.elements = [];


  function showAndSelectElement(element) {
    var rect = element.getBoundingClientRect();
    var elemtop = rect.top + window.pageYOffset;
    document.documentElement.scrollTop = elemtop;
    element.focus();
  }
";
                    var ret = CefSharp.WebBrowserExtensions.EvaluateScriptAsync(_browser, init).Result;
                    
                    inited = true;
                }

                var scr = @"
var e = document.getElementById('inputJS');
window.__seleniumCefSharpDriver.elements.push(e);
window.__seleniumCefSharpDriver.elements.length - 1;
";
                var ret2 = CefSharp.WebBrowserExtensions.EvaluateScriptAsync(_browser, scr).Result;

                int id = (int)ret2.Result;
                var click = $@"
var element = window.__seleniumCefSharpDriver.elements[{id}];
showAndSelectElement(element);
element.focus();
element.click();
";
                var ret3 = CefSharp.WebBrowserExtensions.EvaluateScriptAsync(_browser, click).Result;
            }
        }
    }
}
