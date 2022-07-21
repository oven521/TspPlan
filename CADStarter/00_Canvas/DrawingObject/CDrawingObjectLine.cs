using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MainHMI;

namespace CADEngine.DrawingObject {
    public class CDrawingObjectSingleLine : CDrawingObjectBase {
        private PointF _s;
        private PointF _e;
        bool _bG00 = false;

        double _bulge = 0;
        public double _bulge_r = 0;
        public double _bulge_startangle = 0;
        public double _bulge_endangle = 0;
        public PointF _bulge_center;
        CDrawingObjectArc _ArcObj;
        GLineSeg _ArrowSeg1;
        GLineSeg _ArrowSeg2;

        CSnapPointBase _snapSart;
        CSnapPointBase _snapEnd;
        GLineSeg _thisSeg;

        bool _isInPolyLine;



        public CDrawingObjectSingleLine(PointF start, PointF end, double bulge, CanvasCtrl canvas)
            : base(null, canvas) {
            StartPoint = start;
            EndPoint = end;
            _bG00 = false;

            _snapSart = new CSnapPointBase(StartPoint, 10, 10, canvas);
            _snapEnd = new CSnapPointBase(EndPoint, 10, 10, canvas);
            _thisSeg = CGeometry.MakeALineSeg(StartPoint, EndPoint);
            _bulge = bulge;
            if (_bulge != 0) {
                _bulge_center = CGeometryCircle.GetCenterofBulge(StartPoint, EndPoint, _bulge);
                _bulge_r = CGeometry.dist(StartPoint, _bulge_center);
                _bulge_startangle = CGeometry.SlopeAngleHudu(_bulge_center, StartPoint);
                _bulge_endangle = CGeometry.SlopeAngleHudu(_bulge_center, EndPoint);
                _ArcObj = new CDrawingObjectArc(_bulge_center, (float)_bulge_r, (float)_bulge_startangle, (float)_bulge_endangle, this.m_Canvas, false);

                if (_bulge < 0) {//this Arc is CW
                    _ArcObj.Reverse();
                }
            }
        }
        public override PointF StartPoint {
            get { return _s; }
            set { _s = value; }
        }
        public override PointF EndPoint {
            get { return _e; }
            set { _e = value; }
        }
        public CDrawingObjectArc ToArcObject() {
            return _ArcObj;
        }
        public double Bulge {
            get { return _bulge; }
        }
        public bool IsG00Line {
            get { return _bG00; }
            set { _bG00 = value; }
        }

        private void CreateArrowLines() {
            GLineSeg segArrow = new GLineSeg(this.StartPoint, this.EndPoint);
            //以endPoint为圆心，旋转startPoint，逆时针旋转的角度为45
            _ArrowSeg1 = CGeometry.CalcArrowLine(segArrow, this.m_Canvas.ArrowLength, CGeometry.PI / 8);
            _ArrowSeg2 = CGeometry.CalcArrowLine(segArrow, this.m_Canvas.ArrowLength, 2 * CGeometry.PI - CGeometry.PI / 8);
        }
        public override bool CheckSnapPoints(PointF lptMousePos) {
            //if (CGeometry.dist(lptMousePos, StartPoint) < 25){
            //    _snapSart.Visible = true;
            //    _snapSart.Draw(g);
            //}
            //else{
            //    _snapSart.Visible = false;
            //}
            //if (CGeometry.dist(lptMousePos, EndPoint) < 25){
            //    _snapEnd.Visible = true;
            //}
            //else{
            //    _snapEnd.Visible = false;
            //}
            //return _snapEnd.Visible || _snapSart.Visible;

            return false;
        }
        /// <summary>
        /// 这个是相交的的函数，并没有包含的关系
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public override bool IsIntersectWith(CDrawingObjectBase drawObj) {
            CDrawingObjectRect selectRect = drawObj as CDrawingObjectRect;
            PointF ptIntersect = new PointF();
            if (selectRect != null) {
                foreach (GLineSeg seg in selectRect.GetAllLineSegs()) {
                    bool isIntersected = CGeometryLine.IsLineSegIntersect(_thisSeg, seg, ref ptIntersect);
                    if (isIntersected) {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
        /// <summary>
        /// 判断直线是否被另外一个图形包含
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public override bool IsContainedBy(CDrawingObjectBase drawObj) {
            if (drawObj != null) {
                if (drawObj.ContainsLine(this))
                    return true;
                else
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Reverse Line
        /// </summary>
        public override void Reverse() {
            CGeometry.Swap<PointF>(ref _s, ref _e);
            this._bulge = -this._bulge;
        }
        public override void Draw(Graphics g) {
            if (Selected) {
                _drawPen = GDIDrawMaterails.GreenDashPen;
            }
            else if (!IsInClosedPolyLine) {
                _drawPen = GDIDrawMaterails.UnClosedCurvePen;
            }
            else {
                _drawPen = GDIDrawMaterails.GreenPen2;
            }
            
            CreateArrowLines();
            
            PointF start = StartPoint;
            PointF end = EndPoint;
            if (_bulge == 0 && _ArcObj == null) {
                g.DrawLine(_drawPen, start, end);
                g.DrawLine(_drawPen, new PointF(_ArrowSeg1.s.X, _ArrowSeg1.s.Y), _ArrowSeg1.e);
                g.DrawLine(_drawPen, new PointF(_ArrowSeg2.s.X, _ArrowSeg2.s.Y), _ArrowSeg2.e);
            }
            else {
                _ArcObj.Selected = this.Selected;
                _ArcObj.IsInClosedPolyLine = this.IsInClosedPolyLine;
                _ArcObj.Draw(g);
                
                g.DrawLine(GDIDrawMaterails.YellowDashPen, start, this._bulge_center);
                g.DrawLine(GDIDrawMaterails.YellowDashPen, end, this._bulge_center);
            }
        }
        public override List<CDrawingObjectSingleLine> GetAllLines() {
            List<CDrawingObjectSingleLine> lines = new List<CDrawingObjectSingleLine>();
            lines.Add(this);
            return lines;
        }
        /// <summary>
        /// 图形是否闭合。
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public override bool IsClosed() {
            return false;
        }

    }
}
