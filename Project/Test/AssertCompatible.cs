using System;

namespace Test
{
    static class AssertCompatible
    {
        internal static void ThrowsException<T>(Action p) where T : Exception
        {
            try
            {
                p();
            }
            catch (T)
            {
                return;
            }
            throw new Exception();
        }
        internal static void ThrowsException<T>(Func<object> p) where T : Exception
        {
            try
            {
                p();
            }
            catch (T)
            {
                return;
            }
            throw new Exception();
        }

        internal static void ThrowsException<T>(Action p, string msg) where T : Exception
        {
            try
            {
                p();
            }
            catch (T)
            {
                return;
            }
            throw new Exception(msg);
        }

        internal static void ThrowsException<T>(Func<object> p, string msg) where T : Exception
        {
            try
            {
                p();
            }
            catch (T)
            {
                return;
            }
            throw new Exception(msg);
        }

        internal static void IsInstanceOfType(object value, Type type)
        {
            if (!type.IsAssignableFrom(value.GetType()))
            {
                throw new Exception();
            }
        }
    }
}
