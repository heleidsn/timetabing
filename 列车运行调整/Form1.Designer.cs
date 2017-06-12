namespace 列车运行调整
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.读取运行图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.晚点信息设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.执行调整ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.显示时刻表ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.读取运行图ToolStripMenuItem,
            this.晚点信息设置ToolStripMenuItem,
            this.执行调整ToolStripMenuItem,
            this.显示时刻表ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1122, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 读取运行图ToolStripMenuItem
            // 
            this.读取运行图ToolStripMenuItem.Name = "读取运行图ToolStripMenuItem";
            this.读取运行图ToolStripMenuItem.Size = new System.Drawing.Size(80, 21);
            this.读取运行图ToolStripMenuItem.Text = "标准运行图";
            this.读取运行图ToolStripMenuItem.Click += new System.EventHandler(this.读取运行图ToolStripMenuItem_Click);
            // 
            // 晚点信息设置ToolStripMenuItem
            // 
            this.晚点信息设置ToolStripMenuItem.Name = "晚点信息设置ToolStripMenuItem";
            this.晚点信息设置ToolStripMenuItem.Size = new System.Drawing.Size(92, 21);
            this.晚点信息设置ToolStripMenuItem.Text = "晚点信息设置";
            this.晚点信息设置ToolStripMenuItem.Click += new System.EventHandler(this.晚点信息设置ToolStripMenuItem_Click);
            // 
            // 执行调整ToolStripMenuItem
            // 
            this.执行调整ToolStripMenuItem.Name = "执行调整ToolStripMenuItem";
            this.执行调整ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.执行调整ToolStripMenuItem.Text = "执行调整";
            this.执行调整ToolStripMenuItem.Click += new System.EventHandler(this.执行调整ToolStripMenuItem_Click);
            // 
            // 显示时刻表ToolStripMenuItem
            // 
            this.显示时刻表ToolStripMenuItem.Name = "显示时刻表ToolStripMenuItem";
            this.显示时刻表ToolStripMenuItem.Size = new System.Drawing.Size(80, 21);
            this.显示时刻表ToolStripMenuItem.Text = "显示时刻表";
            this.显示时刻表ToolStripMenuItem.Click += new System.EventHandler(this.显示时刻表ToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1122, 623);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1122, 648);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 读取运行图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 晚点信息设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 执行调整ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 显示时刻表ToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

