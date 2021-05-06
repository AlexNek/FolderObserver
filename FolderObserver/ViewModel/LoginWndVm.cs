using System;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace FolderObserver.ViewModel
{
    internal class LoginWndVm : ViewModelBase
    {
        private string _inputPassword;

        public LoginWndVm()
        {
            SaveCommand = new RelayCommand(OnSaveExec);
        }

        public static bool IsPasswordDefined()
        {
            bool ret = String.IsNullOrEmpty(Properties.Settings.Default.Password);
            return !ret;
        }

        public void LoadData()
        {
            StoredPassword = Properties.Settings.Default.Password;
        }

        private void OnSaveExec()
        {
            DialogResult = InputPassword == StoredPassword;
        }

        public bool? DialogResult { get; set; }

        public string InputPassword
        {
            get
            {
                return _inputPassword;
            }
            set
            {
                _inputPassword = value;
                RaisePropertyChanged(nameof(InputPassword));
            }
        }

        public ICommand SaveCommand { get; }

        public string StoredPassword { get; set; }
    }
}
