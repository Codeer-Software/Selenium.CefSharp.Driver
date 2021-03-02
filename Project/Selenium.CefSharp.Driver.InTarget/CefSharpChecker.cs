namespace Selenium.CefSharp.Driver.InTarget
{
    public class CefSharpChecker
    {
        public static bool IsEvaluateScriptAsyncArgs4()
        {
            var iFrameType = ReflectionAccessor.GetType("CefSharp.IFrame");
            return iFrameType.GetMethod("EvaluateScriptAsync").GetParameters().Length == 4;
        }
    }
}
