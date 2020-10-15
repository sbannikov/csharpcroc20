using System;

namespace CrocCSharpBot
{
    /// <summary>
    /// Главный класс приложения
    /// </summary>
    class Program
    {
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
                Console.WriteLine("Запуск бота в консольном режиме");
            }
            catch (Exception ex)
            {
                // Отображение сообщения, включая все вложенные исключения
                do
                {
                    Console.WriteLine(ex.Message);
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
