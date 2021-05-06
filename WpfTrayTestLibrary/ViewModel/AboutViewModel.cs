namespace WpfTrayTestLibrary.ViewModel
{
    public class AboutViewModel : ViewModelBase
    {
        private System.Collections.ObjectModel.ObservableCollection<ComponentVersionInfo> _componentVersions;

        private System.Windows.Media.ImageSource _icon;

        public AboutViewModel()
        {
            _componentVersions = new System.Collections.ObjectModel.ObservableCollection<ComponentVersionInfo>();
        }

        public void AddVersionInfo(string name, string version)
        {
            foreach (var item in ComponentVersions)
            {
                if (item.Name == name)
                {
                    item.Version = version;
                    OnPropertyChanged("ComponentVersions");
                    return;
                }
            }

            ComponentVersionInfo info = new ComponentVersionInfo();
            info.Name = name;
            info.Version = version;
            ComponentVersions.Add(info);
        }

        public System.Collections.ObjectModel.ObservableCollection<ComponentVersionInfo> ComponentVersions
        {
            get
            {
                return _componentVersions;
            }
            set
            {
                _componentVersions = value;
                OnPropertyChanged("ComponentVersions");
            }
        }

        public System.Windows.Media.ImageSource Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged("Icon");
            }
        }
    }
}
