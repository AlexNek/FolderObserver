using System.Windows;

namespace SampleApplication3_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = new MainWindowViewModel(new WPFMessage());
        InitializeComponent();
    }
}
}
