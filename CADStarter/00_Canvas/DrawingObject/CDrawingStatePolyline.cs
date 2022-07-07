using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CADEngine.DrawingObject;
using Canvas;
using Win32APIDraw;

namespace CADEngine.DrawingState
{
    class CDrawingStatePolyline : CDrawingStateBase, IDrawingStateBase
    {
           Win32XORPenDrawer drawer;

           public CDrawingStatePolyline(CanvasCtrl frm)
            : base(frm)
        {
            drawer = new Win32XORPenDrawer(frm.Handle.ToInt32(), 6);
        }
        public void KeyDown(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                step = 0;
                this.ObjCanvas.Invalidate();
            }
        }
        public void MouseDown(object sender, MouseEventArgs e)
        {

        }
        public void MouseUp(object sender, MouseEventArgs e)
        {
            switch (step)
            {
                case 0:
                    {
                        ObjCanvas.m_Points[0].X = e.X;
                        ObjCanvas.m_Points[0].Y = e.Y;
                        ObjCanvas.LastPoint = ObjCanvas.m_Points[0];
                        ObjCanvas.Cursor = Cursors.Cross;
                        step = 1;

                        _bitmapBackground = new Bitmap(ObjCanvas.Width, ObjCanvas.Height);
                        ObjCanvas.DrawToBitmap(_bitmapBackground, new Rectangle(0, 0, ObjCanvas.Width, ObjCanvas.Height));
                        break;
                    }
                case 1:
                    {
                        ObjCanvas.m_Points[1].X = e.X;
                        ObjCanvas.m_Points[1].Y = e.Y;

                        gCanvas = ObjCanvas.CreateGraphics();


                        PointF screenCenter = ObjCanvas.ScreenCenter;
                        float scaledX = (ObjCanvas.m_Points[0].X - screenCenter.X) / (float)ObjCanvas.ZoomScale;
                        float scaledY = (screenCenter.Y - ObjCanvas.m_Points[0].Y) / (float)ObjCanvas.ZoomScale;
                        PointF logicPointStart = new PointF(scaledX, scaledY);

                        scaledX = (ObjCanvas.m_Points[1].X - screenCenter.X) / (float)ObjCanvas.ZoomScale;
                        scaledY = (screenCenter.Y - ObjCanvas.m_Points[1].Y) / (float)ObjCanvas.ZoomScale;
                        PointF logicPointEnd = new PointF(scaledX, scaledY);


                        CDrawingObjectLine objLine = new CDrawingObjectLine(logicPointStart, logicPointEnd, ObjCanvas);
                        this.ObjCanvas.AddDrawingObject(objLine);

                        //以该点为起点
                        ObjCanvas.m_Points[0].X = e.X;
                        ObjCanvas.m_Points[0].Y = e.Y;
                        ObjCanvas.LastPoint = ObjCanvas.m_Points[0];
                        step = 1;

                        _bitmapBackground = new Bitmap(ObjCanvas.Width, ObjCanvas.Height);
                        ObjCanvas.DrawToBitmap(_bitmapBackground, new Rectangle(0, 0, ObjCanvas.Width, ObjCanvas.Height));
                        break;
                    }

            }
        }
        public void MouseMove(object sender, MouseEventArgs e)
        {
            ObjCanvas.CurrentPoint = new Point(e.X, e.Y);
            ObjCanvas.Cursor = Cursors.Cross;
            if (step == 1)
            {
                //方式1，直接用背景颜色绘图
                //g = ObjCanvas.CreateGraphics();

                //g.DrawLine(new Pen(ObjCanvas.BackColor), ObjCanvas.m_Points[0], ObjCanvas.LastPoint);

                //g.DrawLine(new Pen(Color.Green), ObjCanvas.m_Points[0], ObjCanvas.CurrentPoint);

                //ObjCanvas.LastPoint = ObjCanvas.CurrentPoint;

               //方式2 Win32橡皮筋
                //drawer.DrawLine((int)(ObjCanvas.m_Points[0].X), (int)(ObjCanvas.m_Points[0].Y), 
                //    (int)(ObjCanvas.LastPoint.X), (int)(ObjCanvas.LastPoint.Y));
                //ObjCanvas.LastPoint = ObjCanvas.CurrentPoint;
                //drawer.DrawLine((int)(ObjCanvas.m_Points[0].X), (int)(ObjCanvas.m_Points[0].Y),
                //   (int)(ObjCanvas.LastPoint.X), (int)(ObjCanvas.LastPoint.Y));
               //方式3 双缓冲

                Graphics gCanvas = ObjCanvas.CreateGraphics();

                BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
                BufferedGraphics myBuffer = currentContext.Allocate(gCanvas, new Rectangle(0,0, ObjCanvas.Width,ObjCanvas.Height));
                Graphics gBuffer = myBuffer.Graphics;
                gBuffer.DrawImage(_bitmapBackground, new Point(-1, -1));

                gBuffer.DrawLine(new Pen(Color.Green), ObjCanvas.m_Points[0], ObjCanvas.CurrentPoint);
                
                myBuffer.Render(gCanvas);  //呈现图像至关联的Graphics  
                myBuffer.Dispose();
                gBuffer.Dispose();

                ObjCanvas.LastPoint = ObjCanvas.CurrentPoint;
            }

        }
    }
}
