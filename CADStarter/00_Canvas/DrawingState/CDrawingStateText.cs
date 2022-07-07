using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CADEngine.DrawingObject;
using MainHMI;

namespace CADEngine.DrawingState {
    public class CDrawingStateText : CDrawingStateBase, IDrawingStateBase {

        public CDrawingStateText(CanvasCtrl frm)
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
            switch (step) {
                case 0: {
                        ObjCanvas.m_Points[0].X = e.X;
                        ObjCanvas.m_Points[0].Y = e.Y;
                        ObjCanvas.LastPoint = ObjCanvas.m_Points[0];
                        ObjCanvas.Cursor = Cursors.Cross;

                        PointF screenCenter = ObjCanvas.ScreenCenter;
                        float scaledX = (ObjCanvas.m_Points[0].X - screenCenter.X) / (float)ObjCanvas.ZoomScale;
                        float scaledY = (screenCenter.Y - ObjCanvas.m_Points[0].Y) / (float)ObjCanvas.ZoomScale;
                        PointF logicPointStart = new PointF(scaledX, scaledY);

                        CDrawingObjectFonts font2Draw = new CDrawingObjectFonts(logicPointStart, "隶属", "包俊杰", this.m_Canvas);
                        this.DrawModel.AddDrawingObject(font2Draw);
                        this.DrawModel.DrawCanvas.Invalidate();
                        step = 0;
                        //////以该点为起点
                        //ObjCanvas.m_Points[0].X = e.X;
                        //ObjCanvas.m_Points[0].Y = e.Y;
                        //ObjCanvas.LastPoint = ObjCanvas.m_Points[0];
                        step = 1;
                        break;
                    }
            }
        }
        public void MouseMove(object sender, MouseEventArgs e) {


        }
    }
}
