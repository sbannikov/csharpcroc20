using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlPanel.ViewModel
{
    class MainWindowVM : ViewModelBase
    {
        ViewModelBase currentWindow = new SettingVM();

        public ViewModelBase CurrentWindow
        {
            get => currentWindow;
            set => SetValue(ref currentWindow, value);
        }
    }
}
