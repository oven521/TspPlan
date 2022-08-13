/*
Tsp多项式时间算法（局部最优解）

备注：
    改良圈: ChoiceFlag = 0
    贪心: ChoiceFlag = 1
    改良圈+贪心: ChoiceFlag = 2
    最小代价插入: ChoiceFlag = 3
    凸包: ChoiceFlag = 5
    动态规划: ChoiceFlag = 6
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace Tsp
{
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
    //用于插入性方案
    public class InsertPt
    {
        public EMShape Point;//该点坐标
        public double MinCost;//加入子路径时的最小代价
        public LinkedListNode<EMShape> PreInsertNode;//预备插入节点。该点将插入到PreInsertNode后
    }

    public class TspPlan
    {
        //保存所有点之间的距离表
        private List<List<double>> DistanceMap = new List<List<double>>();
        //总距离
        private double sumDistance = 0;

        private LinkedList<EMShape> PointLinkedList = new LinkedList<EMShape>();

        public TspPlan()
        {

        }
        public TspPlan(xPoint stopPt, List<xPoint> emShapelist1, int ChoiceFlag = 2, xPoint endPt = null)
        {
            List<EMShape> emShapelist2 = new List<EMShape>();
            emShapelist1.ForEach(A =>
            {
                emShapelist2.Add(new EMShape(A));
            });
            TspSort(stopPt, emShapelist2, ChoiceFlag, endPt);
            emShapelist1.Clear();
            emShapelist2.ForEach(a => emShapelist1.Add(a.StartPoint));


        }

        internal void TspSort(xPoint stopPt, List<EMShape> emShapelist2, int ChoiceFlag = 2, xPoint endP = null)
        {
            List<EMShape> PointList = emShapelist2;

            double min = Distance(stopPt, PointList[0].StartPoint);
            int StartPt = 0;//设置起点
            for (int i = 1; i < PointList.Count(); i++)
            {
                double cur = Distance(stopPt, PointList[i].StartPoint);
                if (min > cur)
                {
                    min = cur;
                    StartPt = i;
                }
            }
            SwapPt(PointList, StartPt, 0);

            if (ChoiceFlag == 0)//单改良圈
            {
                SetEndPoint(PointList, endP);//设置终点
                CircleModification(PointList);
            }
            else if (ChoiceFlag == 1)//单贪心
            {
                NearestNeighbor(PointList);
            }
            else if (ChoiceFlag == 2)//贪心+改良圈
            {
                if (endP == null)
                {
                    NearestNeighbor(PointList);
                }
                CircleModification(PointList);
            }
            else if (ChoiceFlag == 3) //最小代价插入
            {
                SetEndPoint(PointList, endP);//设置终点
                Expansion(PointList);
            }
            else if (ChoiceFlag == 5)//凸包
            {
                ConvexHull(PointList);
                //删除终点，解除回路
                PointList.RemoveAt(PointList.Count - 1);
            }
            else if (ChoiceFlag == 6)//动态规划
            {
                Dynamic(PointList);
            }

            CalculateSumDistance(PointList);//计算总长度
        }
        //设置终点
        private void SetEndPoint(List<EMShape> PointList, xPoint endP = null)
        {
            double max = 0;
            int EndPt = 0;
            if (endP != null)
            {
                max = Distance(PointList[PointList.Count - 1].StartPoint, endP);
                for (int i = 1; i < PointList.Count; i++)
                {
                    double cur = Distance(PointList[i].StartPoint, endP);
                    if (max > cur)
                    {
                        max = cur;
                        EndPt = i;
                    }
                }
            }
            else
            {
                for (int i = 1; i < PointList.Count(); i++)
                {
                    double cur = Distance(PointList[0].StartPoint, PointList[i].StartPoint);
                    if (max < cur)
                    {
                        max = cur;
                        EndPt = i;
                    }
                }
            }
            if (EndPt != 0)
            {
                SwapPt(PointList, EndPt, PointList.Count() - 1);
            }
        }
        //交换两点
        private void SwapPt(List<EMShape> PointList, int sp, int ep)
        {
            EMShape temp = PointList[sp];
            PointList[sp] = PointList[ep];
            PointList[ep] = temp;
        }
        //计算两点之间的欧氏距离
        private double Distance(xPoint a, xPoint b)
        {
            return Math.Sqrt(Math.Pow(a.XPos - b.XPos, 2) + Math.Pow(a.YPos - b.YPos, 2));
        }
        public double SumDistance() => sumDistance;
        //计算总距离
        public void CalculateSumDistance(List<EMShape> PointList)
        {
            sumDistance = 0;
            for (int i = 0; i < PointList.Count() - 1; i++)
            {
                sumDistance += Distance(PointList[i].StartPoint, PointList[i + 1].StartPoint);
            }
        }
        //颠倒路径中的顺序
        private void Reverse(int begin, int end, List<EMShape> PointList, double[] OldpathDistance)
        {
            int count;
            if ((begin + end) % 2 == 1)
            {
                count = (begin + end) / 2 + 1;//偶数个交换点
            }
            else
            {
                count = (begin + end) / 2;//奇数个交换点
            }

            for (int i = begin; i < count; i++)
            {
                //交换x，y坐标
                EMShape temp = PointList[end - i + begin];
                PointList[end - i + begin] = PointList[i];
                PointList[i] = temp;

                //交换距离表
                double tempDistance = OldpathDistance[end - i + begin - 1];
                OldpathDistance[end - i + begin - 1] = OldpathDistance[i];
                OldpathDistance[i] = tempDistance;
            }
        }
        #region 改良圈
        /// <summary>
        /// 改良圈算法
        /// </summary>
        private void CircleModification(List<EMShape> PointList)
        {
            int StopFlag;//停止标志
            int CycleIndex = 0;//循环次数
            double[] OldpathDistance = new double[PointList.Count];//此路径应该不能存进PointList类中，在颠倒的时候颠倒过程中所有路径都需要更换
            for (int i = 0; i < PointList.Count() - 1; i++)
            {
                OldpathDistance[i] = Distance(PointList[i].StartPoint, PointList[i + 1].StartPoint);
                sumDistance += OldpathDistance[i];
            }
            for (int k = 0; k < PointList.Count(); k++)
            {
                StopFlag = 0;
                //遍历一轮
                for (int i = 0; i < PointList.Count - 2; i++)
                {
                    for (int j = i + 2; j < PointList.Count() - 1; j++)
                    {
                        CycleIndex++;
                        //比较颠倒顺序前后的总路线长
                        double Newpathi = Distance(PointList[i].StartPoint, PointList[j].StartPoint);
                        double Newpathj = Distance(PointList[i + 1].StartPoint, PointList[j + 1].StartPoint);
                        if (Newpathi + Newpathj < OldpathDistance[i] + OldpathDistance[j])
                        {
                            StopFlag++;

                            Reverse(i + 1, j, PointList, OldpathDistance);
                            OldpathDistance[i] = Newpathi;
                            OldpathDistance[j] = Newpathj;
                        }
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
        #region 最近邻算法
        /// <summary>
        /// 最近邻算法
        /// </summary>
        private void NearestNeighbor(List<EMShape> PointList)
        {
            int testcircle = 0;//循环次数
            for (int i = 0; i < PointList.Count() - 1; i++)
            {
                double min = Distance(PointList[i].StartPoint, PointList[i + 1].StartPoint);
                int NearestPoint = i + 1;
                //遍历list获取当前点的最近点
                for (int j = i + 1; j < PointList.Count(); j++)
                {
                    testcircle++;
                    if (Distance(PointList[i].StartPoint, PointList[j].StartPoint) < min)
                    {
                        NearestPoint = j;
                        min = Distance(PointList[i].StartPoint, PointList[j].StartPoint);
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
        #endregion
        #region 凸包算法
        //判断ab相对bc是不是左转,大于0则左转,等于0则在同一直线上
        private bool IsLeft(xPoint a, xPoint b, xPoint c)
        {
            return (b.XPos - a.XPos) * (c.YPos - b.YPos) - (b.YPos - a.YPos) * (c.XPos - b.XPos) >= 0;
        }
        /*
            插入点代价
            1.根据Distance[i][r]+Distance[r][j]-Distance[i][j] 最小
            2.根据∠irj 最大，角度计算会出现交叉
        */
        private double InsertCost(xPoint Insert, xPoint a, xPoint b)
        {
            return (Distance(Insert, a) + Distance(Insert, b)) - Distance(a, b);
            //计算向量间夹角cos值
            //double delta = (Insert.XPos - a.XPos) * (Insert.XPos - b.XPos) + (Insert.YPos - a.YPos) * (Insert.YPos - b.YPos);
            //return delta / (Math.Sqrt(Math.Pow((Insert.XPos - a.XPos), 2) + Math.Pow((Insert.YPos - a.YPos), 2)) * Math.Sqrt(Math.Pow((Insert.XPos - b.XPos), 2) + Math.Pow((Insert.YPos - b.YPos), 2)));
        }
        private void ConvexHull(List<EMShape> PointList)
        {
            LinkedList<EMShape> ConvexHullListLinked = new LinkedList<EMShape>();
            LinkedList<InsertPt> InternalPtListLinked = new LinkedList<InsertPt>();

            Graham(PointList, ConvexHullListLinked, InternalPtListLinked);
            ConvexHullInsertion(ConvexHullListLinked, InternalPtListLinked);
            //最后赋回PointList中
            PointList.Clear();
            foreach (EMShape Point in ConvexHullListLinked)
            {
                PointList.Add(Point);   
            }
        }

        private void Graham(List<EMShape> PointList, LinkedList<EMShape> ConvexHullListLink, LinkedList<InsertPt> InternalPtListLink)
        {
            //寻找初始点作为原点
            int Initial = 0;
            for (int i = 1; i < PointList.Count; i++)
            {
                if (PointList[i].StartPoint.YPos < PointList[Initial].StartPoint.YPos ||
                    (PointList[i].StartPoint.YPos == PointList[Initial].StartPoint.YPos && PointList[i].StartPoint.XPos < PointList[Initial].StartPoint.XPos))
                {
                    Initial = i;
                }
            }
            EMShape InitialPt = new EMShape(PointList[Initial].StartPoint);//凸包初始点

            //排序
            List<EMShape> templist = PointList.OrderBy(Point => Math.Acos((Point.StartPoint.XPos - InitialPt.StartPoint.XPos) /
                Math.Sqrt(Math.Pow(Point.StartPoint.XPos - InitialPt.StartPoint.XPos, 2) + Math.Pow(Point.StartPoint.YPos - InitialPt.StartPoint.YPos, 2))))
                .ThenBy(Point => Point.StartPoint.XPos).ToList();


            //构建初始凸包,将前两个点放入凸包Linklist中
            ConvexHullListLink.AddLast(templist[0]);
            ConvexHullListLink.AddLast(templist[1]);
            LinkedListNode<EMShape> ConvexHullNode = ConvexHullListLink.Last;
            int templistPt = 2;
            while (templistPt < templist.Count)
            {
                if (IsLeft(ConvexHullListLink.Last.Previous.Value.StartPoint, ConvexHullListLink.Last.Value.StartPoint, templist[templistPt].StartPoint))
                {
                    ConvexHullListLink.AddLast(templist[templistPt]);
                    templistPt++;
                }
                else
                {
                    InsertPt ConvexHullInternalPt = new InsertPt();
                    //删除凸包上的点并将其加入内部点集
                    ConvexHullInternalPt.Point = ConvexHullListLink.Last.Value;
                    InternalPtListLink.AddLast(ConvexHullInternalPt);
                    ConvexHullListLink.RemoveLast();
                }
            }
            ConvexHullListLink.AddLast(ConvexHullListLink.First.Value);//加入终点
        }

        private void ConvexHullInsertion(LinkedList<EMShape> ConvexHullList, LinkedList<InsertPt> InternalPtList)
        {
            //初始距离表
            LinkedListNode<InsertPt> InsertPointNode;
            LinkedListNode<EMShape> ConvexHullNode;
            foreach (InsertPt InternalPoint in InternalPtList)
            {
                for (ConvexHullNode = ConvexHullList.First; ConvexHullNode.Next != null; ConvexHullNode = ConvexHullNode.Next)
                {
                    double CurCost = InsertCost(InternalPoint.Point.StartPoint, ConvexHullNode.Value.StartPoint, ConvexHullNode.Next.Value.StartPoint);
                    if (InternalPoint.MinCost > CurCost || InternalPoint.MinCost == 0)
                    {
                        InternalPoint.MinCost = CurCost;
                        //InternalPt.InsertPtIndex = j;
                        InternalPoint.PreInsertNode = ConvexHullNode;
                    }
                }
            }
            int Count = InternalPtList.Count;
            //插入点集
            for (int i = 0; i < Count; i++)//依次将内部点插入凸包点集中，迭代次数等于内部点个数
            {
                //插入代价最小点
                //int InsertPtIndex = 0;
                LinkedListNode<InsertPt> InsertNode = InternalPtList.First;
                double MinCost = 0;
                for (InsertPointNode = InternalPtList.First; InsertPointNode != null; InsertPointNode = InsertPointNode.Next)
                {
                    double CurCost = InsertPointNode.Value.MinCost;
                    if ((MinCost > CurCost) || MinCost == 0)
                    {
                        InsertNode = InsertPointNode;
                        MinCost = CurCost;
                    }
                }

                ConvexHullList.AddAfter(InsertNode.Value.PreInsertNode, InsertNode.Value.Point);
                //test
                //Console.WriteLine("插入{0},{1}", InsertNode.Value.Point.StartPoint.XPos, InsertNode.Value.Point.StartPoint.YPos);
                //for (OutListNode=OutList.First; OutListNode!=null; OutListNode=OutListNode.Next) Console.WriteLine("{0},{1}", OutListNode.Value.StartPoint.XPos, OutListNode.Value.StartPoint.YPos);
                //Console.WriteLine("------------");
                InternalPtList.Remove(InsertNode.Value);
                for (InsertPointNode = InternalPtList.First; InsertPointNode != null; InsertPointNode = InsertPointNode.Next)
                {
                    //更新路径值时，如果待插入点原有的索引点的路径消失，需要重算该待插入点最小插入代价和对应索引点
                    if (InsertNode.Value.PreInsertNode == InsertPointNode.Value.PreInsertNode)
                    {
                        InsertPointNode.Value.MinCost = 0;
                        
                        for (ConvexHullNode = ConvexHullList.First; ConvexHullNode.Next != null; ConvexHullNode = ConvexHullNode.Next)
                        {
                            double CurCost = InsertCost(InsertPointNode.Value.Point.StartPoint, ConvexHullNode.Value.StartPoint, ConvexHullNode.Next.Value.StartPoint);
                            if (InsertPointNode.Value.MinCost > CurCost || InsertPointNode.Value.MinCost == 0)
                            {
                                InsertPointNode.Value.MinCost = CurCost;
                                InsertPointNode.Value.PreInsertNode = ConvexHullNode;
                            }
                        }
                    }
                    else//如果待插入点原有的索引点的路径未消失，只需比较新增点对应的两条路径与该待插入点原有的最小插入代价和对应索引点
                    {
                        ConvexHullNode = InsertNode.Value.PreInsertNode;
                        for (int j = 0; j < 2; j++, ConvexHullNode = ConvexHullNode.Next)
                        {
                            double CurCost = InsertCost(InsertPointNode.Value.Point.StartPoint, ConvexHullNode.Value.StartPoint, ConvexHullNode.Next.Value.StartPoint);
                            if (InsertPointNode.Value.MinCost > CurCost)
                            {
                                InsertPointNode.Value.MinCost = CurCost;
                                InsertPointNode.Value.PreInsertNode = ConvexHullNode;
                            }
                        }
                    }

                }
            }
        }
        #endregion
        #region 动态规划 存在bug 容易内存溢出
        private void Dynamic(List<EMShape> PointList)
        {
            //List<List<double>> DistanceMap = new List<List<double>>();
            CaculateDistanceMap(PointList);//计算距离矩阵
            int n = PointList.Count;
            double[,] dp = new double[1 << PointList.Count, PointList.Count];
            //填充最大值
            for (int i = 0; i < 1 << PointList.Count; i++)
            {
                for (int j = 0; j < PointList.Count; j++)
                {
                    dp[i, j] = 0xffffffff;
                }
            }
            dp[(1 << n) - 1, 0] = 0;

            //初始距离
            for (int i = (1 << n) - 2; i >= 0; i--)
            {//状态的个数，从n个0到n个1 
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        if ((1 & i >> k) == 0) //必须是访问过的k点且边存在
                            dp[i, j] = Math.Min(dp[i | (1 << k), k] + DistanceMap[k][j], dp[j, k]);
                    }
                }
            }
            sumDistance = dp[0, 0];
        }
        private void CaculateDistanceMap(List<EMShape> PointList)
        {
            for (int i = 0; i < PointList.Count; i++)
            {
                List<double> tempDistanceMap = new List<double>();
                for (int j = 0; j < PointList.Count; j++)
                {
                    tempDistanceMap.Add(Distance(PointList[i].StartPoint, PointList[j].StartPoint));
                }
                DistanceMap.Add(tempDistanceMap);
            }
        }


        #endregion
        #region 最小代价插入
        private void Expansion(List<EMShape> PointList)
        {
            PointList.ForEach(A =>
            {
                PointLinkedList.AddLast(A);
            });
            LinkedList<EMShape> OutList = new LinkedList<EMShape>();

            LinkedListNode<EMShape> SecondPointNode;
            //将起点跟终点存入目标列表
            OutList.AddLast(PointLinkedList.First.Value);
            OutList.AddLast(PointLinkedList.Last.Value);
            PointLinkedList.RemoveFirst();
            PointLinkedList.RemoveLast();

            //将原点集中除了原点和终点以外其他的点存入待插入列表中以及初始距离表
            LinkedList<InsertPt> InsertPointList = new LinkedList<InsertPt>();

            foreach (EMShape Point in PointLinkedList)
            {
                InsertPt InternalPt = new InsertPt();
                InternalPt.Point = Point;

                SecondPointNode = OutList.First;
                for (SecondPointNode = OutList.First; SecondPointNode.Next != null; SecondPointNode = SecondPointNode.Next)
                {
                    double CurDistance = InsertCost(InternalPt.Point.StartPoint, SecondPointNode.Value.StartPoint, SecondPointNode.Next.Value.StartPoint);
                    if (InternalPt.MinCost > CurDistance || InternalPt.MinCost == 0)
                    {
                        InternalPt.MinCost = CurDistance;
                        //InternalPt.InsertPtIndex = j;
                        InternalPt.PreInsertNode = SecondPointNode;
                    }
                }
                InsertPointList.AddLast(InternalPt);
            }

            int Count = InsertPointList.Count;
            LinkedListNode<InsertPt> InsertPointNode;
            LinkedListNode<EMShape> OutListNode;
            //插入点集
            for (int i = 0; i < Count; i++)//依次将内部点插入凸包点集中，迭代次数等于内部点个数
            {
                //插入代价最小点
                //int InsertPtIndex = 0;
                LinkedListNode<InsertPt> InsertNode = InsertPointList.First;
                double MinCost = 0;
                for (InsertPointNode = InsertPointList.First; InsertPointNode != null; InsertPointNode = InsertPointNode.Next)
                {
                    double CurCost = InsertPointNode.Value.MinCost;
                    if ((MinCost > CurCost) || MinCost == 0)
                    {
                        InsertNode = InsertPointNode;
                        MinCost = CurCost;
                    }
                }
                OutList.AddAfter(InsertNode.Value.PreInsertNode, InsertNode.Value.Point);
                //test
                //Console.WriteLine("插入{0},{1}", InsertNode.Value.Point.StartPoint.XPos, InsertNode.Value.Point.StartPoint.YPos);
                //for (OutListNode = OutList.First; OutListNode != null; OutListNode = OutListNode.Next) Console.WriteLine("{0},{1}", OutListNode.Value.StartPoint.XPos, OutListNode.Value.StartPoint.YPos);
                //Console.WriteLine("------------");

                InsertPointList.Remove(InsertNode.Value);
                for (InsertPointNode = InsertPointList.First; InsertPointNode != null; InsertPointNode = InsertPointNode.Next)
                {
                    //更新路径值时，如果待插入点原有的索引点的路径消失，需要重算该待插入点最小插入代价和对应索引点
                    if (InsertNode.Value.PreInsertNode == InsertPointNode.Value.PreInsertNode)
                    {
                        InsertPointNode.Value.MinCost = 0;
                        //
                        for (OutListNode = OutList.First;OutListNode.Next!=null;OutListNode=OutListNode.Next)
                        {
                            double CurCost = InsertCost(InsertPointNode.Value.Point.StartPoint, OutListNode.Value.StartPoint, OutListNode.Next.Value.StartPoint);
                            if (InsertPointNode.Value.MinCost > CurCost || InsertPointNode.Value.MinCost == 0)
                            {
                                InsertPointNode.Value.MinCost = CurCost;
                                InsertPointNode.Value.PreInsertNode = OutListNode;
                            }
                        }
                    }
                    else//如果待插入点原有的索引点的路径未消失，只需比较新增点对应的两条路径与该待插入点原有的最小插入代价和对应索引点
                    {
                        OutListNode = InsertNode.Value.PreInsertNode;
                        for (int j = 0; j <  2; j++, OutListNode=OutListNode.Next)
                        {
                            double CurCost = InsertCost(InsertPointNode.Value.Point.StartPoint, OutListNode.Value.StartPoint, OutListNode.Next.Value.StartPoint);
                            if (InsertPointNode.Value.MinCost > CurCost)
                            {
                                InsertPointNode.Value.MinCost = CurCost;
                                InsertPointNode.Value.PreInsertNode = OutListNode;
                            }
                        }
                    }

                }
            }
            PointList.Clear();
            foreach(EMShape Point in OutList)
            {
                PointList.Add(Point);
            }
        }
        #endregion
    }
}
