/*
Tsp多项式时间算法（局部解）
改良圈+贪心：贪心寻找较好解，再用改良圈优化
改良圈：手动设置终点为起点的最远点
凸包：正在实现

后续考虑算法
生成树：结果范围<2OPT， 时间复杂度O（n^2）
最小权匹配：结果范围<1.5OPT， 时间复杂度O（n^3）

目前正在写的算法：凸包算法
考虑到观感性，凸包算法理论上不会产生交叉
    时间复杂度：O(n^2*lg(n))
Step 1：构造凸包，并将它作为最初的子路径
Step 2：对于所有不在子路径中的点r，找到其相应的在子路径中的点i，j，使得Distance[i][r]+Distance[r][j]-Distance[i][j] 最小
Step 3：对于Step 2中找到的所有（i，j，0r），找到使（Distance[i][r]+Distance[r][j]）/Distance[i][j] 最小的那一组，将这组的r插入到i和j中间
Step 4：回到Step 2，直到所有的点都加入到路径中



改良圈: ChoiceFlag = 0
贪心: ChoiceFlag = 1
改良圈+贪心: ChoiceFlag = 2
双生成树: ChoiceFlag = 3
最小权匹配: ChoiceFlag = 4
凸包: ChoiceFlag = 5
动态规划: ChoiceFlag = 6
*/

using System;
using System.Collections.Generic;
using System.Linq;


namespace tsp
{
    //GetPt()获取排序后的点

    public class xPoint
    {
        public float XPos;
        public float YPos;
    }

    public class EMShape
    {
        public xPoint StartPoint;
        public EMShape(xPoint a)
        {
            StartPoint = new xPoint();
            StartPoint.XPos = a.XPos;
            StartPoint.YPos = a.YPos;
        }
    }

    public class TspPlan
    {
        //private List<EMShape> PointList = new List<EMShape>();

        //List<EMShape> emShapeList

        private List<List<double>> DistanceMap = new List<List<double>>();
        //总距离
        private double sumDistance = 0;
        public TspPlan()
        {

        }
        public TspPlan(xPoint stopPt, List<xPoint> emShapelist1, int ChoiceFlag = 2)
        {
            List<EMShape> emShapelist2 = new List<EMShape>();
            emShapelist1.ForEach(A =>
            {
                emShapelist2.Add(new EMShape(A));
            });
            TspSort(stopPt, emShapelist2, ChoiceFlag);
            emShapelist1.Clear();
            emShapelist2.ForEach(a => emShapelist1.Add(a.StartPoint));
        }

        //private void swapLst(List<EMShape> PointList,int sp,int ep)
        //{
        //    xPoint temp = PointList[StartPt];
        //    PointList[StartPt] = PointList[0];
        //    PointList[0] = temp;
        //}
        //1代表贪心，0代表改良圈，2代表先贪心后改良圈
        internal void TspSort(xPoint stopPt, List<EMShape> emShapelist2, int ChoiceFlag = 2)
        {
            List<EMShape> PointList = emShapelist2;

            double min = Distance(stopPt.XPos, stopPt.YPos, PointList[0].StartPoint.XPos, PointList[0].StartPoint.YPos);
            int StartPt = 0;//设置起点
            for (int i = 1; i < PointList.Count() - 1; i++)
            {
                double cur = Distance(stopPt.XPos, stopPt.YPos, PointList[i].StartPoint.XPos, PointList[i].StartPoint.YPos);
                if (min > cur)
                {
                    min = cur;
                    StartPt = i;
                }
            }
            EMShape temp = PointList[StartPt];
            PointList[StartPt] = PointList[0];
            PointList[0] = temp;


            //
            if (ChoiceFlag == 1)//单贪心
            {
                NearestNeighbor(PointList);
            }
            else if (ChoiceFlag == 0)//单改良圈
            {
                double max = 0;
                int EndPt = 0;//设置终点
                for (int i = 1; i < PointList.Count() - 1; i++)
                {
                    double cur = Distance(stopPt.XPos, stopPt.YPos, PointList[i].StartPoint.XPos, PointList[i].StartPoint.YPos);
                    if (max < cur)
                    {
                        max = cur;
                        EndPt = i;
                    }
                }
                temp = PointList[EndPt];
                PointList[EndPt] = PointList[PointList.Count() - 1];
                PointList[PointList.Count() - 1] = temp;

                CircleModification(PointList);
            }
            else if (ChoiceFlag == 2)//贪心+改良圈，由于贪心本身能达到局部较好解，使得改良圈迭代次数更少，所以可能会出现贪心+改良圈比单贪心运行效率更高的情况
            {
                NearestNeighbor(PointList);
                CircleModification(PointList);
            }
            else if (ChoiceFlag == 3)//双生成树
            {
                CaculateDistanceMap();
            }
        }
        #region 改良圈
        private double Distance(double source_x, double source_y, double target_x, double target_y)
        {
            return /*Math.Sqrt*/(Math.Pow(source_x - target_x, 2) + Math.Pow(source_y - target_y, 2));
        }

        ////public List<xPoint> GetPt() => PointList;
        //public int GetCount() => PointList.Count();
        public double SumDistance() => sumDistance;
        //计算总距离
        private void CalculateSumDistance(List<EMShape> PointList)
        {
            sumDistance = 0;
            for (int i = 0; i < PointList.Count() - 1; i++)
            {
                sumDistance += Math.Sqrt(Distance(PointList[i].StartPoint.XPos, PointList[i].StartPoint.YPos, PointList[i + 1].StartPoint.XPos, PointList[i + 1].StartPoint.YPos));
            }
        }
        //颠倒路径中的顺序
        private void Reverse(int begin, int end, List<EMShape> PointList)
        {
            for (int i = begin; i < (begin + end) / 2 + 1; i++)
            {
                //交换x，y坐标
                EMShape temp = PointList[end - i + begin];
                PointList[end - i + begin] = PointList[i];
                PointList[i] = temp;
            }
        }
        /// <summary>
        /// 改良圈算法
        /// </summary>
        private void CircleModification(List<EMShape> PointList)
        {
            int StopFlag;//停止标志
            int CycleIndex = 0;//循环次数
            CalculateSumDistance(PointList);//计算总长度
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
                        double Newpath = Distance(PointList[i].StartPoint.XPos, PointList[i].StartPoint.YPos, PointList[j].StartPoint.XPos, PointList[j].StartPoint.YPos) + Distance(PointList[i + 1].StartPoint.XPos, PointList[i + 1].StartPoint.YPos, PointList[j + 1].StartPoint.XPos, PointList[j + 1].StartPoint.YPos);
                        double Oldpath = Distance(PointList[i].StartPoint.XPos, PointList[i].StartPoint.YPos, PointList[i + 1].StartPoint.XPos, PointList[i + 1].StartPoint.YPos) + Distance(PointList[j].StartPoint.XPos, PointList[j].StartPoint.YPos, PointList[j + 1].StartPoint.XPos, PointList[j + 1].StartPoint.YPos);
                        if (Newpath < Oldpath)
                        {
                            StopFlag++;
                            sumDistance += Math.Sqrt(Newpath) - Math.Sqrt(Oldpath);//更新总长度
                            Reverse(i + 1, j, PointList);
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
        #endregion
        /// <summary>
        /// 最近邻算法
        /// </summary>
        private void NearestNeighbor(List<EMShape> PointList)
        {
            int testcircle = 0;//循环次数
            for (int i = 0; i < PointList.Count() - 1; i++)
            {
                double min = Distance(PointList[i].StartPoint.XPos, PointList[i].StartPoint.YPos, PointList[i + 1].StartPoint.XPos, PointList[i + 1].StartPoint.YPos);
                int NearestPoint = i + 1;
                //遍历list获取当前点的最近点
                for (int j = i + 1; j < PointList.Count(); j++)
                {
                    testcircle++;
                    if (Distance(PointList[i].StartPoint.XPos, PointList[i].StartPoint.YPos, PointList[j].StartPoint.XPos, PointList[j].StartPoint.YPos) < min)
                    {
                        NearestPoint = j;
                        min = Distance(PointList[i].StartPoint.XPos, PointList[i].StartPoint.YPos, PointList[j].StartPoint.XPos, PointList[j].StartPoint.YPos);
                    }
                }
                //交换x，y坐标
                EMShape temp = PointList[i + 1];
                PointList[i + 1] = PointList[NearestPoint];
                PointList[NearestPoint] = temp;

            }
            //Console.WriteLine(testcircle);
            CalculateSumDistance(PointList);
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
