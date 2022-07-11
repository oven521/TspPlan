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
        private double[] SourceX = new double[max];
        private double[] SourceY = new double[max];
        public TspForm()
        {
            InitializeComponent();
        }

        private void File_Click(object sender, EventArgs e)
        {
            string TestFilePath="";
            string sourcetext;
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

            using (StreamReader stream = new StreamReader(TestFilePath))
            {
                sourcetext = stream.ReadToEnd();
            }

            Store(sourcetext);
            Stopwatch Swatch = new Stopwatch();
            Swatch.Start();
            TspPlan Plan = new TspPlan(SourceX, SourceY, sum);
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
        private void Store(string sourcetext)
        {
            sum = 0;
            string tempstring="";
            bool Xflag = true;
            //test
            //Console.WriteLine(sourcetext);

            //遍历sourcetext
            try
            {
                foreach (char num in sourcetext)
                {
                    //判断读入数据为横坐标或纵坐标
                    if ((num == '\r' || num == '\n' || num == ' ')&& tempstring != "")
                    {
                        if (Xflag)
                        {
                            SourceX[sum] = double.Parse(tempstring);
                        }
                        else
                        {
                            SourceY[sum] = double.Parse(tempstring);
                            sum++;
                        }
                        tempstring = "";
                    }
                    else if(num=='X')
                    {
                        Xflag = true;
                    }
                    else if(num=='Y')
                    {
                        Xflag = false;
                    }
                    else
                    {
                        tempstring += num;
                    }
                }
                if(tempstring != "")
                {
                    SourceY[sum] = double.Parse(tempstring);
                    sum++;
                }
            }
            catch
            {
                MessageBox.Show("文件格式错误");
            }
        }

        private void Output(double[] SpotX, double[] SpotY)
        {

            using(StreamWriter stream= new StreamWriter("路径结果.txt"))
            {
                for(int i=0;i<sum;i++)
                {
                    stream.WriteLine("X" + SpotX[i] + " Y" + SpotY[i]);
                }
            }
        }

    }
}
