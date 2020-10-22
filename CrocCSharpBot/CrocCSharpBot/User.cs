using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CrocCSharpBot
{
    /// <summary>
    /// Пользователь бота
    /// </summary>
    public class User
    {
        /// <summary>
        /// Идентификатор пользователя в Telegram
        /// </summary>
        [XmlAttribute()]
        public long ID;
        /// <summary>
        /// Имя
        /// </summary>
        [XmlElement(ElementName = "Name")]
        public string FirstName;
        /// <summary>
        /// Фамилия
        /// </summary>
        [XmlElement(ElementName = "Family")]
        public string LastName;
        /// <summary>
        /// Имя пользователя (nickname)
        /// </summary>
        public string UserName;
        /// <summary>
        /// Телефон пользователя
        /// </summary>
        public string PhoneNumber;
        /// <summary>
        /// Описание пользователя
        /// </summary>
        [XmlText()]
        public string Description;
    }
}
