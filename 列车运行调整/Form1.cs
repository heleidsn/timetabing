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
            public static int[,] T_plan = new int[14, 10];      //标准运行图
            public static int[,] T_heuristic = new int[14, 10]; //基本启发式算法调整后运行图
            public static int[,] T_FAA = new int[14, 10];       //萤火虫算法调整后运行图
 
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
            public static int delayTrainNo = 6; //晚点列车序号
            public static int delayBlock = 3;   //晚点区间
            public static int delayTime = 40;    //晚点时间

            //萤火虫算法参数
            public static int fireflyNum = 30;    //萤火虫数目
            public static int maxGeneration = 20; //最大迭代次数
            public static int neighborNum1 = 2;   //第一种邻域计算次数   
            public static int neighborNum2 = 2;   //第二种领域计算次数
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

        //绘制运行图基本框架
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

        //绘制运行图
        private void PlotTimeTable(Point startP, int[] lineY, int[,] T_plan, Pen P)
        {
            //绘制框架
            PlotFrame(PublicValue.startP);

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

            

            //绘制标准运行图
            PlotTimeTable(PublicValue.startP, PublicValue.lineY, T_plan, new Pen(Color.Black, 2));

            
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
            //Console.WriteLine(0/2);
            int[] list = { 34, 72, 13, 44, 25, 30, 10 };
            int[] index = { 1, 2, 3, 4, 5, 6, 7 };
            int[] temp = list;
            Array.Sort(list,index);
            foreach (int i in list)
            {
                Console.Write(i + " ");
            }
            foreach (int i in index)
            {
                Console.Write(i + " ");
            }
            
        }

        //晚点信息设置界面加载
        private void 晚点信息设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DelayInfo f2 = new DelayInfo();
            f2.Show();
        }

        //普通启发式算法运算
        private void 启发式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //变量设置
            int[,] T_temp = new int[14, 10];
            int[,] F_js = new int[5, 14];
            int[,] T_plan = PublicValue.T_plan;

            int[] list = new int[14];
            int[] index = new int[14];

            int temp1 = 0;  //记录最后一个受影响的高等级车辆

            //按站进行查询
            for (int i = 0; i < 10; i++)
            {
                //Console.WriteLine(i);
                if (i == 0)  //始发站  没有晚点  直接复制
                {
                    for (int j = 0; j < 14; j++)
                    {
                        T_temp[j, i] = T_plan[j, i];
                    }
                }
                else
                {
                    //到达站情况
                    if (i%2 == 1)
                    {
                        for (int j = 0; j < 14; j++)
                        {
                            if (j == PublicValue.delayTrainNo - 1 && i/2 == PublicValue.delayBlock - 1)  //初始晚点情况
                            {
                                T_temp[j, i] = T_temp[j, i-1] + PublicValue.Ts[i/2, PublicValue.level[j] - 1] + PublicValue.delayTime;
                            }
                            else if (i != 1 && F_js[i/2 - 1,j] == 1) //需要加速情况
                            {
                                T_temp[j, i] = T_temp[j, i - 1] + PublicValue.Tm[i / 2, PublicValue.level[j] - 1];
                                if (T_temp[j,i] < T_plan[j,i])
                                {
                                    T_temp[j, i] = T_plan[j, i];
                                }
                            }
                            else  //正常情况
                            {
                                T_temp[j, i] = T_temp[j, i - 1] + PublicValue.Ts[i / 2, PublicValue.level[j] - 1];
                            }

                            list[j] = T_temp[j, i];
                            index[j] = j;
                        }
                        //按照最小进站时间重新排序
                        Array.Sort(list, index);
                        int flag1 = 0;
                        for (int kkk = 0; kkk < 14; kkk++)
                        {
                            if (index[kkk] != kkk)
                            {
                                flag1 = 1;
                            }
                        }
                        if (flag1 == 1 && i/2 != PublicValue.delayBlock - 1)
                        {
                            for (int j = 0; j < 13; j++)
                            {
                                if (T_temp[index[j + 1], i] - T_temp[index[j], i] < PublicValue.minArriveTime)
                                {
                                    T_temp[index[j + 1], i] = T_temp[index[j], i] + PublicValue.minArriveTime;
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 13; j++)
                            {
                                if (T_temp[j+1,i] - T_temp[j,i] < PublicValue.minArriveTime)
                                {
                                    T_temp[j + 1, i] = T_temp[j, i] + PublicValue.minArriveTime;
                                }
                            }
                        }
                        
                        //确定晚点列车
                        for (int j = 0; j < 14; j++)
                        {
                            if (T_temp[j,i] > T_plan[j,i])
                            {
                                F_js[i / 2, j] = 1;
                            }
                        }
                    }
                    else    //出发站点
                    {
                        //计算调整前出发时间
                        for (int j = 0; j < 14; j++)
                        {
                            T_temp[j, i] = T_temp[j, i - 1] + PublicValue.Tz[i / 2, PublicValue.level[j] - 1];
                        }
                        //按优先级调整发车顺序
                        for (int j = 0; j < 14; j++)
                        {
                            if (F_js[i / 2 - 1, j] == 1)  //如果为晚点车辆
                            {
                                temp1 = 0; //记录最后一个受影响的高等级车辆
                                for (int kk = j + 1; kk < 14; kk++)
                                {
                                    if (PublicValue.level[j] > PublicValue.level[kk])   //如果kk等级高   1最高 4最低
                                    {
                                        int T1 = T_temp[kk,i] + PublicValue.Ts[i/2,PublicValue.level[kk] - 1];  //kk列车下站正常到站时间
                                        int T2 = T_temp[j,i] + PublicValue.Tm[i/2,PublicValue.level[j] - 1] + PublicValue.minArriveTime;  //j列车下站到达时间+ai
                                        if (T1 < T2)
                                        {
                                            temp1 = kk;  //记录最后一个受影响的列车
                                        }
                                    }
                                }
                                if (temp1 != 0)
                                {
                                    T_temp[j, i] = T_temp[temp1, i] + PublicValue.minDepartTime; //将j列车排在最后一个受影响的高等级列车之后
                                    for (int kk = j + 1; kk < 14; kk++)
                                    {
                                        //j列车不能早于同等级列车发车
                                        if (T_temp[j,i] < T_temp[kk,i] + PublicValue.minDepartTime && PublicValue.level[j] > PublicValue.level[kk])
                                        {
                                            T_temp[j, i] = T_temp[kk, i] + PublicValue.minDepartTime;
                                        }
                                    }
                                }
                            }
                        }

                        //按照等级、发车顺序对出站列车进行重新排序
                        for (int ii = 0; ii < 4; ii++)
                        {
                            int temp2 = 0;
                            for (int jj = 0; jj < 14; jj++)
                            {
                                if (PublicValue.level[jj] == ii+1)
                                {
                                    if (temp2 == 0)
                                    {
                                        temp2 = T_temp[jj, i];
                                    }
                                    else
                                    {
                                        if (T_temp[jj,i] < temp2 + PublicValue.minDepartTime)
                                        {
                                            T_temp[jj, i] = temp2 + PublicValue.minDepartTime;
                                            temp2 = T_temp[jj, i];
                                        }
                                        else
                                        {
                                            temp2 = T_temp[jj, i];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //调整结束  进行赋值
            PublicValue.T_heuristic = T_temp;
            Console.WriteLine(GetDelayTime(T_temp));
        }

        //显示调整后运行图
        private void 调整后运行图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlotTimeTable(PublicValue.startP, PublicValue.lineY, PublicValue.T_heuristic, new Pen(Color.Black, 2));
            int[,] T = PublicValue.T_heuristic;
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Console.Write(T[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }

        //获取晚点时间   
        //输入：调整后的时刻表
        //输出：加权晚点时间
        private double GetDelayTime(int[,] timeTable)
        {
            double delayTime = 0;
            for (int i = 0; i < 14; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    delayTime += (timeTable[i, j] - PublicValue.T_plan[i, j]) * PublicValue.weight[i] * 0.1;
                }
            }
            return delayTime;
        }
    }
}
