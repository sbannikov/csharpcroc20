using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocCSharpBot
{
    /// <summary>
    /// Тип хранения данных
    /// </summary>
    public enum StorageType
    {
        FileStorage,
        /// <summary>
        /// База данных ADO.NET
        /// </summary>
        DatabaseStorage,
        /// <summary>
        /// База данных Entity Framework Database First
        /// </summary>
        DatabaseFirstStorage,
        /// <summary>
        /// База данных Entity Framework Code First
        /// </summary>
        CodeFirstStorage
    }
}
