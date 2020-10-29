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
        /// Ведение журнала событий
        /// </summary>
        private NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Собственно бот
        /// </summary>
        private Bot bot ;
        
        /// <summary>
        /// Конструктор
        /// </summary>
        public BotService()
        {
            InitializeComponent();
            // Создание объектов
            bot = new Bot();
        }

        /// <summary>
        /// Событие запуска сервиса
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            try
            {
                log.Trace("|<- OnStart");
                bot.Start();
                log.Info("Сервис запущен");
            }
            catch (Exception ex)
            {
                log.Fatal(ex);
            }
        }

        /// <summary>
        /// Событие останова сервиса
        /// </summary>
        protected override void OnStop()
        {
            try
            {
                log.Trace("|<- OnStop");
                bot.Stop();
                log.Info("Сервис остановлен");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// Событие приостанова сервиса
        /// </summary>
        protected override void OnPause()
        {
            try
            {
                log.Trace("|<- OnPause");
                bot.Stop();
                log.Info("Сервис приостановлен");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// Событие возобновления сервиса
        /// </summary>
        protected override void OnContinue()
        {
            try
            {
                log.Trace("|<- OnContinue");
                bot.Start();
                log.Info("Сервис возобновлён");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
