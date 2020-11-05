namespace BotControlPanel
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tool = new System.Windows.Forms.ToolStrip();
            this.startButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.service = new System.ServiceProcess.ServiceController();
            this.list = new System.Windows.Forms.ListBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.query = new System.Windows.Forms.ToolStripButton();
            this.traceOnButton = new System.Windows.Forms.ToolStripButton();
            this.traceOffButton = new System.Windows.Forms.ToolStripButton();
            this.tool.SuspendLayout();
            this.SuspendLayout();
            // 
            // tool
            // 
            this.tool.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tool.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startButton,
            this.stopButton,
            this.query,
            this.traceOnButton,
            this.traceOffButton});
            this.tool.Location = new System.Drawing.Point(0, 0);
            this.tool.Name = "tool";
            this.tool.Size = new System.Drawing.Size(1076, 27);
            this.tool.TabIndex = 0;
            this.tool.Text = "toolStrip1";
            // 
            // startButton
            // 
            this.startButton.Image = ((System.Drawing.Image)(resources.GetObject("startButton.Image")));
            this.startButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(101, 24);
            this.startButton.Text = "Запустить";
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Image = ((System.Drawing.Image)(resources.GetObject("stopButton.Image")));
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(114, 24);
            this.stopButton.Text = "Остановить";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // service
            // 
            this.service.ServiceName = "BotService";
            // 
            // list
            // 
            this.list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list.FormattingEnabled = true;
            this.list.ItemHeight = 16;
            this.list.Location = new System.Drawing.Point(0, 27);
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(1076, 425);
            this.list.TabIndex = 1;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // query
            // 
            this.query.Image = ((System.Drawing.Image)(resources.GetObject("query.Image")));
            this.query.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.query.Name = "query";
            this.query.Size = new System.Drawing.Size(106, 24);
            this.query.Text = "Запросить";
            this.query.Click += new System.EventHandler(this.query_Click);
            // 
            // traceOnButton
            // 
            this.traceOnButton.Image = ((System.Drawing.Image)(resources.GetObject("traceOnButton.Image")));
            this.traceOnButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.traceOnButton.Name = "traceOnButton";
            this.traceOnButton.Size = new System.Drawing.Size(190, 24);
            this.traceOnButton.Text = "Включить трассировку";
            this.traceOnButton.Click += new System.EventHandler(this.traceOnButton_Click);
            // 
            // traceOffButton
            // 
            this.traceOffButton.Image = ((System.Drawing.Image)(resources.GetObject("traceOffButton.Image")));
            this.traceOffButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.traceOffButton.Name = "traceOffButton";
            this.traceOffButton.Size = new System.Drawing.Size(204, 24);
            this.traceOffButton.Text = "Остановить трассировку";
            this.traceOffButton.Click += new System.EventHandler(this.traceOffButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1076, 452);
            this.Controls.Add(this.list);
            this.Controls.Add(this.tool);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Управление ботом";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.tool.ResumeLayout(false);
            this.tool.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tool;
        private System.Windows.Forms.ToolStripButton startButton;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.ServiceProcess.ServiceController service;
        private System.Windows.Forms.ListBox list;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripButton query;
        private System.Windows.Forms.ToolStripButton traceOnButton;
        private System.Windows.Forms.ToolStripButton traceOffButton;
    }
}

