using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using MainHMI;


namespace CADEngine.DrawingObject {
    public class CDrawingObjectBase {
        public static int ObjNumber;
        private int _ID;
        protected PointF[] m_Points;
        protected CanvasCtrl m_Canvas;
        protected Pen _drawPen;
        protected bool _Visible;
        bool _isInClosedPolyLine;

        //
        bool m_IsSelected = false;

        //生成图时需要的变量
        protected List<CDrawingObjectBase> m_Neighbors = new List<CDrawingObjectBase>();

        protected List<PointF> _PolyPoints = new List<PointF>();

        public virtual bool Visible {
            set {
                _Visible = value;
            }
            get { return _Visible; }
        }
        public List<CDrawingObjectBase> Neighbors {
            get { return m_Neighbors; }
        }
        public bool Selected {
            get { return m_IsSelected; }
            set { m_IsSelected = value; }
        }

        public int ID {
            get { return _ID; }
        }

        public CanvasCtrl ObjCanvas {
            get { return m_Canvas; }
        }
        public CDrawingObjectBase(PointF[] points, CanvasCtrl canvas) {
            m_Canvas = canvas;
            m_Points = new PointF[5];

            ObjNumber++;
            this._ID = ObjNumber;
        }
        public CDrawingObjectBase(CanvasCtrl canvas) {
            m_Canvas = canvas;

            ObjNumber++;
            this._ID = ObjNumber;

        }
        public virtual void Draw(Graphics g) {

        }
        public virtual string ToGcode() {
            string gCode = "";

            return gCode;
        }
        /// <summary>
        /// 判断当前图形是否与别相交
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool IsIntersectWith(CDrawingObjectBase obj) {
            return false;
        }
        /// <summary>
        /// 判断图形是否被别人包含
        /// </summary>
        /// <param name="baseObj"></param>
        /// <returns></returns>
        public virtual bool IsContainedBy(CDrawingObjectBase baseObj) {
            return false;
        }
        /// <summary>
        /// 图形是否闭合。
        /// </summary>
        /// <param name="drawObj"></param>
        /// <returns></returns>
        public virtual bool IsClosed() {
            return false;
        }
        public virtual bool ContainsPoint(PointF point) {
            return false;
        }
        public virtual bool ContainsLine(CDrawingObjectSingleLine line) {
            return false;
        }
        protected virtual void DrawArrowLines() {
        }
        public virtual bool CheckSnapPoints(PointF pt) {

            return false;
        }
        public virtual List<GLineSeg> GetAllLineSegs() {
            return null;
        }
        public virtual List<CDrawingObjectSingleLine> GetAllObjectLines() {
            return null;
        }
        public virtual void Reverse() {

        }
        public virtual PointF StartPoint {
            get;
            set;
        }
        public virtual PointF EndPoint {
            get;
            set;
        }
        public virtual List<CDrawingObjectSingleLine> GetAllLines() {
            return null;
        }
        public bool IsInClosedPolyLine {
            get { return _isInClosedPolyLine; }
            set { _isInClosedPolyLine = value; }
        }



    }
}
