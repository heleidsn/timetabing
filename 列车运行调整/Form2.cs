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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            plot_Frame();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int[,] p = read_Data("data.txt");
            Console.WriteLine(p[1, 2]);
            plot_Timetable(p);
        }

        //绘制框架
        private void plot_Frame()
        {
            //起始点及缩放系数
            Point startP = new Point(100, 50);
            int scale_X = 10;
            int scale_Y = 50;
            int trainNum = 16;
            int stationNum = 23;
            int totalHours = 10;
            int points_Per_Hour = 12;  //每格代表5min
            

            //创建位图并在其上作画
            Bitmap bm = new Bitmap(startP.X + totalHours * points_Per_Hour * scale_X + 100, startP.Y + stationNum * scale_Y + 20);
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
            for (int i = 0; i < stationNum; i++)
            {
                gr.DrawLine(PenGreen2, startP.X, startP.Y + i * scale_Y, startP.X + points_Per_Hour * totalHours * scale_X, startP.Y + i * scale_Y);
                //绘制站名
                gr.DrawString(stationName[i], drawFont, drawBrush, new Point(15, startP.Y + i * scale_Y));
            }

            //画竖线
            for (int i = 0; i <= totalHours * points_Per_Hour; i++)
            {
                if (i % points_Per_Hour == 0)
                {
                    //每小时竖线
                    gr.DrawLine(PenGreen2, startP.X + i*scale_X, startP.Y, startP.X + i * scale_X, startP.Y + (stationNum - 1) * scale_Y);
                    //添加时间
                    string time = (i/points_Per_Hour + 8).ToString() + ":00";
                    gr.DrawString(time, drawFont, drawBrush, new Point(startP.X + i * scale_X - 20, startP.Y - 20));
                }
                else 	
                {
                    gr.DrawLine(PenGreen1, startP.X + i * scale_X, startP.Y, startP.X + i * scale_X, startP.Y + (stationNum - 1) * scale_Y);
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
        private void plot_Timetable(int[,] TimeTable)
        {
            int row = TimeTable.GetLength(0);
            int col = TimeTable.GetLength(1);

            //从picturebox获得位图并进行编辑
            Bitmap bm = (Bitmap)pictureBox1.Image;
            Graphics gr = Graphics.FromImage(bm);

            //起始点及缩放系数
            Point startP = new Point(100, 50);
            int scale_X = 10;
            int scale_Y = 50;
            int trainNum = 16;
            int stationNum = 23;
            int totalHours = 10;
            int points_Per_Hour = 12;  //每格代表5min

            Pen PenGreen2 = new Pen(Color.Green, 2);//绿 实线 粗
            //gr.DrawLine(PenGreen2, 10,10,210,210);

            Pen PenRed1 = new Pen(Color.Magenta, 2);
            int scale_min = points_Per_Hour * scale_X / 60;
            int[] Time = { 0, 92, 94, 229, 231, 281, 283, 306 };
            gr.DrawLine(PenRed1, startP.X, startP.Y, startP.X + 92 * scale_min, startP.Y + scale_Y * 5);
            gr.DrawLine(PenRed1, startP.X + 92 * scale_min, startP.Y + scale_Y * 5, startP.X + 94 * scale_min, startP.Y + scale_Y * 5);
            gr.DrawLine(PenRed1, startP.X + 94 * scale_min, startP.Y + scale_Y * 5, startP.X + 229 * scale_min, startP.Y + scale_Y * 15);
            gr.DrawLine(PenRed1, startP.X + 229 * scale_min, startP.Y + scale_Y * 15, startP.X + 232 * scale_min, startP.Y + scale_Y * 15);
            gr.DrawLine(PenRed1, startP.X + 232 * scale_min, startP.Y + scale_Y * 15, startP.X + 281 * scale_min, startP.Y + scale_Y * 18);
            gr.DrawLine(PenRed1, startP.X + 281 * scale_min, startP.Y + scale_Y * 18, startP.X + 283 * scale_min, startP.Y + scale_Y * 18);
            gr.DrawLine(PenRed1, startP.X + 283 * scale_min, startP.Y + scale_Y * 18, startP.X + 306 * scale_min, startP.Y + scale_Y * 22);

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
                        startPoint.X = startP.X;
                        startPoint.Y = startP.Y + TimeTable[i, 0] * scale_min;
                    }

                }
            }

            gr.Save();
            pictureBox1.Image = bm;

        }
    }
}
