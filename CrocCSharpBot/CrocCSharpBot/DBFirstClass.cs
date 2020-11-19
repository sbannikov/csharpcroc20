using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocCSharpBot
{
    /// <summary>
    /// База данных Database First
    /// </summary>
    public partial class DBFirst : IStorage
    {
        public IUser this[long id]
        {
            get
            {
                Users u = Users.Where(x => x.ID == id).FirstOrDefault();
                if (u == null)
                {
                    // Создание нового пользователя; всё, что мы знаем - его идентификатор
                    u = new Users()
                    {
                        ID = id
                    };
                    Users.Add(u);
                    SaveChanges();
                }
                return u;
            }
        }

        /// <summary>
        /// Список всех пользователей
        /// </summary>
        /// <returns></returns>
        public List<IUser> GetUsers()
        {
            return Users.ToList().Select(x => (IUser)x).ToList();        }

        /// <summary>
        /// Сохранение изменений
        /// </summary>
        /// <param name="user"></param>
        public void Save(IUser user)
        {
            SaveChanges();
        }
    }
}
