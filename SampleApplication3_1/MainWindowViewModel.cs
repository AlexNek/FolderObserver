using System.Windows;
using System.Windows.Input;

using GalaSoft.MvvmLight.Command;

namespace SampleApplication3_1
{
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            CommandClickMe = new RelayCommand(ExecCommandClickMe);
        }

    private void ExecCommandClickMe()
    {
        MessageBoxShow("Hello World!");
    }

    public virtual void MessageBoxShow(string message)
    {
        MessageBox.Show(message);
    }

        public ICommand CommandClickMe { get; }
    }

    public class MainWindowViewModelV2
    {
        public MainWindowViewModelV2()
        {
            CommandClickMe = new RelayCommand(ExecCommandClickMe);
        }

    public virtual void ExecCommandClickMe()
    {
        MessageBox.Show("Hello World");
    }

        public ICommand CommandClickMe { get; }
    }

    
}
