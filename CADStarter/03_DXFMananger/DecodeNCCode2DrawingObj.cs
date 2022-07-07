using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using netDxf;
using netDxf.Header;
using netDxf.Objects;
using netDxf.Collections;
using netDxf.Entities;
using CADEngine.DrawingObject;
using System.Drawing.Drawing2D;
using CADEngine;
using ZedGraph;
using System.IO;
using MainHMI;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace _03_DXFMananger {
    public class DecodeNCCode2DrawingObj {

        string regTxtX = @"X-?(([0-9]+(\.)[0-9]+)|[0-9]+)";
        string regTxtY = @"Y-?(([0-9]+(\.)[0-9]+)|[0-9]+)";
        //string regTxtZ = @"Z-?(([0-9]+(\.)[0-9]+)|[0-9]+)";
        string regTxtI = @"I-?(([0-9]+(\.)[0-9]+)|[0-9]+)";
        string regTxtJ = @"J-?(([0-9]+(\.)[0-9]+)|[0-9]+)";
        //string regTxtK = @"K-?(([0-9]+(\.)[0-9]+)|[0-9]+)";
        //string regTxtT = @"T-?(([0-9]+(\.)[0-9]+)|[0-9]+)";
        //string regTxtR = @"R-?(([0-9]+(\.)[0-9]+)|[0-9]+)";

        string regTxtG90 = @"G90";
        string regTxtG91 = @"G91";

        bool _bAbsolute = true;
        int iMode = 0;


        PointF _prevPos = new PointF(0, 0);
        PointF _curPos = new PointF(0, 0);

        CanvasCtrl _canvas;
        private DrawModel _Model;

        public DrawModel DrawModel {
            set { _Model = value; }
            get { return _Model; }
        }
        public DecodeNCCode2DrawingObj(CanvasCtrl canvas) {
            _canvas = canvas;
        }
        public void Load(string filename) {
            StreamReader reader = null;
            try {
                reader = new StreamReader(filename);
                for (string line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
                    DecodeGCodeLine(line);
                }
            }

            catch (IOException e) {
                Console.WriteLine(e.Message);
            }
            finally {
                if (reader != null)
                    reader.Close();
            }
        }
        public void DecodeGCodeLine(string gcodeLine) {

            if (gcodeLine.Contains(regTxtG90)) { _bAbsolute = true; }
            else if (gcodeLine.Contains(regTxtG91)) { _bAbsolute = false; }

            if (gcodeLine.Contains(@"G01") || gcodeLine.Contains(@"G1")) { iMode = 1; }
            else if (gcodeLine.Contains(@"G02") || gcodeLine.Contains(@"G2")) { iMode = 2; }
            else if (gcodeLine.Contains(@"G03") || gcodeLine.Contains(@"G3")) { iMode = 3; }
            else if (gcodeLine.Contains(@"G00") || gcodeLine.Contains(@"G0")) { iMode = 0; }

            float xpos = 0;
            float ypos = 0;
            float ipos = 0;
            float jpos = 0;
            float rLength = 0;
            float startAngle = 0;
            float endAngle = 0;
            PointF centerPos = new PointF(0, 0);
            CDrawingObjectSingleLine line;
            CDrawingObjectCircleR circleR;
            CDrawingObjectArc arc;
            //是否有圆心坐标
            bool bHasI = false;
            bool bHasJ = false;

            switch (iMode) {
                case 0:
                    #region G00的处理
                    if (GetAxisPostion(regTxtX, gcodeLine, ref xpos)) {
                        if (_bAbsolute)
                            _curPos.X = xpos;
                        else
                            _curPos.X += xpos;
                    }
                    if (GetAxisPostion(regTxtY, gcodeLine, ref ypos)) {
                        if (_bAbsolute)
                            _curPos.Y = ypos;
                        else
                            _curPos.Y += ypos;
                    }
                    line = new CDrawingObjectSingleLine(_prevPos, _curPos, 0, _canvas);
                    line.IsG00Line = true;
                    DrawModel.AddDrawingObject(line);
                    _prevPos = _curPos;
                    break;
                    #endregion
                case 1:
                    #region G01的处理
                    if (GetAxisPostion(regTxtX, gcodeLine, ref xpos)) {
                        if (_bAbsolute)
                            _curPos.X = xpos;
                        else
                            _curPos.X += xpos;
                    }
                    if (GetAxisPostion(regTxtY, gcodeLine, ref ypos)) {
                        if (_bAbsolute)
                            _curPos.Y = ypos;
                        else
                            _curPos.Y += ypos;
                    }
                    line = new CDrawingObjectSingleLine(_prevPos, _curPos, 0, _canvas);
                    DrawModel.AddDrawingObject(line);

                    _prevPos = _curPos;
                    break;
                    #endregion
                case 2: //G02顺圆
                    //if (!GetAxisPostion(regTxtR, gcodeLine, ref rpos))
                    //{
                    //    MessageBox.Show("G02 格式不对，没有半径！");
                    //    return;
                    //}
                    if (GetAxisPostion(regTxtX, gcodeLine, ref xpos)) {
                        if (_bAbsolute)
                            _curPos.X = xpos;
                        else
                            _curPos.X += xpos;
                    }
                    if (GetAxisPostion(regTxtY, gcodeLine, ref ypos)) {
                        if (_bAbsolute)
                            _curPos.Y = ypos;
                        else
                            _curPos.Y += ypos;
                    }
                    //是否有圆心坐标
                    bHasI = GetAxisPostion(regTxtI, gcodeLine, ref ipos);
                    bHasJ = GetAxisPostion(regTxtJ, gcodeLine, ref jpos);
                    if (bHasI || bHasJ) {
                        Debug.WriteLine(ipos.ToString() + " " + jpos.ToString());
                        centerPos.X = _prevPos.X + ipos;
                        centerPos.Y = _prevPos.Y + jpos;

                        rLength = CPublic.CalDis(centerPos, _prevPos);
                        startAngle = CPublic.GetAngleByJiaoDu(centerPos, _prevPos);
                        endAngle = CPublic.GetAngleByJiaoDu(centerPos, _curPos);
                        if (startAngle <= endAngle)
                            startAngle += 360;
                        arc = new CDrawingObjectArc(centerPos, rLength, startAngle, endAngle, _canvas, true);
                        DrawModel.AddDrawingObject(arc);

                    }
                    else//整圆
                    {
                        centerPos.X = _curPos.X - rLength;
                        centerPos.Y = _curPos.Y;

                        circleR = new CDrawingObjectCircleR(centerPos, rLength, _canvas);
                        DrawModel.AddDrawingObject(circleR);

                    }
                    _prevPos = _curPos;
                    break;
                case 3:
                    //if (!GetAxisPostion(regTxtR, gcodeLine, ref rpos))
                    //{
                    //    MessageBox.Show("G03 格式不对,没有半径");
                    //    return;
                    //}
                    if (GetAxisPostion(regTxtX, gcodeLine, ref xpos)) {
                        if (_bAbsolute)
                            _curPos.X = xpos;
                        else
                            _curPos.X += xpos;
                    }
                    if (GetAxisPostion(regTxtY, gcodeLine, ref ypos)) {
                        if (_bAbsolute)
                            _curPos.Y = ypos;
                        else
                            _curPos.Y += ypos;
                    }
                    //是否有圆心坐标
                    bHasI = GetAxisPostion(regTxtI, gcodeLine, ref ipos);
                    bHasJ = GetAxisPostion(regTxtJ, gcodeLine, ref jpos);
                    if (bHasI || bHasJ) {
                        centerPos.X = _prevPos.X + ipos;
                        centerPos.Y = _prevPos.Y + jpos;

                        Debug.WriteLine(ipos.ToString() + " " + jpos.ToString());

                        rLength = CPublic.CalDis(centerPos, _prevPos);
                        startAngle = CPublic.GetAngleByJiaoDu(centerPos, _prevPos);
                        endAngle = CPublic.GetAngleByJiaoDu(centerPos, _curPos);
                        if (startAngle >= endAngle)
                            endAngle += 360;
                        arc = new CDrawingObjectArc(centerPos, rLength, startAngle, endAngle, _canvas, true);
                        DrawModel.AddDrawingObject(arc);

                    }
                    else//整圆
                    {
                        centerPos.X = _curPos.X - rLength;
                        centerPos.Y = _curPos.Y;

                        circleR = new CDrawingObjectCircleR(centerPos, rLength, _canvas);
                        DrawModel.AddDrawingObject(circleR);

                    }
                    _prevPos = _curPos;
                    break;
            }
            //所有的对象解析完成后，再统一做一个Invalidate
            this._canvas.Invalidate();
        }
        public bool GetAxisPostion(string regPattern, string GCodeLine, ref float fPos) {

            Regex reg = new Regex(regPattern);
            foreach (Match number in reg.Matches(GCodeLine)) {
                fPos = (float)Convert.ToDouble(number.Value.ToString().Substring(1));
                return true;
            }
            fPos = 0;
            return false;


        }

    }
}
