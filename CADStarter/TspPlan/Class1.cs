using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
class Melkman
{
    static double[] tmp;
    PointF[] pointArray; //坐标数组
    int N; //数据个数
    int[] D; //数组索引，双向表
    public Melkman(List<PointF> pList)
    {
        pointArray = pList.ToArray();
        var array = pointArray.Select(n => n.Y).ToArray();
        int num = Array.IndexOf(array, array.Min());
        tmp = new double[] { pointArray[num].X, pointArray[num].Y };
        N = pList.Count;
        D = new int[2 * N];
    }
    class Mycomparer : IComparer<PointF>
    {
        public int Compare(PointF a, PointF b) //按角度(距离)从小到大排序，角度相同时比较距离
        {
            double delta_x, delta_y, dist_a, dist_b, rad_a, rad_b;
            delta_x = a.X - tmp[0];
            delta_y = a.Y - tmp[1];
            dist_a = Math.Sqrt(delta_x * delta_x + delta_y * delta_y); //得到顶点i到tmp点的线段长度  
            if (delta_y < 0) delta_x = -Math.Abs(delta_x);
            rad_a = Math.Acos(delta_x / dist_a); //得到该线段与x轴的角度
            rad_a = double.IsNaN(rad_a) ? 0 : rad_a;
            delta_x = b.X - tmp[0];
            delta_y = b.Y - tmp[1];
            dist_b = Math.Sqrt(delta_x * delta_x + delta_y * delta_y); //得到顶点i到tmp点的线段长度  
            if (delta_y < 0) delta_x = -Math.Abs(delta_x);
            rad_b = Math.Acos(delta_x / dist_b); //得到该线段与x轴的角度
            rad_b = double.IsNaN(rad_b) ? 0 : rad_b;
            return rad_a - rad_b > 1e-6 ? 1 : Math.Abs(rad_a - rad_b) < 1e-6 ? (dist_a > dist_b ? 1 : (Math.Abs(dist_a - dist_b) < 1e-6 ? 0 : -1)) : -1;
        }
    }
    public PointF[] getTubaoPoint()
    {
        
        int j = 0, index = 0, t;
        int bot = N - 1;
        int top = N;
        D[top++] = 0;
        D[top++] = 1;
        Array.Sort(pointArray, new Mycomparer());
        for (j = 2; j < N; j++) //寻找第三个点 要保证3个点不共线！==0 代表共线
        {
            if (isLeft(pointArray[D[top - 2]], pointArray[D[top - 1]], pointArray[j]) != 0) break;
            D[top - 1] = j; //共线就更换顶点
        }
        D[bot--] = j;
        D[top++] = j; //j是第三个点 不共线！
        if (isLeft(pointArray[D[N]], pointArray[D[N + 1]], pointArray[D[N + 2]]) < 0)
        {
            //此时队列中有3个点，要保证3个点a,b,c是成逆时针的，不是就调换ab
            t = D[N];
            D[N] = D[N + 1];
            D[N + 1] = t;
        }
        for (int i = j + 1; i < N; i++)
        {
            //如果成立就是i在凸包内，跳过 //top=n+3 bot=n-2
            if (isLeft(pointArray[D[top - 2]], pointArray[D[top - 1]], pointArray[i]) > 0 &&
                isLeft(pointArray[D[bot + 1]], pointArray[D[bot + 2]], pointArray[i]) > 0)
            {
                continue;
            }
            while (isLeft(pointArray[D[top - 2]], pointArray[D[top - 1]], pointArray[i]) <= 0)
            {
                top--;  //顺时针，退一步
            }
            D[top++] = i; //逆时针时确定并保留位置
            //反向表非左转 则退栈
            while (isLeft(pointArray[D[bot + 1]], pointArray[D[bot + 2]], pointArray[i]) <= 0)
            {
                bot++;
            }
            D[bot--] = i;
        }
        //凸包构造完成，D数组里bot+1至top-1内就是凸包的序列(头尾是同一点)
        PointF[] resultPoints = new PointF[top - bot - 1];
        for (int i = bot + 1; i < top; i++)
        {
            resultPoints[index++] = pointArray[D[i]];
        }
        return resultPoints;
    }
    double isLeft(PointF o, PointF a, PointF b) 
        //判断ba相对ao是不是左转,大于0则左转
    {
        double aoX = a.X - o.X;
        double aoY = a.Y - o.Y;
        double baX = b.X - a.X;
        double baY = b.Y - a.Y;
        double vec = aoX * baY - aoY * baX;
        return Math.Round(vec, 8);
    }
}
