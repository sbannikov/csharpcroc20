using System;
using System.Collections.Generic;
using System.Linq;
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
    public class BotState
    {
        /// <summary>
        /// Массив пользователей
        /// </summary>
        [XmlElement(ElementName = "User")]
        public User[] Users;

        /// <summary>
        /// Добавление пользователя в массив
        /// </summary>
        /// <param name="user"></param>
        public void AddUser(User user)
        {
            if (Users == null)
            {
                Users = new User[1] { user };
            }
            else
            {
                Array.Resize(ref Users, Users.Length + 1);
                Users[Users.Length - 1] = user;
            }
        }

        /// <summary>
        /// Загрузка объекта из XML-файла
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static BotState Load(string name)
        {
            // Объект, выполняющий сериализацию
            XmlSerializer s = new XmlSerializer(typeof(BotState));
            // Файл для чтения данных
            using (XmlReader r = XmlReader.Create(name))
            {
                return (BotState)s.Deserialize(r);
            }
        }

        /// <summary>
        /// Сохранение состояния в виде XML-файла
        /// </summary>
        /// <param name="name">Имя файла</param>
        public void Save(string name)
        {
            // Объект, выполняющий сериализацию
            XmlSerializer s = new XmlSerializer(typeof(BotState));
            // Настройка формирования XML-файла
            var settings = new XmlWriterSettings()
            {
                Indent = true // Человекочитаемый XML
            };
            // Файл для записи данных
            using (XmlWriter w = XmlWriter.Create(name, settings))
            {
                // Сериализация!
                s.Serialize(w, this);
            }
        }
    }
}
