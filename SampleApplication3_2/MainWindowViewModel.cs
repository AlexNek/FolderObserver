using System.Windows.Input;

using GalaSoft.MvvmLight.Command;

namespace SampleApplication3_2
{
    public class MainWindowViewModel
    {
        private readonly IMessage _message;

        public MainWindowViewModel(IMessage message)
        {
            _message = message;
            CommandClickMe = new RelayCommand(ExecCommandClickMe);
        }

        private void ExecCommandClickMe()
        {
            _message?.Show("Hello World!");
        }

        public ICommand CommandClickMe { get; }
    }
}