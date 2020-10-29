using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CrocCSharpBot
{
    /// <summary>
    /// Служба операционной системы
    /// </summary>
    partial class BotService : ServiceBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public BotService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Событие запуска сервиса
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
        }

        /// <summary>
        /// Событие останова сервиса
        /// </summary>
        protected override void OnStop()
        {
        }

        /// <summary>
        /// Событие приостанова сервиса
        /// </summary>
        protected override void OnPause()
        {
        }

        /// <summary>
        /// Событие возобновления сервиса
        /// </summary>
        protected override void OnContinue()
        {
        }
    }
}
