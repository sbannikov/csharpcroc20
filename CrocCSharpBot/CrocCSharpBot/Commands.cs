using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrocCSharpBot
{
    /// <summary>
    /// Список команд бота
    /// </summary>
    enum Commands
    {
        /// <summary>
        /// Начало работы с ботом
        /// </summary>
        [Description("Начало работы с ботом")]
        Start,
        /// <summary>
        /// Список возможных команд
        /// </summary>
        [Description("Список возможных команд")]
        Help
    }
}
