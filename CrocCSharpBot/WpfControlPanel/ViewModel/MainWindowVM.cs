using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WpfControlPanel.Command;

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

        public MainWindowVM()
        {
            OpenEventsCommand = new RelayCommand(
                obj => CurrentWindow = new EventsVM(
                    new SettingVM().RefreshTimeSpan
                    )
                );
            OpenCrocCommand = new RelayCommand(obj => CurrentWindow = new CrocVM());
            OpenSettingsCommand = new RelayCommand(obj => CurrentWindow = new SettingVM());
        }

        public ICommand OpenEventsCommand { get; set; }
        public ICommand OpenCrocCommand { get; set; }
        public ICommand OpenSettingsCommand { get; set; }
    }
}
