# How to create system tray application

This type of application is a kind of hybrid application: it acts like a service, which running in the background until you give it focus, then it allowing you to interact with it like a normal GUI application.

There some requirements for system tray application:  
- Only one instance of the app may run  
- We must have the tray icon  
- We must have the tray menu  
- Application window must not be visible on the task bar.

The main component for .NET is *NotifyIcon*. I create *SystemTrayApp* and *WpfTrayTestLibrary* projects into sample solution.
*SystemTrayApp* project contains all files that you need for creation any tray application.
*WpfTrayTestLibrary* project contains all UI parts. Pay attention that *DataContext* for the views will be set into *SystemTrayApp*.

There is additional [description](https://www.red-gate.com/simple-talk/dotnet/net-framework/creating-tray-applications-in-net-a-practical-guide/)