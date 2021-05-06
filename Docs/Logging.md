# Logging

Every developer encounters a situations when some component of the application 
works out in a strange way, gives the wrong result or stops working.
Using logging will help us in such situations.
When we talk about logging, we usually mean saving a message in a storage. 
It could be a file or text on the monitor.
Normally we want to know from the message: the date and time, from which software part was sent and what is happens.
And normally we don't want to write something complicated.
Of course the better way is using some of standard library.

Some of free loggings framework for C#:

- [NLog](https://nlog-project.org/)
- [Log4Net](https://logging.apache.org/log4net/)
- [Serilog](https://serilog.net/)
- [ELMAH](https://code.google.com/archive/p/elmah/wikis/MVC.wiki)

In addition it is possible to use crash reporting tools like [CrashReporter.NET](https://github.com/ravibpatel/CrashReporter.NET) but in most cases logging will be enought.

I personally use Log4net for a long time and will use it for the sample application.
One big disadvantage for Log4net now - lack of support .NET Core 2.x and all versions above.

But from other side easy to configure through configuration file,
possibility to write the log into more that 20 different targets,
output layout/pattern customization, like  "%timestamp [%thread] %-5level %logger - %message%newline"

For the using you need:  
1. Add the Log4Net package to the project.  
2. Create configuration file.  
[Samples](https://logging.apache.org/log4net/release/config-examples.html) of different configuration   
3. Initialize loggin engine
````csharp
 XmlConfigurator.Configure(new FileInfo("log4Net.config"));
````  
4. Create log variable as class data member.
````csharp
private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
````
5. Use log
````csharp
_log.Debug("Start Application *******************");
````
