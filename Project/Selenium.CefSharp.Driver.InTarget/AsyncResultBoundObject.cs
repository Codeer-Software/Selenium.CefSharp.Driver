namespace Selenium.CefSharp.Driver.InTarget
{
    public class AsyncResultBoundObject
    {
        public void Complete(object value)
        {
            this.Value = JSResultConverter.ConvertToSelializable(value);
            this.IsCompleted = true;
        }

        public bool IsCompleted { get; private set; } = false;

        public object Value { get; private set; } = null;
    }
}
