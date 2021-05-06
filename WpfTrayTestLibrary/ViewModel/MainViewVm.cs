using System.Collections.Generic;

namespace WpfTrayTestLibrary.ViewModel
{
    public class MainViewVm : ViewModelBase
    {
        private System.Windows.Media.ImageSource _icon;

        private bool _isRunning;

        private System.Collections.ObjectModel.ObservableCollection<KeyValuePair<string, string>> _statusFlags;

        public MainViewVm()
        {
        }


        public System.Windows.Media.ImageSource Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged("Icon");
            }
        }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                _isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }
    }
}
