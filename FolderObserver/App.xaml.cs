using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using FolderObserver.ViewModel;
using FolderObserver.Views;

using log4net;
using log4net.Config;
using log4net.Util;

namespace FolderObserver
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static App()
        {
            //NOTE: uncomment if any troubles with logging
            //LogLog.InternalDebugging = true;

            XmlConfigurator.Configure(new FileInfo("log4Net.config"));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            _log.Debug("Exit Application *******************"); //MLHIDE

            //System.Windows.Threading.Dispatcher.InvokeShutdown();
            //Dispatcher.InvokeShutdown();
            //System.Windows.Threading.Dispatcher.ExitAllFrames();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //very important line for dialog before startup
            ShutdownMode = ShutdownMode.OnExplicitShutdown;
            
            StartupInit();
            bool startApplication = true;

            if (LoginWndVm.IsPasswordDefined())
            {
                LoginWnd dlg = new LoginWnd();
                bool? result = dlg.ShowDialog();
                startApplication = result.HasValue && result.Value;
            }

            if (startApplication)
            {
                StartupUri = new Uri("Views/MainWindow.xaml", UriKind.Relative);
            }
            else
            {
                _log.Error("Wrong password");
                // wrong password
                Shutdown(1);
            }
        }

        private static string GetVersion(FileVersionInfo fvi)
        {
            //FileMajorPart = 1
            //FileMinorPart = 0
            //FileBuildPart = 2
            //FilePrivatePart = 3
            //string version = fvi.FileVersion;

            string version = String.Format(
                "{0}.{1}.{2}",
                fvi.ProductMajorPart,
                fvi.ProductMinorPart,
                fvi.ProductBuildPart);
            return version;
        }

        private static void HandleNotExpectedException(Exception ex, string errorMessage)
        {
            //UnhandledExceptionEventArgs args;
            Current.Dispatcher.Invoke(
                () =>
                    {
                        if (Current.MainWindow != null)
                        {
                            MessageBox.Show(Current.MainWindow, ex.ToString(), errorMessage);
                        }
                        else
                        {
                            MessageBox.Show(ex.ToString(), errorMessage);
                        }
                    });

            Console.WriteLine("Error, UnhandledException caught : " + ex.Message);
            _log.Error(ex);
        }

        /// <summary>
        /// The application dispatcher unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void MainAppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            HandleNotExpectedException(ex, "Sorry, not handled application error");
            e.Handled = true;
            Application.Current.Shutdown();
        }

        private void StartupInit()
        {
            Thread.CurrentThread.Name = "Main GUI";
            
            string appPath = Assembly.GetExecutingAssembly().Location;

            _log.Debug("Start Application *******************");

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += UnhandledExceptionHandler;
            // could be for Debug time only - not important exceptions but customer think it is a problem
            //currentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            DispatcherUnhandledException += MainAppDispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            
            Assembly assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName().Name;
            var gitVersionInformationType = assembly.GetType(assemblyName + ".GitVersionInformation");
            if (gitVersionInformationType != null)
            {
                var fields = gitVersionInformationType.GetFields();

                foreach (var field in fields)
                {
                    Trace.WriteLine(string.Format("{0}: {1}", field.Name, field.GetValue(null)));
                }
            }
            else
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                string mVersion = GetVersion(fvi);
                string mProduct = fvi.ProductName;
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            AggregateException aggregateException = e.Exception;
            Exception exception = aggregateException.GetBaseException();
            HandleNotExpectedException(exception, "Sorry, TaskScheduler Unrecoverable application error");
            Application.Current.Shutdown();
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;
            HandleNotExpectedException(ex, "Sorry, Unrecoverable application error");
            //ILog logger = LogManager.GetLogger(LoggingService.Defaultlog);
            //logger.Debug(String.Format("Runtime terminating: {0}", args.IsTerminating));
            _log.Debug(string.Format("Runtime terminating: {0}", args.IsTerminating));
            Application.Current.Shutdown();
        }
    }
}
/*
 * https://www.codeproject.com/Articles/1173686/A-Csharp-System-Tray-Application-using-WPF-Forms
 */