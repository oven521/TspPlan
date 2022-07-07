using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CADEngine.DrawingObject;
using System.Drawing;
using System.Windows.Forms;
using MainHMI;

namespace CADEngine.DrawingState {
    public class CDrawingStateCircle3P : CDrawingStateBase, IDrawingStateBase {
        PointF center;
        float radius = 0;
        public CDrawingStateCircle3P(CanvasCtrl frm)
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
                        ObjCanvas.CurrentPoint = ObjCanvas.m_Points[0];
                        ObjCanvas.LastPoint = ObjCanvas.m_Points[0];
                        ObjCanvas.Cursor = Cursors.Cross;
                        step = 1;
                        break;
                    }
                case 1: {
                        ObjCanvas.m_Points[1].X = lgPt.X;
                        ObjCanvas.m_Points[1].Y = lgPt.Y;
                        ObjCanvas.Cursor = Cursors.Cross;
                        step = 2;
                        break;
                    }
                case 2: {
                        ObjCanvas.m_Points[2].X = lgPt.X;
                        ObjCanvas.m_Points[2].Y = lgPt.Y;

                        if (CPublic.GetCenterRadius3p(ObjCanvas.m_Points[0], ObjCanvas.m_Points[1], ObjCanvas.m_Points[2], ref center, ref radius)) {
                            CDrawingObjectCircle3p objCircle3p = new CDrawingObjectCircle3p(ObjCanvas.m_Points, ObjCanvas);
                            this.ObjCanvas.DrawModel.AddDrawingObject(objCircle3p);
                            ObjCanvas.DrawAndBackupBufferGraphic();
                            this.ObjCanvas.Refresh();
                        }
                        else {
                            MessageBox.Show("这三点无法画圆！");
                        }
                        ObjCanvas.Cursor = Cursors.Arrow;
                        step = 0;
                        break;
                    }
            }
        }
        public void MouseMove(object sender, MouseEventArgs e) {
            PointF lgPt = this.m_Canvas.SPt2LPt(new PointF(e.X, e.Y));
            ObjCanvas.CurrentPoint = lgPt;
            switch (step) {
                case 1: {
                        ObjCanvas.LastPoint = ObjCanvas.CurrentPoint;
                        break;
                    }
                case 2: {
                        break;
                    }
            }
        }
    }
}
