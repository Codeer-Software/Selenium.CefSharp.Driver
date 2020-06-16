using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using CefSharp;
using CefSharp.Wpf;
using Codeer.Friendly.Windows.Grasp.Inside;
using Selenium.CefSharp.Driver;

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
            //_browser.Address = @"https://www.codeer.co.jp/";

            //dynamic life = CefSharpWindowManagerFactory.Create();

            //dynamic x = _browser;
            //x.LifeSpanHandler = CefSharpWindowManagerFactory.Create();

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

        class FrameNode
        { 
            public IFrame Frame { get; set; }
            public List<FrameNode> Children { get; } = new List<FrameNode>();
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

                IFrame frame = _browser.GetMainFrame();

                var prop = frame.GetType().GetProperty("Parent");



                var parent = frame.Parent;
            //    var parentX = ((dynamic)frame).Parent;


                var names = new[] { "", "iframe2_name", "" }.ToList();
                IFrame frame1 = (IFrame)FindFrame(_browser, _browser.GetMainFrame(), names, 0);
                IFrame frame2 = (IFrame)FindFrame(_browser, _browser.GetMainFrame(), names, 1);
                IFrame frameX = (IFrame)FindFrame(_browser, _browser.GetMainFrame(), names, 2);
                var names2 = new[] { "iframe3_name" }.ToList();
                IFrame frame3 = (IFrame)FindFrame(_browser, frame2, names2, 0);
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

        static IFrame FindFrameOld(ChromiumWebBrowser browser, IFrame frame, int childIndex)
        {
            if (childIndex < 0) return null;
            var node = MakeFrameTree(browser);
            var frameNode = FindFrameNode(node, frame);
            if (frameNode == null) return null;
            if (frameNode.Children.Count <= childIndex) return null;
            return frameNode.Children[childIndex].Frame;
        }


        static object FindFrame(object chromiumWebBrowser, object parentFrame, List<string> frameNames, int childIndex)
        {
            if (childIndex < 0) return null;
            var children = GetChildren(chromiumWebBrowser, parentFrame, frameNames);
            if (children.Length <= childIndex) return null;
            return children[childIndex];
        }


        static object[] GetChildren(object chromiumWebBrowser, object parentFrame, List<string> frameNames)
        {
            var webBrowserAcs = new ReflectionAccessor(chromiumWebBrowser);
            var browserAcs = new ReflectionAccessor(webBrowserAcs.InvokeMethod<object>("GetBrowser"));

            var allFrames = new List<object>();
            foreach (var e in browserAcs.InvokeMethod<IEnumerable>("GetFrameIdentifiers"))
            {
                allFrames.Add(browserAcs.InvokeMethodByType<object>("GetFrame", e));
            }

            var parentFrameIdentifier = new ReflectionAccessor(parentFrame).GetProperty<long>("Identifier");
            var children = new List<ReflectionAccessor>();
            foreach (var frame in allFrames)
            {
                var frameAcs = new ReflectionAccessor(frame);
                var parent = frameAcs.GetProperty<object>("Parent");
                if (parent == null) continue;

                var parentAcs = new ReflectionAccessor(parent);
                if (parentAcs.GetProperty<long>("Identifier") == parentFrameIdentifier)
                {
                    children.Add(frameAcs);
                }
            }

            children = children.OrderBy(e => e.GetProperty<string>("Name")).ToList();
            if (children.Count < frameNames.Count) return children.ToArray();

            var sortedChildren = new object[children.Count];

            for (int i = 0; i < children.Count; i++)
            {
                var e = children[i];
                var index = frameNames.IndexOf(e.GetProperty<string>("Name"));
                if (index == -1) continue;
                sortedChildren[index] = e.Object;
                children[i] = null;
            }

            int j = 0;
            for (int i = 0; i < children.Count; i++)
            {
                var e = children[i];
                if (e == null) continue;

                for (; j < sortedChildren.Length; j++)
                {
                    if (sortedChildren[j] == null)
                    {
                        sortedChildren[j] = e.Object;
                        j++;
                        break;
                    }
                }
            }
            return sortedChildren;
        }

        
        static FrameNode FindFrameNode(FrameNode node, IFrame frame)
        {
            if (frame.Identifier == node.Frame.Identifier) return node;
            foreach (var e in node.Children)
            {
                var hit = FindFrameNode(e, frame);
                if (hit != null) return hit;
            }
            return null;
        }

        static FrameNode MakeFrameTree(ChromiumWebBrowser browser)
        {
            var list = browser.GetBrowser().GetFrameIdentifiers().Select(y => browser.GetBrowser().GetFrame(y)).ToList();
            var parent = browser.GetMainFrame();
            var node = new FrameNode { Frame = parent };
            MakeFrameTree(node, list);
            return node;
        }

        static void MakeFrameTree(FrameNode node, List<IFrame> list)
        {
            foreach (var x in list)
            {
                if (x.Identifier == node.Frame.Identifier)
                {
                    continue;
                }
                else if (x.Parent != null && x.Parent.Identifier == node.Frame.Identifier)
                {
                    var nextNode = new FrameNode { Frame = x };
                    node.Children.Add(nextNode);

                    MakeFrameTree(nextNode, list);
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
