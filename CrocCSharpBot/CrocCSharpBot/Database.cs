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
            conn.ConnectionString = Properties.Settings.Default.ConnectionString;
            conn.Open();
        }

        /// <summary>
        /// Чтение строки с обработкой NULL
        /// </summary>
        /// <param name="reader">Читатель</param>
        /// <param name="column">Имя столбца запроса</param>
        /// <returns></returns>
        private string GetString(SqlDataReader reader, string column)
        {
            // Порядковый номер столбца
            int n = reader.GetOrdinal(column);
            return reader.IsDBNull(n) ? null : reader.GetString(n);
        }

        public IUser this[long id]
        {
            get
            {
                Storage.User u;
                SqlCommand c1 = conn.CreateCommand();
                c1.CommandText = "SELECT [ID], [FirstName], [LastName], [UserName], [PhoneNumber], [Description], [State], [TimeStamp] FROM [Users] WHERE ID = " + id;
                // Поиск пользователя по идентификатору в БД
                using (var reader = c1.ExecuteReader())
                {
                    // Проверка на наличие пользователя в БД
                    if (reader.Read())
                    {
                        // Формирование объекта
                        u = new Storage.User()
                        {
                            ID = reader.GetInt64(reader.GetOrdinal("ID")),
                            FirstName = GetString(reader, "FirstName"),
                            LastName = GetString(reader, "LastName"),
                            UserName = GetString(reader, "UserName"),
                            PhoneNumber = GetString(reader, "PhoneNumber"),
                            Description = GetString(reader, "Description"),
                            UState = (UserState)reader.GetInt32(reader.GetOrdinal("State")),
                            TimeStamp = reader.GetDateTime(reader.GetOrdinal("TimeStamp"))
                        };
                        return u;
                    }
                }
                // Создание нового пользователя; всё, что мы знаем - его идентификатор
                u = new Storage.User()
                {
                    ID = id,
                    UState = UserState.None,
                    TimeStamp = DateTime.Now
                };
                SqlCommand c2 = conn.CreateCommand();
                c2.CommandText = $"INSERT INTO Users (ID, State, TimeStamp) VALUES ({id}, 0, @ts)";
                c2.Parameters.AddWithValue("ts", u.TimeStamp);
                c2.ExecuteNonQuery();
                return u;
            }
        }

        /// <summary>
        /// Список всех пользователей
        /// </summary>
        /// <returns></returns>
        public List<IUser> GetUsers()
        {
            // [!] надо реализовать, пока - пустой список
            // Домашнее задание - доделать
            return new List<IUser>();
        }      

        /// <summary>
        /// Сохранение пользователя в БД
        /// </summary>
        /// <param name="user"></param>
        public void Save(IUser user)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"UPDATE Users SET [FirstName] = @firstName
                ,[LastName] = @lastName
                ,[UserName] = @userName
                ,[PhoneNumber] = @phoneNumber
                ,[Description] = @description
                ,[State] = @state
                ,[TimeStamp] = @timeStamp WHERE ID = @id";
            cmd.Parameters.AddWithValue("id", user.ID);
            cmd.Parameters.AddWithValue("firstName", user.FirstName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("lastName", user.LastName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("userName", user.UserName ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("phoneNumber", user.PhoneNumber ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("description", user.Description ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("state", user.State);
            cmd.Parameters.AddWithValue("timeStamp", user.TimeStamp);
            cmd.ExecuteNonQuery();
        }
    }
}

