using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using MainHMI;

namespace CADEngine.DrawingObject {
    public class CDrawingObjectCircleR : CDrawingObjectBase {
        float width;
        float height;
        float _radius;

        PointF _center;


        GLineSeg _ArrowSeg1;
        GLineSeg _ArrowSeg2;

        public PointF Center { get { return _center; } }
        public double Radius { get { return _radius; } }
        public CDrawingObjectCircleR(PointF[] points, CanvasCtrl canvas)
            : base(points, canvas) {

            _radius = CPublic.CalDis(points[0], points[1]);
            width = 2 * _radius;
            height = 2 * _radius;
            _center = points[0];
            this.m_Canvas = canvas;
        }
        public CDrawingObjectCircleR(PointF center, float r, CanvasCtrl canvas)
            : base(canvas) {
            _radius = r;
            width = 2 * _radius;
            height = 2 * _radius;
            _center = center;

            this.m_Canvas = canvas;
        }
        public override void Draw(Graphics g) {
            if (Selected) {
                _drawPen = GDIDrawMaterails.GreenDashPen;
            }
            else {
                _drawPen = GDIDrawMaterails.GreenPen2;
            }

            CreateArrowlines();

            g.DrawEllipse(this._drawPen, new RectangleF(_center.X - _radius, _center.Y - _radius, 2 * _radius, 2 * _radius));
            g.DrawLine(this._drawPen, new PointF(_ArrowSeg1.s.X, _ArrowSeg1.s.Y), _ArrowSeg1.e);
            g.DrawLine(this._drawPen, new PointF(_ArrowSeg2.s.X, _ArrowSeg2.s.Y), _ArrowSeg2.e);

        }
        private void CreateArrowlines() {
            PointF ptLeftMiddle = new PointF(_center.X - _radius, _center.Y);
            PointF ptLeftMiddleUp = new PointF(ptLeftMiddle.X, ptLeftMiddle.Y + 15);
            GLineSeg segArrow = new GLineSeg(ptLeftMiddleUp, ptLeftMiddle);
            //以endPoint为圆心，旋转startPoint，逆时针旋转的角度为45
            _ArrowSeg1 = CGeometry.CalcArrowLine(segArrow, this.m_Canvas.ArrowLength, CGeometry.PI / 8);
            _ArrowSeg2 = CGeometry.CalcArrowLine(segArrow, this.m_Canvas.ArrowLength, 2 * CGeometry.PI - CGeometry.PI / 8);
            // _Arrow1 = new CDrawingObjectLine()
        }
        /// <summary>
        /// 这个是相交的的函数，并没有包含的关系
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public override bool IsIntersectWith(CDrawingObjectBase drawObj) {
            CDrawingObjectRect selectRect = drawObj as CDrawingObjectRect;
            PointF foot = new PointF();
            bool footOnLine;
            if (selectRect != null) {
                foreach (GLineSeg seg in selectRect.GetAllLineSegs()) {
                    double dist = CGeometryLine.ptolinesegdist(this._center, seg, ref foot, out footOnLine);
                    //只要distance小于圆心可以认为圆和矩形相交。
                    if (dist < this._radius)
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断圆是否被另外一个图形包含,因为是激光切割，我们采用一个简单的算法，就是圆心被包含了，
        /// 就认为整个圆被包含了。
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public override bool IsContainedBy(CDrawingObjectBase drawObj) {
            CDrawingObjectRect rect = drawObj as CDrawingObjectRect;
            PointF foot = new PointF();
            bool footOnLine;
            if (rect != null) {
                //首先必须满足圆心在矩形内部
                if (!rect.ContainsPoint(this._center))
                    return false;
                //只要有一条边到矩形的距离大于半径，那么也不包含
                foreach (GLineSeg seg in rect.GetAllLineSegs()) {
                    double dist = CGeometryLine.ptolinesegdist(this._center, seg, ref foot, out footOnLine);
                    //只要有圆心与其中一条边的距离小于半径，则圆不包含在矩形里
                    if (dist < this._radius)
                        return false;
                }
                //包含
                return true;
            }
            return drawObj.ContainsPoint(this._center);


        }
        public override bool ContainsPoint(PointF point) {
            return CGeometry.dist(this._center, point) <= this._radius;
        }

        public override string ToGcode() {
            string gCode = "G00 X" + (_center.X + _radius).ToString() + "Y" + (_center.Y).ToString() + "\r\n";
            gCode += "G02 I" + this._radius + "\r\n";
            return gCode;
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
