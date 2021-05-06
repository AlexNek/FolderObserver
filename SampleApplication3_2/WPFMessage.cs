using System.Windows;

namespace SampleApplication3_2
{
    public class WPFMessage : IMessage
    {
        public void Show(string message)
        {
            MessageBox.Show(message);
        }
    }
}