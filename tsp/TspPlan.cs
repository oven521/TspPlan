using System;
using System.Collections.Generic;
using System.Linq;

namespace tsp
{
    public class xPoint
    {
        public double XPos;
        public double YPos;
    }
    class TspPlan
    {//GetPt()获取排序后的点
        private List<xPoint> PointList = new List<xPoint>();

        //1代表贪心，0代表改良圈，2代表先贪心后改良圈
        private int ChoiceFlag = 2;
        //总距离
        private double sumDistance = 0;

        public TspPlan(xPoint stopPt, List<xPoint> emShapelist)
        {
            PointList = emShapelist;

            double min = Distance(stopPt.XPos, stopPt.YPos, PointList[0].XPos, PointList[0].YPos);
            int StartPt = 0;//设置起点
            for (int i = 1; i < PointList.Count() - 1; i++)
            {
                double cur = Distance(stopPt.XPos, stopPt.YPos, PointList[i].XPos, PointList[i].YPos);
                if (min > cur)
                {
                    min = cur;
                    StartPt = i;
                }
            }
            xPoint temp = PointList[StartPt];
            PointList[StartPt] = PointList[0];
            PointList[0] = temp;

            if (ChoiceFlag == 1)
            {
                greed();
            }
            else if (ChoiceFlag == 0)
            {
                CircleModification();
            }
            else if (ChoiceFlag == 2)
            {
                greed();
                CircleModification();
            }
        }

        private double Distance(double source_x, double source_y, double target_x, double target_y)
        {
            return Math.Sqrt(Math.Pow(source_x - target_x, 2) + Math.Pow(source_y - target_y, 2));
        }

        public List<xPoint> GetPt() => PointList;
        public int GetCount() => PointList.Count();
        public double SumDistance() => sumDistance;
        //计算总距离
        private void CalculateSumDistance()
        {
            sumDistance = 0;
            for (int i = 0; i < PointList.Count() - 1; i++)
            {
                sumDistance += Distance(PointList[i].XPos, PointList[i].YPos, PointList[i + 1].XPos, PointList[i + 1].YPos);
            }
        }
        //颠倒路径中的顺序
        private void Reverse(int begin, int end)
        {
            for (int i = begin; i < (begin + end) / 2 + 1; i++)
            {
                //交换x，y坐标
                xPoint temp = PointList[end - i + begin];
                PointList[end - i + begin] = PointList[i];
                PointList[i] = temp;
            }
        }

        //改良圈算法
        private void CircleModification()
        {
            int StopFlag;//停止标志
            int CycleIndex = 0;//循环次数
            CalculateSumDistance();//计算总长度
            for (int k = 0; k < PointList.Count(); k++)
            {
                StopFlag = 0;
                //遍历一轮
                for (int i = 0; i < PointList.Count() - 2; i++)
                {
                    for (int j = i + 2; j < PointList.Count() - 1; j++)
                    {
                        CycleIndex++;
                        //比较颠倒顺序前后的总路线长
                        double Newpath = Distance(PointList[i].XPos, PointList[i].YPos, PointList[j].XPos, PointList[j].YPos) + Distance(PointList[i + 1].XPos, PointList[i + 1].YPos, PointList[j + 1].XPos, PointList[j + 1].YPos);
                        double Oldpath = Distance(PointList[i].XPos, PointList[i].YPos, PointList[i + 1].XPos, PointList[i + 1].YPos) + Distance(PointList[j].XPos, PointList[j].YPos, PointList[j + 1].XPos, PointList[j + 1].YPos);
                        if (Newpath < Oldpath)
                        {
                            StopFlag++;
                            sumDistance += Newpath - Oldpath;//更新总长度
                            Reverse(i + 1, j);
                        }
                        //Console.WriteLine(sumDistance);
                        //Console.WriteLine(CycleIndex);
                    }
                    //Console.WriteLine("------------------------------");
                }
                if (StopFlag == 0)
                {
                    break;
                }
            }
            //Console.WriteLine(testcircle);
        }

        private void greed()
        {
            int testcircle = 0;//循环次数
            for (int i = 0; i < PointList.Count() - 1; i++)
            {
                double min = Distance(PointList[i].XPos, PointList[i].YPos, PointList[i + 1].XPos, PointList[i + 1].YPos);
                int NearestPoint = i + 1;
                //遍历list获取当前点的最近点
                for (int j = i + 1; j < PointList.Count(); j++)
                {
                    testcircle++;
                    if (Distance(PointList[i].XPos, PointList[i].YPos, PointList[j].XPos, PointList[j].YPos) < min)
                    {
                        NearestPoint = j;
                        min = Distance(PointList[i].XPos, PointList[i].YPos, PointList[j].XPos, PointList[j].YPos);
                    }
                }
                //交换x，y坐标
                xPoint temp = PointList[i + 1];
                PointList[i + 1] = PointList[NearestPoint];
                PointList[NearestPoint] = temp;

            }
            //Console.WriteLine(testcircle);
            CalculateSumDistance();
            //Console.WriteLine(sumDistance);
        }
    }
}
