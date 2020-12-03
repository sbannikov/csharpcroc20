using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlPanel.Model
{
    public class EventModel
    {
        /// <summary>
        /// Время возникновение события
        /// </summary>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }

        public EventModel(string message, DateTime eventTime)
        {
            this.Message = message;
            this.EventTime = eventTime;
        }

        public EventModel(string message) : this(message, DateTime.Now) { }

        public EventModel() : this("test message") { }
    }
}
