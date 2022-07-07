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


        private double[] SourceX = new double[2000];
        private double[] SourceY = new double[2000];

        private int SpotNum = 0;
        //总距离
        private double sumDistance = 0;

        public TspPlan(double[] sourceX,double[] sourceY,int spotNum)
        {
            SpotNum = spotNum;
            this.SourceX = sourceX;
            this.SourceY = sourceY;
            CircleModification();
            
        }

        private double Distance(double source_x, double source_y, double target_x, double target_y)
        {
            return Math.Sqrt(Math.Pow(source_x - target_x, 2) + Math.Pow(source_y - target_y, 2));
        }

        public double[] GetX()
        {
            return SourceX;
        }

        public double[] GetY()
        {
            return SourceY;
        }

        public double SumDistance
        {
            get { return sumDistance; }
        }

        private void CircleModification()
        {
            for (int i = 0; i < SpotNum-2; i++)
            {
                for(int j = i + 2; j < SpotNum; j++)
                {
                    //
                    if(Distance(SourceX[i],SourceY[i], SourceX[j], SourceY[j])+ Distance(SourceX[i+1], SourceY[i+1], SourceX[j+1], SourceY[j+1])> 
                        Distance(SourceX[i], SourceY[i], SourceX[i+1], SourceY[i + 1]) + Distance(SourceX[j], SourceY[j], SourceX[j+1], SourceY[j+1]))
                    {
                        //将i+1和j之间的顺序颠倒
                    }
                }
            }

            CalculateSumDistance();

        }

        private void CalculateSumDistance()
        {
            for(int i=0;i<SpotNum;i++)
            {
                sumDistance += Distance(SourceX[i], SourceY[i], SourceX[i + 1], SourceY[i + 1]);
            }
        }

    }
}
