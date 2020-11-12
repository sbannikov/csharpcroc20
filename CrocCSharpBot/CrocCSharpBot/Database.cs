using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace CrocCSharpBot
{
    /// <summary>
    /// Работа с базой данных через ADO.NET
    /// </summary>
    public class Database : IStorage
    {
        /// <summary>
        /// Соединение с БД
        /// </summary>
        private SqlConnection conn;

        /// <summary>
        /// Конструктор базы данных
        /// </summary>
        public Database()
        {
            conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=CSHARP20;Integrated Security=SSPI;App=CrocCSharpBot;";
            conn.Open();
        }

        public User this[long id]
        {
            get
            {
                User u;
                SqlCommand c1 = conn.CreateCommand();
                c1.CommandText = "SELECT [ID], [FirstName], [LastName], [UserName], [PhoneNumber], [Description], [State], [TimeStamp] FROM [Users] WHERE ID = " + id;
                // Поиск пользователя по идентификатору в БД
                var reader = c1.ExecuteReader();

                // Проверка на наличие пользователя в БД
                if (reader.Read())
                {
                    // Формирование объекта
                    u = new User()
                    {
                        ID = reader.GetInt64(reader.GetOrdinal("ID")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        UserName = reader.GetString(reader.GetOrdinal("UserName")),
                        PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        State = (UserState)reader.GetInt32(reader.GetOrdinal("State")),
                        TimeStamp = reader.GetDateTime(reader.GetOrdinal("TimeStamp"))
                    };
                    return u;
                }
                // Создание нового пользователя; всё, что мы знаем - его идентификатор
                u = new User()
                {
                    ID = id
                };
                SqlCommand c2= conn.CreateCommand();
                c2.CommandText = $"INSERT INTO Users (ID) VALUE ({id})";
                c2.ExecuteNonQuery();
                return u;
            }
        }

        /// <summary>
        /// Список всех пользователей
        /// </summary>
        /// <returns></returns>
        public List<User> GetUsers()
        {
            return new List<User>();
        }

        /// <summary>
        /// Сохранение в файл
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            // Отдельного сохранения не требуется
        }
    }
}

