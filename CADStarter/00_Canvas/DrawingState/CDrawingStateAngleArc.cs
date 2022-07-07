using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CADEngine.DrawingObject;
using MainHMI;

namespace CADEngine.DrawingState
{
    public class CDrawingStateAngleArc: CDrawingStateBase, IDrawingStateBase
    {

        PointF center0 = new PointF();
        float r0 = 0;
        float startAngle = 0;
        float endAngle = 0;
        bool isCW = false;


        public CDrawingStateAngleArc(CanvasCtrl frm)
            : base(frm)
        { 
        
        }
        public void Init()
        {
            step = 0;
        }
        public void KeyDown(Keys keyData)
        {

        }
        public void MouseDown(object sender, MouseEventArgs e)
        {

        }
        public void MouseUp(object sender, MouseEventArgs e)
        {
            PointF lgPt = this.m_Canvas.SPt2LPt(new PointF(e.X, e.Y));

            switch (step)
            {
                case 0:
                    {
                        ObjCanvas.m_Points[0].X = lgPt.X;
                        ObjCanvas.m_Points[0].Y = lgPt.Y;

                        ObjCanvas.Cursor = Cursors.Cross;
                        step = 1;

                        break;
                    }
                case 1:
                    {
                        ObjCanvas.m_Points[1].X = lgPt.X;
                        ObjCanvas.m_Points[1].Y = lgPt.Y;

                        step = 2 ;
                        break;
                    }
                case 2:
                    {
                        ObjCanvas.m_Points[2].X = lgPt.X;
                        ObjCanvas.m_Points[2].Y = lgPt.Y;
               
                        PointF logicPoint0 = ObjCanvas.m_Points[0];
                        PointF logicPoint1 = ObjCanvas.m_Points[1];
                        PointF logicPoint2 = ObjCanvas.m_Points[2];

                        PointF center = new PointF();
                        float r =0 ;

                        if (CGeometryCircle.cocircle( logicPoint0,  logicPoint1, logicPoint2, ref center,ref r))
                        {
                            float startAngle = CGeometryCircle.ArcAngleHudu(center,logicPoint0);
                            float endAngle = CGeometryCircle.ArcAngleHudu(center,logicPoint2);

                            CDrawingObjectArc objArc = new CDrawingObjectArc(center, r, startAngle,endAngle,this.m_Canvas,false);
                            if (this.isCW)
                                objArc.Reverse();

                            this.ObjCanvas.DrawModel.AddDrawingObject(objArc);
                            ObjCanvas.DrawAndBackupBufferGraphic();
                            this.ObjCanvas.Refresh();
                        }
                        step = 0;
                        break;
                    }

            }
        }
        public void MouseMove(object sender, MouseEventArgs e)
        {
            PointF lgPt = this.m_Canvas.SPt2LPt(new PointF(e.X, e.Y));
            ObjCanvas.CurrentPoint = lgPt;
            ObjCanvas.Cursor = Cursors.Cross;
            if (step == 1){
                ObjCanvas.RestoreBufferGraphic();
                ObjCanvas.BufferGrahicHandle.DrawLine(new Pen(Color.Green), ObjCanvas.m_Points[0], ObjCanvas.CurrentPoint);
                ObjCanvas.Invalidate();
            }
            else if (step == 2)
            {
                ObjCanvas.RestoreBufferGraphic();
                PointF pt0 = this.ObjCanvas.m_Points[0];
                PointF pt1 = ObjCanvas.m_Points[1];
                PointF pt2 = ObjCanvas.CurrentPoint;

                double multiplyValue =  CGeometry.multiply(pt0,pt1,pt2);
                if (multiplyValue > 0)
                    isCW = false; //逆时针
                else if (multiplyValue < 0)
                    isCW = true; //顺时针
                else
                    return;//三点共线，不能组成圆

                if (CGeometryCircle.cocircle(pt0, pt1, pt2, ref center0, ref r0))
                {
                    //先用绿色的画出当前圆
                    RectangleF rectCur = CGeometryCircle.circleRect(center0, r0);
                    startAngle = CGeometryCircle.ArcAngleJiaodu(center0, pt0);
                    endAngle = CGeometryCircle.ArcAngleJiaodu(center0, pt2);
                    if (isCW)
                        ObjCanvas.BufferGrahicHandle.DrawArc(new Pen(Color.Green), rectCur, endAngle, (startAngle - endAngle + 360) % 360);
                    else
                        ObjCanvas.BufferGrahicHandle.DrawArc(new Pen(Color.Green), rectCur, startAngle, (endAngle - startAngle + 360) % 360);
                    ObjCanvas.BufferGrahicHandle.DrawLine(new Pen(Color.Green), ObjCanvas.m_Points[0], ObjCanvas.m_Points[1]);
                    ObjCanvas.Invalidate();
                }
            }

          
     
        }
    }
}
