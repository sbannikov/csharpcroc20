using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace BotControlPanel
{
    /// <summary>
    /// Главная форма приложения
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Клиент управляющего веб-сервиса
        /// </summary>
        private ControlReference.ContolServiceClient client;

        private UdpClient udp;

        private System.Collections.Concurrent.ConcurrentQueue<string> queue;

        /// <summary>
        /// Конструктор формы
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            client = new ControlReference.ContolServiceClient();
            udp = new UdpClient(9999);
            queue = new System.Collections.Concurrent.ConcurrentQueue<string>();
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
                udp.BeginReceive(new AsyncCallback(OnUdpUpdate), udp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Асинхронный прием сообщений по UDP
        /// </summary>
        /// <param name="result"></param>
        private void OnUdpUpdate(IAsyncResult result)
        {
            UdpClient socket = result.AsyncState as UdpClient;
            IPEndPoint source = null;
            byte[] message = socket.EndReceive(result, ref source);
            string s = Encoding.UTF8.GetString(message);
            queue.Enqueue(s);
            udp.BeginReceive(new AsyncCallback(OnUdpUpdate), udp);
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
                // Прием сообщений из очереди
                string s;
                while (queue.TryDequeue(out s))
                {
                    list.Items.Add(s);
                }
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

        /// <summary>
        /// Запрос состояния бота
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void query_Click(object sender, EventArgs e)
        {
            try
            {
                string result = client.Query();
                list.Items.Add(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Включить трассировку в консоль
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void traceOnButton_Click(object sender, EventArgs e)
        {
            try
            {
                client.StartTrace("127.0.0.1");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Выключить трассировку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void traceOffButton_Click(object sender, EventArgs e)
        {
            try
            {
                client.StopTrace();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
