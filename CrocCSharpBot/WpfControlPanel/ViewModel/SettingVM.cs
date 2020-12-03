using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlPanel.Model;

namespace WpfControlPanel.ViewModel
{
    public class SettingVM : ViewModelBase
    {
        private SettingsModel Model { get; } = new SettingsModel();

        public DateTime RefreshTimeSpan
        {
            get => Model.RefreshTimeSpan;
            set
            {
                Model.RefreshTimeSpan = value;
                NotifyPropertyChanged();
            }
        }
    }
}
