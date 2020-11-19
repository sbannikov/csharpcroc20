using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocCSharpBot
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public interface IUser
    {
        long ID { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string UserName { get; set; }
        string PhoneNumber { get; set; }
        string Description { get; set; }
        int State { get; set; }
        DateTime TimeStamp { get; set; }
    }
}
