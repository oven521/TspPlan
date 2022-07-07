using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using MainHMI;

namespace CADEngine.DrawingObject {
    public class CDrawingObjectLWPolyLine : CDrawingObjectBase {
        public List<CDrawingObjectSingleLine> _SinglelineList = new List<CDrawingObjectSingleLine>();
        public CDrawingObjectLWPolyLine(CanvasCtrl canvas)
            : base(canvas) {
        }

        public void AddLine(CDrawingObjectSingleLine line) {
            line.IsInClosedPolyLine = true;

            _SinglelineList.Add(line);
        }
        public void ClearLines() {
            _SinglelineList.Clear();
        }
        public List<CDrawingObjectSingleLine> Lines {
            get { return _SinglelineList; }
        }
        public override PointF StartPoint {
            get {
                return _SinglelineList[0].StartPoint;
            }

        }
        public override PointF EndPoint {
            get {
                return _SinglelineList[_SinglelineList.Count - 1].EndPoint;
            }
        }
        public override List<CDrawingObjectSingleLine> GetAllObjectLines() {
            return _SinglelineList;
        }
        /// <summary>
        /// 多线段是否闭合。
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public override bool IsClosed() {
            if (_SinglelineList.Count >= 2) {
                if (CGeometry.dist(_SinglelineList[0].StartPoint, _SinglelineList[_SinglelineList.Count - 1].EndPoint) <= 0.1)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 这个是相交的的函数，并没有包含的关系
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public override bool IsIntersectWith(CDrawingObjectBase drawObj) {
            CDrawingObjectRect rect = drawObj as CDrawingObjectRect;

            foreach (CDrawingObjectSingleLine objLine in this._SinglelineList) {
                if (objLine.IsIntersectWith(rect)) {
                    this.Selected = true;
                    return true;
                }
            }
            this.Selected = false;
            return false;
        }
        /// <summary>
        /// 多线段是否被另外一个图形包含。（注意这个代码有问题，没有考虑到弧的问题）
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public override bool IsContainedBy(CDrawingObjectBase drawObj) {
            CDrawingObjectRect rect = drawObj as CDrawingObjectRect;

            if (rect != null) {
                foreach (CDrawingObjectSingleLine objLine in this._SinglelineList) {
                    if (!objLine.IsContainedBy(rect)) {
                        this.Selected = false;
                        return false;
                    }
                }
            }
            this.Selected = true;
            return true;
        }
        /// <summary>
        /// Reverse  poly Line
        /// </summary>
        public override void Reverse() {
            _SinglelineList.Reverse();
            foreach (CDrawingObjectSingleLine line in _SinglelineList) {
                line.Reverse();
            }
        }

        public override List<CDrawingObjectSingleLine> GetAllLines() {
            return _SinglelineList;
        }

        public override void Draw(Graphics g) {
            foreach (CDrawingObjectSingleLine line in _SinglelineList) {
                line.Selected = this.Selected;
                line.IsInClosedPolyLine = this.IsClosed();
                line.Draw(g);
            }
        }

    }
}
