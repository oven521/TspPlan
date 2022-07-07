using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using MainHMI;

namespace CADEngine.DrawingObject {
    public class CDrawingObjectCircle3p : CDrawingObjectBase {
        public CDrawingObjectCircle3p(PointF[] points, CanvasCtrl canvas)
            : base(points, canvas) {
            m_Points = points;

        }
        public override void Draw(Graphics g) {

            PointF center = new PointF();
            float radius = 0;

            if (CPublic.GetCenterRadius3p(m_Points[0], m_Points[1], m_Points[2], ref center, ref radius)) {

                double showValue = radius;

                g.DrawLine(Pens.Green, m_Points[0], m_Points[1]);
                g.DrawLine(Pens.Green, m_Points[1], m_Points[2]);
                g.DrawLine(Pens.Green, m_Points[0], m_Points[2]);

                using (Pen pen = new Pen(Color.Green, 1.5F / (float)m_Canvas.ZoomScale)) {
                    g.DrawEllipse(pen, new RectangleF(center.X - radius, center.Y - radius, 2 * radius, 2 * radius));
                }
            }
            else {
                MessageBox.Show("这三点无法画圆！");
            }
        }

        /// <summary>
        /// 图形是否闭合。
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public override bool IsClosed() {
            return true;
        }


    }
}
