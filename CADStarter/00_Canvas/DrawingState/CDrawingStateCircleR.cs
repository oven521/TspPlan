using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CADEngine.DrawingObject;
using MainHMI;


namespace CADEngine.DrawingState {
    public class CDrawingStateCircleR : CDrawingStateBase, IDrawingStateBase {
        public CDrawingStateCircleR(CanvasCtrl frm)
            : base(frm) {

        }
        public void Init() {
            step = 0;
        }
        public void KeyDown(Keys keyData) {

        }

        public void MouseDown(object sender, MouseEventArgs e) {

        }
        public void MouseUp(object sender, MouseEventArgs e) {
            PointF lgPt = this.m_Canvas.SPt2LPt(new PointF(e.X, e.Y));
            switch (step) {
                case 0: {
                        ObjCanvas.m_Points[0].X = lgPt.X;
                        ObjCanvas.m_Points[0].Y = lgPt.Y;
                        ObjCanvas.LastPoint = ObjCanvas.m_Points[0];
                        ObjCanvas.Cursor = Cursors.Cross;
                        step = 1;
                        break;
                    }
                case 1: {
                        ObjCanvas.m_Points[1].X = lgPt.X;
                        ObjCanvas.m_Points[1].Y = lgPt.Y;

                        PointF logicPoint0 = ObjCanvas.m_Points[0];//new PointF(scaledX, scaledY);
                        PointF logicPoint1 = ObjCanvas.m_Points[1];//new PointF(scaledX, scaledY);
                        float r = (float)CGeometry.dist(logicPoint0, logicPoint1);
                        CDrawingObjectCircleR objCircleR = new CDrawingObjectCircleR(logicPoint0, r, ObjCanvas);

                        this.ObjCanvas.DrawModel.AddDrawingObject(objCircleR);
                        ObjCanvas.DrawAndBackupBufferGraphic();
                        this.ObjCanvas.Refresh();

                        ObjCanvas.Cursor = Cursors.Arrow;
                        step = 0;
                        break;
                    }
            }
        }

        public void MouseMove(object sender, MouseEventArgs e) {
            PointF lgPt = this.m_Canvas.SPt2LPt(new PointF(e.X, e.Y));
            ObjCanvas.Cursor = Cursors.Cross;

            ObjCanvas.CurrentPoint = new PointF(lgPt.X, lgPt.Y);
            if (step == 1) {
                ObjCanvas.RestoreBufferGraphic();
                int r = (int)CPublic.CalDis(ObjCanvas.m_Points[0], ObjCanvas.CurrentPoint);

                ObjCanvas.BufferGrahicHandle.DrawEllipse(new Pen(Color.Green), new Rectangle((int)(ObjCanvas.m_Points[0].X - r), (int)(ObjCanvas.m_Points[0].Y - r), 2 * r, 2 * r));
                ObjCanvas.BufferGrahicHandle.DrawLine(new Pen(Color.Green), ObjCanvas.m_Points[0], ObjCanvas.CurrentPoint);

                ObjCanvas.Invalidate();
            }

        }
    }
}
