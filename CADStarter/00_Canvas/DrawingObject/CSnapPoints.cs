using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MainHMI;

namespace CADEngine.DrawingObject {
    class CSnapPointBase : CDrawingObjectBase {

        RectangleF _rectBoundery;
        private static PointF _currentSnap;
        public static bool _isSnapOn;
        PointF _snapCenter;


        public CSnapPointBase(PointF center, float width, float height, CanvasCtrl canvas)
            : base(null, canvas) {
            _rectBoundery = CGeometry.MakeRect(center, width, height);
            _snapCenter = center;

        }
        public override void Draw(Graphics g) {
            // //_polyLines.Draw(g);
            //if (this.Visible)
            // g.FillRectangle(Brushes.Blue, _rectBoundery);
        }

        public static PointF CurrentSnap {
            set { _currentSnap = value; }
            get { return _currentSnap; }
        }
        public static bool IsSnapOn {
            set { _isSnapOn = value; }
            get { return _isSnapOn; }
        }
        public override bool Visible {
            set {
                if (value) {
                    CSnapPointBase.IsSnapOn = true;
                    CSnapPointBase.CurrentSnap = this._snapCenter;
                }
                if (value != _Visible) {
                    _Visible = value;
                    m_Canvas.Invalidate();
                }
            }
            get { return _Visible; }
        }


    }
    //class GridSnapPoint : CSnapPointBase
    //{
    //    public GridSnapPoint(ICanvas canvas, UnitPoint snappoint)
    //        : base(canvas, null, snappoint)
    //    {
    //    }
    //    #region ISnapPoint Members
    //    public override void Draw(ICanvas canvas)
    //    {
    //        DrawPoint(canvas, Pens.Gray, null);
    //    }
    //    #endregion
    //}
    //class VertextSnapPoint : CSnapPointBase
    //{
    //    public VertextSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
    //        : base(canvas, owner, snappoint)
    //    {
    //    }
    //    public override void Draw(ICanvas canvas)
    //    {
    //        DrawPoint(canvas, Pens.Blue, Brushes.YellowGreen);
    //    }
    //}
    //class MidpointSnapPoint : CSnapPointBase
    //{
    //    public MidpointSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
    //        : base(canvas, owner, snappoint)
    //    {
    //    }
    //    public override void Draw(ICanvas canvas)
    //    {
    //        DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
    //    }
    //}
    //class IntersectSnapPoint : CSnapPointBase
    //{
    //    public IntersectSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
    //        : base(canvas, owner, snappoint)
    //    {
    //    }
    //    public override void Draw(ICanvas canvas)
    //    {
    //        DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
    //    }
    //}
    //class NearestSnapPoint : CSnapPointBase
    //{
    //    public NearestSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
    //        : base(canvas, owner, snappoint)
    //    {
    //    }
    //    #region ISnapPoint Members
    //    public override void Draw(ICanvas canvas)
    //    {
    //        DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
    //    }
    //    #endregion
    //}
    //class QuadrantSnapPoint : CSnapPointBase
    //{
    //    public QuadrantSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
    //        : base(canvas, owner, snappoint)
    //    {
    //    }
    //    public override void Draw(ICanvas canvas)
    //    {
    //        DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
    //    }
    //}
    //class DivisionSnapPoint : CSnapPointBase
    //{
    //    public DivisionSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
    //        : base(canvas, owner, snappoint)
    //    {
    //    }
    //    public override void Draw(ICanvas canvas)
    //    {
    //        DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
    //    }
    //}
    //class CenterSnapPoint : CSnapPointBase
    //{
    //    public CenterSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
    //        : base(canvas, owner, snappoint)
    //    {
    //    }
    //    public override void Draw(ICanvas canvas)
    //    {
    //        DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
    //    }
    //}
    //class PerpendicularSnapPoint : CSnapPointBase
    //{
    //    public PerpendicularSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
    //        : base(canvas, owner, snappoint)
    //    {
    //    }
    //    public override void Draw(ICanvas canvas)
    //    {
    //        DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
    //    }
    //}
    //class TangentSnapPoint : CSnapPointBase
    //{
    //    public TangentSnapPoint(ICanvas canvas, IDrawObject owner, UnitPoint snappoint)
    //        : base(canvas, owner, snappoint)
    //    {
    //    }
    //    public override void Draw(ICanvas canvas)
    //    {
    //        DrawPoint(canvas, Pens.White, Brushes.YellowGreen);
    //    }
    //}
}
