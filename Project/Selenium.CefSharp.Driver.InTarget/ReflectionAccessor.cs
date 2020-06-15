using System;
using System.Reflection;
using System.Linq;

namespace Selenium.CefSharp.Driver.InTarget
{
    public class ReflectionAccessor
    {
        public object Object { get; }

        public ReflectionAccessor(object obj) => Object = obj;

        public T GetProperty<T>(string name)
            => (T)Object.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(Object, new object[0]);

        public T GetField<T>(string name)
            => (T)Object.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(Object);

        public T InvokeMethod<T>(string name, params object[] args)
             => (T)Object.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(Object, args);

        public T InvokeMethodByType<T>(string name, params object[] args)
             => (T)Object.GetType().GetMethod(name, args.Select(e => e.GetType()).ToArray()).Invoke(Object, args);
    }
}
