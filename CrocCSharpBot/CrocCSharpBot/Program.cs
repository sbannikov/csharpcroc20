using System;

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
                Bot bot;
                bot = new Bot();
                bot.Run();
                log.Info("Запуск бота в консольном режиме");
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
                Console.WriteLine("Нажмите Enter для завершения");
                Console.ReadLine();
            }
        }
    }
}
