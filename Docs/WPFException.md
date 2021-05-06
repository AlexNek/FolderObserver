# Exception Handling in WPF

Some developers thinking that try/catch statement is useful to handle unexpected or runtime exceptions
and it is enough for application.
````csharp
try
{
    //Do work
}
catch(Exception ex)
{
    //Handle exception
}
````
But after some day they receive a message that software is crashed. Why - nobody know.
To prevent such a situation recommended to handle all system error messages. 
You can not prevent application crash of course but you will know what action crashed your application.

There is three events:

- AppDomain UnhandledException is invoked whenever an unhandled exception is thrown in the default application domain
- Application DispatcherUnhandledException Occurs when an exception is thrown by an application but not handled. Need for logging purpose only.
- TaskScheduler  UnobservedTaskException - Occurs when a faulted task's unobserved exception is about to trigger exception escalation policy

Here is a sample of how to do it:
        
````csharp
public partial class App : Application
{
    private static ReportCrash _reportCrash;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        Application.Current.DispatcherUnhandledException += DispatcherOnUnhandledException;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        
    }

    private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
    {
       //Handle exception
       //Not handled application error
    }

    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
    {
       //Handle exception
       //Unrecoverable application error
    }

    private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
    {
       //Handle exception
       //TaskScheduler Unrecoverable application error
    }
    
}
````