# How to create login dialog for WPF application

At first glance, everything looks simple:

````csharp
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        bool startApplication = true;

        if (IsLoginEnabled())
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
}
````

We show login dialog, check password and show set main window.
Curiously we can can see or login window or main window bur never both.
After some investigation this behaviour will be clear - By default, a WPF application terminates when all of its windows are closed.  This corresponds to a value of OnLastWindowClose for the main Application object’s [ShutdownMode](https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.shutdownmode?view=netframework-4.7.2) property.

So, by default, application will be closed if any window will be closed. We must disable it and close application itself.


````csharp
ShutdownMode = ShutdownMode.OnExplicitShutdown;

````