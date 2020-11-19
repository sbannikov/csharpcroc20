using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CrocCSharpBot
{
    /// <summary>
    /// База данных Code First
    /// </summary>
    public class DB : DbContext, IStorage
    {
        /// <summary>
        /// Конструктор базы данных
        /// </summary>
        public DB() : base("CodeFirst")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<DB, Migrations.Configuration>());
        }

        /// <summary>
        /// Список пользователей
        /// </summary>
        public virtual DbSet<User> Users { get; set; }

        /// <summary>
        /// Индексатор по идентификатору пользователя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IUser this[long id]
        {
            get
            {
                User u = Users.Where(x => x.ID == id).FirstOrDefault();
                if (u == null)
                {
                    // Создание нового пользователя; всё, что мы знаем - его идентификатор
                    u = new User()
                    {
                        ID = id,
                        TimeStamp = DateTime.Now
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
            return Users.ToList().Select(x => (IUser)x).ToList();
        }

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
