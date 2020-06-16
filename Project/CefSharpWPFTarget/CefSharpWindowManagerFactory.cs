using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using CefSharpWPFTarget.Properties;
using Codeer.Friendly.DotNetExecutor;
using Microsoft.CSharp;

namespace Codeer.Friendly.Windows.Grasp.Inside
{
    static class CefSharpWindowManagerFactory
    {
        internal static dynamic Create() => Activator.CreateInstance(CreateType());

        static Type CreateType()
        {
            var finder = new TypeFinder();
            Type t = finder.GetType("Selenium.CefSharp.Driver.CefSharpWindowManager");
            if (t != null)
            {
                return t;
            }

            //参照
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
            return finder.GetType("Selenium.CefSharp.Driver.CefSharpWindowManager");
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
