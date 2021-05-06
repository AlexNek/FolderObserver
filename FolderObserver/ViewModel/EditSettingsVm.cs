using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace FolderObserver.ViewModel
{
    internal class EditSettingsVm : ViewModelBase
    {
        private string _textPassword;

        public EditSettingsVm()
        {
            SaveCommand = new RelayCommand(OnSaveExec);
        }

        public void LoadData()
        {
            TextPassword = Properties.Settings.Default.Password;
        }

        private void OnSaveExec()
        {
            DialogResult = true;
            Properties.Settings.Default.Password = TextPassword;
            Properties.Settings.Default.Save();
            //CurrentWindowService.Close();
        }

        public bool? DialogResult { get; set; }

        public ICommand SaveCommand { get; }

        public string TextPassword
        {
            get
            {
                return _textPassword;
            }
            set
            {
                _textPassword = value;
                RaisePropertyChanged(nameof(TextPassword));
            }
        }
    }
}
