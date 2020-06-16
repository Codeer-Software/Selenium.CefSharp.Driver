using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using Microsoft.CSharp;
using Selenium.CefSharp.Driver.InTarget.Properties;

namespace Selenium.CefSharp.Driver.InTarget
{
    public class CefSharpWindowManagerFactory
    {
        const string CefSharpWindowManagerTypeFullName = "Selenium.CefSharp.Driver.CefSharpWindowManager";

        public static object InstallCefSharpWindowManager(dynamic browser)
        {
            if (browser.LifeSpanHandler != null)
            {
                if (browser.LifeSpanHandler.GetType().FullName == CefSharpWindowManagerTypeFullName)
                {
                    return browser.LifeSpanHandler;
                }
                return null;
            }
            browser.LifeSpanHandler = Create();
            return browser.LifeSpanHandler;
        }

        static dynamic Create() => Activator.CreateInstance(CreateType());

        static Type CreateType()
        {
            Type t = ReflectionAccessor.GetType(CefSharpWindowManagerTypeFullName);
            if (t != null)
            {
                return t;
            }

            var reference = new List<string>();
            reference.Add("System.dll");
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                switch (asm.GetName().Name)
                {
                    case "CefSharp":
                    case "CefSharp.Core":
                    case "CefSharp.Wpf":
                        reference.Add(asm.Location);
                        break;
                }
            }
            Compile(reference.ToArray(), Resources.CefSharpWindowManager);
            return ReflectionAccessor.GetType(CefSharpWindowManagerTypeFullName);
        }

        static CompilerResults Compile(string[] reference, string code)
        {
            if (reference == null)
            {
                throw new ArgumentNullException("reference");
            }
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters param = new CompilerParameters();
            param.GenerateInMemory = true;

            for (int i = 0; i < reference.Length; i++)
            {
                param.ReferencedAssemblies.Add(reference[i]);
            }
            param.IncludeDebugInformation = true;

            return codeProvider.CompileAssemblyFromSource(param, code);
        }
    }
}
