using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace CrocCSharpBot
{
    /// <summary>
    /// Состояние бота
    /// </summary>
    [XmlRoot(ElementName = "State", Namespace = "http://www.orioner.ru")]
    public class BotState : IStorage
    {
        /// <summary>
        /// Имя файла
        /// </summary>
        private string fileName;

        /// <summary>
        /// Массив пользователей
        /// </summary>
        [XmlElement(ElementName = "User")]
        public IUser[] Users;

        /// <summary>
        /// Индексатор по идентификатору пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Всегда возвращает пользователя, создает его при необходимости</returns>
        public IUser this[long id]
        {
            get
            {
                IUser user = Users.Where(a => a.ID == id).FirstOrDefault();
                if (user == null)
                {
                    // Создание нового пользователя; всё, что мы знаем - его идентификатор
                    user = new User()
                    {
                        ID = id
                    };
                    Array.Resize(ref Users, Users.Length + 1);
                    Users[Users.Length - 1] = user;
                }
                return user;
            }
        }

        /// <summary>
        /// Загрузка объекта из XML-файла
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static BotState Load(string name)
        {
            BotState result;
            try
            {
                // Объект, выполняющий сериализацию
                XmlSerializer s = new XmlSerializer(typeof(BotState));
                // Файл для чтения данных
                using (XmlReader r = XmlReader.Create(name))
                {
                    result = (BotState)s.Deserialize(r);
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                // Файл не найден - новое пустое состояние
                result = new BotState();
            }
            // Сохранить имя файла
            result.fileName = name;
            return result;
        }

        /// <summary>
        /// Список пользователей
        /// </summary>
        /// <returns></returns>
        public List<IUser> GetUsers()
        {
            return Users.ToList();
        }

        /// <summary>
        /// Сохранение пользователя 
        /// </summary>
        /// <param name="user"></param>
        public void Save(IUser user)
        {
            // Объект, выполняющий сериализацию
            XmlSerializer s = new XmlSerializer(typeof(BotState));
            // Настройка формирования XML-файла
            var settings = new XmlWriterSettings()
            {
                Indent = true // Человекочитаемый XML
            };
            // Файл для записи данных
            using (XmlWriter w = XmlWriter.Create(fileName, settings))
            {
                // Сериализация!
                s.Serialize(w, this);
            }
        }
    }
}
