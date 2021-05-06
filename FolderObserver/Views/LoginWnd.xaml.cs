using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using FolderObserver.ViewModel;

namespace FolderObserver.Views
{
    /// <summary>
    /// Interaction logic for EditSettings.xaml
    /// </summary>
    public partial class LoginWnd : Window
    {
        private readonly LoginWndVm _viewModel;

        public LoginWnd()
        {
            _viewModel = new LoginWndVm();
            DataContext = _viewModel;
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            DialogResult = _viewModel.DialogResult;
            base.OnClosing(e);
        }

        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            (sender as Button)?.Focus();
            _viewModel.SaveCommand.Execute(null);
            Close();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadData();
        }
        
    }
}
