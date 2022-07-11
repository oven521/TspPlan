using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CADEngine.DrawingObject;
using MainHMI;

namespace CADEngine.DrawingState {
    public class CDrawingStateLine : CDrawingStateBase, IDrawingStateBase {

        public CDrawingStateLine(CanvasCtrl frm)
            : base(frm) {

        }
        public void Init() {
            step = 0;
        }
        public void KeyDown(Keys keyData) {
            if (keyData == Keys.Escape) {
                step = 0;
                this.ObjCanvas.Invalidate();
            }
        }
        public void MouseDown(object sender, MouseEventArgs e) {

        }
        public void MouseUp(object sender, MouseEventArgs e) {

            PointF lgPt = this.m_Canvas.SPt2LPt(new PointF(e.X, e.Y));
            switch (step) {
                case 0: {
                        ObjCanvas.m_Points[0].X = lgPt.X;
                        ObjCanvas.m_Points[0].Y = lgPt.Y;
                        if (CSnapPointBase.IsSnapOn)
                            ObjCanvas.m_Points[0] = this.m_Canvas.LPt2SPt(CSnapPointBase.CurrentSnap);

                        ObjCanvas.LastPoint = ObjCanvas.m_Points[0];
                        ObjCanvas.Cursor = Cursors.Cross;
                        step = 1;

                        break;
                    }
                case 1: {
                        ObjCanvas.m_Points[1].X = lgPt.X;
                        ObjCanvas.m_Points[1].Y = lgPt.Y;
                        if (CSnapPointBase.IsSnapOn)
                            ObjCanvas.m_Points[1] = this.m_Canvas.LPt2SPt(CSnapPointBase.CurrentSnap);

                        PointF logicPointStart = ObjCanvas.m_Points[0];
                        PointF logicPointEnd = ObjCanvas.m_Points[1];

                        CDrawingObjectSingleLine objLine = new CDrawingObjectSingleLine(logicPointStart, logicPointEnd, 0, ObjCanvas);

                        this.ObjCanvas.DrawModel.AddDrawingObject(objLine);
                        ObjCanvas.DrawAndBackupBufferGraphic();
                        this.ObjCanvas.Refresh();

                        step = 0;
                        break;
                    }

            }

        }
        public void MouseMove(object sender, MouseEventArgs e) {
            PointF lgPt = this.m_Canvas.SPt2LPt(new PointF(e.X, e.Y));
            ObjCanvas.CurrentPoint = lgPt;
            ObjCanvas.Cursor = Cursors.Cross;
            if (step == 1) {
                ObjCanvas.RestoreBufferGraphic();

                ObjCanvas.BufferGrahicHandle.DrawLine(new Pen(Color.Green), ObjCanvas.m_Points[0], ObjCanvas.CurrentPoint);

                ObjCanvas.LastPoint = ObjCanvas.CurrentPoint;

                ObjCanvas.Invalidate();
            }
            //首先关闭所有后的Snap
            CSnapPointBase.IsSnapOn = false;
            foreach (CDrawingObjectBase obj in this.DrawModel.DrawingObjectList) {
                CDrawingObjectSingleLine objLine = obj as CDrawingObjectSingleLine;
                if (objLine != null) {
                    PointF lPt = ObjCanvas.CurrentPoint;
                    //找到任何一个Snap就退出循环。
                    if (objLine.CheckSnapPoints(lPt))
                        break;
                }

            }

        }
    }
}
