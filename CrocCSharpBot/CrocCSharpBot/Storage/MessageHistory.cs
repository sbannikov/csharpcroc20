using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocCSharpBot.Storage
{
    /// <summary>
    /// История сообщений пользователя
    /// </summary>
    public class MessageHistory : Entity
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [Column("User_ID")]
        [Index("IX_USER", 1)]
        public Guid UserID { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Метка времени
        /// </summary>
        [Index("IX_USER", 2)]
        public DateTime TimeStamp { get; set; }
    }
}
