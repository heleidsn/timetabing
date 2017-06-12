using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 列车运行调整
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class PublicValue   //全局变量设置
        {
            public static int[,] T_plan = new int[14,10];
            public int B;
        }

        private void 读取运行图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //int[,] T_plan = new int[14,10];
            //读取标准运行图数据
            int[,] T_plan = {
            {0,32,32,68,68,79,85,109,109,137},
            {6,38,38,74,74,85,91,115,115,143},
            {12,44,44,80,80,91,97,121,121,149},
            {18,57,57,101,106,120,126,156,156,191},
            {24,63,63,107,112,126,132,162,162,197},
            {30,78,78,132,138,155,162,198,204,248},
            {36,84,84,138,144,161,168,204,210,254},
            {42,90,90,144,150,167,174,210,216,260},
            {48,96,96,150,156,173,180,216,222,266},
            {54,102,102,156,162,179,186,222,228,272},
            {172,197,197,226,226,235,241,261,261,284},
            {178,203,203,232,232,241,247,267,267,290},
            {184,209,209,238,238,247,253,273,273,296},
            {190,215,215,244,244,253,259,279,279,302}};
            
            

            //变量定义
            Point startP = new Point(100, 100);
            Point endP = new Point(200, 200);

            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics gr = Graphics.FromImage(pictureBox1.Image);
            gr.Clear(Color.White);
            Pen PenGreen = new Pen(Color.Green, 1);//绿 虚线 细
            Pen PenBlack = new Pen(Color.Black, 3);
            //gr.DrawLine(PenGreen, startP,endP);


            //绘制框架
            int stationNum = 6;
            int[] Y = {0, 48, 54, 17, 36, 44 };
            int[] lineY = new int[stationNum];
            string[] stationName = { "车站1", "车站2", "车站3", "车站4", "车站5", "车站6" };
            lineY[0] = startP.Y;
            gr.DrawLine(PenGreen, 100, lineY[0], 1050, lineY[0]);
            Font drawFont = new Font("Arial", 16);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            gr.DrawString(stationName[0], drawFont,drawBrush, new Point(20, lineY[0]-10));
            for (int i = 1; i < stationNum; i++)
            {
                lineY[i] = lineY[i - 1] + Y[i] * 2;
                gr.DrawLine(PenGreen,100,lineY[i],1050,lineY[i]);
                gr.DrawString(stationName[i], drawFont, drawBrush, new Point(20, lineY[i] - 10));
            }



            //绘制标准运行图
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (j % 2 == 0) //发车
                    {
                        Point p1 = new Point(startP.X + T_plan[i, j]*3, lineY[j / 2]);
                        Point p2 = new Point(startP.X + T_plan[i, j + 1]*3, lineY[j / 2 + 1]); 
                        gr.DrawLine(PenBlack,p1,p2);
                    }
                    else
                    {
                        Point p1 = new Point(startP.X + T_plan[i, j] * 3, lineY[j / 2 + 1]);
                        Point p2 = new Point(startP.X + T_plan[i, j + 1] * 3, lineY[j / 2 + 1]);
                        gr.DrawLine(PenBlack, p1, p2);
                    }
                }
            }



        }

        private void 执行调整ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PublicValue.A = 10;
            test();
        }

        private void test()
        {
            PublicValue.A = 20;
            Console.WriteLine(PublicValue.A);
        }
    }
}
