using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CADEngine.DrawingObject;
using MainHMI;

namespace CADEngine.DrawingState {
    public class CDrawingStateIdle : CDrawingStateBase, IDrawingStateBase {

        public CDrawingStateIdle(CanvasCtrl frm)
            : base(frm) {

        }
        public void Init() {
            step = 0;
        }
        public void MouseDown(object sender, MouseEventArgs e) {

            step += 1;
            ObjCanvas.m_Points[0].X = e.X;
            ObjCanvas.m_Points[0].Y = e.Y;
            ObjCanvas.Cursor = Cursors.Hand;

        }
        public void MouseUp(object sender, MouseEventArgs e) {
            step -= 1;
            if (step != 0) {
                step = 0;
                return;
            }
            ObjCanvas.m_Points[1].X = e.X;
            ObjCanvas.m_Points[1].Y = e.Y;

            float offsetX = ObjCanvas.m_Points[1].X - ObjCanvas.m_Points[0].X;
            float offsetY = ObjCanvas.m_Points[1].Y - ObjCanvas.m_Points[0].Y;
            this.ObjCanvas.AddDeviceOffsetXY(offsetX, offsetY);

            ObjCanvas.Cursor = Cursors.Arrow;

            System.Diagnostics.Debug.WriteLine("MouseUP!");
        }
        public void KeyDown(Keys keyData) {

        }

        public void MouseMove(object sender, MouseEventArgs e) {

        }

    }
}
