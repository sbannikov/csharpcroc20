using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BotControlPanel
{
    /// <summary>
    /// Главная форма приложения
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Конструктор формы
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Запуск сервиса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, EventArgs e)
        {
            try
            {
                service.Start();
                list.Items.Add("Служба успешно запущена");
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Останов сервиса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            try
            {
                service.Stop();
                list.Items.Add("Служба успешно остановлена");
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Загрузка главной формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                UpdateButtons();
                timer.Interval = Properties.Settings.Default.TimerIntervalInMilliseconds;
                timer.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Обновление состояния основных кнопок
        /// </summary>
        private void UpdateButtons()
        {
            startButton.Enabled = true;
            stopButton.Enabled = true;

            // Обязательное обновление состояния службы
            service.Refresh();
            var status = service.Status;

            switch (status)
            {
                case System.ServiceProcess.ServiceControllerStatus.Running:
                    startButton.Enabled = false;
                    break;

                case System.ServiceProcess.ServiceControllerStatus.Stopped:
                    stopButton.Enabled = false;
                    break;

                default: // во всех остальных состояниях управление сервисом невозможно
                    startButton.Enabled = false;
                    stopButton.Enabled = false;
                    break;
            }
        }

        /// <summary>
        /// Такт таймера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                timer.Enabled = false;
                UpdateButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                timer.Enabled = true;
            }
        }
    }
}
