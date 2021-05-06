using System;
using System.Reflection;
using System.Windows.Forms;

using WpfTrayTestLibrary.View;
using WpfTrayTestLibrary.ViewModel;

namespace SystemTrayApp
{
    public class ViewManager
    {
        private readonly AboutViewModel _aboutViewModel;

        private readonly System.ComponentModel.IContainer _components;

        // This allows code to be run on a GUI thread
        private readonly System.Windows.Window _hiddenWindow;

        private readonly MainViewVm _mainViewVm;

        // The Windows system tray class
        private readonly NotifyIcon _notifyIcon;

        private AboutView _aboutView;

        private ToolStripMenuItem _exitMenuItem;

        private MainView _mainView;

        //private ToolStripMenuItem _startDeviceMenuItem;

        //private ToolStripMenuItem _stopDeviceMenuItem;

        public ViewManager()
        {
            _components = new System.ComponentModel.Container();
            _notifyIcon = new NotifyIcon(_components)
                              {
                                  ContextMenuStrip = new ContextMenuStrip(),
                                  Icon = Properties.Resources.AppIcon,
                                  Text = "Folder Observer Tray App",
                                  Visible = true,
                              };

            _notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            _notifyIcon.DoubleClick += NotifyIconDoubleClick;
            _notifyIcon.MouseUp += NotifyIconMouseUp;

            _aboutViewModel = new AboutViewModel();
            _mainViewVm = new MainViewVm();

            _mainViewVm.Icon = AppIcon;
            _aboutViewModel.Icon = _mainViewVm.Icon;

            _hiddenWindow = new System.Windows.Window();
            _hiddenWindow.Hide();
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;

            if (_notifyIcon.ContextMenuStrip.Items.Count == 0)
            {
                
                _notifyIcon.ContextMenuStrip.Items.Add(
                    ToolStripMenuItemWithHandler("Main &Form", "Shows the main form", showStatusItem_Click));
                _notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("&About", "Shows the About dialog", showHelpItem_Click));
                //_notifyIcon.ContextMenuStrip.Items.Add(
                //    ToolStripMenuItemWithHandler("Code Project &Web Site", "Navigates to the Code Project Web Site", ShowWebSiteClick));
                //_notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
                _exitMenuItem = ToolStripMenuItemWithHandler("&Exit", "Exits Folder observer App", ExitItemClick);
                _notifyIcon.ContextMenuStrip.Items.Add(_exitMenuItem);
            }

            SetMenuItems();
        }

        private void ExitItemClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void NotifyIconDoubleClick(object sender, EventArgs e)
        {
            ShowAboutView();
        }

        private void NotifyIconMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(_notifyIcon, null);
            }
        }

        private void SetMenuItems()
        {
        }

        private void ShowAboutView()
        {
            if (_aboutView == null)
            {
                _aboutView = new AboutView();
                _aboutView.DataContext = _aboutViewModel;
                _aboutView.Closing += (arg_1, arg_2) => _aboutView = null;
                _aboutView.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

                _aboutView.Show();
            }
            else
            {
                _aboutView.Activate();
            }

            _aboutView.Icon = AppIcon;

            _aboutViewModel.AddVersionInfo("Version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            //_aboutViewModel.AddVersionInfo("Serial Number", "142573462354");
        }

        private void showHelpItem_Click(object sender, EventArgs e)
        {
            ShowAboutView();
        }

        private void showStatusItem_Click(object sender, EventArgs e)
        {
            ShowStatusView();
        }

        private void ShowStatusView()
        {
            if (_mainView == null)
            {
                _mainView = new MainView();
                _mainView.DataContext = _mainViewVm;

                _mainView.Closing += (arg1, arg2) => _mainView = null;
                _mainView.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                _mainView.Show();
                UpdateStatusView();
            }
            else
            {
                _mainView.Activate();
            }

            _mainView.Icon = AppIcon;
        }

        private void StartStopReaderItemClick(object sender, EventArgs e)
        {
        }

        private ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, string tooltipText, EventHandler eventHandler)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null)
            {
                item.Click += eventHandler;
            }

            item.ToolTipText = tooltipText;
            return item;
        }

        private void UpdateStatusView()
        {
            if (_mainViewVm != null)
            {
                //List<KeyValuePair<string, bool>> flags = _deviceManager.StatusFlags;
                //List<KeyValuePair<string, string>> statusItems =
                //    flags.Select(n => new KeyValuePair<string, string>(n.Key, n.Value.ToString())).ToList();
                //statusItems.Insert(0, new KeyValuePair<string, string>("Device", _deviceManager.DeviceName));
                //statusItems.Insert(1, new KeyValuePair<string, string>("Status", _deviceManager.Status.ToString()));
                //_mainViewVm.SetStatusFlags(statusItems);
            }
        }

        private System.Windows.Media.ImageSource AppIcon
        {
            get
            {
                System.Drawing.Icon icon = Properties.Resources.AppIcon;
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    System.Windows.Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
        }
    }
}
