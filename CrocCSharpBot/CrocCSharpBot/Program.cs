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
            Console.WriteLine("Запуск бота в консольном режиме. Нажмите Enter для завершения");
            Bot bot;
            bot = new Bot();
            bot.Run();
            Console.ReadLine();
        }
    }
}
