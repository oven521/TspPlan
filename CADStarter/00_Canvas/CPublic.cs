using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CADEngine
{
   public class CPublic
    {
        public static float CalDis(PointF p1, PointF p2)
        {
             return (float)Math.Sqrt( (p1.X-p2.X)*(p1.X-p2.X) +(p1.Y-p2.Y)*(p1.Y-p2.Y) );
        
        }

        //计算线的角度
        public static float GetAngleByJiaoDu(PointF startP, PointF endP)
        {
            return (float) (GetAngleByHuDu(startP, endP) * 180 / Math.PI);
        }

        //计算线的弧度
        public static double GetAngleByHuDu(PointF centerP, PointF endP)
        {
            double k;
            if ( Math.Abs(centerP.X - endP.X)<0.0001 )
            {
                if (Math.Abs(centerP.Y - endP.Y)<0.0001)
                {
                    return 0;
                }
                else if (endP.Y > centerP.Y)
                {
                    return Math.PI / 2;
                }
                else
                {
                    return 3 * Math.PI / 2;
                }
            }
            else
            {
                double dy = (endP.Y - centerP.Y);
                double dx = (endP.X - centerP.X);

                k = Math.Abs(dy / dx);
                if (k < 10E-5)
                {
                    if (endP.X > centerP.X)
                    {
                        return Math.PI;
                    }
                }
            }

            //第一象限
            if (endP.X > centerP.X && endP.Y >= centerP.Y)
            {
                return Math.Atan(k);
            }
            //第二象限
            else if (endP.X < centerP.X && endP.Y >= centerP.Y)
            {
                return Math.PI - Math.Atan(k);
            }
             //第三象限
            else if (endP.X < centerP.X && endP.Y <= centerP.Y)
            {
                return Math.PI + Math.Atan(k);
            }
             //第四象限
            else if (endP.X > centerP.X && endP.Y <= centerP.Y)
            {
                if ((2 * Math.PI - Math.Atan(k)) > 2*Math.PI)
                    return 2*Math.PI;
                else
                    return 2 * Math.PI - Math.Atan(k);
            }
            else
            {
                return 0;
            }

        }

        public static bool GetCenterRadius3p(PointF pt1, PointF pt2, PointF pt3, ref PointF center, ref float radius)
        {

            float a, b, c, d, e, F, G, H;
            a = pt2.X - pt1.X;
            b = pt2.Y - pt1.Y;
            c = pt2.X + pt1.X;
            d = pt2.Y + pt1.Y;

            e = pt3.X - pt1.X;
            F = pt3.Y - pt1.Y;
            G = pt3.X + pt1.X;
            H = pt3.Y + pt1.Y;

            if (a * F == b * e)
            {
                return false;
            }
            else
            {
                center.Y = ((e * G + F * H) * a - (a * c + b * d) * e) / (2 * a * F - 2 * b * e);
                center.X = ((e * G + F * H) * b - (a * c + b * d) * F) / (2 * b * e - 2 * a * F);

                radius = (float)CPublic.CalDis(center, pt1);
                return true;
            }
        }
    }
}
