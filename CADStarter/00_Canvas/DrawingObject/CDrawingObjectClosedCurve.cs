using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MainHMI;


namespace CADEngine.DrawingObject {
    public class CDrawingObjectClosedCurve : CDrawingObjectBase {
        PointF m_Start;
        PointF m_End;
        List<CDrawingObjectBase> _memberCurves = new List<CDrawingObjectBase>();

        public CDrawingObjectClosedCurve(PointF start, PointF end, CanvasCtrl canvas)
            : base(null, canvas) {
            m_Start = start;
            m_End = end;

        }
        public void AddMemberCurves(CDrawingObjectBase curve) {
            _memberCurves.Add(curve);
        }

        public override void Draw(Graphics g) {
            PointF start = m_Start;
            PointF end = m_End;


        }

    }
}
