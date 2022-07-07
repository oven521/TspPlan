

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tsp
{

    class TspPlan
    {

        //存储输入坐标点
        //private struct Spot
        //{
        //    double x;
        //    double y;
        //}
        //private Spot[] source1=new Spot[2000];
        private static int max = 100;
        private double[] TargetX = new double[max];
        private double[] TargetY = new double[max];

        private int SpotNum = 0;
        //总距离
        private double sumDistance = 0;

        public TspPlan(double[] sourceX, double[] sourceY, int spotNum)
        {
            SpotNum = spotNum;
            this.TargetX = sourceX;
            this.TargetY = sourceY;
            CircleModification();
        }

        private double Distance(double source_x, double source_y, double target_x, double target_y)
        {
            return Math.Sqrt(Math.Pow(source_x - target_x, 2) + Math.Pow(source_y - target_y, 2));
        }

        public double[] GetX() => TargetX;
        public double[] GetY() => TargetY;
        public int GetLength() => SpotNum;
        public double SumDistance() => sumDistance;

        private void CalculateSumDistance()
        {
            sumDistance = 0;
            for (int i = 0; i < SpotNum - 1; i++)
            {
                sumDistance += Distance(TargetX[i], TargetY[i], TargetX[i + 1], TargetY[i + 1]);
            }
        }
        //颠倒路径中的顺序
        private void Reverse(int begin, int end)
        {
            double temp;
            for (int i = begin; i < (begin + end) / 2; i++)
            {
                temp = TargetX[i];
                TargetX[i] = TargetX[end - i + begin];
                TargetX[end - i + begin] = temp;

                temp = TargetY[i];
                TargetY[i] = TargetY[end - i + 1];
                TargetY[end - i + 1] = temp;
            }
        }

        private void CircleModification()
        {
            for (int i = 0; i < SpotNum-2; i++)
            {
                for(int j = i + 2; j < SpotNum-1; j++)
                {
                    //比较颠倒顺序前后的总路线长
                    if(Distance(TargetX[i],TargetY[i], TargetX[j], TargetY[j])+ Distance(TargetX[i+1], TargetY[i+1], TargetX[j+1], TargetY[j+1])<
                        Distance(TargetX[i], TargetY[i], TargetX[i+1], TargetY[i + 1]) + Distance(TargetX[j], TargetY[j], TargetX[j+1], TargetY[j+1]))
                    {
                        //test

                        Reverse(i+1,j);//将i+1和j之间的顺序颠倒
                        CalculateSumDistance();
                        Console.WriteLine(sumDistance);

                    }
                }
            }

            CalculateSumDistance();
            Console.WriteLine(sumDistance);
        }


    }
}
