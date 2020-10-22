using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CrocCSharpBot
{
    /// <summary>
    /// Расширение для перечислимых типов
    /// http://softblog.violet-tape.ru/2010/12/24/enum-with-friendly-name/
    /// </summary>
    public static class EnumExtenders
    {
        /// <summary>
        /// Наименование элемента перечислимого типа
        /// (на основании атрибута Description)
        /// </summary>
        /// <param name="enumerate">Перечислимый тип</param>
        /// <returns></returns>
        public static string ToDescription(this Enum enumerate)
        {
            var type = enumerate.GetType();
            var fieldInfo = type.GetField(enumerate.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : enumerate.ToString();
        }
    }
}
