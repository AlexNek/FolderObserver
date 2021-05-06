using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using FolderObserver.Common;
using FolderObserver.Model;
using FolderObserver.Views;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using log4net;

using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FolderObserver.ViewModel
{
    internal class MainWindowVm : ViewModelBase
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly DataSerializer _dataSerializer = new DataSerializer();

        private readonly DirectoryWatcher _directoryWatcher = new DirectoryWatcher();

        private readonly IErrorHandler _errorHandler = new ErrorHandlerTrace();

        private readonly MainWorker _mainWorker;

        private bool _isObservationRunning;

        private string _sourceFolder;

        private string _targetFolder;

        public MainWindowVm()
        {
            CommandEditSettings = new RelayCommand(ExecEditSettings);
            CommandSourceFolder = new RelayCommand(ExecSetSourceFolder, CanChangeFolder);
            CommandTargetFolder = new RelayCommand(ExecSetTargetFolder, CanChangeFolder);
            CommandExport = new AsyncCommand(ExecDataExportAsync, CanAlways, _errorHandler);

            CommandStartObservation = new RelayCommand(ExecStartObservation);
            CommandStopObservation = new RelayCommand(ExecStopObservation);

            CommandSetFlag = new RelayCommand(ExecSetFlag);
            CommandResetFlag = new RelayCommand(ExecResetFlag);

            CommandGridSelectionChanged = new RelayCommand<IList<object>>(ExecGridSelectionChanged);
            CommandGridRowDblClick = new AsyncCommand<FileItem>(ExecGridRowDblClickAsync, CanRunGridRowDblClick, _errorHandler);
            //CommandRemoveCheckedItems = new AsyncCommand(ExecRemoveCheckedItemsAsync, CanAlways, _errorHandler);

            CommandSelectAll = new RelayCommand(ExecSelectAll);
            CommandDeleteSelected = new AsyncCommand(ExecDeleteSelectedAsync, CanAlways, _errorHandler);

            SourceFolder = Properties.Settings.Default.SorceFolder;
            TargetFolder = Properties.Settings.Default.TargetFolder;
            _directoryWatcher.NewFileAdded += DirectoryWatcherNewFileAdded;

            DataItems dataItems = _dataSerializer.Load();
            Items.Clear();
            foreach (FileItem dataItem in dataItems)
            {
                Items.Add(dataItem);
            }

            _mainWorker = new MainWorker();
            _mainWorker.StateChanged += MainWorkerStateChanged;
            _mainWorker.TargetFolder = TargetFolder;
            //Items.CollectionChanged += Items_CollectionChanged;
        }

        //private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.Action == NotifyCollectionChangedAction.Remove)
        //    {

        //    }
        //}

        public void Exit()
        {
            ExecStopObservation();
            //We need it for ShutdownMode="OnExplicitShutdown" 
            Application.Current.Shutdown();
        }

        private bool CanAlways()
        {
            return true;
        }

        private bool CanChangeFolder()
        {
            return !IsObservationRunning && !_mainWorker.IsRunning;
            //return true;
        }

        //private bool CanDeleteSelected()
        //{
        //    return true;
        //}

        //private bool CanRemoveCheckedItems()
        //{
        //    return true;
        //}

        private bool CanRunGridRowDblClick(FileItem arg)
        {
            return true;
        }

        private static CommonOpenFileDialog CreateFolderSelectionDialog(string dialogTile, string currentDirectory)
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog();

            dlg.Title = dialogTile;
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = currentDirectory;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = currentDirectory;
            dlg.EnsureFileExists = false;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;
            return dlg;
        }

        private void DirectoryWatcherNewFileAdded(object sender, FileSystemEventArgs e)
        {
            _mainWorker.AddWorkItem(e);
            _mainWorker.RunHandlingOfWorkingItems(
                (fileItem) =>
                    {
                        Application.Current.Dispatcher.Invoke(
                            () =>
                                {
                                    Items.Add(fileItem);
                                });

                    },
                () =>
                    {
                        _dataSerializer.Store(Items);
                    },
                (ex, fileItem) =>
                    {
                        Application.Current.Dispatcher.Invoke(
                            () =>
                                {
                                    fileItem.IsError = true;
                                });

                    },
                false
            );


        }

        private async Task ExecDataExportAsync()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV file (*.csv)|*.csv|Text file (*.txt)|*.txt";
            saveFileDialog.InitialDirectory = Properties.Settings.Default.LastExportDirectory;
            saveFileDialog.FileName = "export";
            if (saveFileDialog.ShowDialog() == true)
            {
                Properties.Settings.Default.LastExportDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                Properties.Settings.Default.Save();

                DataExportCsv export = new DataExportCsv(saveFileDialog.FileName);
                await export.ExportAsync(Items);
            }
        }

        private async Task ExecDeleteSelectedAsync()
        {
            _log.Debug("Delete selected items");
            for (int i = 0; i < Items.Count;)
            {
                FileItem dataItem = Items[i];
                if (dataItem.Selected)
                {
                    Items.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            await _dataSerializer.StoreAsync(Items);
        }

        private void ExecEditSettings()
        {
            EditSettingsWnd dlg = new EditSettingsWnd();
            dlg.ShowDialog();
        }

        private async Task ExecGridRowDblClickAsync(FileItem arg)
        {
            _log.Debug($"Open target file {arg.TargetFilePath}");
            if (File.Exists(arg.TargetFilePath))
            {
                Process.Start(arg.TargetFilePath);
            }
            else
            {
                MessageBox.Show($"File not found {arg.TargetFilePath}");
            }

            //// Use ProcessStartInfo class
            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //startInfo.CreateNoWindow = true;
            //startInfo.UseShellExecute = false;
            //startInfo.FileName = arg.TargetFilePath;
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            ////startInfo.Arguments = arg.TargetFilePath;
            //try
            //{
            //    // Start the process with the info specified
            //    // Call WaitForExit and then the using-statement will close
            //    using (Process exeProcess = Process.Start(startInfo))
            //    {
            //        exeProcess.WaitForExit();
            //        //// Register a log of the successful operation
            //        //CustomLogEvent(string.Format(
            //        //    "Succesful operation --> Executable: {0} --> Arguments: {1}",
            //        //    executableFile, argumentList));
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _log.Error(ex);
            //}

            await Task.FromResult(1);
        }

        private void ExecGridSelectionChanged(IList<object> args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            SelectedItems.Clear();
            foreach (FileItem item in args)
            {
                SelectedItems.Add(item);
                //Trace.WriteLine("Selection:" + item.Name);
            }

            if (args.Count == Items.Count && args.Count>1)
            {
                foreach (FileItem item in Items)
                {
                    item.Selected = true;
                }
            }
        }

        //private async Task ExecRemoveCheckedItemsAsync()
        //{
        //    await Task.FromResult(1);
        //}

        private void ExecResetFlag()
        {
            SetSelectionFlag(false);
        }

        private void ExecSelectAll()
        {
            _log.Debug("Select all items");
            foreach (FileItem item in Items)
            {
                item.Selected = true;
            }
        }

        private void ExecSetFlag()
        {
            SetSelectionFlag(true);
        }

        private void ExecSetSourceFolder()
        {
            ExecStopObservation();

            CommonOpenFileDialog dlg = CreateFolderSelectionDialog("Select source folder", SourceFolder);

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string folder = dlg.FileName;
                SourceFolder = folder;
                Properties.Settings.Default.SorceFolder = folder;
                Properties.Settings.Default.Save();
                _log.Debug($"Source folder changed to {folder}");
            }
        }

        private void ExecSetTargetFolder()
        {
            ExecStopObservation();

            CommonOpenFileDialog dlg = CreateFolderSelectionDialog("Select Target folder", TargetFolder);

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string folder = dlg.FileName;
                TargetFolder = folder;
                Properties.Settings.Default.TargetFolder = folder;
                Properties.Settings.Default.Save();
                _log.Debug($"Target folder changed to {folder}");
            }
        }

        private void ExecStartObservation()
        {
            IsObservationRunning = true;
            CommandManager.InvalidateRequerySuggested();
            _directoryWatcher.Start(SourceFolder);
            _log.Debug($"Start observation for {SourceFolder}");
        }

        private void ExecStopObservation()
        {
            IsObservationRunning = false;
            CommandManager.InvalidateRequerySuggested();
            _directoryWatcher.Stop();
            _mainWorker.CancellationPending = true;
            _log.Debug("Stop observation");
        }

        private void MainWorkerStateChanged(object sender, bool isRunning)
        {
            Application.Current?.Dispatcher.Invoke(
                () =>
                    {
                        CommandManager.InvalidateRequerySuggested();
                    });
        }

        private void SetSelectionFlag(bool flagState)
        {
            foreach (FileItem selectedItem in SelectedItems)
            {
                selectedItem.Selected = flagState;
            }
        }

        public ICommand CommandDeleteSelected { get; }

        public ICommand CommandEditSettings { get; }

        public ICommand CommandExport { get; }

        public ICommand CommandGridRowDblClick { get; }

        public ICommand CommandGridSelectionChanged { get; }

        //public ICommand CommandRemoveCheckedItems { get; }

        /// <summary>
        /// Gets the command  - reset selection flag.
        /// </summary>
        /// <value>The command reset flag.</value>
        public ICommand CommandResetFlag { get; }

        public ICommand CommandSelectAll { get; }

        /// <summary>
        /// Gets the command - set selection flag.
        /// </summary>
        /// <value>The command set flag.</value>
        public ICommand CommandSetFlag { get; }

        public ICommand CommandSourceFolder { get; }

        public ICommand CommandStartObservation { get; }

        public ICommand CommandStopObservation { get; }

        public ICommand CommandTargetFolder { get; }

        public bool IsObservationRunning
        {
            get
            {
                return _isObservationRunning;
            }
            set
            {
                if (_isObservationRunning != value)
                {
                    _isObservationRunning = value;
                    RaisePropertyChanged(nameof(IsObservationRunning));
                }
            }
        }

        public DataItems Items { get; } = new DataItems();

        public int ProgressValue { get; set; }

        public string SourceFolder
        {
            get
            {
                return _sourceFolder;
            }
            set
            {
                if (_sourceFolder != value)
                {
                    _sourceFolder = value;
                    RaisePropertyChanged(nameof(SourceFolder));
                }
            }
        }

        public string TargetFolder
        {
            get
            {
                return _targetFolder;
            }
            set
            {
                if (_targetFolder != value)
                {
                    _targetFolder = value;
                    if (_mainWorker != null)
                    {
                        _mainWorker.TargetFolder = value;
                    }

                    RaisePropertyChanged(nameof(TargetFolder));
                }
            }
        }

        private IList<FileItem> SelectedItems { get; } = new List<FileItem>();
    }
}
/*
 * time  1 2:00
 * time  2 - 13:06-13:36 0:30
 * time  3 - 14:15-14:30 0:15
 * time  4 - 19:28-19:58 0:20
 * time  5 - 22:38-23:34 0:56
 * time  6 - 10:34-11:54 1:20
 * time  7 - 12:08-13:58 1:06
 * time  8 - 14:10-15:05 0:55
 * time  9 - 17:13-17:43 0:30 - 352 min
 * time 10 - 20:31-21:09 0:39
 * time 11 - 21:31-22:11 0:40
 * time 12 - 22:29-23:20 0:51
 *  zip unit tests
 * time 12 - 11:21-11:35 0:14
 * time 13 - 12:07-12:14 0:07
 * time 14 - 16:48-17:10 0:18
 * time 15 - 17:40-18:47 1:07
 * time 16 - 22:20-23:37 1:17
 * time 17 - 13:37-14:00 0:23
 * time 18 - 19:32-19:55 0:23
 * time 19 - 21:10-21:28 0:18
 * time 20 - 23:11-23:24 0:13 - 390+352=742=12:22
 * tray app
 * time 21 - 19:20-20:10 0:50
 * time 22 - 16:23-17:05 0:42
 * time 23 - 17:18-17:24 0:06 - 98
 * unit test ext
 * time 24 - 22:20-22:38 0:18 - 116+742=858=14:18
 */
