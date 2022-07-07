using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CADEngine.DrawingObject;
using MainHMI;

namespace CADEngine.DrawingState {
    public class CDrawingStateRect : CDrawingStateBase, IDrawingStateBase {
        public CDrawingStateRect(CanvasCtrl canvas)
            : base(canvas) {

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

                        int width = (int)Math.Abs(ObjCanvas.m_Points[1].X - ObjCanvas.m_Points[0].X);
                        int height = (int)Math.Abs(ObjCanvas.m_Points[1].Y - ObjCanvas.m_Points[0].Y);

                        PointF screenCenter = ObjCanvas.ScreenCenter;

                        float scaledP1X = ObjCanvas.m_Points[0].X;
                        float scaledP2X = ObjCanvas.m_Points[1].X;

                        float scaledP1Y = ObjCanvas.m_Points[0].Y;
                        float scaledP2Y = ObjCanvas.m_Points[1].Y;

                        float scaledleftDownX = scaledP1X < scaledP2X ? scaledP1X : scaledP2X;
                        float scaledleftDownY = scaledP1Y < scaledP2Y ? scaledP1Y : scaledP2Y;

                        PointF logicPointLeftDown = new PointF(scaledleftDownX, scaledleftDownY);


                        CDrawingObjectRect objRect = new CDrawingObjectRect(logicPointLeftDown, (float)(width), (float)(height), ObjCanvas);
                        this.DrawModel.AddDrawingObject(objRect);
                        ObjCanvas.DrawAndBackupBufferGraphic();
                        ObjCanvas.Refresh();

                        ObjCanvas.Cursor = Cursors.Arrow;
                        step = 0;

                        break;
                    }
            }
        }
        public void MouseMove(object sender, MouseEventArgs e) {
            PointF lgPt = this.m_Canvas.SPt2LPt(new PointF(e.X, e.Y));
            ObjCanvas.CurrentPoint = new PointF(lgPt.X, lgPt.Y);
            ObjCanvas.Cursor = Cursors.Cross;
            if (step == 1) {
                ObjCanvas.RestoreBufferGraphic();

                float width = (int)Math.Abs(ObjCanvas.CurrentPoint.X - ObjCanvas.m_Points[0].X);
                float height = (int)Math.Abs(ObjCanvas.CurrentPoint.Y - ObjCanvas.m_Points[0].Y);

                float downleftX = ObjCanvas.CurrentPoint.X < ObjCanvas.m_Points[0].X ? ObjCanvas.CurrentPoint.X : ObjCanvas.m_Points[0].X;
                float downleftY = ObjCanvas.CurrentPoint.Y < ObjCanvas.m_Points[0].Y ? ObjCanvas.CurrentPoint.Y : ObjCanvas.m_Points[0].Y;

                ObjCanvas.BufferGrahicHandle.DrawRectangle(new Pen(Color.Green), downleftX, downleftY, width, height);
                ObjCanvas.Invalidate();

                ObjCanvas.LastPoint = ObjCanvas.CurrentPoint;

            }

        }

    }
}
