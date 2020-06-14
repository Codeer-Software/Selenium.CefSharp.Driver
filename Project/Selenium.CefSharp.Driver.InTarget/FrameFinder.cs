using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Selenium.CefSharp.Driver.InTarget
{
    public class FrameFinder
    {
        public static object FindFrame(object chromiumWebBrowser, object parentFrame, List<string> frameNames, int childIndex)
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

            //sort
            //For names with names on dom, use the order of appearance.
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
    }
}
