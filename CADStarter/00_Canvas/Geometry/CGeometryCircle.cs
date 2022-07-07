using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CADEngine
{
    public class CGeometryCircle : CGeometry
    {
        /*************************\ 
        * * 
        * 圆的基本运算 * 
        * * 
        \*************************/
        /****************************************************************************** 
        返回值 ： 点p在圆内(包括边界)时，返回true 
        用途 ： 因为圆为凸集，所以判断点集，折线，多边形是否在圆内时， 
        只需要逐一判断点是否在圆内即可。 
        *******************************************************************************/
        public static bool point_in_circle(PointF o, double r, PointF p)
        {
            double d2 = (p.X - o.X) * (p.X - o.X) + (p.Y - o.Y) * (p.Y - o.Y);
            double r2 = r * r;
            return d2 < r2 || Math.Abs(d2 - r2) < EP;
        }
        public static RectangleF circleRect(PointF center, float r)
        {
            return new RectangleF(center.X - r, center.Y - r, 2 * r, 2 * r);
        }
        /****************************************************************************** 
        用 途 ：求不共线的三点确定一个圆 
        输 入 ：三个点p1,p2,p3 
        返回值 ：如果三点共线，返回false；反之，返回true。圆心由q返回，半径由r返回 
        *******************************************************************************/
        public static bool cocircle(PointF p1, PointF p2, PointF p3, ref PointF q, ref float r)
        {
            double x12 = p2.X - p1.X;
            double y12 = p2.Y - p1.Y;
            double x13 = p3.X - p1.X;
            double y13 = p3.Y - p1.Y;
            double z2 = x12 * (p1.X + p2.X) + y12 * (p1.Y + p2.Y);
            double z3 = x13 * (p1.X + p3.X) + y13 * (p1.Y + p3.Y);
            double d = 2.0 * (x12 * (p3.Y - p2.Y) - y12 * (p3.X - p2.X));
            if (Math.Abs(d) < EP) //共线，圆不存在 
                return false;
            q.X = (float)((y13 * z2 - y12 * z3) / d);
            q.Y = (float)((x12 * z3 - x13 * z2) / d);
            r = (float)dist(p1, q);
            return true;
        }

        /// <summary>
        /// 根据圆心和圆上一点返回，对应的弧度，范围0-2*PI
        /// </summary>
        /// <param name="center"></param>
        /// <param name="pt"></param>
        /// <returns></returns>

        public static float ArcAngleHudu(PointF center, PointF pt)
        {
            float deltaX = pt.X - center.X;
            float deltaY = pt.Y - center.Y;
            double slope = Math.Atan2(deltaY, deltaX);
            if (slope < 0)
                slope += 2 * PI;
            return (float)slope;
        }
        /// <summary>
        /// 根据圆心和圆上一点返回，对应的弧度，范围0-2*PI
        /// </summary>
        /// <param name="center"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public static float ArcAngleJiaodu(PointF center, PointF pt)
        {
            double hudu = ArcAngleHudu(center, pt);

            return (float)(hudu * 180 / PI);
        }


        /* circle_circle_intersection() *
        * Determine the points where 2 circles in a common plane intersect.
        * http://paulbourke.net/geometry/circlesphere/
        * int circle_circle_intersection(
        *                                // center and radius of 1st circle
        *                                double x0, double y0, double r0,
        *                                // center and radius of 2nd circle
        *                                double x1, double y1, double r1,
        *                                // 1st intersection point
        *                                double *xi, double *yi,              
        *                                // 2nd intersection point
        *                                double *xi_prime, double *yi_prime)
        *
        * This is a public domain work. 3/26/2005 Tim Voght
        *
        */
        public static int circle_circle_intersection(PointF c0, double r0,
                                       PointF c1, double r1,
                                       ref double xi, ref double yi,
                                       ref double xi_prime, ref double yi_prime)
        {
            double x0, y0, x1, y1;
            double a, dx, dy, d, h, rx, ry;
            double x2, y2;
            x0 = c0.X;
            y0 = c0.Y;
            x1 = c1.X;
            y1 = c1.Y;

            /* dx and dy are the vertical and horizontal distances between
             * the circle centers.
             */
            dx = x1 - x0;
            dy = y1 - y0;

            /* Determine the straight-line distance between the centers. */
            d = Math.Sqrt((dy * dy) + (dx * dx));
            //d = hypot(dx,dy); // Suggested by Keith Briggs

            /* Check for solvability. */
            if (d > (r0 + r1))
            {
                /* no solution. circles do not intersect. */
                return 0;
            }
            if (d < Math.Abs(r0 - r1))
            {
                /* no solution. one circle is contained in the other */
                return 0;
            }
            //两个圆的圆心在一起也不行！
            if (d == 0)// && c1r == c2r)
                return -1;

            /* 'point 2' is the point where the line through the circle
             * intersection points crosses the line between the circle
             * centers.  
             */

            /* Determine the distance from point 0 to point 2. */
            a = ((r0 * r0) - (r1 * r1) + (d * d)) / (2.0 * d);

            /* Determine the coordinates of point 2. */
            x2 = x0 + (dx * a / d);
            y2 = y0 + (dy * a / d);

            /* Determine the distance from point 2 to either of the
             * intersection points.
             */
            h = Math.Sqrt((r0 * r0) - (a * a));

            /* Now determine the offsets of the intersection points from
             * point 2.
             */
            rx = -dy * (h / d);
            ry = dx * (h / d);

            /* Determine the absolute intersection points. */
            xi = x2 + rx;
            xi_prime = x2 - rx;
            yi = y2 + ry;
            yi_prime = y2 - ry;

            return 1;
        }
        /* http://www.lee-mac.com/bulgeconversion.html
         * bulge凸度的定义是夹角的1/4的tan值，这个1/4真他妈的牛逼，正好就把凸度限制到了第一象限，
         * 所有的计算不用他妈的考虑正负号了
         * bluge = b = tag(theta/4)
         * theta = 4 * arctan(b)
         * d = r*sin(theta/2)
         * r = d/sin(theta/2)
         */
        public static PointF GetCenterofBulge(PointF start, PointF end, double bulge)
        {
            //PointF center = new PointF();
            //double theta = 4 * Math.Atan(bulge);
            //double d = dist(start, end) / 2;
            //double r = d / Math.Sin(theta/2);
            //double x0 = 0;
            //double y0 = 0;
            //double x1 = 0;
            //double y1 = 0;
            //int result = CGeometry.circle_circle_intersection(start,r,end,r,ref x0,ref y0,ref x1,ref y1);
            //if (result == 1)
            //{
            //    PointF center0 = new PointF((float)x0, (float)y0);
            //    PointF center1 = new PointF((float)x1, (float)y1);

            //    double angle1 = CGeometry.SlopeAngle(center0, start);
            //    double angle2 = CGeometry.SlopeAngle(center0, end);
            //    double thetaDelta0 = (angle2 - angle1);

            //    if (thetaDelta0 > 0)
            //        return center0;
            //    else
            //        return center1;
            //    //angle1 = CGeometry.SlopeAngle(center1, start);
            //    //angle2 = CGeometry.SlopeAngle(center1, end);
            //    //double thetaDelta1 = (angle2 - angle1);

            //}
            //else
            //{
            //    Console.WriteLine("this is not a correct line with bulge!");
            //}
            //return center;
            //新浪伙计的代码！！！！！
            //http://blog.sina.com.cn/s/blog_66349acf0102vivw.html
            //            void calc_center_by_bulge(const PointF& begin, const PointF& end, double bulge, PointF& center)
            //{
            //             double
            //             x1 = begin.x, x2 = end.x,
            //             y1 = begin.y, y2 = end.y;
            //             bulge = 0.5*(1/bulge-bulge);
            //             center.x = 0.5*((x1+x2)-(y2-y1)*bulge);
            //             center.y = 0.5*((y1+y2)+(x2-x1)*bulge);
            //}
            double
            x1 = start.X, x2 = end.X,
            y1 = start.Y, y2 = end.Y;

            PointF center = new PointF();
            bulge = 0.5 * (1 / bulge - bulge);
            center.X = (float)(0.5f * ((x1 + x2) - (y2 - y1) * bulge));
            center.Y = (float)(0.5f * ((y1 + y2) + (x2 - x1) * bulge));

            return center;

        }
        //圆和直线关系： 
        //0----相离 1----相切 2----相交 
        public static int clpoint(PointF p, double r, double a, double b, double c, ref PointF rp1, ref PointF rp2)
        {
            int res = 0;

            c = c + a * p.X + b * p.Y;
            double tmp;
            if (a == 0 && b != 0)
            {
                tmp = (-c / b);
                if (r * r < tmp * tmp)
                    res = 0;
                else if (r * r == tmp * tmp)
                {
                    res = 1;
                    rp1.Y = (float)tmp;
                    rp1.X = 0;
                }
                else
                {
                    res = 2;
                    rp1.Y = rp2.Y = (float)tmp;
                    rp1.X = (float)Math.Sqrt(r * r - tmp * tmp);
                    rp2.X = -rp1.X;
                }
            }
            else if (a != 0 && b == 0)
            {
                tmp = (-c / a);
                if (r * r < tmp * tmp)
                    res = 0;
                else if (r * r == tmp * tmp)
                {
                    res = 1;
                    rp1.X = (float)tmp;
                    rp1.Y = 0;
                }
                else
                {
                    res = 2;
                    rp1.X = rp2.X = (float)tmp;
                    rp1.Y = (float)Math.Sqrt(r * r - tmp * tmp);
                    rp2.Y = -rp1.Y;
                }
            }
            else if (a != 0 && b != 0)
            {
                double delta;
                delta = b * b * c * c - (a * a + b * b) * (c * c - a * a * r * r);
                if (delta < 0)
                    res = 0;
                else if (delta == 0)
                {
                    res = 1;
                    rp1.Y = (float)(-b * c / (a * a + b * b));
                    rp1.X = (float)((-c - b * rp1.Y) / a);
                }
                else
                {
                    res = 2;
                    rp1.Y = (float) ( (-b * c + Math.Sqrt(delta)) / (a * a + b * b));
                    rp2.Y = (float) ( (-b * c - Math.Sqrt(delta)) / (a * a + b * b));
                    rp1.X = (float) ((-c - b * rp1.Y) / a);
                    rp2.X = (float) ((-c - b * rp2.Y) / a);
                }
            }
            rp1.X += p.X;
            rp1.Y += p.Y;
            rp2.X += p.X;
            rp2.Y += p.Y;
            return res;
        }
    }
}
