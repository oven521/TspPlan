using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CADEngine.DrawingObject;
using MainHMI;
namespace CADEngine.DrawingState {

    public class CDrawingStateSelect : CDrawingStateBase, IDrawingStateBase {

        private bool _top2Bottom;
        public CDrawingStateSelect(CanvasCtrl canvas)
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

                        PointF lPt1 = ObjCanvas.m_Points[0];
                        PointF lPt2 = ObjCanvas.m_Points[1];
                        float scaledleftDownX = lPt1.X < lPt2.X ? lPt1.X : lPt2.X;
                        float scaledleftDownY = lPt1.Y < lPt2.Y ? lPt1.Y : lPt2.Y;

                        PointF logicPointLeftDown = new PointF(scaledleftDownX, scaledleftDownY);

                        int width = (int)Math.Abs(lPt1.X - lPt2.X);
                        int height = (int)Math.Abs(lPt1.Y - lPt2.Y);

                        if (lPt2.Y < lPt1.Y)
                            _top2Bottom = true;
                        else
                            _top2Bottom = false;

                        CDrawingObjectRect rect = new CDrawingObjectRect(logicPointLeftDown, width, height, this.m_Canvas);
                        foreach (CDrawingObjectBase obj in this.DrawModel.DrawingObjectList) {
                            obj.Selected = false;

                            if (_top2Bottom) {
                                if (obj.IsContainedBy(rect)) {
                                    obj.Selected = true;
                                }
                            }
                            else {
                                if (obj.IsIntersectWith(rect) || obj.IsContainedBy(rect)) {
                                    obj.Selected = true;
                                }
                            }
                        }
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

                ObjCanvas.BufferGrahicHandle.DrawRectangle(new Pen(Color.Yellow), downleftX, downleftY, width, height);
                ObjCanvas.Invalidate();

                ObjCanvas.LastPoint = ObjCanvas.CurrentPoint;

            }

        }

    }
}
