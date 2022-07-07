using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CADEngine
{
    public struct GLineSeg
    {
        public PointF s;
        public PointF e;
        public GLineSeg(PointF a, PointF b) { s = a; e = b; }

    };
    // 直线的解析方程 a*x+b*y+c=0  为统一表示，约定 a >= 0 
    public struct GLine
    {
        public double a;
        public double b;
        public double c;
        public GLine(double d1 = 1, double d2 = -1, double d3 = 0) { a = d1; b = d2; c = d3; }
    };
    public class CGeometry
    {   
        
       /* 常用的常量定义 */ 
       public static readonly  double	INF		= 1E200; 
       public static readonly  double	EP		= 1E-10;
       public static readonly  int		MAXV	= 300;
       public static readonly double    PI = 3.14159265; 
     
        // 返回点p以点o为圆心逆时针旋转alpha(单位：弧度)后所在的位置 
        // 注意C#Math命名空间下的三角函数以弧度为单位
        public static PointF PointRotate(PointF o, double alpha, PointF p)
        {
            PointF tp = new PointF(0, 0);
            p.X -= o.X;
            p.Y -= o.Y;
            tp.X = (float)(p.X * Math.Cos(alpha) - p.Y * Math.Sin(alpha) + o.X);
            tp.Y = (float)(p.Y * Math.Cos(alpha) + p.X * Math.Sin(alpha) + o.Y);
            return tp;
        }
        public static PointF GetLineMiddlePT(PointF p1, PointF p2) {

            return new PointF(p1.X / 2.0F + p2.X / 2.0F, p1.Y / 2.0F + p2.Y / 2.0F);
        }
       public static GLineSeg MakeALineSeg(PointF p1, PointF p2)
       {
           GLineSeg lineSegObj = new GLineSeg(p1,p2);
           return lineSegObj;
       }
       //通过LineSeg获取直线的解析方程
       public static GLine MakeALine(GLineSeg lineSeg)
       {
           return MakeALine(lineSeg.s,lineSeg.e);
       }
       //根据线段上一个点，求出过这个点的这条直线的垂线
       public static GLine GetVeticalLine(GLineSeg lineSeg, PointF foot)
       {
           GLine oline = MakeALine(lineSeg.s, lineSeg.e);

           return GetVeticalLine(oline,foot);

       }
       //根据直线上一个点，求出过这个点的这条直线的垂线
       public static GLine GetVeticalLine(GLine oline,PointF foot)
       {
           GLine verticalLine = new GLine();

           verticalLine.a = oline.b;
           verticalLine.b = -oline.a;
           verticalLine.c = -(verticalLine.a * foot.X + verticalLine.b * foot.Y);

           int sign = 1;
           if (verticalLine.a < 0)
               sign = -1;

           verticalLine.a = verticalLine.a * sign;
           verticalLine.b = verticalLine.b * sign;
           verticalLine.c = verticalLine.c * sign;

           return verticalLine;
       }
        //根据圆心和圆上一点，求出过这一点的圆的切线。
        public static GLine GetCutLine(PointF center, float r,PointF pointOnArc)
        {
            GLine lineCenter2Arc = CGeometry.MakeALine(center, pointOnArc);

            return GetVeticalLine(lineCenter2Arc, pointOnArc);
        }
            
        // 根据已知两点坐标，求过这两点的直线解析方程： a*x+b*y+c = 0  (a >= 0)  
       public static GLine MakeALine(PointF p1, PointF p2)
        {
            GLine tl;
            int sign = 1;
            tl.a = p2.Y - p1.Y;
            if (tl.a < 0)
            {
                sign = -1;
                tl.a = sign * tl.a;
            }
            tl.b = sign * (p1.X - p2.X);
            tl.c = sign * (p1.Y * p2.X - p1.X * p2.Y);
            return tl;
        }

       // 根据两点返回直线的倾斜角度，角度范围0-2*PI
       public static double SlopeAngleHudu(GLineSeg l)
       {
           float deltaX = l.e.X - l.s.X;
           float deltaY = l.e.Y - l.s.Y;

           return SlopeAngleHudu(l.s, l.e);
       }
       // 根据两点返回直线的倾斜角度，角度范围0-2*PI
       public static double SlopeAngleHudu(PointF start,PointF end)
       {
           float deltaX = end.X - start.X;
           float deltaY = end.Y - start.Y;
           double slope = Math.Atan2(deltaY, deltaX);
           if (slope < 0)
               slope += 2 * PI;
           return slope;
       }

       // 根据两点返回直线的倾斜角度，角度范围0-2*PI
       public static double SlopeAngleJiaodu(GLineSeg l)
       {
           return SlopeAngleHudu(l) * 360.0 / (2 * Math.PI);
       }
       // 根据两点返回直线的倾斜角度，角度范围0-2*PI
       public static double SlopeAngleJiaodu(PointF start, PointF end)
       {
           return SlopeAngleHudu(start,end)*360.0/(2*Math.PI);
       }

        public static double dist(PointF p0, PointF p1)
        {
            double deltax = Math.Abs(p1.X - p0.X);
            double deltay = Math.Abs(p1.Y - p0.Y);

            return Math.Sqrt(deltax*deltax + deltay*deltay);
        }
       
       public static PointF translation(PointF P0, GLineSeg lineSeg, double length)
        {
            double slopeAngle = SlopeAngleHudu(lineSeg);
            double x = 0;
            double y = 0;

            x = lineSeg.e.X + length * Math.Cos(slopeAngle);
            y = lineSeg.e.Y + length * Math.Sin(slopeAngle);

            PointF newPoint = new PointF((float)x,(float)y);

            return newPoint;

        }
       public static GLineSeg CalcArrowLine(GLineSeg lineSeg, double arrowLength,double arrowAngle)
        {
            PointF arrowStart = translation(lineSeg.e, lineSeg, -arrowLength);
            PointF arrowEnd = lineSeg.e;
            //以arrowEnd为圆心，逆时针旋转arrowStart，旋转的角度为arrowAngle
            arrowStart = PointRotate(arrowEnd, arrowAngle, arrowStart);

            GLineSeg lineSegObj = new GLineSeg(arrowStart, arrowEnd);

            return lineSegObj;
            
        }
       
        /****************************************************************************** 
        r=multiply(sp,ep,op),得到(sp-op)和(ep-op)的叉积 
        r>0：ep在矢量opsp的逆时针方向； 
        r=0：opspep三点共线； 
        r<0：ep在矢量opsp的顺时针方向 
        *******************************************************************************/
        public static double multiply(PointF sp, PointF ep, PointF op)
        {
            return ((sp.X - op.X) * (ep.Y - op.Y) - (ep.X - op.X) * (sp.Y - op.Y));
        }
        /* 
        r=dotmultiply(p1,p2,op),得到矢量(p1-op)和(p2-op)的点积，如果两个矢量都非零矢量 
        r<0：两矢量夹角为钝角； 
        r=0：两矢量夹角为直角； 
        r>0：两矢量夹角为锐角 
        *******************************************************************************/
       public static double dotmultiply(PointF p1, PointF p2, PointF p0)
        {
            return ((p1.X - p0.X) * (p2.X - p0.X) + (p1.Y - p0.Y) * (p2.Y - p0.Y));
        } 


        public static RectangleF MakeRect(PointF rectCenter, float w, float h)
        {
            RectangleF _rectBoundery = new RectangleF(rectCenter.X-w/2,rectCenter.Y-h/2,w,h);
            return _rectBoundery;
        }
        /// <summary>
        /// 根据圆心，半径和角度，求出此点在圆上的位置
        /// </summary>
        /// <param name="center"></param>
        /// <param name="r"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static PointF PointOnCircle(PointF center, double r, double angle)
        {
            PointF pt = new PointF();
            pt.X = (float)(center.X + r * Math.Cos(angle));
            pt.Y = (float)(center.Y + r * Math.Sin(angle));

            return pt;
        }
        public static void Swap<T>(ref T my, ref T other)
        {
            T temp = my;
            my = other;
            other = temp;
        }
    }
}
