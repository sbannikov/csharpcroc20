using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocCSharpBot.Storage
{
    public abstract class Entity
    {
        /// <summary>
        /// Первичный ключ
        /// </summary>
        [Key()]
        [Column("ID")]
        public Guid RecordID { get; set; }

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public Entity()
        {
            RecordID = Guid.NewGuid();
        }
    }
}
