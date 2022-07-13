#define DEBUG
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace tsp
{
    public partial class TspForm : Form
    {
        private int sum = 0;//坐标点的总数
        private static int max = 1000;//数组参数


        public List<xPoint> emShapelist = new List<xPoint>();
        public xPoint stopPt = new xPoint();
        public TspForm()
        {
            InitializeComponent();
        }

        private void File_Click(object sender, EventArgs e)
        {
            string TestFilePath="";
            try
            {
                OpenFileDialog OpenImage = new OpenFileDialog();
                if (OpenImage.ShowDialog() == DialogResult.OK)
                {
                    TestFilePath = OpenImage.FileName;
                }
            }
            catch
            {
                MessageBox.Show("文件打开失败,请选择txt文件");
            }

            string[] sourcetext = System.IO.File.ReadAllLines(
                TestFilePath,
                Encoding.Default);
            Store(sourcetext);
            Stopwatch Swatch = new Stopwatch();
            Swatch.Start();
            TspPlan Plan = new TspPlan(stopPt, emShapelist);
            Swatch.Stop();
            Console.WriteLine(Swatch.Elapsed.ToString());

#if DEBU//test
            Console.WriteLine(sourcetext);
            double[] testX = Plan.GetX();
            double[] testY = Plan.GetY();
            for(int i=0;i<Plan.GetLength();i++)
            {
                Console.WriteLine($"{testX[i]},{testY[i]}");
            }
#endif

            Console.WriteLine($"最短路径为 {Plan.SumDistance()}");
            Output(Plan.GetX(),Plan.GetY());
        }

        //将读入的文件存入数组,根据文件格式可能需要改变
        private void Store(string[] sourcetext)
        {
            sum = 0;
            string tempstring="";
            bool Xflag = true;
            //test
            //Console.WriteLine(sourcetext);

            //遍历sourcetext
            try
            {
                foreach (string num in sourcetext)
                {
                    xPoint point = new xPoint();
                    //判断读入数据为横坐标或纵坐标
                    string[] temp = num.Split(' ');
                    point.XPos = double.Parse(temp[0]);
                    point.YPos = double.Parse(temp[1]);
                    emShapelist.Add(point);
                }
            }
            catch
            {
                MessageBox.Show("文件格式错误");
            }
        }

        private void Output(double[] SpotX, double[] SpotY)
        {

            using(StreamWriter stream= new StreamWriter("结果.txt"))
            {
                for(int i=0;i< emShapelist.Count(); i++)
                {
                    stream.WriteLine("X" + emShapelist[i].XPos + " Y" + emShapelist[i].YPos);
                }
            }
        }

    }
}
