using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CADEngine.DrawingObject;
using MainHMI;

namespace CADEngine.DrawingState {
    public class CDrawingStatePolyLine : CDrawingStateBase, IDrawingStateBase {

        CDrawingObjectLWPolyLine _polyLine = null;
        PointF _headPt;
        public CDrawingStatePolyLine(CanvasCtrl frm)
            : base(frm) {
            _polyLine = new CDrawingObjectLWPolyLine(frm);
        }
        public void Init() {
            step = 0;
            if (null != _polyLine) {
                _polyLine.ClearLines();
                _polyLine = null;
            }
        }
        public void KeyDown(Keys keyData) {
            if (keyData == Keys.Escape) {
                step = 0;
                this.ObjCanvas.Invalidate();
            }
            else if (keyData == Keys.L) {
                step = 1;
                //this.ObjCanvas.Invalidate();
            }
            else if (keyData == Keys.R) {
                step = 2;
                //this.ObjCanvas.Invalidate();
            }
            else if (keyData == Keys.C) {
                CDrawingObjectSingleLine objLine = new CDrawingObjectSingleLine(ObjCanvas.m_Points[0], _headPt, 0, ObjCanvas);
                if (_polyLine != null)
                    _polyLine.AddLine(objLine);

                this.ObjCanvas.DrawModel.AddDrawingObject(_polyLine);
                ObjCanvas.DrawAndBackupBufferGraphic();
                this.ObjCanvas.Refresh();
                step = 0;
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
                        ObjCanvas.LastPoint = ObjCanvas.m_Points[0];
                        ObjCanvas.Cursor = Cursors.Cross;
                        step = 1;
                        //记录下整个多线段曲线的起始点
                        _headPt = ObjCanvas.m_Points[0];

                        _polyLine = new CDrawingObjectLWPolyLine(this.m_Canvas);
                        break;
                    }
                case 1: {
                        ObjCanvas.m_Points[1].X = lgPt.X;
                        ObjCanvas.m_Points[1].Y = lgPt.Y;

                        PointF logicPointStart = ObjCanvas.m_Points[0];
                        PointF logicPointEnd = ObjCanvas.m_Points[1];

                        CDrawingObjectSingleLine objLine = new CDrawingObjectSingleLine(logicPointStart, logicPointEnd, 0, ObjCanvas);

                        _polyLine.AddLine(objLine);
                        //this.DrawModel.AddDrawingObject(objLine);
                        this.ObjCanvas.Invalidate();
                        ////以该点为起点
                        ObjCanvas.m_Points[0].X = lgPt.X;
                        ObjCanvas.m_Points[0].Y = lgPt.Y;
                        ObjCanvas.LastPoint = ObjCanvas.m_Points[0];
                        
                        break;
                    }
                case 2: {
                        ObjCanvas.m_Points[1].X = lgPt.X;
                        ObjCanvas.m_Points[1].Y = lgPt.Y;

                        PointF logicPointStart = ObjCanvas.m_Points[0];
                        PointF logicPointEnd = ObjCanvas.m_Points[1];

                        int lineNum = _polyLine._SinglelineList.Count;

                        CDrawingObjectSingleLine objLastLine = _polyLine._SinglelineList[lineNum - 1];

                        GLine lineQiexian = CGeometryLine.MakeALine(objLastLine.StartPoint,objLastLine.EndPoint);
                        //获得过起点的垂线
                        GLine vLineOnStartPoint = CGeometryLine.GetVeticalLine(lineQiexian, new PointF(objLastLine.EndPoint.X,objLastLine.EndPoint.Y));
                        //获得弦的
                        GLine xianLine = CGeometryLine.MakeALine(logicPointStart, logicPointEnd);
                        GLine vLinexian = CGeometryLine.GetVeticalLine(xianLine,CGeometryLine.GetLineMiddlePT(logicPointStart,logicPointEnd));
                        //获得圆心：
                        PointF center = new PointF();
                        if (CGeometryLine.Islineintersect(vLinexian, vLineOnStartPoint, ref center)){

                            float startAngle = CGeometryCircle.ArcAngleHudu(center, logicPointStart);
                            float endAngle = CGeometryCircle.ArcAngleHudu(center, logicPointEnd);
                            float r = (float)CGeometryLine.dist(center, logicPointEnd);

                            CDrawingObjectArc objArc = new CDrawingObjectArc(center, r, startAngle, endAngle, this.m_Canvas, false);
                            //这里要区分是逆时针还是顺时针！！！！先认为指定是逆时针

                            double multiplyValue = CGeometry.multiply(objLastLine.StartPoint, objLastLine.EndPoint, lgPt);
                            bool isCW = false;
                            if (multiplyValue > 0)
                                isCW = false; //逆时针
                            else if (multiplyValue < 0)
                                isCW = true; //顺时针
                            else
                                return;//三点共线，不能组成圆

                            if (isCW)
                                objArc.Reverse();

                            _polyLine.AddLine(objArc.ToLineWithBulge());

                            this.ObjCanvas.Invalidate();
                            ////以该点为起点
                            ObjCanvas.m_Points[0].X = lgPt.X;
                            ObjCanvas.m_Points[0].Y = lgPt.Y;
                            ObjCanvas.LastPoint = ObjCanvas.m_Points[0];

                        }
                        break;
                    }
                case 3://Close curve
                    {
                        break;
                    }

            }
        }
        public void MouseMove(object sender, MouseEventArgs e) {
            PointF lgPt = this.m_Canvas.SPt2LPt(new PointF(e.X, e.Y));

            ObjCanvas.RestoreBufferGraphic();
            ObjCanvas.CurrentPoint = new PointF(lgPt.X, lgPt.Y);

            ObjCanvas.Cursor = Cursors.Cross;
            switch (step) {
                case 1:
                    if (_polyLine != null) {
                        foreach (CDrawingObjectSingleLine objLine in _polyLine._SinglelineList) {
                            objLine.Draw(ObjCanvas.BufferGrahicHandle);
                        }
                    }
                    ObjCanvas.BufferGrahicHandle.DrawLine(new Pen(Color.Green), ObjCanvas.m_Points[0], ObjCanvas.CurrentPoint);
                    ObjCanvas.LastPoint = ObjCanvas.CurrentPoint;
                    ObjCanvas.Invalidate();
                    break;
                case 2:
                    if (_polyLine != null) {
                        foreach (CDrawingObjectSingleLine objLine in _polyLine._SinglelineList) {
                           // ObjCanvas.BufferGrahicHandle.DrawLine(new Pen(Color.Green), objLine.StartPoint, objLine.EndPoint);
                            objLine.Draw(ObjCanvas.BufferGrahicHandle);
                        }
                    }
                    
                    int lineNum = _polyLine._SinglelineList.Count;

                    CDrawingObjectSingleLine objLastLine = _polyLine._SinglelineList[lineNum - 1];
                    GLine lineQiexian = CGeometryLine.MakeALine(objLastLine.StartPoint, objLastLine.EndPoint);
                    //获得过起点的垂线
                    GLine vLineOnStartPoint = CGeometryLine.GetVeticalLine(lineQiexian, new PointF(objLastLine.EndPoint.X, objLastLine.EndPoint.Y));
                    //获得弦的
                    PointF logicPointStart = objLastLine.EndPoint;
                    PointF logicPointEnd = lgPt;
                    GLine xianLine = CGeometryLine.MakeALine(logicPointStart, logicPointEnd);
                    GLine vLinexian = CGeometryLine.GetVeticalLine(xianLine,CGeometryLine.GetLineMiddlePT(logicPointStart,logicPointEnd));
                    //获得圆心：
                    PointF center = new PointF();
                    if (CGeometryLine.Islineintersect(vLinexian, vLineOnStartPoint, ref center)) {

                        float startAngle = CGeometryCircle.ArcAngleHudu(center, logicPointStart);
                        float endAngle = CGeometryCircle.ArcAngleHudu(center, logicPointEnd);
                        float r = (float)CGeometryLine.dist(center, logicPointEnd);

                        CDrawingObjectArc objArc = new CDrawingObjectArc(center, r, startAngle, endAngle, this.m_Canvas, false);
                        //这里要区分是逆时针还是顺时针！！！！先认为指定是逆时针

                        double multiplyValue = CGeometry.multiply(objLastLine.StartPoint, objLastLine.EndPoint, lgPt);
                        bool isCW = false;
                        if (multiplyValue > 0)
                            isCW = false; //逆时针
                        else if (multiplyValue < 0)
                            isCW = true; //顺时针
                        else
                            return;//三点共线，不能组成圆

                        if (isCW)
                            objArc.Reverse();

                        CDrawingObjectSingleLine singLineWihtArc= objArc.ToLineWithBulge();
                        singLineWihtArc.Draw(ObjCanvas.BufferGrahicHandle);
                        ObjCanvas.Invalidate();
                    }
                    ObjCanvas.Invalidate();
                    break;
            }
               
        }


    }
}
