using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using MainHMI;
namespace CADEngine.DrawingObject {
    /// <summary>
    /// Arc只做绘图用，所有的Arc必须转化成带凸度的直线进行计算
    /// </summary>
    public class CDrawingObjectArc : CDrawingObjectBase {

        PointF m_Center = new PointF();
        float m_r = 0;
        float m_startAngle = 0;
        float m_endAngle = 0;
        //bool m_CW = false;
        double _bulge;
        CDrawingObjectSingleLine _bulge_line;

        public CDrawingObjectArc(PointF center, float r, float startAngle, float endAngle, CanvasCtrl canvas, bool IsDegree)
            : base(canvas) {
            m_Center = center;
            m_r = r;

            if (IsDegree) {
                m_startAngle = (float)(startAngle * Math.PI / 180.0);
                m_endAngle = (float)(endAngle * Math.PI / 180.0);
            }
            else {
                m_startAngle = startAngle;
                m_endAngle = endAngle;
            }
        }
        public CDrawingObjectSingleLine ToLineWithBulge() {
            //弧的凸度永远为正，并且只有由4象限跨入1象限才会start大于end angle
            double theta = (m_endAngle - m_startAngle);
            if (theta < 0)
                theta += 2 * Math.PI;
            _bulge = Math.Tan(theta / 4);

            PointF startPt = CGeometry.PointOnCircle(this.m_Center, this.m_r, this.m_startAngle);
            PointF endPt = CGeometry.PointOnCircle(this.m_Center, this.m_r, this.m_endAngle);
            //注意下面这行代码不能放到构造函数里，否则会与CDrawingObjectline构造函数里形成死循环调用
            _bulge_line = new CDrawingObjectSingleLine(startPt, endPt, _bulge, this.m_Canvas);

            return _bulge_line;
        }
        public double Radius {
            get { return m_r; }

        }
        public PointF Center {
            get { return this.m_Center; }
        }

        public double StartAngle {
            get { return m_startAngle; }
        }
        public double EndAngle {
            get { return m_endAngle; }
        }
        public override void Reverse() {
            CGeometry.Swap<float>(ref m_startAngle, ref m_endAngle);
        }
        public override void Draw(Graphics g) {
            RectangleF rect = new RectangleF(new PointF(m_Center.X - m_r, m_Center.Y - m_r), new SizeF(m_r * 2, m_r * 2));

            Single startangleDegre = (float)(m_startAngle * 180 / Math.PI);
            Single endangleDegree = (float)(m_endAngle * 180 / Math.PI);

            if (Selected) {
                _drawPen = GDIDrawMaterails.GreenDashPen;
            }
            else if (!IsInClosedPolyLine) {
                _drawPen = GDIDrawMaterails.UnClosedCurvePen;
            }
            else {
                _drawPen = GDIDrawMaterails.GreenPen2;
            }

            //AutoCAD 本身的弧都是逆时针的，但是由带凸度的直线生成的弧是有方向的，这个一点一定要区分。
            //只有在绘图的时候，CW才有意义。
            g.DrawArc(_drawPen, rect, startangleDegre, (endangleDegree - startangleDegre + 360) % 360);

        }
        /// <summary>
        /// 图形是否闭合。
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public override bool IsClosed() {
            return false;
        }
        public bool IsPointOnArc(PointF point) {
            if (Math.Abs(CGeometry.dist(point, this.Center) - this.Radius) > 0.01)
                return false;

            double angle = CGeometry.SlopeAngleHudu(this.Center, point);
            if (
                (angle >= this.StartAngle && angle <= this.EndAngle)
                || (angle <= this.StartAngle && angle >= this.EndAngle)
               )
                return true;
            else
                return false;
        }
    }
}
