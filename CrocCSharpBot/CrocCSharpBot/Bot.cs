using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
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
        private void MessageProcessor(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            try
            {
                log.Trace("|<- MessageProcessor");
                switch (e.Message.Type)
                {
                    case Telegram.Bot.Types.Enums.MessageType.Contact: // телефон
                        if (e.Message.Contact.UserId != e.Message.Chat.Id)
                        {
                            client.SendTextMessageAsync(e.Message.Chat.Id, $"Некорректный контакт");
                            return;
                        }
                        string phone = e.Message.Contact.PhoneNumber;
                        client.SendTextMessageAsync(e.Message.Chat.Id, $"Твой телефон: {phone}");
                        log.Trace(phone);
                        // Регистрация пользователя
                        // (i) Использование инициализатора
                        var user = new User()
                        {
                            ID = e.Message.Contact.UserId,
                            FirstName = e.Message.Contact.FirstName,
                            LastName = e.Message.Contact.LastName,
                            UserName = e.Message.Chat.Username,
                            PhoneNumber = phone
                        };
                        state.AddUser(user);
                        state.Save(Properties.Settings.Default.FileName);
                        break;

                    case Telegram.Bot.Types.Enums.MessageType.Text: // текстовое сообщение
                        if (e.Message.Text.Substring(0, 1) == "/")
                        {
                            CommandProcessor(e.Message);
                        }
                        else
                        {
                            client.SendTextMessageAsync(e.Message.Chat.Id, $"Ты сказал мне: {e.Message.Text}");
                            log.Trace(e.Message.Text);
                        }
                        break;

                    default:
                        client.SendTextMessageAsync(e.Message.Chat.Id, $"Ты прислал мне {e.Message.Type}, но я это пока не понимаю");
                        log.Info(e.Message.Type);
                        break;
                }
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
        /// Обработка команды
        /// </summary>
        /// <param name="message"></param>
        private void CommandProcessor(Telegram.Bot.Types.Message message)
        {
            // Отрезаем первый символ (который должен быть '/')
            string command = message.Text.Substring(1).ToLower();

            switch (command)
            {
                case "start":
                    var button = new KeyboardButton("Поделись телефоном");
                    button.RequestContact = true;
                    var array = new KeyboardButton[] { button };
                    var reply = new ReplyKeyboardMarkup(array, true, true);
                    client.SendTextMessageAsync(message.Chat.Id, $"Привет, {message.Chat.FirstName}, скажи мне свой телефон", replyMarkup: reply);
                    break;

                default:
                    client.SendTextMessageAsync(message.Chat.Id, $"Я пока не понимаю команду {command}");
                    break;
            }
        }

        /// <summary>
        /// Запуск бота
        /// </summary>
        public void Run()
        {
            // Запуск приема сообщений
            client.StartReceiving();
        }
    }
}
