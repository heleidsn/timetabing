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
            //标准运行图到发时刻 T_plan
            public static int[,] T_plan = new int[14, 10];
 
            //等级矩阵和权重矩阵
            public static int[] level = { 2, 2, 2, 3, 3, 4, 4, 4, 4, 4, 1, 1, 1, 1 };
            public static int[] weight = { 3, 3, 3, 2, 2, 1, 1, 1, 1, 1, 4, 4, 4, 4 };

            //首列车发车时间
            public static int[] startTime = { 0, 6, 12, 18, 24, 30, 36, 42, 48, 54, 172, 178, 184, 190 };

            //标准运行时分  4个等级列车在5个区间
            public static int[,] Ts = new int[5, 4] { { 25, 32, 39, 48 }, { 29, 36, 44, 54 }, { 9, 11, 14, 17 }, { 20, 24, 30, 36 }, { 23, 28, 35, 44 } };
            //最小运行时分
            public static int[,] Tm = new int[5, 4] { { 20, 27, 36, 45 }, { 22, 29, 40, 50 }, { 7, 9, 13, 16 }, { 17, 21, 28, 34 }, { 19, 24, 32, 41 } };
            //6个车站对应作业时间
            public static int[,] Tz = new int[6, 4] { { 5, 5, 5, 6 }, { 0, 0, 0, 0 }, { 0, 0, 5, 6 }, { 6, 6, 6, 7 }, { 0, 0, 0, 6 }, { 5, 5, 6, 7 } };

            //最小到达和出发间隔
            public static int minArriveTime = 6;
            public static int minDepartTime = 6;

            //绘图起点
            public static Point startP = new Point(100,100);
            public static int[] lineY = new int[6];

            //晚点信息
            public static int delayTrainNo; //晚点列车序号
            public static int delayBlock;   //晚点区间
            public static int delayTime;    //晚点时间
        }

        //计算标准运行图到发时刻 T_plan
        private void GetPlanTimetable()
        {
            for (int i = 0; i < 14; i++)  //对14列车进行遍历
            {
                for (int j = 0; j < 10; j++)  //对6个站点进行遍历 一共10个时刻
                {
                    if (j == 0)
                    {
                        PublicValue.T_plan[i, j] = PublicValue.startTime[i];
                    }
                    else
                    {
                        if (j % 2 == 1)  //到达点
                        {
                            PublicValue.T_plan[i, j] = PublicValue.T_plan[i, j - 1] + PublicValue.Ts[j / 2, PublicValue.level[i]-1];
                        }
                        else
                        {
                            PublicValue.T_plan[i, j] = PublicValue.T_plan[i, j - 1] + PublicValue.Tz[j / 2, PublicValue.level[i]-1];
                        }
                    }
                }
            }
        }
        private void PlotFrame(Point startP)
        {
            //创建画布
            Graphics gr = pictureBox1.CreateGraphics();
            gr.Clear(Color.White);

            //设置画笔
            Pen PenGreen = new Pen(Color.Green, 1);
            Pen PenBlack = new Pen(Color.Black, 3);

            //绘制框架
            int stationNum = 6;
            int[] Y = { 0, 48, 54, 17, 36, 44 };
            //int[] lineY = new int[stationNum];
            string[] stationName = { "车站1", "车站2", "车站3", "车站4", "车站5", "车站6" };
            PublicValue.lineY[0] = startP.Y;
            gr.DrawLine(PenGreen, 100, PublicValue.lineY[0], 1050, PublicValue.lineY[0]);
            Font drawFont = new Font("Arial", 16);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            gr.DrawString(stationName[0], drawFont, drawBrush, new Point(20, PublicValue.lineY[0] - 10));
            for (int i = 1; i < stationNum; i++)
            {
                PublicValue.lineY[i] = PublicValue.lineY[i - 1] + Y[i] * 2;    //2为比例系数 可以进行缩放
                gr.DrawLine(PenGreen, 100, PublicValue.lineY[i], 1050, PublicValue.lineY[i]);
                gr.DrawString(stationName[i], drawFont, drawBrush, new Point(20, PublicValue.lineY[i] - 10));
            }
        }

        private void PlotTimeTable(Point startP, int[] lineY, int[,] T_plan, Pen P)
        {
            Graphics gr = pictureBox1.CreateGraphics();
            //绘制标准运行图
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (j % 2 == 0) //发车
                    {
                        Point p1 = new Point(startP.X + T_plan[i, j] * 3, lineY[j / 2]);
                        Point p2 = new Point(startP.X + T_plan[i, j + 1] * 3, lineY[j / 2 + 1]);
                        gr.DrawLine(P, p1, p2);
                    }
                    else
                    {
                        Point p1 = new Point(startP.X + T_plan[i, j] * 3, lineY[j / 2 + 1]);
                        Point p2 = new Point(startP.X + T_plan[i, j + 1] * 3, lineY[j / 2 + 1]);
                        gr.DrawLine(P, p1, p2);
                    }
                }
            }
        }

        private void 读取运行图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //读取标准运行图
            GetPlanTimetable();
            int[,] T_plan = new int[14,10];
            T_plan = PublicValue.T_plan;

            //绘制框架
            PlotFrame(PublicValue.startP);

            //绘制标准运行图
            PlotTimeTable(PublicValue.startP, PublicValue.lineY, T_plan, new Pen(Color.Black, 3));
        }

        private void 执行调整ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            //Graphics gr = Graphics.FromImage(pictureBox1.Image);
            Graphics gr = pictureBox1.CreateGraphics();
            Pen PenBlack = new Pen(Color.Black, 3);
            gr.DrawLine(PenBlack, 100, 100, 1050, 200);
        }


        //测试
        private void 显示时刻表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine(PublicValue.delayTrainNo);
            Console.WriteLine(PublicValue.delayBlock);
            Console.WriteLine(PublicValue.delayTime);
        }

        private void 晚点信息设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DelayInfo f2 = new DelayInfo();
            f2.Show();
        }
    }
}
