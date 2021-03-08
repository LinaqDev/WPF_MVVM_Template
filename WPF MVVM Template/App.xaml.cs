using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_MVVM_Template
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitLog();
            Log.Information($"Starting App {DateTime.Now}");
            var currentDomain = AppDomain.CurrentDomain;
            Log.Information($"Base Directory: {currentDomain.BaseDirectory}");
            Log.Information($"Target Framework Name: {currentDomain.SetupInformation.TargetFrameworkName}");
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void InitLog()
        {
            string LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "WPF_MVVM_Template", "log.log");
            if (!Directory.Exists(Path.GetDirectoryName(LogFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .WriteTo.File(LogFilePath, rollingInterval: RollingInterval.Month)
               .CreateLogger();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Log.Error("UnhandledException caught : " + ex.Message);
            Log.Error("UnhandledException StackTrace : " + ex.StackTrace);
            Log.Fatal("Runtime terminating: {0}", e.IsTerminating);
            MessageBox.Show($"{ex.Message} \n\n\n\nApplication will close now.", "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information($"Application was running for: {DateTime.Now.Subtract(System.Diagnostics.Process.GetCurrentProcess().StartTime).Duration()}");
            Log.Information("Closing...");
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
