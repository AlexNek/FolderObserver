using System;
using System.IO;

namespace FolderObserver.Common
{
    internal class DirectoryWatcher
    {
        public event EventHandler<FileSystemEventArgs> NewFileAdded;

        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();

        public void Start(string directoryName)
        {
            _watcher.Path = directoryName;
            // Don't watch subdirectories.  
            _watcher.IncludeSubdirectories = false;
            // Watch all files.  
            _watcher.Filter = "*.*";
            _watcher.Created += Watcher_Created;
            _watcher.NotifyFilter = NotifyFilters.FileName;
            //Start monitoring.  
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            //Stop monitoring.  
            _watcher.EnableRaisingEvents = false;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string extension = Path.GetExtension(e.Name);
            if (extension == ".zip")
            {
                return;
            }

            //e.FullPath
            NewFileAdded?.Invoke(sender, e);
        }
    }
}
