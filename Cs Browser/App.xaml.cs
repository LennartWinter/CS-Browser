using System.Windows;
using CefSharp;
using CefSharp.Wpf;

namespace Cs_Browser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize CefSharp with settings
            CefSettings settings = new CefSettings();

            // Obtain an absolute path for the cache directory
            string cachePath = "cef/cache";
            settings.CachePath = System.IO.Path.GetFullPath(cachePath);

            // Enable local storage
            settings.CefCommandLineArgs.Add("enable-localstorage", "1");

            Cef.Initialize(settings);

            // Other application startup code
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Shutdown CefSharp when the application exits
            Cef.Shutdown();

            base.OnExit(e);
        }
    }
}