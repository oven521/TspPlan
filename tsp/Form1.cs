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
        public List<xPoint> emShapelist = new List<xPoint>();
        public xPoint stopPt = new xPoint();

        private int ChoiceFlag = 2;
        public TspForm()
        {
            InitializeComponent();
        }

        private void File_Click(object sender, EventArgs e)
        {
            string TestFilePath = "";
            try
            {
                OpenFileDialog OpenImage = new OpenFileDialog();
                if (OpenImage.ShowDialog() == DialogResult.OK)
                {
                    TestFilePath = OpenImage.FileName;
                }
                string[] sourcetext = System.IO.File.ReadAllLines(
                TestFilePath,
                Encoding.Default);
                Store(sourcetext);
            }
            catch
            {
                MessageBox.Show("文件打开失败,请选择txt文件");
            }
            Stopwatch Swatch = new Stopwatch();
            Swatch.Start();
            TspPlan Plan = new TspPlan(stopPt, emShapelist, ChoiceFlag);
            Swatch.Stop();
            Console.WriteLine(Swatch.Elapsed.ToString());

            Console.WriteLine($"最短路径为 {Plan.SumDistance()}");
            Output(emShapelist);
        }

        //将读入的文件存入数组,根据文件格式可能需要改变
        private void Store(string[] sourcetext)
        {
            //遍历sourcetext
            try
            {
                foreach (string num in sourcetext)
                {
                    xPoint point = new xPoint();
                    //判断读入数据为横坐标或纵坐标
                    string[] temp = num.Split(' ');
                    point.XPos = float.Parse(temp[0]);
                    point.YPos = float.Parse(temp[1]);
                    emShapelist.Add(point);
                }
            }
            catch
            {
                MessageBox.Show("文件格式错误");
            }
        }

        private void Output(List<xPoint> xPoints)
        {

            using (StreamWriter stream = new StreamWriter("结果.txt"))
            {
                for (int i = 0; i < xPoints.Count(); i++)
                {
                    stream.WriteLine(xPoints[i].XPos + " " + xPoints[i].YPos);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "贪心": ChoiceFlag = 1; break;
                case "改良圈": ChoiceFlag = 0; break;
                case "改良圈+贪心": ChoiceFlag = 2; break;
                case "双生成树":; break;
                case "最小权匹配":; break;


            }


        }
    }
}
