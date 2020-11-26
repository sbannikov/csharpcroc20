using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Google.Cloud.Dialogflow.V2;
using Google.Apis.Auth.OAuth2;

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
        private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

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
        /// Клиент DialogFlow by Google
        /// </summary>
        private SessionsClient dialog;

        /// <summary>
        /// Код проекта DialogFlow
        /// </summary>
        private string project;

        /// <summary>
        /// Диагностика для базы данных
        /// </summary>
        /// <param name="s"></param>
        private static void logging(string s)
        {
            log.Trace(s);
        }

        /// <summary>
        /// Хранилище данных
        /// </summary>
        /// <returns></returns>
        private static IStorage Storage()
        {
            IStorage storage;
            StorageType st;
            if (!Enum.TryParse(Properties.Settings.Default.Storage, out st))
            {
                throw new Exception($"Некорректная конфигурация хранилища: {Properties.Settings.Default.Storage}");
            }
            switch (st)
            {
                case StorageType.FileStorage:
                    storage = BotState.Load(Properties.Settings.Default.FileName);
                    // log.Info("Используется файловое хранилище");
                    break;

                case StorageType.DatabaseStorage:
                    storage = new Database();
                    // log.Info("Используется база данных Microsoft SQL Server в режиме ADO.NET");
                    break;

                case StorageType.DatabaseFirstStorage:
                    storage = new DBFirst();
                    // log.Info("Используется база данных Microsoft SQL Server в режиме Entity Framework Database First");
                    break;

                case StorageType.CodeFirstStorage:
                    var db = new DB();
                    // Включение протоколирования SQL-запросов
                    // (будет очень много текста)
                    // * db.Database.Log = logging;
                    storage = db;
                    // log.Info("Используется база данных Microsoft SQL Server в режиме Entity Framework Code First");
                    break;

                default:
                    throw new Exception($"Некорректная конфигурация хранилища: {st}");
            }

            return storage;
        }

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

            // Инициализаиця хранилища
            state = Storage();
            // Таймер
            timer = new System.Timers.Timer(Properties.Settings.Default.TimerTickInMilliseconds);
            timer.Elapsed += TimerTick;

            // Сервис
            control = new ControlService(this);
            host = new ServiceHost(control);

            // Инициализация клиента DialogFlow

            // Имя исполняемого в текущий момент файла (.EXE)
            string file = System.Reflection.Assembly.GetExecutingAssembly().Location;
            // Каталог размещения исполняемого файла
            string path = System.IO.Path.GetDirectoryName(file);
            // Полное имя файла конфигурации
            string json = $@"{path}\dialogflow.json";
            // Задание переменной среды операционной системы
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", json);
            // Создание клиента DialogFlow
            dialog = SessionsClient.Create();
            // Чтение конфигурации соединения - нужен только идентификатор проекта
            using (var stream = new System.IO.FileStream(json,System.IO.FileMode.Open))
            {
                var credentials = ServiceAccountCredential.FromServiceAccountData(stream);
                project = credentials.ProjectId;
            }
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
                var db = Storage();
                timer.Stop();
                // Текущая метка времени
                DateTime now = DateTime.Now;
                // Проверка на неактивных пользователей
                foreach (IUser user in db.GetUsers())
                {
                    double delay = (now - user.TimeStamp).TotalSeconds;
                    if ((delay > Properties.Settings.Default.TimeOutInSeconds) &&
                        (user.State != (int)UserState.None))
                    {
                        user.State = (int)UserState.None;
                        // Сохранить состояние бота
                        db.Save(user);
                        client.SendTextMessageAsync(user.ID, $"Я скучаю, ты про меня забыл");
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
                IUser user = state[e.Message.Chat.Id];
                user.TimeStamp = DateTime.Now;
                // Сохранить состояние бота
                state.Save(user);

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
                log.Trace(message.Text);
                // Создание запроса в DialogFlow
                var request = new DetectIntentRequest();
                // Задание уникального (для пользователя) сеанса
                request.SessionAsSessionName = new SessionName(project, message.Chat.Id.ToString());
                // Заполнение тела запроса
                request.QueryInput = new QueryInput()
                {
                    Text = new TextInput()
                    {
                        LanguageCode = "ru",
                        Text = message.Text
                    }
                };
                // Отправим текст в DialogFlows
                DetectIntentResponse response = dialog.DetectIntent(request);
                // Обработка ответа
                QueryResult qr = response.QueryResult;
                string action = qr.Action;
                string answer = qr.FulfillmentText;

                client.SendTextMessageAsync(message.Chat.Id, $"{action}: {answer}");
                log.Trace($"{action}: {answer}");

                foreach (var p in qr.Parameters.Fields)
                {
                    client.SendTextMessageAsync(message.Chat.Id, $"{p.Key} = {p.Value}");
                    log.Trace($"{p.Key} = {p.Value}");
                }
            }
        }

        /// <summary>
        /// Обрабокта контакта
        /// </summary>
        /// <param name="message"></param>
        public void ContactProcessor(Telegram.Bot.Types.Message message)
        {
            IUser user = state[message.Chat.Id];

            // Проверка состояния пользователя
            if ((UserState)user.State != UserState.Register)
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
            user.State = (int)UserState.None;
            state.Save(user);
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
            IUser user = state[message.Chat.Id];

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
            user.State = (int)UserState.Register;
            // Сохранить состояние бота
            state.Save(user);
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
