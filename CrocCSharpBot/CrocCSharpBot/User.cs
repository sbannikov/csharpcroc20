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
    public class User : IUser
    {
        /// <summary>
        /// Идентификатор пользователя в Telegram
        /// </summary>
        [XmlAttribute()]
        public long ID { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        [XmlElement(ElementName = "Name")]
        public string FirstName { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        [XmlElement(ElementName = "Family")]
        public string LastName { get; set; }
        /// <summary>
        /// Имя пользователя (nickname)
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Телефон пользователя
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Описание пользователя
        /// </summary>
        [XmlText()]
        public string Description { get; set; }
        /// <summary>
        /// Состояние пользователя
        /// </summary>
        public UserState UState
        {
            get
            {
                if (!Enum.IsDefined(typeof(UserState), State))
                {
                    throw new Exception("Некорректное значение State");
                }
                return (UserState)State;
            }
            set
            {
                State = (int)value;
            }
        }
        /// <summary>
        /// Метка времени последнего сообщения от пользователя
        /// </summary>
        public DateTime TimeStamp { get; set; }
        /// <summary>
        /// Состояние пользователя в виде целого числа для хранения в БД
        /// </summary>
        public int State { get; set; }
    }
}
