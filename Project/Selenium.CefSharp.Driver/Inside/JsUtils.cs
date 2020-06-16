using System.Collections.Generic;
using System.Text;

namespace Selenium.CefSharp.Driver.Inside
{
    public static class JsUtils
    {

        private static readonly bool[] escapeFlags = new bool[128];
        static JsUtils()
        {
            var escapeChars = new List<char>
            {
                '\n', '\r', '\t', '\\', '\f', '\b', '"', '\'',
            };
            for (int i = 0; i < ' '; i++)
            {
                escapeChars.Add((char)i);
            }
            foreach(var c in escapeChars)
            {
                escapeFlags[c] = true;
            }
        }

        public static string ToJsEscapedString(string value)
        {
            var builder = new StringBuilder();
            foreach(var c in value)
            {
                if(c > 128 || !escapeFlags[c])
                {
                    builder.Append(c);
                }
                else
                {
                    if (c == '\r') builder.Append("\\r");
                    else if (c == '\n') builder.Append("\\n");
                    else builder.Append("\\").Append(c);
                }
            }
            return builder.ToString();
        }

    }
}
