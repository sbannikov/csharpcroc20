using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CrocCSharpBot
{
    /// <summary>
    /// Интерфейс хранилища данных
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Индексатор по идентификатору пользователя
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Всегда возвращает пользователя, создает его при необходимости</returns>
        IUser this[long id] { get; }

        /// <summary>
        /// Сохранение пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        void Save(IUser user);     

        /// <summary>
        /// Список всех пользователей
        /// </summary>
        /// <returns></returns>
        List<IUser> GetUsers();
    }
}
