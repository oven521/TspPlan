using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CADEngine
{
    public class CGeometryLine : CGeometry
    {
    /*****************************\ 
    * * 
    * 线段及直线的基本运算 * 
    * * 
    \*****************************/

        /* 判断点与线段的关系,用途很广泛 
        本函数是根据下面的公式写的，P是点C到线段AB所在直线的垂足 
 
                        AC dot AB 
                r = --------- 
                         ||AB||^2 
                     (Cx-Ax)(Bx-Ax) + (Cy-Ay)(By-Ay) 
                  = ------------------------------- 
                                  L^2 
 
            r has the following meaning: 
 
                r=0 P = A 
                r=1 P = B 
                r<0 P is on the backward extension of AB 
          r>1 P is on the forward extension of AB 
                0<r<1 P is interior to AB 
        */
        //****************************************************************************** 
        //判断点p是否在线段l上 
        //条件：(p在线段l所在的直线上) && (点p在以线段l为对角线的矩形内) 
        //这个条件的判断比较严苛，必须保证一点没有误差。
        //*******************************************************************************/ 
        public static bool IsOnlineByDist(GLineSeg l, PointF p)
       {

           return (
               (ptoldist(p,l)<0.01)
               && (((p.X - l.s.X) * (p.X - l.e.X) <= 0)
               && ((p.Y - l.s.Y) * (p.Y - l.e.Y) <= 0))

               );
       }

        //    /****************************************************************************** 
        //判断点p是否在线段l上 
        //条件：(p在线段l所在的直线上) && (点p在以线段l为对角线的矩形内) 
        //这个条件的判断比较严苛，必须保证一点没有误差。
        //*******************************************************************************/ 
        public static bool IsOnlineByMultiply(GLineSeg l, PointF p)
        {
            return (
                (multiply(l.e, p, l.s) == 0)
                && (((p.X - l.s.X) * (p.X - l.e.X) <= 0)
                && ((p.Y - l.s.Y) * (p.Y - l.e.Y) <= 0))

                );
        }

        public static double relation(PointF p, GLineSeg l)
        {
            GLineSeg tl;
            tl.s = l.s;
            tl.e = p;
            return dotmultiply(tl.e, l.e, l.s) / (dist(l.s, l.e) * dist(l.s, l.e));
        }
        //// 求点C到线段AB所在直线的垂足 P 
        public static PointF perpendicular(PointF p, GLineSeg l)
        {
            double r = relation(p, l);
            PointF tp = new PointF();
            tp.X =(float) ( l.s.X + r * (l.e.X - l.s.X));
            tp.Y = (float) (l.s.Y + r * (l.e.Y - l.s.Y));
            return tp;
        }
        /// <summary>
        /// 求点P点到线段l的垂直距离，如果在线段footOnSeg为True
        /// </summary>
        /// <param name="p">线外的一个点</param>
        /// <param name="l">线段</param>
        /// <param name="np">垂足</param>
        /// <param name="footOnSeg">垂足在线段上为true，否则为false</param>
        /// <returns>垂线的距离</returns>
        public static double ptolinesegdist(PointF p, GLineSeg l, ref PointF np, out bool footOnSeg) 
        { 
             double r=relation(p,l); 
             if(r<0) 
             { 
              np=l.s;
              footOnSeg = false;
              return dist(p,l.s); 
             } 
             if(r>1) 
             { 
              np=l.e;
              footOnSeg = false;
              return dist(p,l.e); 
             } 
             np=perpendicular(p,l);
             footOnSeg = true;
             return dist(p,np); 
        } 
    //// 求点p到线段l所在直线的距离,请注意本函数与上个函数的区别 
        public static double ptoldist(PointF p, GLineSeg l)
        {
            return Math.Abs(multiply(p, l.e, l.s)) / dist(l.s, l.e);
        } 
    ///* 计算点到折线集的最近距离,并返回最近点. 
    //注意：调用的是ptolineseg()函数 */ 
     public static  double ptopointset(int vcount,PointF[] pointset,PointF p, ref PointF q) 
    { 
         int i; 
         double cd=Double.MaxValue,td;
         bool isFootOnSeg;
         GLineSeg l; 
         PointF tq = new PointF();
         PointF cq = new PointF(); 
  
         for(i=0;i<vcount-1;i++) 
         { 
              l.s=pointset[i]; 
  
              l.e=pointset[i+1];
              td = ptolinesegdist(p, l, ref tq, out isFootOnSeg); 
              if(td<cd) 
              { 
               cd=td; 
               cq=tq; 
              } 
         } 
         q=cq; 
         return cd; 
    }
     // 如果两条直线 l1(a1*x+b1*y+c1 = 0), l2(a2*x+b2*y+c2 = 0)相交，返回true，且返回交点p 
     public static bool Islineintersect(GLine l1, GLine l2, ref PointF p) // 是 L1，L2 
     {
         double d = l1.a * l2.b - l2.a * l1.b;
         if (Math.Abs(d) < EP) // 不相交 
             return false;

         double xpos = (l2.c * l1.b - l1.c * l2.b) / d;
         double ypos = (l2.a * l1.c - l1.a * l2.c) / d;
         p.X = (float)xpos;
         p.Y = (float)ypos;
         return true;
     }
     //// 如果线段l1和l2相交，返回true且交点由(inter)返回，否则返回false 
     public static bool IsLineSegIntersect(GLineSeg l1, GLineSeg l2, ref PointF inter)
     {
         GLine ll1, ll2;
         ll1 = MakeALine(l1.s, l1.e);
         ll2 = MakeALine(l2.s, l2.e);
         if (Islineintersect(ll1, ll2, ref inter))
             return IsOnlineByDist(l1, inter) && IsOnlineByDist(l2, inter);
         else
             return false;
     } 

    }
}
