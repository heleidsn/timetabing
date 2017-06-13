using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 列车运行调整
{
    public partial class DelayInfo : Form
    {
        public DelayInfo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO 检测输入是否合法
            Form1.PublicValue.delayTrainNo = int.Parse(textBox1.Text);
            Form1.PublicValue.delayBlock = int.Parse(textBox2.Text);
            Form1.PublicValue.delayTime = int.Parse(textBox3.Text);
            MessageBox.Show("设置成功");
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
