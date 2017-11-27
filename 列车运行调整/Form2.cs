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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Image img = Image.FromFile(@"E:\2017-01-04.png");
            //pictureBox1.Image = img;
            plot_Frame();
        }

        //绘制框架
        private void plot_Frame()
        {
            //创建位图并在其上作画
            Bitmap bm = new Bitmap(2000, 1000);
            Graphics gr = Graphics.FromImage(bm);

            gr.Clear(Color.White);

            Point startP = new Point(100,100);
            Pen PenGreen = new Pen(Color.Green, 1);

            int scale_X = 10;
            int scale_Y = 30;
            int trainNum = 16;
            int stationNum = 23;

            for (int i = 0; i < stationNum; i++)
            {
                gr.DrawLine(PenGreen, startP.X, startP.Y + i * scale_Y, 1000, startP.Y + i * scale_Y);
            }
            for (int i = 0; i < 300; i++)
            {
                gr.DrawLine(PenGreen, startP.X+i*scale_X, startP.Y, startP.X+i*scale_X, 500);
            }
            gr.DrawLine(PenGreen, startP.X, startP.Y, 1300, startP.Y);
            gr.DrawLine(PenGreen, startP.X, startP.Y, startP.X, 1000);
            gr.Save();

            pictureBox1.Image = bm;
            //pictureBox1.Refresh();
        }
    }
}
