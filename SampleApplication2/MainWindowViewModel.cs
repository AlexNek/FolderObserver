using System.Windows;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace SampleApplication2
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            CommandClickMe = new RelayCommand(ExecCommandClickMe);
        }

        private void ExecCommandClickMe()
        {
            MessageBox.Show("Hello world");
        }

        public ICommand CommandClickMe { get; }
    }
}