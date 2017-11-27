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
            Pen PenGreen = new Pen(Color.Green, 1);//绿 虚线 细
            PenGreen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;//选择线段样式
            Pen PenGreen1 = new Pen(Color.Green, 1);//绿 实线 细
            Pen PenGreen2 = new Pen(Color.Green, 2);//绿 实线 粗

            int scale_X = 10;
            int scale_Y = 30;
            int trainNum = 16;
            int stationNum = 23;
            int totalHours = 10;
            int points_Per_Hour = 12;

            //画横线
            for (int i = 0; i < stationNum; i++)
            {
                gr.DrawLine(PenGreen2, startP.X, startP.Y + i * scale_Y, points_Per_Hour*totalHours*scale_X, startP.Y + i * scale_Y);
            }
            //画竖线
            for (int i = 0; i < totalHours * points_Per_Hour; i++)
            {
                if (i % points_Per_Hour == 0)
                {
                    //每小时竖线
                    gr.DrawLine(PenGreen2, startP.X+i*scale_X, startP.Y, startP.X+i*scale_X, 500);
                }
                else 	
                {
                    gr.DrawLine(PenGreen1, startP.X + i * scale_X, startP.Y, startP.X + i * scale_X, 500);
	            }
                //gr.DrawLine(PenGreen, startP.X+i*scale_X, startP.Y, startP.X+i*scale_X, 500);
            }
            //gr.DrawLine(PenGreen, startP.X, startP.Y, 1300, startP.Y);
            //gr.DrawLine(PenGreen, startP.X, startP.Y, startP.X, 1000);
            gr.Save();

            pictureBox1.Image = bm;
            //pictureBox1.Refresh();
        }
    }
}
