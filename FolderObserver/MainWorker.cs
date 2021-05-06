using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using FolderObserver.Common;
using FolderObserver.Model;

using log4net;

namespace FolderObserver
{
    internal class MainWorker
    {
        public event EventHandler<bool> StateChanged;

        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //private readonly IDataSerializer _dataSerializer;

        private readonly ConcurrentQueue<FileSystemEventArgs> _filesQueue = new ConcurrentQueue<FileSystemEventArgs>();

        //private readonly DataItems _items;

        private readonly object _lockObj = new object();

        public void AddNewItem1(FileSystemEventArgs e, DataItems items, IDataSerializer dataSerializer, string targetFolder)
        {
            try
            {
                string sourceFilePath = e.FullPath;
                //string archiveName = FileCompressor.GetArchiveFileName(e.FullPath);
                string fileName = Path.GetFileName(sourceFilePath);
                string destFilePath = targetFolder;
                Task.Run(
                    () =>
                        {
                            if (FileMover.Move(sourceFilePath, destFilePath))
                            {
                                FileCompressor.Compress(destFilePath);
                                dataSerializer.Store(items);
                            }
                        }
                );
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }

        public void AddWorkItem(FileSystemEventArgs e)
        {
            _filesQueue.Enqueue(e);
        }

        public virtual void CompressFile(string sourceFilePath)
        {
            FileCompressor.Compress(sourceFilePath);
        }

        public virtual void DeleteFile(string sourceFilePath)
        {
            File.Delete(sourceFilePath);
        }

        public virtual DateTime GetFileCreationTime(string fileFullPath)
        {
            DateTime creationTime = File.GetCreationTime(fileFullPath);
            return creationTime;
        }

        public virtual void MoveFile(string archiveName, string destFilePath)
        {
            FileMover.Move(archiveName, destFilePath);
        }

        public void RunHandlingOfWorkingItems(
            Action<FileItem> addItemToListAction,
            Action storeItems,
            Action<Exception, FileItem> errorAction,
            bool exitAfterFinishAllItems = true)
        {
            bool isRunning;

            lock (_lockObj)
            {
                isRunning = IsRunning;
            }

            if (isRunning)
            {
                return;
            }

            lock (_lockObj)
            {
                IsRunning = true;
                OnStateChanged(IsRunning);
                _log.Debug($"MainWorker will be started for {_filesQueue.Count} files. Cansellation pending {CancellationPending}");
            }

            Task.Run(
                () =>
                    {
                        _log.Debug($" MainWorker started");
                        Thread.CurrentThread.Name = "Main worker";
                        while (!CancellationPending)
                        {
                            

                            if (_filesQueue.TryDequeue(out FileSystemEventArgs moveItem))
                            {
                                _log.Debug($" Do work for {moveItem.Name}");
                                FileItem fileItem = new FileItem { Name = moveItem.Name, CopyDate = DateTime.Now };
                                //_log.Debug($"New item detected {e.FullPath}. Zip and move it");
                                DateTime creationTime = GetFileCreationTime(moveItem.FullPath);
                                //item.TimeStamp = creationTime.ToShortDateString() + " " + creationTime.ToShortTimeString();
                                fileItem.TimeStamp = creationTime;

                                try
                                {
                                    addItemToListAction?.Invoke(fileItem);

                                    string sourceFilePath = moveItem.FullPath;
                                    string archiveName = FileCompressor.GetArchiveFileName(sourceFilePath);
                                    string fileName = Path.GetFileName(archiveName);
                                    string destFilePath = Path.Combine(TargetFolder, fileName);

                                    CompressFile(sourceFilePath);

                                    MoveFile(archiveName, destFilePath);
                                    fileItem.TargetFilePath = destFilePath;

                                    DeleteFile(sourceFilePath);

                                    storeItems?.Invoke();

                                    //_dataSerializer.Store(_items);
                                }
                                catch (Exception ex)
                                {
                                    _log.Error(ex);

                                    if (errorAction != null)
                                    {
                                        errorAction(ex, fileItem);
                                    }

                                    //Not good as exception could come in theory but we need to store error state
                                    storeItems?.Invoke();
                                }
                            }
                            else
                            {
                                if (exitAfterFinishAllItems && _filesQueue.Count == 0)
                                {
                                    _log.Debug($" MainWorker - break by zero items");
                                    break;
                                }
                            }

                            Thread.Sleep(50);
                        }

                        lock (_lockObj)
                        {
                            IsRunning = false;
                            OnStateChanged(IsRunning);
                        }

                        _log.Debug($"MainWorker stopped. CancellationPending={CancellationPending}");
                    });
        }

        protected virtual void OnStateChanged(bool e)
        {
            StateChanged?.Invoke(this, e);
        }

        public bool CancellationPending { get; set; }

        public bool IsRunning { get; private set; }

        public string TargetFolder { get; set; }

        public int WorkingQueueCount
        {
            get
            {
                return _filesQueue.Count;
            }
        }
    }
}
