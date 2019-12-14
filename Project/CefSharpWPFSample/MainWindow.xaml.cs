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
           // _browser.Address = @"C:\GitHub\Selenium.CefSharp.Driver\Project\Test\Controls.html";
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (Keyboard.Modifiers != ModifierKeys.Control) return;

            //test
            if (e.Key == Key.U)
            {
                var init = @"

       (function () {
        if (!(window.__seleniumCefSharpDriver == null)) return;

        window.__seleniumCefSharpDriver = {};
        window.__seleniumCefSharpDriver.elements = [];


        window.__seleniumCefSharpDriver.showAndSelectElement = function (element) {
            var rect = element.getBoundingClientRect();
            var elemtop = rect.top + window.pageYOffset;
            document.documentElement.scrollTop = elemtop;
            element.focus();
        }

    })();
    ";
                var ret = CefSharp.WebBrowserExtensions.EvaluateScriptAsync(_browser, init).Result;

                var id = "inputJS";
                var scr = $@"
    (function () {{
        var e = document.getElementById('{id}');
        if (e === null) return -1;
        var index = window.__seleniumCefSharpDriver.elements.findIndex(x => {{
            return x === e;
        }});
        if (index != -1) return index;

        window.__seleniumCefSharpDriver.elements.push(e);
        return window.__seleniumCefSharpDriver.elements.length - 1;

    }})();
";
                var ret2 = CefSharp.WebBrowserExtensions.EvaluateScriptAsync(_browser, scr).Result;

                int index = (int)ret2.Result;
                var click = $@"
(function () {{
var element = window.__seleniumCefSharpDriver.elements[{index}];
window.__seleniumCefSharpDriver.showAndSelectElement(element);
element.focus();
element.click();
return element;
 }})();
";
                var ret3 = CefSharp.WebBrowserExtensions.EvaluateScriptAsync(_browser, click).Result;
            }
        }
    }
}
