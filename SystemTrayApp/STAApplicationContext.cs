using System.Windows.Forms;

namespace SystemTrayApp
{
    public class STAApplicationContext : ApplicationContext
    {
        private ViewManager _viewManager;

        public STAApplicationContext()
        {
            _viewManager = new ViewManager();

        }

        // Called from the Dispose method of the base class
        protected override void Dispose(bool disposing)
        {
            _viewManager = null;
        }
    }
}
