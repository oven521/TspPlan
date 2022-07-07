using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MainHMI;

namespace CADEngine.DrawingObject {
    public class CDrawingObjectRect : CDrawingObjectBase {


        float _width;
        float _height;
        PointF _leftdown;

        PointF _leftup;
        PointF _rightup;
        PointF _rightbottom;
        PointF _leftbottom;

        CDrawingObjectLWPolyLine _innerLWPolyLine;
        List<GLineSeg> _innerLineSegs;

        public CDrawingObjectRect(PointF leftdown, float width, float height, CanvasCtrl canvas)
            : base(null, canvas) {
            this._width = width;
            this._height = height;

            this._leftdown = leftdown;
            _innerLWPolyLine = new CDrawingObjectLWPolyLine(canvas);
            _innerLineSegs = new List<GLineSeg>();

            _leftup = new PointF(_leftdown.X, _leftdown.Y + _height);
            _rightup = new PointF(_leftdown.X + _width, _leftdown.Y + _height);
            _rightbottom = new PointF(_leftdown.X + _width, _leftdown.Y);
            _leftbottom = _leftdown;

            _PolyPoints.Add(_leftup);
            _PolyPoints.Add(_rightup);
            _PolyPoints.Add(_rightbottom);
            _PolyPoints.Add(_leftbottom);


            _innerLWPolyLine.AddLine(new CDrawingObjectSingleLine(_leftup, _rightup, 0, canvas));
            _innerLWPolyLine.AddLine(new CDrawingObjectSingleLine(_rightup, _rightbottom, 0, canvas));
            _innerLWPolyLine.AddLine(new CDrawingObjectSingleLine(_rightbottom, _leftbottom, 0, canvas));
            _innerLWPolyLine.AddLine(new CDrawingObjectSingleLine(_leftbottom, _leftup, 0, canvas));

            _innerLineSegs.Add(new GLineSeg(_leftup, _rightup));
            _innerLineSegs.Add(new GLineSeg(_rightup, _rightbottom));
            _innerLineSegs.Add(new GLineSeg(_rightbottom, _leftbottom));
            _innerLineSegs.Add(new GLineSeg(_leftbottom, _leftup));

        }
        public override void Draw(Graphics g) {
            _innerLWPolyLine.Draw(g);

        }
        public override List<CDrawingObjectSingleLine> GetAllLines() {
            return _innerLWPolyLine.GetAllLines();
        }
        public override List<GLineSeg> GetAllLineSegs() {
            return _innerLineSegs;
        }
        /// <summary>
        /// 这个是相交的的函数，并没有包含的关系
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public override bool IsIntersectWith(CDrawingObjectBase drawObj) {
            return _innerLWPolyLine.IsIntersectWith(drawObj);
        }
        /// <summary>
        /// 多线段是否被另外一个图形包含。（注意这个代码有问题，没有考虑到弧的问题）
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public override bool IsContainedBy(CDrawingObjectBase drawObj) {
            return _innerLWPolyLine.IsContainedBy(drawObj);
        }
        public override bool ContainsPoint(PointF point) {
            List<PointF> PolygonLines = _PolyPoints;
            int verticesCount = PolygonLines.Count;
            int nCross = 0;
            for (int i = 0; i < verticesCount; ++i) {
                PointF p1 = PolygonLines[i];
                PointF p2 = PolygonLines[(i + 1) % verticesCount];

                // 求解 y=p.y 与 p1 p2 的交点 （平行的条件已经在下面的==里包含了）
                //if (p1.Y == p2.Y)
                //{   // p1p2 与 y=p0.y平行
                //    continue;
                //}
                if (point.Y < Math.Min(p1.Y, p2.Y)) { // 交点在p1p2延长线上 
                    continue;
                }
                //如果正好在顶点上，那么顶点下面的图形不算，这里的等号就是区分了这个特殊条件
                if (point.Y >= Math.Max(p1.Y, p2.Y)) { // 交点在p1p2延长线上 
                    continue;
                }
                //判断点是不是在边上，如果是在边上，那么也算包含
                if (0 == CGeometry.multiply(p1, p2, point))
                    return true;

                // 求交点的 X 坐标
                float x = (point.Y - p1.Y) * (p2.X - p1.X)
                            / (p2.Y - p1.Y) + p1.X;
                //这里没有考虑点正好在线上的问题。
                if (x > point.X) { // 只统计单边交点
                    nCross++;
                }
            }
            // 单边交点为偶数，点在多边形之外
            return (nCross % 2 == 1);
        }

        public override bool ContainsLine(CDrawingObjectSingleLine line) {
            return ContainsPoint(line.StartPoint) && ContainsPoint(line.EndPoint);
        }
        /// <summary>
        /// 图形是否闭合。
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public override bool IsClosed() {
            return true;
        }
        /// <summary>
        /// Reverse Line
        /// </summary>
        public override void Reverse() {

            _innerLWPolyLine._SinglelineList.Reverse();
            foreach (CDrawingObjectSingleLine line in _innerLWPolyLine._SinglelineList) {
                line.Reverse();
            }

        }


    }
}
