using System;

using GalaSoft.MvvmLight;

namespace FolderObserver.Model
{
    /// <summary>
    /// Class FileItem.
    /// Used in Ui. We could change selected property
    /// Implements the <see cref="GalaSoft.MvvmLight.ObservableObject" />
    /// </summary>
    /// <seealso cref="GalaSoft.MvvmLight.ObservableObject" />
    public class FileItem : ObservableObject
    {
        private bool _isError;

        private bool _selected;

        public DateTime CopyDate { get; set; }

        public bool IsError
        {
            get
            {
                return _isError;
            }
            set
            {
                if (_isError != value)
                {
                    _isError = value;
                    RaisePropertyChanged(nameof(IsError));
                }
            }
        }

        public string Name { get; set; }

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (_selected != value)
                {
                    _selected = value;
                    RaisePropertyChanged(nameof(Selected));
                }
            }
        }

        public DateTime TimeStamp { get; set; }

        public string TargetFilePath { get; set; }

    }
}
