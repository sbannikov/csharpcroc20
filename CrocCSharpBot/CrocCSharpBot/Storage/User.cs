using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrocCSharpBot.Storage
{
    /// <summary>
    /// Пользователь бота
    /// </summary>
    public class User : Entity, IUser
    {
        /// <summary>
        /// Идентификатор пользователя в Telegram
        /// </summary>
        [XmlAttribute()]
        [Column("TelegramID")]
        public long ID { get; set; }
        
        /// <summary>
        /// Имя
        /// </summary>
        [XmlElement(ElementName = "Name")]
        [MaxLength(255)]
        public string FirstName { get; set; }
        
        /// <summary>
        /// Фамилия
        /// </summary>
        [XmlElement(ElementName = "Family")]
        [MaxLength(255)]
        public string LastName { get; set; }
        
        /// <summary>
        /// Имя пользователя (nickname)
        /// </summary>
        [MaxLength(255)]
        public string UserName { get; set; }
        
        /// <summary>
        /// Телефон пользователя
        /// </summary>
        [MaxLength(63)]
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// Описание пользователя
        /// </summary>
        [XmlText()]
        [MaxLength(255)]
        public string Description { get; set; }
       
        /// <summary>
        /// Электрическая почта
        /// </summary>
        [MaxLength(255)]
        public string EMail { get; set; }

        /// <summary>
        /// История сообщений пользователя
        /// </summary>
        public virtual HashSet<MessageHistory> Messages { get; set; }
       
        /// <summary>
        /// Состояние пользователя
        /// </summary>
        [NotMapped()]
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
