using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CrocCSharpBot
{
    /// <summary>
    /// Управляющий сервис
    /// </summary>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Single)]
    public class ControlService : IContolService
    {
        /// <summary>
        /// Бот
        /// </summary>
        private Bot bot;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="bot"></param>
        public ControlService(Bot bot)
        {
            this.bot = bot;
        }

        public string Query()
        {
            return "Привет!";
        }

        public void StartTrace(string ip)
        {
            bot.StartTrace(ip);
        }

        public void StopTrace()
        {
            bot.StopTrace();
        }
    }
}
