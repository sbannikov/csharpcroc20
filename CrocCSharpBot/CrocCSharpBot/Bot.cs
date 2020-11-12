using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
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
        /// Состояние бота
        /// </summary>
        private IStorage state;

        /// <summary>
        /// Таймер
        /// </summary>
        private System.Timers.Timer timer;

        /// <summary>
        /// Управляющий сервис
        /// </summary>
        private ControlService control;

        /// <summary>
        /// Домик для сервиса
        /// </summary>
        private ServiceHost host;

        /// <summary>
        /// Для передачи диагностики
        /// </summary>
        private System.Net.Sockets.UdpClient udp;

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

            // Таймер
            timer = new System.Timers.Timer(Properties.Settings.Default.TimerTickInMilliseconds);
            timer.Elapsed += TimerTick;

            // Сервис
            control = new ControlService(this);
            host = new ServiceHost(control);
        }

        /// <summary>
        /// Событие таймера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                timer.Stop();
                // Текущая метка времени
                DateTime now = DateTime.Now;
                // Проверка на неактивных пользователей
                foreach (User user in state.GetUsers())
                {
                    double delay = (now - user.TimeStamp).TotalSeconds;
                    if ((delay > Properties.Settings.Default.TimeOutInSeconds) &&
                        (user.State != UserState.None))
                    {
                        user.State = UserState.None;
                        client.SendTextMessageAsync(user.ID, $"Я скучаю, ты про меня забыл");
                        // Сохранить состояние бота
                        state.Save(Properties.Settings.Default.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Warn(ex);
            }
            finally
            {
                timer.Start();
            }
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

                // Трассировка для консоли управления
                if (udp != null)
                {
                    // Отправить пакет по UDP
                    string udps = e.Message.Text;
                    byte[] data = Encoding.UTF8.GetBytes(udps);
                    int n = udp.Send(data, data.Length);
                }

                // Фиксируем факт взаимодействия с пользователем
                User user = state[e.Message.Chat.Id];
                user.TimeStamp = DateTime.Now;
                // Сохранить состояние бота
                state.Save(Properties.Settings.Default.FileName);

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
            User user = state[message.Chat.Id];

            // Проверка состояния пользователя
            if (user.State != UserState.Register)
            {
                client.SendTextMessageAsync(message.Chat.Id, $"Мне сейчас это не нужно");
                return;
            }

            // Проверка на подмену контакта
            if (message.Contact.UserId != message.Chat.Id)
            {
                client.SendTextMessageAsync(message.Chat.Id, $"Некорректный контакт");
                return;
            }
            string phone = message.Contact.PhoneNumber;
            log.Trace(phone);
            // Регистрация пользователя
            user.FirstName = message.Contact.FirstName;
            user.LastName = message.Contact.LastName;
            user.UserName = message.Chat.Username;
            user.PhoneNumber = phone;

            // Возврат к базовому состоянию пользователя
            user.State = UserState.None;

            state.Save(Properties.Settings.Default.FileName);
            client.SendTextMessageAsync(message.Chat.Id, $"Твой телефон добавлен в базу: {phone}");
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
            User user = state[message.Chat.Id];

            // Проверка на наличие номера телефона
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                client.SendTextMessageAsync(message.Chat.Id, $"{message.Chat.FirstName}, ты уже зарегистрирован", replyMarkup: null);
                return;
            }

            var button = new KeyboardButton("Поделись телефоном");
            button.RequestContact = true;
            var array = new KeyboardButton[] { button };
            var reply = new ReplyKeyboardMarkup(array, true, true);
            client.SendTextMessageAsync(message.Chat.Id, $"Привет, {message.Chat.FirstName}, скажи мне свой телефон", replyMarkup: reply);
            // Задать состояние пользователя - ждем регистрационных данных
            user.State = UserState.Register;
            // Сохранить состояние бота
            state.Save(Properties.Settings.Default.FileName);
        }

        /// <summary>
        /// Запуск бота
        /// </summary>
        public void Start()
        {
            // Запуск приема сообщений
            client.StartReceiving();
            // Запуск таймера
            timer.Start();
            // Открытие хоста сервиса
            host.Open();
        }

        /// <summary>
        /// Останов бота
        /// </summary>
        public void Stop()
        {
            // Закрытие хоста сервиса
            host.Close();
            // Останов приёма сообщений
            client.StopReceiving();
            // Останов таймера
            timer.Stop();
        }

        public void StartTrace(string ip)
        {
            udp = new System.Net.Sockets.UdpClient(ip, 9999);
        }

        public void StopTrace()
        {
            udp = null;
        }
    }
}
