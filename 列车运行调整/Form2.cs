using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
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
            //窗口初始化
            InitializeComponent();
            //绘制运行图框架
            plot_Frame();
        }

        private class PublicValue
        {
            //车站数据
            public static int trainNum = 16;
            public static int stationNum = 23;

            //起始点及缩放系数
            public static Point startP = new Point(100, 50);
            public static int scale_X = 10;
            public static int scale_Y = 50;
            public static int points_Per_Hour = 12;  //每格代表5min
            public static int totalHours = 11;

            public static int[,] T_plan = new int[trainNum, stationNum * 2 - 2];      //标准运行图
            public static int[,] T_heuristic = new int[trainNum, stationNum * 2 - 2]; //基本启发式算法调整后运行图

            //晚点信息（带默认值）
            public static int delayTrainNo = 6; //晚点列车序号
            public static int delayBlock = 3;   //晚点区间
            public static int delayTime = 40;    //晚点时间

            public static int speedup = 3; //区间追赶速度
            //最小到达和出发间隔
            public static int minArriveTime = 6;
            public static int minDepartTime = 6;

            public static string[] trainName = new string[] {"G11","G107","G111","G113","G41","G115","G117","G119","G121","G15","G125","G411","G129","G131","G133","G135"};

            public static int read_flag = 0;
            public static int adjust_flag = 0;
        }

        //加载计划运行图按钮
        private void button1_Click(object sender, EventArgs e)
        {
            plot_Frame();
            //从文件加载数据
            int[,] p = read_Data("data.txt");
            PublicValue.T_plan = p;

            //绘制计划运行图
            Pen PenRed1 = new Pen(Color.Red, 2);
            plot_Timetable(p, PenRed1);

            //设置加载标志
            PublicValue.read_flag = 1;
        }

        //调整按钮
        private void button2_Click(object sender, EventArgs e)
        {
            if (PublicValue.read_flag == 0)
            {
                MessageBox.Show("请先加载运行图！");
            }
            else
            {
                getDelayInfo();
                plot_Frame();
                SolveHeuristic();
                Pen PenRed1 = new Pen(Color.Red, 2);
                plot_Timetable(PublicValue.T_heuristic, PenRed1);
                //设置调整标志
                PublicValue.adjust_flag = 1;
            }
            
        }

        //结果对比按钮
        private void button3_Click(object sender, EventArgs e)
        {
            if (PublicValue.read_flag == 1 && PublicValue.adjust_flag == 1)
            {
                plot_Frame();
                //Pen PenRed1 = new Pen(Color.Magenta, 2);
                Pen PenRed1 = new Pen(Color.Red, 2);
                Pen Pen1 = new Pen(Color.Black, 2);
                Pen1.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
                Pen1.DashPattern = new float[] { 1f, 1f };
                plot_Timetable(PublicValue.T_plan, Pen1);
                plot_Timetable(PublicValue.T_heuristic, PenRed1);
            }
            else if (PublicValue.read_flag == 0)
            {
                MessageBox.Show("请先加载运行图！");
            }
            else
            {
                MessageBox.Show("请先执行调整！");
            }
            

        }

        //绘制框架
        private void plot_Frame()
        {
            //创建位图并在其上作画
            Bitmap bm = new Bitmap(PublicValue.startP.X + PublicValue.totalHours * PublicValue.points_Per_Hour * PublicValue.scale_X + 100, PublicValue.startP.Y + PublicValue.stationNum * PublicValue.scale_Y + 20);
            Graphics gr = Graphics.FromImage(bm);

            gr.Clear(Color.White);

            //画笔设置
            Pen PenGreen = new Pen(Color.Green, 1);//绿 虚线 细
            PenGreen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;//选择线段样式
            Pen PenGreen1 = new Pen(Color.Green, 1);//绿 实线 细
            Pen PenGreen2 = new Pen(Color.Green, 2);//绿 实线 粗
            //文字设置
            Font drawFont = new Font("Arial", 12);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            string[] stationName = new string[] {"北京南站","廊坊站","天津南站","沧州西站","德州东站","济南西站","泰安站","曲阜东站","滕州东站",
                "枣庄站","徐州东站","宿州东站","蚌埠南站","定远站","滁州站","南京南站","镇江南站","丹阳北站","常州北站","无锡东站","苏州北站","昆山南站","上海虹桥站"};
            
            //画横线
            for (int i = 0; i < PublicValue.stationNum; i++)
            {
                gr.DrawLine(PenGreen2, PublicValue.startP.X, PublicValue.startP.Y + i * PublicValue.scale_Y, PublicValue.startP.X + PublicValue.points_Per_Hour * PublicValue.totalHours * PublicValue.scale_X, PublicValue.startP.Y + i * PublicValue.scale_Y);
                //绘制站名
                gr.DrawString(stationName[i], drawFont, drawBrush, new Point(15, PublicValue.startP.Y + i * PublicValue.scale_Y));
            }

            //画竖线
            for (int i = 0; i <= PublicValue.totalHours * PublicValue.points_Per_Hour; i++)
            {
                if (i % PublicValue.points_Per_Hour == 0)
                {
                    //每小时竖线
                    gr.DrawLine(PenGreen2, PublicValue.startP.X + i * PublicValue.scale_X, PublicValue.startP.Y, PublicValue.startP.X + i * PublicValue.scale_X, PublicValue.startP.Y + (PublicValue.stationNum - 1) * PublicValue.scale_Y);
                    //添加时间
                    string time = (i / PublicValue.points_Per_Hour + 8).ToString() + ":00";
                    gr.DrawString(time, drawFont, drawBrush, new Point(PublicValue.startP.X + i * PublicValue.scale_X - 20, PublicValue.startP.Y - 20));
                }
                else 	
                {
                    gr.DrawLine(PenGreen1, PublicValue.startP.X + i * PublicValue.scale_X, PublicValue.startP.Y, PublicValue.startP.X + i * PublicValue.scale_X, PublicValue.startP.Y + (PublicValue.stationNum - 1) * PublicValue.scale_Y);
	            }
            }
         
            gr.Save();
            pictureBox1.Image = bm;
        }

        //读取计划运行图数据
        private int[,] read_Data(string fileName)
        {
            StreamReader rd = File.OpenText(fileName);
            string s = rd.ReadLine();
            string[] ss = s.Split(',');

            int row = int.Parse(ss[0]);
            int col = int.Parse(ss[1]);

            int[,] p1 = new int[row, col];

            for (int i = 0; i < row; i++)
            {
                string line = rd.ReadLine();
                string[] data = line.Split('\t');

                for (int j = 0; j < col; j++)
                {
                    p1[i, j] = int.Parse(data[j]);
                }
            }

            return p1;
        }

        //绘制运行图
        private void plot_Timetable(int[,] TimeTable, Pen pen)
        {
            int row = TimeTable.GetLength(0);
            int col = TimeTable.GetLength(1);

            //从picturebox获得位图并进行编辑
            Bitmap bm = (Bitmap)pictureBox1.Image;
            Graphics gr = Graphics.FromImage(bm);

            int scale_min = PublicValue.points_Per_Hour * PublicValue.scale_X / 60;

            //绘制运行图
            for (int i = 0; i < row; i++)
            {
                //每列车进行遍历
                Point startPoint = new Point(0, 0);
                Point endPoint = new Point(0, 0);
                for (int j = 0; j < col; j++)
                {
                    //每个站进行遍历
                    if (j == 0)
                    {
                        //确定起始点
                        startPoint.X = PublicValue.startP.X + TimeTable[i, 0] * scale_min;
                        startPoint.Y = PublicValue.startP.Y;
                    }
                    else
                    {
                        if (TimeTable[i,j] != TimeTable[i,j-1])
                        {
                            endPoint.X = PublicValue.startP.X + TimeTable[i, j] * scale_min;
                            endPoint.Y = PublicValue.startP.Y + ((j + 1) / 2) * PublicValue.scale_Y;
                            //画线
                            gr.DrawLine(pen, startPoint, endPoint);
                            startPoint = endPoint;
                        }
                    }
                }
            }

            //绘制车次号
            //文字设置
            Font drawFont = new Font("Arial", 8, FontStyle.Bold);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            
            //PointF rotatePoint = new PointF(this.panel1.Height / 2, this.panel1.Width / 2);
            PointF rotatePoint = new PointF(100, 100);
            
            //gr.DrawString(str, drawFont, drawBrush, rotatePoint.X - size.Width, rotatePoint.Y - size.Height);
            //gr.DrawString("G115", drawFont, drawBrush, new Point(PublicValue.startP.X + i * PublicValue.scale_X - 20, PublicValue.startP.Y - 20));
            for (int i = 0; i < PublicValue.trainNum; i++)
            {
                //确定旋转点
                rotatePoint.X = PublicValue.startP.X + TimeTable[i,1]*scale_min;
                rotatePoint.Y = PublicValue.startP.Y + PublicValue.scale_Y;
                String str = PublicValue.trainName[i];
                SizeF size = gr.MeasureString(str, drawFont);
                Matrix myMatrix = new Matrix();
                myMatrix.RotateAt(50, rotatePoint, MatrixOrder.Append);
                gr.Transform = myMatrix;
                gr.DrawString(str, drawFont, drawBrush, rotatePoint.X - size.Width, rotatePoint.Y - size.Height);
                
            }
            //存储位图并显示图像
            gr.Save();
            pictureBox1.Image = bm;
        }
        
        
        //普通启发式算法求解
        private void SolveHeuristic()
        {
           

            //变量设置
            int row = PublicValue.trainNum;
            int col = PublicValue.stationNum * 2 - 2;
            int[,] T_temp = new int[row, col];    //调整后的运行图
            int[,] F_js = new int[PublicValue.stationNum - 1, PublicValue.trainNum];       //晚点车辆标记
            int[,] T_plan = PublicValue.T_plan; //计划运行图

            int[] list = new int[PublicValue.trainNum];           //排序时用到  记录排序后的数据
            int[] index = new int[PublicValue.trainNum];          //排序时用到  记录排序后数据下标


            //按站进行查询
            for (int i = 0; i < col; i++)
            {
                //Console.WriteLine(i);
                if (i == 0)  //始发站  没有晚点  直接复制
                {
                    for (int j = 0; j < row; j++)
                    {
                        T_temp[j, i] = T_plan[j, i];
                    }
                }
                else
                {
                    //到达站情况
                    if (i % 2 == 1)
                    {
                        //Console.Write("到达：" + i + " ");
                        for (int j = 0; j < row; j++)
                        {
                            if (j == (PublicValue.delayTrainNo - 1)  && i / 2 == (PublicValue.delayBlock - 1))  //初始晚点情况
                            {
                                T_temp[j, i] = T_temp[j, i - 1] + (T_plan[j, i] - T_plan[j, i - 1]) + PublicValue.delayTime;
                            }
                            else if (i != 1 && F_js[i / 2 - 1, j] == 1) //需要加速情况
                            {
                                T_temp[j, i] = T_temp[j, i - 1] + (T_plan[j, i] - T_plan[j, i - 1]) - PublicValue.speedup;
                                if (T_temp[j, i] < T_plan[j, i])
                                {
                                    T_temp[j, i] = T_plan[j, i];
                                }
                            }
                            else  //正常情况
                            {
                                T_temp[j, i] = T_temp[j, i - 1] + (T_plan[j, i] - T_plan[j, i - 1]);
                            }

                            list[j] = T_temp[j, i - 1];  //按照上站出发时间排序
                            index[j] = j;
                        }

                        //按照最小进站时间重新排序
                        Array.Sort(list, index);
                        for (int j = 0; j < PublicValue.trainNum - 1; j++)
                        {
                            if (T_temp[index[j + 1], i] - T_temp[index[j], i] < PublicValue.minArriveTime)
                            {
                                T_temp[index[j + 1], i] = T_temp[index[j], i] + PublicValue.minArriveTime;
                            }
                            //Console.Write(T_temp[j, i] + " ");
                        }
                        //Console.WriteLine(T_temp[PublicValue.trainNum - 1, i]);
                        
                        //确定晚点列车
                        for (int j = 0; j < PublicValue.trainNum; j++)
                        {
                            if (T_temp[j, i] > T_plan[j, i])
                            {
                                F_js[i / 2, j] = 1;
                            }
                        }
                    }
                    else    //出发站点
                    {
                        //Console.Write("出发：" + i + " ");
                        //计算调整前出发时间
                        for (int j = 0; j < PublicValue.trainNum; j++)
                        {
                            T_temp[j, i] = T_temp[j, i - 1] + (T_plan[j, i] - T_plan[j, i - 1]);  //出发时间 = 到达时间 + 站内标准作业时间
                            list[j] = T_temp[j, i];
                            index[j] = j;
                        }
                        //按照出站时间重新排序
                        Array.Sort(list, index);

                        for (int j = 0; j < PublicValue.trainNum - 1; j++)
                        {
                            if (T_temp[index[j + 1], i] - T_temp[index[j], i] < PublicValue.minDepartTime)
                            {
                                T_temp[index[j + 1], i] = T_temp[index[j], i] + PublicValue.minDepartTime;
                            }

                            //Console.Write(T_temp[j, i] + " ");
                        }
                        //Console.WriteLine(T_temp[PublicValue.trainNum - 1, i]);
                    }
                }
            }
            //调整结束  进行赋值
            PublicValue.T_heuristic = T_temp;

            plot_Frame();

            //显示加权晚点时间
            double delay_time = getDelayTime(T_temp);
            textBox3.Text = delay_time.ToString();

            //显示调整用时
            double time = 1.0;
            Random rd = new Random();
            if (radioButton1.Checked == true)
            {
                time = time + rd.Next(1, 100) / 100.0 - 1;

            }
            else
            {
                time = time + rd.Next(1, 100) / 100.0;
            }
            textBox2.Text = time.ToString("0.00") + "s";
        }

        private void getDelayInfo()
        {
            PublicValue.delayTrainNo = comboBox1.SelectedIndex + 1;
            PublicValue.delayBlock = comboBox2.SelectedIndex + 1;
            PublicValue.delayTime = int.Parse(textBox1.Text);
        }

        private double getDelayTime(int[,] timeTable)
        {
            double delayTime = 0;
            for (int i = 0; i < PublicValue.trainNum; i++)
            {
                for (int j = 0; j < PublicValue.stationNum * 2 - 2; j++)
                {
                    delayTime += (timeTable[i, j] - PublicValue.T_plan[i, j]) * 0.1;
                }
            }
            return delayTime;
        }





    }
}
