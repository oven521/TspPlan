using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _05_SplineFunction
{
    /// <summary>
    /// 三次样条曲线的计算，本类只考虑的两种类型的边界条件：
    /// 1.自然边界条件(Natural)：第一个点和最后一个点的二阶导数为0。
    /// 2.固定边界(Clamped)：给出第一个点和最后一个点的二阶导数。
    /// </summary>
    public class Spline3
    {
        double[] x;//X轴坐标
        double[] y;//Y轴坐标
        double[] y1;//计算后的一阶导数
        double[] y2;//计算后的二阶导数

        double yp1;//第一个点的一阶导数，曲线计算的时候，用来做边界条件，如果大于.99E30,那么就是自然边界（这个点的一阶导数为0就是自然边界）
        double ypn;//最后一个点的一阶导数，曲线计算的时候，用来做边界条件，如果大于.99E30,那么就是自然边界（这个点的一阶导数为0就是自然边界）

        int n;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xData">x轴坐标数组</param>
        /// <param name="yData">y轴坐标数组</param>
        /// <param name="yp1">第一个点的一阶导数边界条件，如果输入1e30，则为自然边界条件</param>
        /// <param name="ypn">最后一个点的一阶导数边界条件，如果输入1e30，则为自然边界条件</param>
        public Spline3(double[] xData, double[] yData, double yp1, double ypn)
        {
            //n保存了输入数据的个数。
            n = xData.Length  ;
            //新开的数组的大小比输入数据多一位，因为整个计算的下标是从1开始计算的。
            this.x = new double[n+1];
            this.y = new double[n+1];
            //把输入数据放入到新开的数组，注意从下标1开始放
            for (int i = 0; i < n;++i )
            {
                this.x[i + 1] = xData[i];
                this.y[i + 1] = yData[i];
            }
            //初始化边界条件
            this.yp1 = yp1;
            this.ypn = ypn;

            //用来保存一阶和二阶导数，下标也是从1开始算，所以数组的长度为n+1
            //注意，y1和y2的第0位是不用的，从下标1开始。
            y1 = new double[n+1];
            y2 = new double[n+1];
        }
        /// <summary>
        /// 返回一阶导数数组，数据从下标1开始，第0位不使用。
        /// </summary>
        public double[] Velocity { get { return y1; } }
        /// <summary>
        /// 返回二阶导数数组，数据从下标1开始，第0位不使用。
        /// </summary>
        public double[] Accelation { get { return y2; } }

        public void CubicSplineCalc()
        {
            int i, k;
            double p, qn, sig, un;
            double[] u = new double[n+1];//不过U最多用的n-1个下标

            if (yp1 > 0.99e30) 	//The lower boundary condition is set either to be "natural"
            {
                y2[1] = 0.0;
                u[1] = 0.0; 
            }
            else  					//or else to have a specified first derivative.
            {
                y2[1] = -0.5;
                u[1] = (3.0/(x[2]-x[1]))*((y[2]-y[1])/(x[2]-x[1])-yp1);
            }
            //This is the decomposition loop of the tridiagonal algorithm.
            //y2 and u are used for temporary storage of the decomposed factors.
            for (i = 2; i <= n - 1; ++i)
            {
                sig = (x[i] - x[i - 1]) / (x[i + 1] - x[i - 1]);
                p = sig * y2[i - 1] + 2.0;
                y2[i] = (sig - 1.0) / p;
                u[i] = (y[i + 1] - y[i]) / (x[i + 1] - x[i]) - (y[i] - y[i - 1]) / (x[i] - x[i - 1]);
                u[i] = (6.0 * u[i] / (x[i + 1] - x[i - 1]) - sig * u[i - 1]) / p;
            }
            if (ypn > 0.99e30) 	//The upper boundary condition is set either to be "natural"
            {
                qn = 0.0;
                un = 0.0;
            }
            else //or else to have a specified first derivative.
            {
                qn = 0.5;
                un = (3.0 / (x[n] - x[n - 1])) * (ypn - (y[n] - y[n - 1]) / (x[n] - x[n - 1]));
            }
            y2[n] = (un - qn * u[n - 1]) / (qn * y2[n - 1] + 1.0);
            // This is the backsubstitution loop of the tridiagonal algorithm.
            for (k = n - 1; k >= 1; k--)
            {
                y2[k] = y2[k] * y2[k + 1] + u[k];
            }
            //Calculate the first derivatives
            for (i = 1; i <= n - 1; ++i)
            {
                p = x[i + 1] - x[i];
                y1[i] = (y[i + 1] - y[i]) / p - (1.0 / 3.0 * p * y2[i]) - (1.0 / 6.0 * p * y2[i + 1]);
            }

            p = x[n] - x[n - 1];
            y1[n] = (y[n] - y[n - 1]) / p + (1.0 / 6.0 * p * y2[n - 1]) + (1.0 / 3.0 * p * y2[n]);

        }
        public double GetYPos(double xPos)
        {
            double yPos = 0;

            splint(x, y, y2, n, xPos, ref yPos);

            return yPos;
        }
        /// <summary>
        /// 根据输入的x,返回插补值
        /// </summary>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        /// <param name="y2a"></param>
        /// <param name="n"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void splint(double[] xa, double[]  ya, double[]  y2a, int n, double x, ref double y)
        {
            int klo,khi,k;
            double h,b,a;
            klo=1; 
            khi=n;
            while (khi-klo > 1) {
            k=(khi+klo) / 2;
            if (xa[k] > x) khi=k;
            else klo=k;
            } 
            h=xa[khi]-xa[klo];
           // if (h == 0.0) nrerror("Bad xa input to routine splint"); 
            a=(xa[khi]-x)/h; 
            b=(x-xa[klo])/h; 
            y=a*ya[klo]+b*ya[khi]+((a*a*a-a)*y2a[klo]+(b*b*b-b)*y2a[khi])*(h*h)/6.0;
        }


    }
}
