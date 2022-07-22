/*
改进方向：不设置起点终点，按照回路的方式进行计算（即终点和起点相同）

目前正在写的算法：凸包算法
考虑到观感性，凸包算法理论上不会产生交叉
    时间复杂度：O(n^2*lg(n))
Step 1：构造凸包，并将它作为最初的子路径
Step 2：对于所有不在子路径中的点r，找到其相应的在子路径中的点i，j，使得Distance[i][r]+Distance[r][j]-Distance[i][j] 最小
Step 3：对于Step 2中找到的所有（i，j，0r），找到使（Distance[i][r]+Distance[r][j]）/Distance[i][j] 最小的那一组，将这组的r插入到i和j中间
Step 4：回到Step 2，直到所有的点都加入到路径中

考虑写一个动态规划找最优解
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using CADEngine.DrawingObject;
using CADEngine.DrawingState;
using CADEngine;
using ContourProgramming;
using System.IO;
using _05_SplineFunction;
using System.Diagnostics;

namespace Tsp
{
    public class xPoint
    {
        public float XPos;
        public float YPos;
    }
    //GetPt()获取排序后的点
    public class TspPlan
    {
        private List<xPoint> PointList = new List<xPoint>();

        private List<List<double>> DistanceMap = new List<List<double>>();
        //总距离
        private double sumDistance = 0;
        //1代表贪心，0代表改良圈，2代表先贪心后改良圈
        public TspPlan(xPoint stopPt, List<xPoint> emShapelist, int ChoiceFlag = 2)
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


            //
            if (ChoiceFlag == 1)//单贪心
            {
                NearestNeighbor();
            }
            else if (ChoiceFlag == 0)//单改良圈
            {
                double max = 0;
                int EndPt = 0;//设置终点
                for (int i = 1; i < PointList.Count() - 1; i++)
                {
                    double cur = Distance(stopPt.XPos, stopPt.YPos, PointList[i].XPos, PointList[i].YPos);
                    if (max < cur)
                    {
                        max = cur;
                        EndPt = i;
                    }
                }
                temp = PointList[EndPt];
                PointList[EndPt] = PointList[PointList.Count() - 1];
                PointList[PointList.Count() - 1] = temp;

                CircleModification();
            }
            else if (ChoiceFlag == 2)//贪心+改良圈，由于贪心本身能达到局部较好解，使得改良圈迭代次数更少，所以可能会出现贪心+改良圈比单贪心运行效率更高的情况
            {
                NearestNeighbor();
                CircleModification();
            }
            else if (ChoiceFlag == 3)//双生成树
            {
                CaculateDistanceMap();
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
        /// <summary>
        /// 改良圈算法
        /// </summary>
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

                }
                if (StopFlag == 0)
                {
                    break;
                }
            }
            //Console.WriteLine(testcircle);
        }
        /// <summary>
        /// 最近邻算法
        /// </summary>
        private void NearestNeighbor()
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

        //凸包算法
        // 时间复杂度：O(n^2*lg(n))
        //Step 1：构造凸包，并将它作为最初的子路径

        //Step 2：对于所有不在子路径中的点r，找到其相应的在子路径中的点i，j，使得Distance[i][r]+Distance[r][j]-Distance[i][j] 最小

        //Step 3：对于Step 2中找到的所有（i，j，0r），找到使（Distance[i][r]+Distance[r][j]）/Distance[i][j] 最小的那一组，将这组的r插入到i和j中间

        // Step 4：回到Step 2，直到所有的点都加入到路径中




        //双生成树
        private void CaculateDistanceMap()
        {

        }
    }
}
