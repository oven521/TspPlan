

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


namespace tsp
{
    public class xPoint
    {
        public double XPos;
        public double YPos;
    }
    class TspPlan
    {
        private int max = 1000;
        //后续可能会改成结构体或者链表的结构

        private List<xPoint> SpotList = new List<xPoint>();
        private xPoint[] TargetSpot;
        //private LinkedList<xPoint> LinkedSpots;
        private double[] SourceX;
        private double[] SourceY;

        //test:1代表贪心，0代表改良圈，2代表先贪心后改良圈
        int ChoiceFlag = 0;
        //点个数
        private int SpotNum = 0;
        //总距离
        private double sumDistance = 0;

        public TspPlan(xPoint stopPt,List<xPoint> emShapelist)
        {
            TargetSpot = new xPoint[max];
            SpotList = emShapelist;

            double min = Distance(stopPt.XPos, stopPt.YPos, SpotList[0].XPos, SpotList[0].YPos);
            int target=0;
            for (int i=1;i<SpotList.Count()-1;i++)
            {
                double cur = Distance(stopPt.XPos, stopPt.YPos, SpotList[i].XPos, SpotList[i].YPos);
                if (min > cur) 
                {
                    min = cur;
                    target = i;
                }

            }
            xPoint temp = SpotList[target];
            SpotList[target] = SpotList[0];
            SpotList[0] = temp;

            if (ChoiceFlag==2)
            {
               greed();
            }
            else if(ChoiceFlag==0)
            {
               CircleModification();
            }
            else if(ChoiceFlag==2)
            {
                greed();
                CircleModification();
            }
        }

        public double Distance(double source_x, double source_y, double target_x, double target_y)
        {
            return Math.Sqrt(Math.Pow(source_x - target_x, 2) + Math.Pow(source_y - target_y, 2));
        }

        public double[] GetX() => SourceX;
        public double[] GetY() => SourceY;
        public int GetLength() => SpotNum;
        public double SumDistance() => sumDistance;

        private void CalculateSumDistance()
        {
            sumDistance = 0;
            for (int i = 0; i < SpotNum - 1; i++)
            {
                sumDistance += Distance(SourceX[i], SourceY[i], SourceX[i + 1], SourceY[i + 1]);
            }
        }
        //颠倒路径中的顺序 用链表处理可能会快点
        private void Reverse(int begin, int end)
        {
            for (int i = begin; i < (begin + end) / 2+1; i++)
            {
                //交换x，y坐标
                xPoint temp = SpotList[end - i + begin];
                SpotList[end - i + begin] = SpotList[i];
                SpotList[i] = temp;
            }
        }
        
        //改良圈算法
        private void CircleModification()
        {
            int StopFlag;//停止标志
            int testcircle = 0;//循环次数
            CalculateSumDistance();
            for (int k=0;k< SpotNum; k++)
            {
                StopFlag = 0;
                //遍历一轮
                for (int i = 0; i < SpotNum - 2; i++)
                {
                    for (int j = i + 2; j < SpotNum - 1; j++)
                    {
                         testcircle++;
                        //比较颠倒顺序前后的总路线长
                        double Newpath = Distance(SpotList[i].XPos, SpotList[i].YPos, SpotList[j].XPos, SpotList[j].YPos) + Distance(SpotList[i + 1].XPos, SpotList[i + 1].YPos, SpotList[j + 1].XPos, SpotList[j + 1].YPos);
                        double Oldpath = Distance(SpotList[i].XPos, SpotList[i].YPos, SpotList[i + 1].XPos, SpotList[i + 1].YPos) + Distance(SpotList[j].XPos, SpotList[j].YPos, SpotList[j + 1].XPos, SpotList[j + 1].YPos);
                        if (Newpath < Oldpath)
                        {
                            StopFlag++;
                            sumDistance += Newpath - Oldpath;
                            Reverse(i + 1, j);//将i+1和j之间的顺序颠倒
                        }
                        //Console.WriteLine(sumDistance);
                    }
                    //Console.WriteLine("------------------------------");
                }
                if (StopFlag == 0)
                {
                    break;
                }
            }
            Console.WriteLine(testcircle);
            //CalculateSumDistance();
        }

        private void greed()
        {
            int testcircle = 0;
            for (int i=0;i<SpotNum-1;i++)
            {
                double min = Distance(SpotList[i].XPos, SpotList[i].YPos, SpotList[i+1].XPos, SpotList[i+1].YPos);
                int MinSpot=i+1;
                for (int j=i+1;j<SpotNum;j++)
                {
                    testcircle++;//test
                    if (Distance(SpotList[i].XPos, SpotList[i].YPos, SpotList[j].XPos, SpotList[j].YPos)<min)
                    {
                        MinSpot = j;
                        min = Distance(SpotList[i].XPos, SpotList[i].YPos, SpotList[j].XPos, SpotList[j].YPos);
                    }
                }
                //交换x，y坐标
                xPoint temp = SpotList[i + 1];
                SpotList[i + 1] = SpotList[MinSpot];
                SpotList[MinSpot] = temp;

            }
            Console.WriteLine(testcircle);
            CalculateSumDistance();
            //Console.WriteLine(sumDistance);
        }
    }
}
