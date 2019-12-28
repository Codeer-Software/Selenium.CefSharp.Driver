using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Selenium.CefSharp.Driver.InTarget
{
    public class JSResultConverter
    {
        public static object ConvertToSelializable(object src)
        {
            if (src is ExpandoObject expandObjet)
            {
                var dst = new Dictionary<string, object>();
                foreach (var e in expandObjet)
                {
                    dst[e.Key] = ConvertToSelializable(e.Value);
                }
                return dst;
            }
            else if (src is List<object> list)
            {
                return list.Select(i => ConvertToSelializable(i)).ToList();
            }

            //TODO Add any other necessary conversions.

            return src;
        }
    }
}
