using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlPanel.Model;
using System.Windows.Threading;
using System.Timers;

namespace WpfControlPanel.ViewModel
{
    public class EventsVM : ViewModelBase
    {
        DateTime RefreshTime { get; set; }

        public ObservableCollection<EventModel> Events { get; set; }
            = new ObservableCollection<EventModel>();

        public EventsVM(DateTime refreshTime)
        {
            this.RefreshTime = refreshTime;
            Timer = new Timer(RefreshTime.TimeOfDay.TotalMilliseconds);
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(
        (Action)(() => { Events.Add(new EventModel("My event")); }));
        }

        Timer Timer { get; set; }
    }
}
