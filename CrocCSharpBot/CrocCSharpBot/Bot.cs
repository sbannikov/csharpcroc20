using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace CrocCSharpBot
{
    /// <summary>
    /// Основной модуль бота
    /// </summary>
    public class Bot
    {
        /// <summary>
        /// Клиент Telegram
        /// </summary>
        private TelegramBotClient client;

        /// <summary>
        /// Ведение журнала событий
        /// </summary>
        private NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Состояние ботаы
        /// </summary>
        private BotState state;

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public Bot()
        {
            // Создание клиента для Telegram
            string token = Properties.Settings.Default.Token;
            client = new TelegramBotClient(token);
            var user = client.GetMeAsync();
            string name = user.Result.Username;
            client.OnMessage += MessageProcessor;

            // Чтение сохраненного состояния из файла 
            state = BotState.Load(Properties.Settings.Default.FileName);
        }

        /// <summary>
        /// Обработка входящего сообщения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageProcessor(object sender, MessageEventArgs e)
        {
            try
            {
                log.Trace("|<- MessageProcessor");

                // Построение имени метода для вызова
                string method = $"{e.Message.Type}Processor";

                // Ищем метод по имени
                System.Reflection.MethodInfo info = GetType().GetMethod(method);
                if (info == null)
                {
                    log.Info(e.Message.Type);
                    client.SendTextMessageAsync(e.Message.Chat.Id, $"Я пока не понимаю такого: {e.Message.Type}");
                    return;
                }

                // Вызов метода по имени
                info.Invoke(this, new object[] { e.Message });
            }
            catch (Exception ex)
            {
                log.Warn(ex);
            }
            finally
            {
                log.Trace("|-> MessageProcessor");
            }
        }

        /// <summary>
        /// Обработка текста
        /// </summary>
        /// <param name="message"></param>
        public void TextProcessor(Telegram.Bot.Types.Message message)
        {
            if (message.Text.Substring(0, 1) == "/")
            {
                CommandProcessor(message);
            }
            else
            {
                client.SendTextMessageAsync(message.Chat.Id, $"Ты сказал мне: {message.Text}");
                log.Trace(message.Text);
            }
        }

        /// <summary>
        /// Обрабокта контакта
        /// </summary>
        /// <param name="message"></param>
        public void ContactProcessor(Telegram.Bot.Types.Message message)
        {
            if (message.Contact.UserId != message.Chat.Id)
            {
                client.SendTextMessageAsync(message.Chat.Id, $"Некорректный контакт");
                return;
            }
            string phone = message.Contact.PhoneNumber;
            log.Trace(phone);
            // Регистрация пользователя
            // (i) Использование инициализатора
            var user = new User()
            {
                ID = message.Contact.UserId,
                FirstName = message.Contact.FirstName,
                LastName = message.Contact.LastName,
                UserName = message.Chat.Username,
                PhoneNumber = phone
            };
            if (state.AddUser(user))
            {
                state.Save(Properties.Settings.Default.FileName);
                client.SendTextMessageAsync(message.Chat.Id, $"Твой телефон добавлен в базу: {phone}");
            }
            else
            {
                client.SendTextMessageAsync(message.Chat.Id, $"Твой телефон уже есть в базе: {phone}");
            }
        }

        /// <summary>
        /// Обработка команды
        /// </summary>
        /// <param name="message"></param>
        private void CommandProcessor(Telegram.Bot.Types.Message message)
        {
            try
            {
                log.Trace("|<- CommandProcessor");

                // Отрезаем первый символ (который должен быть '/')
                string command = message.Text.Substring(1).ToLower();

                // Построение имени метода для вызова
                string method = command.Substring(0, 1).ToUpper() + command.Substring(1) + "Command";

                // Ищем метод по имени
                System.Reflection.MethodInfo info = GetType().GetMethod(method);
                if (info == null)
                {
                    client.SendTextMessageAsync(message.Chat.Id, $"Я пока не понимаю команду {command}");
                    return;
                }

                // Вызов метода по имени
                info.Invoke(this, new object[] { message });
            }
            finally
            {
                log.Trace("|-> CommandProcessor");
            }
        }

        /// <summary>
        /// Список всех команд
        /// </summary>
        /// <param name="message"></param>
        public void HelpCommand(Telegram.Bot.Types.Message message)
        {
            string m = "Список возможных команд:\n";
            foreach (Command s in Enum.GetValues(typeof(Command)))
            {
                string cmd = s.ToString().ToLower();
                string descr = s.ToDescription();
                m += $"/{cmd} - {descr}\n";
            }
            client.SendTextMessageAsync(message.Chat.Id, m, replyMarkup: null);
        }

        /// <summary>
        /// Начало работы с ботом
        /// </summary>
        /// <param name="message"></param>
        [Description("Начало работы с ботом")]
        public void StartCommand(Telegram.Bot.Types.Message message)
        {
            client.SendTextMessageAsync(message.Chat.Id, $"Привет, {message.Chat.FirstName}, для начала прошу зарегистрироваться при помощи команды /register");
        }

        /// <summary>
        /// Регистрация пользователя в списке
        /// </summary>
        /// <param name="message"></param>
        public void RegisterCommand(Telegram.Bot.Types.Message message)
        {
            var button = new KeyboardButton("Поделись телефоном");
            button.RequestContact = true;
            var array = new KeyboardButton[] { button };
            var reply = new ReplyKeyboardMarkup(array, true, true);
            client.SendTextMessageAsync(message.Chat.Id, $"Привет, {message.Chat.FirstName}, скажи мне свой телефон", replyMarkup: reply);
        }

        /// <summary>
        /// Запуск приёма сообщений
        /// </summary>
        public void Start()
        {
            // Запуск приема сообщений
            client.StartReceiving();
        }

        /// <summary>
        /// Останов приёма сообщений
        /// </summary>
        public void Stop()
        {
            client.StopReceiving();
        }
    }
}
