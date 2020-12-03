using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlPanel.Model
{
    public class SettingsModel
    {
        public DateTime RefreshTimeSpan
        {
            get => DateTime.Parse(Properties.Settings.Default.RefreshTimeSpan);
            set
            {
                Properties.Settings.Default.RefreshTimeSpan = value.ToLongTimeString();
                Properties.Settings.Default.Save();
            }
        }
    }
}
