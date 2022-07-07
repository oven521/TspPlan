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

namespace tsp
{
    public partial class TspForm : Form
    {

        private int sum = 0;
        private static int max = 2000;
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
                    //MessageBox.Show(TestFilePath);
                }

            }
            catch
            {
                MessageBox.Show("文件打开失败");
            }
            //test
            //Console.WriteLine(TestFilePath);

            using (StreamReader stream = new StreamReader(TestFilePath))
            {
                
                sourcetext = stream.ReadToEnd();

            }

            Store(sourcetext);
            TspPlan Plan = new TspPlan(SourceX, SourceY, sum);
#if DEBUG
            Console.WriteLine(sourcetext);
            double[] testX = Plan.GetX();
            double[] testY = Plan.GetY();
            for(int i=0;i<Plan.GetLength();i++)
            {
                Console.WriteLine($"{testX[i]},{testY[i]}");
            }

#endif


            Console.WriteLine($"最短路径为 {Plan.SumDistance()}");
            
        }

        //将读入的文件存入数组,根据文件格式可能需要改变
        private void Store(string sourcetext)
        {
            sum = 0;
            string tempstring="";
            bool Xflag = true;
            //遍历sourcetext
            foreach (char num in sourcetext)
            {
                //判断读入数据为横坐标或纵坐标,(目前不能读取换行符)
                if(num==' '&& tempstring!="")
                {
                    if (Xflag)
                    {
                        SourceX[sum] = double.Parse(tempstring);
                        Xflag = false;
                    }
                    else
                    {
                        SourceY[sum] = double.Parse(tempstring);
                        Xflag = true;
                        sum++;
                    }
                    tempstring = "";
                }
                else
                {
                    tempstring += num;
                }

            }
            //
            
        }
    }
}
