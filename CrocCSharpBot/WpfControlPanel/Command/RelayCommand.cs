using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfControlPanel.Command
{
    /// <summary>
    /// Базовая реализация комманд
    /// </summary>
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Указатель на функция, которую нужно испольнить
        /// </summary>
        Action<object> execute;

        /// <summary>
        /// Указатель на функцию, которая проверит может ли комманда отработать
        /// </summary>
        Func<object, bool> canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            execute?.Invoke(parameter);
        }
    }
}
