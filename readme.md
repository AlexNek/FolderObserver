# Folder observer - C# WPF application sample

I try to describe how to write well designed software application.
In addition it is important to know how we want test and support our application.

There is a lot of rules for writting good/Clean code. I'll try to expand this description if you need it.

# Overview

We try to develop a WPF application which could monitor specific folder and do some actions if new file will be detected.
Our application will have the unit test and logging possibility.
In addition we try to write system tray application.

Some themes will be described more detailed:

[Why MVVM is better approach?](Docs/mvvm.md)?  
How to write testable application?  
How to write unit test for system calls?   
What is mocking?  
[Details here](Docs/UnitTests.md)

[How to use Logging?](Docs/Logging.md)    
[How to handle exceptions in WPF?](Docs/WPFException.md)    

# Project description

The requirements for sample application was not created by me. From one side it is simple tasks but some tasks have pitfalls.

1. Create a tool to monitor a specific folder for the new files.  
2. The tool should automatically move files into another folder.  
3. Files must be compessed before moving.  
4. Display list of the moved files in UI.  
5. Show date/time of each file.  
6. Allow delete an item from the list.
7. Allow delete all.
8. Export list as CSV-file.
9. Implement Double-click on list item to open zip-file using associated default windows handler.  
10. The list should be persist between tool restarts. 
11. Monitored and output paths must be configurable.
12. Implement menu to pause/resume monitoring.
13. Log all the actions.
14. Add unit tests.  
15. Protect tool with configurable password.
16. Create system tray application. 

# Project implementation notes

R1 implemented as WPF application.

![Folder Observer screenshot](Images/folder-observer.png)  
List of files contain additional columns as we need to know when file was moved and visualize the error state.
In addition there is the selector box for removing selected items only.
For r11 missed requirement htat after observation start must not be possible to chage the directories.
For r16 created additional system tray apllication. 

For directory observation *FileSystemWatcher* used. From [description](https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher?view=netframework-4.7.2) some field settings could be misuderstanding. For sample *IncludeSubdirectories* mean then subdirectories content must be observed in addition. If we want to watch files only, we
need to use *NotifyFilter*
 
````csharp
_watcher.IncludeSubdirectories = false;
_watcher.NotifyFilter = NotifyFilters.FileName;
````
R3 added additional troubles:  
- we can't use watcher notification for immediate compession as for the big files we need a lot of time and we can miss sequentual notification. As a solution we put all notification into thread-safe queueand then process the items one by one in one thread. 
- by compression start new file will be created in observed directory so we must ignore this file. As simple solution we ignore all *.zip files.  

R9 implemented very easy
````csharp
Process.Start(FilePath);
````

For another requierements I have more detailed description:  
[R13 How to use Logging?](Docs/Logging.md)  
[R14 How to use Unit test](Docs/UnitTests.md) - project local tests are commented on.    
[R15 How to show dialog before main application?](Docs/WPF2Windows.md)   
[R16 How To create System tray application?](Docs/SystemTrayApp.md) 
# Updates
Update I - added xUnit project with fluent assertions. [See](Docs/UnitTests.md) 

# Further reading

[SOLID Principles In C#](https://www.c-sharpcorner.com/UploadFile/damubetha/solid-principles-in-C-Sharp/)  
[The Don’t-Repeat-Yourself (DRY) design principle in .NET](https://dotnetcodr.com/2013/10/17/the-dont-repeat-yourself-dry-design-principle-in-net-part-1/)  
[KISS — One Best Practice to Rule Them All](https://simpleprogrammer.com/kiss-one-best-practice-to-rule-them-all/)  
[YAGNI design principle from Martin Fowler](https://www.martinfowler.com/bliki/Yagni.html)  
