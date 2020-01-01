using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CefSharpWPFSample
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //var cefSettings = new CefSettings();
            //var customScheme = new CefCustomScheme();
            //customScheme.SchemeName = "CSPOff";
            //customScheme.IsCSPBypassing = true;
            //cefSettings.RegisterScheme(customScheme);
            //Cef.Initialize(cefSettings);
        }
    }
}
