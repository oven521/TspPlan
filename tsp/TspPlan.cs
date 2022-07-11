

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace tsp
{
    class TspPlan
    {
        private static int max = 1000;
        //后续可能会改成结构体或者链表的结构
        private double[] SourceX = new double[max];
        private double[] SourceY = new double[max];

        //test:1代表贪心，0代表改良圈
        int ChoiceFlag = 0;

        private int SpotNum = 0;
        //总距离
        private double sumDistance = 0;

        public TspPlan(double[] sourceX, double[] sourceY, int spotNum)
        {
            this.SourceX = sourceX;
            //SourceX[spotNum] = SourceX[0];//将终点设为起始点
            
            this.SourceY = sourceY;
            //SourceY[spotNum] = SourceY[0];
            this.SpotNum = spotNum;
            //SpotNum = spotNum+1;

            if(ChoiceFlag==1)
            {
               greed();
            }
            else if(ChoiceFlag==0)
            {
               CircleModification();
            }
        }

        private double Distance(double source_x, double source_y, double target_x, double target_y)
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
            double temp;
            for (int i = begin; i < (begin + end) / 2; i++)
            {
                //交换x，y坐标
                temp = SourceX[i];
                SourceX[i] = SourceX[end - i + begin];
                SourceX[end - i + begin] = temp;

                temp = SourceY[i];
                SourceY[i] = SourceY[end - i + begin];
                SourceY[end - i + begin] = temp;
            }
        }
        //改良圈算法
        private void CircleModification()
        {
            int StopFlag;//停止标志
            int testcircle = 0;//循环次数
            for(int k=0;k< SpotNum/2; k++)
            {
                StopFlag = 0;
                //遍历一轮
                for (int i = 0; i < SpotNum - 2; i++)
                {
                    for (int j = i + 2; j < SpotNum - 1; j++)
                    {
                         testcircle++;
                        //比较颠倒顺序前后的总路线长
                        //if (Distance(SourceX[i], SourceY[i], SourceX[j], SourceY[j]) + Distance(SourceX[i + 1], SourceY[i + 1], SourceX[j + 1], SourceY[j + 1]) <
                        //Distance(SourceX[i], SourceY[i], SourceX[i + 1], SourceY[i + 1]) + Distance(SourceX[j], SourceY[j], SourceX[j + 1], SourceY[j + 1]))
                        double temp = Distance(SourceX[i], SourceY[i], SourceX[j], SourceY[j]) + Distance(SourceX[i + 1], SourceY[i + 1], SourceX[j + 1], SourceY[j + 1]);
                        double temp2 = Distance(SourceX[i], SourceY[i], SourceX[i + 1], SourceY[i + 1]) + Distance(SourceX[j], SourceY[j], SourceX[j + 1], SourceY[j + 1]);
                        if (temp < temp2)
                        {
                            StopFlag++;
                            Reverse(i + 1, j);//将i+1和j之间的顺序颠倒
                        }
                        CalculateSumDistance();
                        Console.WriteLine(sumDistance);
                    }
                        //Console.WriteLine("------------------------------");
                }
                if (StopFlag == 0)
                {
                    break;
                }
                //CalculateSumDistance();
                //Console.WriteLine(sumDistance);
                
            }
            Console.WriteLine(testcircle);
            CalculateSumDistance();
        }

        private void greed()
        {
            double temp;
            int testcircle = 0;
            for (int i=0;i<SpotNum-2;i++)
            {
                double min = Distance(SourceX[i], SourceY[i], SourceX[i+1], SourceY[i+1]);
                int MinSpot=i+1;
                for (int j=i+1;j<SpotNum;j++)
                {
                    testcircle++;//test
                    if (Distance(SourceX[i], SourceY[i], SourceX[j], SourceY[j])<min)
                    {
                        MinSpot = j;
                        min = Distance(SourceX[i], SourceY[i], SourceX[j], SourceY[j]);
                    }
                }
                //交换x，y坐标
                temp = SourceX[i + 1];
                SourceX[i + 1] = SourceX[MinSpot];
                SourceX[MinSpot] = temp;

                temp = SourceY[i + 1];
                SourceY[i + 1] = SourceY[MinSpot];
                SourceY[MinSpot] = temp;

            }
            Console.WriteLine(testcircle);
            CalculateSumDistance();
            //Console.WriteLine(sumDistance);
        }
    }
}
