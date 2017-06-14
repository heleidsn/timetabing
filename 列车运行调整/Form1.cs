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

            public static int[] departTimeTemp = new int[14]; //用于参数传递 GetNextStationDelayTime()  
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
            /*
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
            
            int[] index = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14};
            int[] temp = Shuffle(index,6);
            foreach (int i in index)
            {
                Console.Write(i + "\t");
            }
            Console.WriteLine();
            
            for (int i = 0; i < 10; i++)
            {
                Random rd = new Random(Guid.NewGuid().GetHashCode());
                double dResult;
                dResult = rd.NextDouble();
                Console.WriteLine(dResult);
            }
             * */

            int[] index = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };
            //Console.WriteLine(Array.IndexOf(index, 15));
            index = faaInsert(index, 6);
            

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
            SolveHeuristic();
  
        }

        //显示调整后运行图
        private void 调整后运行图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlotTimeTable(PublicValue.startP, PublicValue.lineY, PublicValue.T_heuristic, new Pen(Color.Black, 2));

            PrintTimeTable("普通：", PublicValue.T_heuristic);
            PrintTimeTable("萤火虫：", PublicValue.T_FAA);
        }

        private void PrintTimeTable(string name, int[,] T)
        {
            Console.WriteLine(name);
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

        //普通启发式算法求解
        private void SolveHeuristic()
        {
            //变量设置
            int[,] T_temp = new int[14, 10];    //调整后的运行图
            int[,] F_js = new int[5, 14];       //晚点车辆标记
            int[,] T_plan = PublicValue.T_plan; //计划运行图

            int[] list = new int[14];           //排序时用到  记录排序后的数据
            int[] index = new int[14];          //排序时用到  记录排序后数据下标

            int temp1 = 0;                      //记录最后一个受影响的高等级车辆

            //按站进行查询  共10个节点
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
                    if (i % 2 == 1)
                    {
                        for (int j = 0; j < 14; j++)
                        {
                            if (j == PublicValue.delayTrainNo - 1 && i / 2 == PublicValue.delayBlock - 1)  //初始晚点情况
                            {
                                T_temp[j, i] = T_temp[j, i - 1] + PublicValue.Ts[i / 2, PublicValue.level[j] - 1] + PublicValue.delayTime;
                            }
                            else if (i != 1 && F_js[i / 2 - 1, j] == 1) //需要加速情况
                            {
                                T_temp[j, i] = T_temp[j, i - 1] + PublicValue.Tm[i / 2, PublicValue.level[j] - 1];
                                if (T_temp[j, i] < T_plan[j, i])
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
                        int flag1 = 0;  //判断有无越行
                        for (int kkk = 0; kkk < 14; kkk++)
                        {
                            if (index[kkk] != kkk)
                            {
                                flag1 = 1;
                            }
                        }
                        if (flag1 == 1 && i / 2 != PublicValue.delayBlock - 1)
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
                                if (T_temp[j + 1, i] - T_temp[j, i] < PublicValue.minArriveTime)
                                {
                                    T_temp[j + 1, i] = T_temp[j, i] + PublicValue.minArriveTime;
                                }
                            }
                        }

                        //确定晚点列车
                        for (int j = 0; j < 14; j++)
                        {
                            if (T_temp[j, i] > T_plan[j, i])
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
                                        int T1 = T_temp[kk, i] + PublicValue.Ts[i / 2, PublicValue.level[kk] - 1];  //kk列车下站正常到站时间
                                        int T2 = T_temp[j, i] + PublicValue.Tm[i / 2, PublicValue.level[j] - 1] + PublicValue.minArriveTime;  //j列车下站到达时间+ai
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
                                        if (T_temp[j, i] < T_temp[kk, i] + PublicValue.minDepartTime && PublicValue.level[j] > PublicValue.level[kk])
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
                                if (PublicValue.level[jj] == ii + 1)
                                {
                                    if (temp2 == 0)
                                    {
                                        temp2 = T_temp[jj, i];
                                    }
                                    else
                                    {
                                        if (T_temp[jj, i] < temp2 + PublicValue.minDepartTime)
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

        //萤火虫算法求解
        private void SolveFAA()
        {
            //变量定义
            int[,] T_plan = PublicValue.T_plan;
            int[,] T_final = new int[14, 10];
            int[,] Index = new int[6, 14];  //记录各站调整得到的发车序号
            

            int faaStartFlag = 0;  //算法运行标志

            //按站进行调整
            for (int i = 0; i < 6; i++)  //对6个车站进行遍历
            {
                Console.WriteLine(i);
                if (i == 0)   //始发站
                {
                    for (int j = 0; j < 14; j++)
                    {
                        T_final[j, i] = T_plan[j, i];
                        Index[i, j] = j;  //按顺序发车
                    }
                }
                else if (i == 5)   //终点站
                {
                    int ii = i * 2 - 1;
                    //只需要计算最后的到达时刻
                    int[] temp = new int[14];
                    int[] departTimeTemp = new int[14];
                    int[] indexTemp = new int[14];
                    for (int j = 0; j < 14; j++)
                    {
                        departTimeTemp[j] = T_final[j, ii - 1];
                        indexTemp[j] = Index[i - 1, j];
                    }
                    temp = GetArriveTime(indexTemp, departTimeTemp, i);
                    for (int j = 0; j < 14; j++)
                    {
                        T_final[j, ii] = temp[j];
                    }
                    
                }
                else  //中间站点  需要计算到达和发车时间
                {
                    if (i != PublicValue.delayBlock && faaStartFlag == 0) //没有发生晚点情况
                    {
                        for (int j = 0; j < 14; j++)
                        {
                            T_final[j, i * 2 - 1] = T_plan[j, i * 2 - 1];
                            T_final[j, i * 2] = T_plan[j, i * 2];
                            Index[i, j] = j;
                        }
                    }
                    else
                    {
                        //--------Part 1 计算到站时刻-----------------
                        int ii = i * 2 - 1; //记录当前需要计算的列号
                        if (i == PublicValue.delayBlock)  //初次晚点
                        {
                            for (int j = 0; j < 14; j++)  //计算每个列车的到站时间
                            {
                                if (j == PublicValue.delayTrainNo - 1)
                                {
                                    T_final[j, ii] = T_plan[j, ii] + PublicValue.delayTime;
                                }
                                else
                                {
                                    T_final[j, ii] = T_plan[j, ii];
                                }
                            }

                            //按最小进站间隔调整到站时间
                            for (int j = 1; j < 14; j++)
                            {
                                if (T_final[j,ii] < T_final[j-1,ii] + PublicValue.minArriveTime)
                                {
                                    T_final[j, ii] = T_final[j - 1, ii] + PublicValue.minArriveTime;
                                }
                            }
                            faaStartFlag = 1;  //标志算法已经开始
                        }
                        else   //非初次晚点  根据上一站的发车顺序、区间运行时分来计算本站到达时刻
                        {
                            int[] temp = new int[14];
                            int[] departTimeTemp = new int[14];
                            int[] indexTemp = new int[14];
                            for (int j = 0; j < 14; j++)
			                {
                                departTimeTemp[j] = T_final[j,ii-1];
                                indexTemp[j] = Index[i-1,j];
			                }
                            temp = GetArriveTime(indexTemp, departTimeTemp, i);
                            for (int j = 0; j < 14; j++)
                            {
                                T_final[j, ii] = temp[j];
                            }
                        }

                        //--------Part 2 发车时间计算 调用萤火虫算法得到最优发车次序-------------
                        ii = i * 2 ; //记录要计算的列号 发车线

                        int firstDelayTrain = 0;  //记录首个晚点车辆序号   因为前面车辆顺序不需要重新排
                        for (int j = 0; j < 14; j++)
                        {
                            if (T_final[j,ii-1] > T_plan[j,ii-1])
                            {
                                firstDelayTrain = j;
                                break;
                            }
                        }

                        if (firstDelayTrain != 0)
                        {
                            //调用萤火虫算法调整该站发车顺序并得到发车时间
                            //Console.WriteLine(firstDelayTrain);
                            int[,] temp = new int[2, 14];
                            int[] arriveTime = new int[14];
                            for (int j = 0; j < 14; j++)
			                {
                                arriveTime[j] = T_final[j,ii-1];
			                }
                            temp = GetIndexFAA(i, arriveTime, firstDelayTrain);
                            for (int j = 0; j < 14; j++)
                            {
                                T_final[j, ii] = temp[0, j];
                                Index[i, j] = temp[1, j];
                            }
                        }
                        else
                        {
                            //恢复计划运行
                            for (int j = 0; j < 14; j++)
			                {
                                T_final[j,ii] = T_plan[j,ii];
                                Index[i,j] = j;
		                	}
                        }
                    }
                }
                PrintArray2(T_final);
                PrintArray2(Index);
            }

            PublicValue.T_FAA = T_final;
            Console.WriteLine(GetDelayTime(PublicValue.T_FAA));
        }

        private void 萤火虫算法ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SolveFAA();
        }

        //根据发车次序计算到达时刻
        private int[] GetArriveTime(int[] index, int[] departTime, int stationNo)
        {
            int[] temp = new int[14];     //所求发车时间
            int[] indexTmp = new int[14]; //发车顺序排序下标
            for (int i = 0; i < 14; i++)
            {
                indexTmp[i] = i;
            }

            Array.Sort(index, indexTmp);
       
            for (int i = 0; i < 14; i++)
            {
                temp[indexTmp[i]] = departTime[indexTmp[i]] + PublicValue.Tm[stationNo - 1, PublicValue.level[indexTmp[i]] - 1];
                if (temp[indexTmp[i]] < PublicValue.T_plan[indexTmp[i],stationNo*2-1])
                {
                    temp[indexTmp[i]] = PublicValue.T_plan[indexTmp[i], stationNo * 2 - 1];
                }
                if (i != 0 && temp[indexTmp[i]] < temp[indexTmp[i-1]] + PublicValue.minArriveTime)
                {
                    temp[indexTmp[i]] = temp[indexTmp[i - 1]] + PublicValue.minArriveTime;
                }
            }
            return temp;
        }

        //根据调整站序号、本站到达时间、首次晚点车辆  利用萤火虫算法求最佳发车次序
        //输出调整后发车时间和最佳发车次序
        private int[,] GetIndexFAA(int stationNum, int[] arriveTime, int firstDelayTrainNum)
        {
            int[,] resultData = new int[2, 14]; //用于返回得到的发车时间和发车次序
            int[] departTime = new int[14];
            int[] indexBest = new int[14];

            int[] indexTemp = new int[14];  //存放临时发车顺序
            int[,] departTimeTempTable = new int[PublicValue.fireflyNum, 14];  //所有萤火虫对应发车时间

            int[,] tempX = new int[PublicValue.fireflyNum, 14];      //所有萤火虫排序结果
            double[] delayTime = new double[PublicValue.fireflyNum]; //所有萤火虫下站晚点时刻

            //初始化萤火虫
            tempX = FFAInit(PublicValue.fireflyNum, firstDelayTrainNum);

            //按照列车等级进行重新排序--------------------后面还会用到----------------------
            int temp = 0;  //数据交换
            for (int i = 0; i < PublicValue.fireflyNum; i++)
            {
                for (int j = 0; j < 14 - 1; j++)  //冒泡排序
                {
                    for (int k = 0; k < 14 - 1 - j; k++)
                    {
                        if (PublicValue.level[k] == PublicValue.level[k+1] && tempX[i,k] > tempX[i,k+1])   //如果等级相同但是发车顺序有误 交换
                        {
                            temp = tempX[i, k];
                            tempX[i, k] = tempX[i, k + 1];
                            tempX[i, k + 1] = temp;
                        }
                    }
                }
            }

            //Console.WriteLine("排序后：");
            //PrintArray2(tempX);

            //迭代计算
            for (int i = 0; i < PublicValue.maxGeneration; i++)
            {
                //1、计算绝对亮度
                double[] I = new double[PublicValue.fireflyNum]; //绝对亮度
                for (int k = 0; k < PublicValue.fireflyNum; k++)
                {
                    for (int j = 0; j < 14; j++)
		         	{
                        indexTemp[j] = tempX[k,j];
			        }
                    delayTime[k] = GetNextStationDelayTime(stationNum, arriveTime, indexTemp);
                    I[k] = 1 / delayTime[k];
                    for (int j = 0; j < 14; j++)
                    {
                        departTimeTempTable[k, j] = PublicValue.departTimeTemp[j];
                    }
                }

                //2、按照绝对亮度进行排序
                int[] indexTemp1 = new int[PublicValue.fireflyNum];
                double[] Lightn = new double[PublicValue.fireflyNum];
                I.CopyTo(Lightn, 0); //复制数组
                for (int j = 0; j < PublicValue.fireflyNum; j++)
                {
                    indexTemp1[j] = j;
                }
                Array.Sort(Lightn, indexTemp1);
                Console.WriteLine(1 / Lightn[PublicValue.fireflyNum - 1]);

                //3、记录排序后的亮度和对应序号
                for (int j = 0; j < 14; j++)
                {
                    indexBest[j] = tempX[indexTemp1[PublicValue.fireflyNum - 1], j];
                    departTime[j] = departTimeTempTable[indexTemp1[PublicValue.fireflyNum - 1], j];
                }

                
                //4、萤火虫进行移动
                //生成xn和xo
                int[,] xn = new int[PublicValue.fireflyNum, 14];
                int[,] xo = new int[PublicValue.fireflyNum, 14];
                //按照排序对xn进行赋值
                for (int j = 0; j < PublicValue.fireflyNum; j++)
                {
                    for (int k = 0; k < 14; k++)
                    {
                        xn[j, k] = tempX[indexTemp1[j], k];
                    }
                }
                xo = (int[,])xn.Clone();
                //xn.CopyTo(xo, 0);
                double[] Lighto = new double[PublicValue.fireflyNum];
                Lightn.CopyTo(Lighto, 0);
                //移动
                tempX = faaMove(xn, Lightn, xo, Lighto, stationNum, arriveTime);
            }

            for (int i = 0; i < 14; i++)
            {
                resultData[0, i] = departTime[i];
                resultData[1, i] = indexBest[i];
            }
            return resultData;
        }

        //萤火虫移动
        private int[,] faaMove(int[,] xn, double[] Lightn, int[,] xo, double[] Lighto, int stationNum, int[] arriveTime)
        {
            int[,] result = new int[PublicValue.fireflyNum, 14];

            for (int i = 0; i < PublicValue.fireflyNum; i++)
            {
                for (int j = 0; j < PublicValue.fireflyNum; j++)
                {
                    //计算距离
                    int r = 0;
                    for (int k = 0; k < 14; k++)
                    {
                        if (xn[i,k] != xn[j,k])
                        {
                            r += 1;
                        }
                    }
                    //向亮度更高的萤火虫移动
                    if (Lightn[i] < Lighto[j])
                    {
                        double beta0 = 1;
                        double gamma = 1;
                        double beta = 0;
                        beta = beta0 * Math.Exp(-gamma * r * r);

                        //萤火虫移动
                        int[] Xi = new int[14];
                        int[] Xj = new int[14];
                        int[] temp = new int[14];
                        for (int kk = 0; kk < 14; kk++)
			            {
                            Xi[kk] = xn[i,kk];
                            Xj[kk] = xo[j,kk];
			            }
                        temp = faaExchange(Xi, Xj, beta, stationNum, arriveTime);
                        for (int kk = 0; kk < 14; kk++)
                        {
                            xn[i, kk] = temp[kk];
                        }
                    }
                    
                }
            }

            result = xn;
            return result;
        }

        //萤火虫移动+变异
        private int[] faaExchange(int[] Xi, int[] Xj, double beta, int stationNum, int[] arriveTime)
        {
            int[] X = new int[14];
            for (int i = 0; i < 14; i++)
            {
                X[i] = -1;
            }
            int[] useFlag = new int[14];
            for (int i = 0; i < PublicValue.delayTrainNo - 1; i++)
            {
                X[i] = Xi[i];
                useFlag[X[i]] = 1;
            }

            //二者择- -!
            for (int i = PublicValue.delayTrainNo -1; i < 14; i++)
            {
                Random rd = new Random(Guid.NewGuid().GetHashCode());
                double dResult;
                dResult = rd.NextDouble();
                if (dResult < beta && Array.IndexOf(X, Xi[i]) == -1)
                {
                    X[i] = Xi[i];
                    useFlag[X[i]] = 1;
                }
                else if (Array.IndexOf(X, Xi[i]) == -1)
                {
                    X[i] = Xj[i];
                    useFlag[X[i]] = 1;
                }
            }

            //查缺补漏- -！
            for (int i = 0; i < 14; i++)
            {
                if (X[i] == -1)
                {
                    for (int j = 0; j < 14; j++)
                    {
                        if (useFlag[j] != 1)
                        {
                            X[i] = j;
                            useFlag[j] = 1;
                            break;
                        }
                    }
                }
            }

            //变异部分-------TODO
            //定义两种邻域个数
            int neighbor1 = 2;
            int neighbor2 = 2;
            int[,] temp = new int[neighbor1 + neighbor2, 14]; //所有变异得到的发车顺序
            double[] delayTemp = new double[neighbor1 + neighbor2]; //所有变异得到的晚点时间
            int[] temp1 = new int[14]; //接收变异得到的顺序
            for (int i = 0; i < neighbor1 + neighbor1; i++)
            {
                if (i < neighbor1)
                {
                    temp1 = faaInsert(X, PublicValue.delayTrainNo);
                    for (int j = 0; j < 14 - 1; j++)  //冒泡排序
                    {
                        for (int k = 0; k < 14 - 1 - j; k++)
                        {
                            if (PublicValue.level[k] == PublicValue.level[k + 1] && X[k] > X[k + 1])   //如果等级相同但是发车顺序有误 交换
                            {
                                int temp2 = X[k];
                                X[k] = X[k + 1];
                                X[k + 1] = temp2;
                            }
                        }
                    }
                    for (int j = 0; j < 14; j++)
                    {
                        temp[i, j] = X[j];
                    }
                    delayTemp[i] = GetNextStationDelayTime(stationNum, arriveTime, X);
                }
                else
                {
                    temp1 = faaSwap(X, PublicValue.delayTrainNo);
                    for (int j = 0; j < 14 - 1; j++)  //冒泡排序
                    {
                        for (int k = 0; k < 14 - 1 - j; k++)
                        {
                            if (PublicValue.level[k] == PublicValue.level[k + 1] && X[k] > X[k + 1])   //如果等级相同但是发车顺序有误 交换
                            {
                                int temp2 = X[k];
                                X[k] = X[k + 1];
                                X[k + 1] = temp2;
                            }
                        }
                    }
                    for (int j = 0; j < 14; j++)
                    {
                        temp[i, j] = X[j];
                    }
                    delayTemp[i] = GetNextStationDelayTime(stationNum, arriveTime, X);
                }
            }

            int[] indexTemp1 = new int[neighbor1 + neighbor2];
            for (int i = 0; i < neighbor1 + neighbor2; i++)
            {
                indexTemp1[i] = i;
            }
            Array.Sort(delayTemp, indexTemp1);

            for (int i = 0; i < 14; i++)
            {
                X[i] = temp[indexTemp1[0], i];
            }

            return X;
        }

        private int[] faaInsert(int[] data, int startPoint)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int[] ra = new int[2];
            while (ra[0] == ra[1])
            {
                ra[0] = random.Next(startPoint-1, data.Length);
                ra[1] = random.Next(startPoint-1, data.Length);
            }
            Array.Sort(ra);
            int temp = data[ra[0]];
            for (int i = ra[0]; i < ra[1]; i++)
            {
                data[i] = data[i + 1];
            }
            data[ra[1]] = temp;
            //Console.WriteLine(ra[0] + " " + ra[1]);
            return data;
        }

        private int[] faaSwap(int[] data, int startPoint)
        {
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int[] ra = new int[2];
            while (ra[0] == ra[1])
            {
                ra[0] = random.Next(startPoint - 1, data.Length);
                ra[1] = random.Next(startPoint - 1, data.Length);
            }
            int temp = data[ra[0]];
            data[ra[0]] = data[ra[1]];
            data[ra[1]] = temp;
            return data;
        }

        private double GetNextStationDelayTime(int stationNum,int[] arriveTime, int[] index)
        {
            int[] departTime = new int[14];  //用于存放发车时间

            int[] indexCopy = index;
            int[] indexTemp = new int[14];
            //确定最小发车时间
            for (int i = 0; i < 14; i++)
            {
                departTime[i] = arriveTime[i] + PublicValue.Tz[stationNum, PublicValue.level[i]-1];
                indexTemp[i] = i; //下标矩阵初始化
            }

            //对发车顺序进行排序  并按照最小发车间隔重新计算发车时间和到达时间
            Array.Sort(indexCopy, indexTemp);
            for (int i = 1; i < 14; i++)
            {
                if (departTime[indexTemp[i]] < departTime[indexTemp[i-1]] + PublicValue.minDepartTime)
                {
                    departTime[indexTemp[i]] = departTime[indexTemp[i - 1]] + PublicValue.minDepartTime;
                }
            }

            //按照发车顺序计算下站到达时间
            int[] arriveTimeNext = new int[14];
            for (int i = 0; i < 14; i++)
            {
                arriveTimeNext[indexTemp[i]] = departTime[indexTemp[i]] + PublicValue.Tm[stationNum, PublicValue.level[indexTemp[i]] - 1];
                if (arriveTimeNext[indexTemp[i]] < PublicValue.T_plan[indexTemp[i], stationNum * 2 + 1])
                {
                    arriveTimeNext[indexTemp[i]] = PublicValue.T_plan[indexTemp[i], stationNum * 2 + 1];
                }
                if (i !=0 && arriveTimeNext[indexTemp[i]] - arriveTimeNext[indexTemp[i-1]] < PublicValue.minArriveTime)
                {
                    arriveTimeNext[indexTemp[i]] = arriveTimeNext[indexTemp[i - 1]] + PublicValue.minArriveTime;
                }
            }

            //计算加权晚点时刻
            double delayTime = 0;
            for (int i = 0; i < 14; i++)
            {
                delayTime += (arriveTimeNext[i] - PublicValue.T_plan[i, stationNum * 2 + 1]) * PublicValue.weight[i] * 0.1;
            }
            //参数回传  直接传递晚点时刻  简介传递发车时间
            PublicValue.departTimeTemp = departTime;
            return delayTime;
        }

        //显示二维数组
        private void PrintArray2(int[,] arr)
        {
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    Console.Write(arr[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }


        //初始化萤火虫
        private int[,] FFAInit(int ffaNum, int firstDelayTrainNum)
        {
            int[,] result = new int[ffaNum, 14];
            int[] temp = new int[14];

            for (int i = 0; i < ffaNum; i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    temp[j] = j;
                }
                temp = Shuffle(temp, firstDelayTrainNum);
                for (int j = 0; j < 14; j++)
                {
                    result[i, j] = temp[j];
                }
                
            }

            return result;
        }

        //随机洗牌函数  需要指定起始点
        private int[] Shuffle(int[] array, int startPoint)
        {
            int len = array.Length;
            Random random = new Random(Guid.NewGuid().GetHashCode());
            for (int i = startPoint; i < len; i++)
            {
                int idx = random.Next(i, len);

                //swap elements
                int tmp = array[i];
                array[i] = array[idx];
                array[idx] = tmp;
            }

            return array;
        }
    }
}
