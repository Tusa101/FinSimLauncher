using CefSharp.Wpf;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FinSimLauncher
{
    public static class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            var exitCode = CefSharp.BrowserSubprocess.SelfHost.Main(args);

            if (exitCode >= 0)
            {
                return exitCode;
            }

            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache"),
                BrowserSubprocessPath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName
            };

            //IMPORTANT: MUST set performDependencyCheck false
            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);

            var app = new App();
            app.InitializeComponent();
            return app.Run();
        }
    }
}
