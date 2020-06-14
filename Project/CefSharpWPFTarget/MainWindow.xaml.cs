using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using CefSharp;
using CefSharp.Wpf;

namespace CefSharpWPFTarget
{
    public class CallbackBoundObject 
    {
        public void Complete(object value)
        {
            this.Value = value;
            this.IsCompleted = true;
        }

        public bool IsCompleted { get; private set; } = false;

        public object Value { get; private set; } = null;
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //  _browser.Address = @"C:\GitHub\Selenium.CefSharp.Driver\Project\Test\Controls.html";

            _browser.Loaded += _browser_Loaded;
            _browser.FrameLoadStart += _browser_FrameLoadStart;
            _browser.FrameLoadEnd += _browser_FrameLoadEnd;
          
         //   IFrame x = null;
         //   x.load
            /*
            IFrame x;

            x.Browser.GetFrame();

            _browser.JavascriptObjectRepository

            */

            //   WebBrowserExtensions.GetPa
        }

        class IFrameNode
        { 
            public IFrame Frame { get; set; }
            public List<IFrameNode> Children { get; } = new List<IFrameNode>();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (Keyboard.Modifiers != ModifierKeys.Control) return;

            if (e.Key == Key.W)
            {
                _browser.ShowDevTools();
            }

            if (e.Key == Key.Q)
            {
                _browser.Address = @"C:\GitHub\Selenium.CefSharp.Driver\Project\Test\Controls.html";
            }

            if (e.Key == Key.R)
            {
                _browser.Address = @"C:\GitHub\Selenium.CefSharp.Driver\Project\Test\Frame.html";
            }
            if (e.Key == Key.E)
            {

                var list = _browser.GetBrowser().GetFrameIdentifiers().Select(y => _browser.GetBrowser().GetFrame(y)).ToList();
                /*
                var x = _browser.GetBrowser().GetFrame(_browser.GetBrowser().GetFrameIdentifiers()[2]);

                x.SelectAll();

                var z = _browser.GetBrowser().GetFrameNames();


                var y1 = _browser.GetBrowser().GetFrameIdentifiers();
                var y = x.Browser.GetFrameIdentifiers();

                x.Browser.GoBack();*/

                var parent = _browser.GetMainFrame();
                var node = new IFrameNode { Frame = parent };
                XXX(node, list);
            }

            if (e.Key == Key.B)
            {
                var callbackObj = new CallbackBoundObject();
                _browser.JavascriptObjectRepository.Register("_asyncScriptBoundObject", callbackObj, true, BindingOptions.DefaultBinder);
                var script = @"
CefSharp.BindObjectAsync('_asyncScriptBoundObject').then(() => {
    setTimeout(() => {
        _asyncScriptBoundObject.complete({a:123});
    }, 5000);
});";
                var x = CefSharp.WebBrowserExtensions.EvaluateScriptAsync(_browser, script).Result;

                while(!callbackObj.IsCompleted)
                {
                    Thread.Sleep(10);
                }
                _browser.JavascriptObjectRepository.UnRegister("_asyncScriptBoundObject");
                var res = callbackObj.Value;
            }
          
            //test
            if (e.Key == Key.U)
            {
                var html = _browser.GetSourceAsync().Result;


                var ary = @"
var x = [];
x.push(1);
x;
";


                var a = _browser.GetTextAsync().Result;
                _browser.Address = @"C:\GitHub\Selenium.CefSharp.Driver\Project\Test\Controls.html";
                
                var y = _browser.GetSourceAsync().Result;
                var z= _browser.GetTextAsync().Result;

                var vvv = _browser.GetMainFrame().IsValid;
                var xxx = _browser.GetMainFrame().EvaluateScriptAsync(ary).Result;
                //(string script, string scriptUrl = "about:blank", int startLine = 1, TimeSpan? timeout = null);

                var x = CefSharp.WebBrowserExtensions.EvaluateScriptAsync(_browser, ary).Result;

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

        private void XXX(IFrameNode node, List<IFrame> list)
        {
            foreach (var x in list)
            {
                if (x.Identifier == node.Frame.Identifier)
                {
                    continue;
                }
                else if (x.Parent != null && x.Parent.Identifier == node.Frame.Identifier)
                {
                    var nextNode = new IFrameNode { Frame = x };
                    node.Children.Add(nextNode);

                    XXX(nextNode, list);
                }
            }
        }

        private void _browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
        }
        private void _browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
        }


        private void _browser_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
