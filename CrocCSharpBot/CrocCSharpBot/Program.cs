using System;
using System.Linq;
using System.ServiceProcess;

namespace CrocCSharpBot
{
    /// <summary>
    /// Главный класс приложения
    /// </summary>
    class Program
    {
        /// <summary>
        /// Ведение журнала событий
        /// </summary>
        private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Точка входа в приложение
        /// </summary>
        /// <param name="args">Параметры командной строки</param>
        static void Main(string[] args)
        {
            try
            {
                // Первый параметр командной строки
                string arg1 = args.Count() > 0 ? args[0] : string.Empty;
                // Приведение к строчным буквам
                arg1 = arg1.ToLower();

                switch (arg1)
                {
                    case "console":
                        Bot bot;
                        bot = new Bot();
                        bot.Start();
                        log.Info("Запуск бота в консольном режиме");
                        break;

                    case "":
                        var svc = new BotService();
                        ServiceBase.Run(svc);
                        break;

                    default: // другой параметр
                        Console.WriteLine($"Неправильный параметр: {arg1}");
                        // [!] дописать вывод справки
                        break;
                }

            }
            catch (Exception ex)
            {
                // Отображение сообщения, включая все вложенные исключения
                do
                {
                    log.Fatal(ex);
                    ex = ex.InnerException;
                }
                while (ex != null);
            }
            finally
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine("Нажмите Enter для завершения");
                    Console.ReadLine();
                }
            }
        }
    }
}
