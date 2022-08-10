/*
Tsp多项式时间算法（局部最优解）
    贪心（最邻近点法）：每次找到离他最近的点插入  O（n^2）
    改良圈+贪心：贪心寻找较好解，再用改良圈优化  O（n^2logn）
    改良圈：手动设置终点为起点的最远点   O（n^2logn）
    动态规划：时间复杂度跟空间复杂度高达n*2^n,暂时不考虑
    凸包算法：构建凸包，根据插入代价逐渐将内部点插入凸包当中  O（n^2logn）
    最小代价插入：思路类似凸包，先定义起点终点，然后根据插入代价将其余点插入路径中  O（n^2logn）

C-W节约法：
    时间复杂度：O(n^2）
      构建费用表，将费用表C从大到小排序，然后从费用表C的顶端开始向下，一次将可以连接的路径（i,j）连接，直至形成完整路径
      回路性算法，需要转变成路径，局部解应该要比最邻近点算法好一点，时间上应该略高于最邻近点算法

已排除算法
贪婪算法
    时间复杂度：O(n^2*log2(n))
      给所有边按长度排序，选择最短的满足条件的边加入到路径中。条件：不会构成小于N条边的环路
      结果上来说可能和凸包等同复杂度算法差不多，但是算法本身没有杜绝交叉边出现，观感可能不太好

最邻近插入，最远插入，任意插入
    时间复杂度：O(n^2）
      三种低复杂度的插入算法，但是随机性过高，

    最邻近插入
       Step 1：任选一个点 i，作为子路径（只包含一个结点的子路径）
       Step 2：找到距离i最近的点r，形成子路径 i-r-i
       Step 3：选择。找到不在子路径中的点r，使它到子路径中的点的最小距离是所有不在子路径中的点到子路径中的点的最小距离中最小的。这个距离是连接这两类点的最小距离
       Step 4：插入。在子路径中找到这样一条边（i，j），使Distance[i][r]+Distance[r][j]-Distance[i][j]最小。将r插入到i和j中间
       Step 5：执行Step 3，直到所有点都加入到了路径中


匈牙利法
    最优解算法，时间复杂度过高，

备注：
    改良圈: ChoiceFlag = 0
    贪心: ChoiceFlag = 1
    改良圈+贪心: ChoiceFlag = 2
    最小代价插入: ChoiceFlag = 3
    最小权匹配: ChoiceFlag = 4
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
        public double MinCost;//加入凸包子路径时找到的最小代价
        public LinkedListNode<EMShape> PreInsertNode;//预备插入节点

        public int InsertPtIndex;//加入凸包子路径时对应的索引点，即Distance[i][r]+Distance[r][i+1]-Distance[i][i+1]的i点索引
        public bool InsertFlag;//已插入标志，true代表已经插入
    }

    public class TspPlan
    {
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
            //
            emShapelist2.ForEach(A =>
            {
                PointLinkedList.AddLast(A);
            });

            LinkedListNode<EMShape> a = PointLinkedList.First;
            LinkedListNode<EMShape> b = a;
            Console.WriteLine(b.Value.StartPoint.XPos + "," + b.Value.StartPoint.YPos);
            PointLinkedList.RemoveFirst();
            Console.WriteLine(a.Value.StartPoint.XPos + "," + a.Value.StartPoint.YPos);
            Console.WriteLine(b.Value.StartPoint.XPos + "," + b.Value.StartPoint.YPos);

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

            //
            if (ChoiceFlag == 1)//单贪心
            {
                NearestNeighbor(PointList);
            }
            else if (ChoiceFlag == 0)//单改良圈
            {
                SetEndPoint(PointList, endP);//设置终点

                CircleModification(PointList);
            }
            else if (ChoiceFlag == 2)//贪心+改良圈
            {
                if (endP == null)
                {
                    NearestNeighbor(PointList);
                }
                CircleModification(PointList);
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
            else if (ChoiceFlag == 3) //最小代价插入
            {
                SetEndPoint(PointList, endP);//设置终点
                Expansion(PointList);
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
        // 时间复杂度：O(n^2*lg(n))
        //Step 1：构造凸包，并将它作为最初的子路径(Graham扫描法)
        //Step 2：对于所有不在子路径中的点r，找到其相应的在子路径中的点i，j，使得Distance[i][r]+Distance[r][j]-Distance[i][j] 最小
        //Step 3：对于Step 2中找到的所有（i，j，0r），找到使（Distance[i][r]+Distance[r][j]）/Distance[i][j] 最小的那一组，将这组的r插入到i和j中间
        // Step 4：回到Step 2，直到所有的点都加入到路径中

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
            List<EMShape> ConvexHullList = new List<EMShape>();
            List<InsertPt> InternalPtList = new List<InsertPt>();


            Graham(PointList, ConvexHullList, InternalPtList);//Graham扫描法构建凸包
            ConvexHullInsertion(ConvexHullList, InternalPtList);//插入内部点

            //最后赋回
            PointList.Clear();
            ConvexHullList.ForEach(a => PointList.Add(a));
        }

        private void Graham(List<EMShape> PointList, List<EMShape> ConvexHullList, List<InsertPt> InternalPtList)
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


            //构建初始凸包:栈方法实现
            ConvexHullList.Add(templist[0]);
            ConvexHullList.Add(templist[1]);
            int ConvexHullTop = 1;
            int templistPt = 2;
            while (templistPt < templist.Count)
            {
                if (IsLeft(ConvexHullList[ConvexHullTop - 1].StartPoint, ConvexHullList[ConvexHullTop].StartPoint, templist[templistPt].StartPoint))
                {
                    ConvexHullList.Add(templist[templistPt]);
                    ConvexHullTop++;
                    templistPt++;
                }
                else
                {
                    InsertPt ConvexHullInternalPt = new InsertPt();
                    //删除凸包上的点并将其加入内部点集
                    ConvexHullInternalPt.Point = ConvexHullList[ConvexHullTop];
                    InternalPtList.Add(ConvexHullInternalPt);
                    ConvexHullList.RemoveAt(ConvexHullTop);

                    ConvexHullTop--;
                }
            }
            ConvexHullList.Add(ConvexHullList[0]);//加入终点
        }


        //插入其余点
        private void ConvexHullInsertion(List<EMShape> ConvexHullList, List<InsertPt> InternalPtList)
        {
            //初始距离表
            for (int i = 0; i < InternalPtList.Count; i++)
            {
                //计算增加的长度
                for (int j = 0; j < ConvexHullList.Count - 1; j++)
                {
                    double CurDistance = InsertCost(InternalPtList[i].Point.StartPoint, ConvexHullList[j].StartPoint, ConvexHullList[j + 1].StartPoint);
                    if (InternalPtList[i].MinCost > CurDistance || InternalPtList[i].MinCost == 0)
                    {
                        InternalPtList[i].MinCost = CurDistance;
                        InternalPtList[i].InsertPtIndex = j;
                    }
                }
            }

            //插入点集
            for (int i = 0; i < InternalPtList.Count; i++)//依次将内部点插入凸包点集中，迭代次数等于内部点个数
            {
                //test
                int InsertPtIndex = 0;
                double MinDistanceProportion = 0;
                //遍历所有内部点，根据距离增量插入一个点
                for (int j = 0; j < InternalPtList.Count; j++)
                {
                    if (InternalPtList[j].InsertFlag) continue;
                    double CurDistanceProportion = InternalPtList[j].MinCost;
                    if ((MinDistanceProportion > CurDistanceProportion) || MinDistanceProportion == 0)
                    {
                        InsertPtIndex = j;
                        MinDistanceProportion = CurDistanceProportion;
                    }
                }
                ConvexHullList.Insert(InternalPtList[InsertPtIndex].InsertPtIndex + 1, InternalPtList[InsertPtIndex].Point);

                //test
                //Console.WriteLine("插入{0},{1}", InternalPtList[InsertPtIndex].InternalPt.StartPoint.XPos, InternalPtList[InsertPtIndex].InternalPt.StartPoint.YPos);
                //for (int a = 0; a < ConvexHullList.Count; a++) Console.WriteLine("{0},{1}", ConvexHullList[a].StartPoint.XPos, ConvexHullList[a].StartPoint.YPos);
                //Console.WriteLine("------------");

                InternalPtList[InsertPtIndex].InsertFlag = true;
                if (i == InternalPtList.Count - 1) break;
                //插入后ConvexHullList索引改变，MinDistancePt内的值加一,更新路径值
                for (int k = 0; k < InternalPtList.Count; k++)
                {
                    if (InternalPtList[k].InsertPtIndex > InternalPtList[InsertPtIndex].InsertPtIndex) InternalPtList[k].InsertPtIndex++;
                    if (InternalPtList[k].InsertFlag) continue;
                    //更新路径值时，插入点前一个点的路径消失，需要重算路径点
                    if (InternalPtList[InsertPtIndex].InsertPtIndex == InternalPtList[k].InsertPtIndex)
                    {
                        InternalPtList[k].MinCost = 0;
                        for (int j = 0; j < ConvexHullList.Count - 1; j++)
                        {
                            double CurDistance = InsertCost(InternalPtList[k].Point.StartPoint, ConvexHullList[j].StartPoint, ConvexHullList[j + 1].StartPoint);
                            if (InternalPtList[k].MinCost > CurDistance || InternalPtList[k].MinCost == 0)
                            {
                                InternalPtList[k].MinCost = CurDistance;
                                InternalPtList[k].InsertPtIndex = j;
                            }
                        }
                    }
                    else
                    {
                        for (int j = InternalPtList[InsertPtIndex].InsertPtIndex; j < InternalPtList[InsertPtIndex].InsertPtIndex + 2; j++)
                        {
                            double CurDistance = InsertCost(InternalPtList[k].Point.StartPoint, ConvexHullList[j].StartPoint, ConvexHullList[j + 1].StartPoint);
                            if (InternalPtList[k].MinCost > CurDistance)
                            {
                                InternalPtList[k].MinCost = CurDistance;
                                InternalPtList[k].InsertPtIndex = j;
                            }
                        }
                    }

                }
                //InternalPtList.RemoveAt(InsertPtIndex);
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
            List<EMShape> OutList = new List<EMShape>();
            //将起点跟终点存入目标列表
            OutList.Add(PointList[0]);
            OutList.Add(PointList.Last());

            //将原点集中除了原点和终点以外其他的点存入待插入列表中
            int Count = PointList.Count;
            List<InsertPt> InternalPtList = new List<InsertPt>();
            for (int i = 1; i < PointList.Count - 1; i++)
            {
                InsertPt InternalPt = new InsertPt();
                InternalPt.Point = PointList[i];
                InternalPtList.Add(InternalPt);
            }

            //初始距离表
            for (int i = 0; i < InternalPtList.Count; i++)
            {
                //计算插入代价，存储每个点的最小插入代价和对应索引点
                for (int j = 0; j < OutList.Count - 1; j++)
                {
                    double CurDistance = InsertCost(InternalPtList[i].Point.StartPoint, OutList[j].StartPoint, OutList[j + 1].StartPoint);
                    if (InternalPtList[i].MinCost > CurDistance || InternalPtList[i].MinCost == 0)
                    {
                        InternalPtList[i].MinCost = CurDistance;
                        InternalPtList[i].InsertPtIndex = j;
                    }
                }
            }

            //插入点集
            for (int i = 0; i < InternalPtList.Count; i++)//依次将内部点插入凸包点集中，迭代次数等于内部点个数
            {
                //插入代价最小点
                int InsertPtIndex = 0;
                double MinDistanceProportion = 0;
                for (int j = 0; j < InternalPtList.Count; j++)
                {
                    if (InternalPtList[j].InsertFlag) continue;
                    double CurDistanceProportion = InternalPtList[j].MinCost;
                    if ((MinDistanceProportion > CurDistanceProportion) || MinDistanceProportion == 0)
                    {
                        InsertPtIndex = j;
                        MinDistanceProportion = CurDistanceProportion;
                    }
                }

                OutList.Insert(InternalPtList[InsertPtIndex].InsertPtIndex + 1, InternalPtList[InsertPtIndex].Point);
                //test
                //Console.WriteLine("插入{0},{1}", InternalPtList[InsertPtIndex].StartPoint.StartPoint.XPos, InternalPtList[InsertPtIndex].StartPoint.StartPoint.YPos);
                //for (int a = 0; a < OutList.Count; a++) Console.WriteLine("{0},{1}", OutList[a].StartPoint.XPos, OutList[a].StartPoint.YPos);
                //Console.WriteLine("------------");

                InternalPtList[InsertPtIndex].InsertFlag = true;
                if (i == InternalPtList.Count - 1) break;
                //插入后InsertPtIndex索引改变,更新路径值
                for (int k = 0; k < InternalPtList.Count; k++)
                {
                    //进行一次插入之后，插入点后面所有点的索引+1
                    if (InternalPtList[k].InsertPtIndex > InternalPtList[InsertPtIndex].InsertPtIndex) InternalPtList[k].InsertPtIndex++;
                    if (InternalPtList[k].InsertFlag) continue;
                    //更新路径值时，如果待插入点原有的索引点的路径消失，需要重算该待插入点最小插入代价和对应索引点
                    if (InternalPtList[InsertPtIndex].InsertPtIndex == InternalPtList[k].InsertPtIndex)
                    {
                        InternalPtList[k].MinCost = 0;
                        for (int j = 0; j < OutList.Count - 1; j++)
                        {
                            double CurDistance = InsertCost(InternalPtList[k].Point.StartPoint, OutList[j].StartPoint, OutList[j + 1].StartPoint);
                            if (InternalPtList[k].MinCost > CurDistance || InternalPtList[k].MinCost == 0)
                            {
                                InternalPtList[k].MinCost = CurDistance;
                                InternalPtList[k].InsertPtIndex = j;
                            }
                        }
                    }
                    else//如果待插入点原有的索引点的路径未消失，只需比较新增点对应的两条路径与该待插入点原有的最小插入代价和对应索引点
                    {
                        for (int j = InternalPtList[InsertPtIndex].InsertPtIndex; j < InternalPtList[InsertPtIndex].InsertPtIndex + 2; j++)
                        {
                            double CurDistance = InsertCost(InternalPtList[k].Point.StartPoint, OutList[j].StartPoint, OutList[j + 1].StartPoint);
                            if (InternalPtList[k].MinCost > CurDistance)
                            {
                                InternalPtList[k].MinCost = CurDistance;
                                InternalPtList[k].InsertPtIndex = j;
                            }
                        }
                    }

                }
                //InternalPtList.RemoveAt(InsertPtIndex);
            }

            PointList.Clear();
            OutList.ForEach(Point => PointList.Add(Point));
        }
        #endregion

        private void LinkExpansion(LinkedList<EMShape> PointLinkedList)
        {
            LinkedList<EMShape> OutList = new LinkedList<EMShape>();

            LinkedListNode<EMShape> SecondPointNode;
            //将起点跟终点存入目标列表
            OutList.AddLast(PointLinkedList.First);
            OutList.AddLast(PointLinkedList.Last);
            PointLinkedList.RemoveFirst();
            PointLinkedList.RemoveLast();


            //将原点集中除了原点和终点以外其他的点存入待插入列表中以及初始距离表
            LinkedList<InsertPt> InternalPtList = new LinkedList<InsertPt>();

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
                InternalPtList.AddLast(InternalPt);
            }

            //初始距离表
            //for (int i = 0; i < InternalPtList.Count; i++)
            //{
            //    //计算插入代价，存储每个点的最小插入代价和对应索引点
            //    for (int j = 0; j < OutList.Count - 1; j++)
            //    {
            //        double CurDistance = InsertCost(InternalPtList[i].StartPoint.StartPoint, OutList[j].StartPoint, OutList[j + 1].StartPoint);
            //        if (InternalPtList[i].MinCost > CurDistance || InternalPtList[i].MinCost == 0)
            //        {
            //            InternalPtList[i].MinCost = CurDistance;
            //            InternalPtList[i].InsertPtIndex = j;
            //        }
            //    }
            //}
            int Count = InternalPtList.Count;
            LinkedListNode<InsertPt> InsertPointNodeList;
            LinkedListNode<EMShape> OutListNode;
            //插入点集
            for (int i = 0; i < Count; i++)//依次将内部点插入凸包点集中，迭代次数等于内部点个数
            {
                //插入代价最小点
                //int InsertPtIndex = 0;
                LinkedListNode<InsertPt> InsertNode = InternalPtList.First;
                double MinCost = 0;
                for (InsertPointNodeList = InternalPtList.First; InsertPointNodeList.Value != null; InsertPointNodeList = InsertPointNodeList.Next)
                {
                    if (InsertPointNodeList.Value.InsertFlag) continue;

                    double CurCost = InsertPointNodeList.Value.MinCost;
                    if ((MinCost > CurCost) || MinCost == 0)
                    {
                        InsertNode = InsertPointNodeList;
                        MinCost = CurCost;
                    }
                }

                OutList.AddAfter(InsertNode.Value.PreInsertNode, InsertNode.Value.Point);

                //test
                //Console.WriteLine("插入{0},{1}", InternalPtList[InsertPtIndex].StartPoint.StartPoint.XPos, InternalPtList[InsertPtIndex].StartPoint.StartPoint.YPos);
                //for (int a = 0; a < OutList.Count; a++) Console.WriteLine("{0},{1}", OutList[a].StartPoint.XPos, OutList[a].StartPoint.YPos);
                //Console.WriteLine("------------");

                //if (i == InternalPtList.Count - 1) break;
                //插入后InsertPtIndex索引改变,更新路径值
                for (InsertPointNodeList = InternalPtList.First; InsertPointNodeList.Value != null; InsertPointNodeList = InsertPointNodeList.Next)
                {
                    //更新路径值时，如果待插入点原有的索引点的路径消失，需要重算该待插入点最小插入代价和对应索引点
                    if (InsertNode.Value.PreInsertNode == InsertPointNodeList.Value.PreInsertNode)
                    {
                        InsertPointNodeList.Value.MinCost = 0;
                        //
                        for (OutListNode = OutList.First;OutListNode.Next!=null;OutListNode=OutListNode.Next)
                        {
                            double CurDistance = InsertCost(InsertPointNodeList.Value.Point.StartPoint, OutListNode.Value.StartPoint, OutListNode.Next.Value.StartPoint);
                            if (InsertPointNodeList.Value.MinCost > CurDistance || InsertPointNodeList.Value.MinCost == 0)
                            {
                                InsertPointNodeList.Value.MinCost = CurDistance;
                                InsertPointNodeList.Value.PreInsertNode = OutListNode;
                            }
                        }
                    }
                    else//如果待插入点原有的索引点的路径未消失，只需比较新增点对应的两条路径与该待插入点原有的最小插入代价和对应索引点
                    {
                        OutListNode = InsertNode.Value.PreInsertNode;
                        for (int j = 0; j <  2; j++, OutListNode=OutListNode.Next)
                        {
                            double CurDistance = InsertCost(InsertPointNodeList.Value.Point.StartPoint, OutListNode.Value.StartPoint, OutListNode.Next.Value.StartPoint);
                            if (InsertPointNodeList.Value.MinCost > CurDistance)
                            {
                                InsertPointNodeList.Value.MinCost = CurDistance;
                                InsertPointNodeList.Value.PreInsertNode = OutListNode;
                            }
                        }
                    }

                }
                InternalPtList.Remove(InsertNode.Value);
            }

            PointLinkedList = OutList;
        }
    }
}
