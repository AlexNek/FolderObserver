using System.Windows;

using FolderObserver.ViewModel;

namespace FolderObserver.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowVm _viewModel;

        public MainWindow()
        {
            _viewModel = new MainWindowVm();
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            _viewModel.Exit();
        }
    }
}
