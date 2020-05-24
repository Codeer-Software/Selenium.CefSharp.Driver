using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selenium.CefSharp.Driver
{
    public class AsyncResultBoundObject
    {
        public void Complete(object value)
        {
            this.Value = value;
            this.IsCompleted = true;
        }

        public bool IsCompleted { get; private set; } = false;

        public object Value { get; private set; } = null;
    }
}
