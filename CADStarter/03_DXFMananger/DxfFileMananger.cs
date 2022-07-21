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

namespace _03_DXFMananger {
    public class DxfFileMananger {
        string _strDxfFilePath;
        DrawModel _model;

        public DxfFileMananger(string filePath, DrawModel model) {
            _strDxfFilePath = filePath;
            _model = model;

        }

        public void ImportDxfFile(string file, DrawModel model) {

            List<CDrawingObjectBase> drawObjectList = model.DrawingObjectList;
            //drawObjectList.Clear();
            //for (int i = 0; i < 100; ++i)
            //    for (int j = 0; j < 100; ++j) {

            //        drawObjectList.Add(new CDrawingObjectCircleR(new PointF(i, j), 100, model.DrawCanvas));
            //    }

            //// sample.dxf contains all supported entities by netDxf
            bool isBinary;
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file, out isBinary);
            if (dxfVersion < DxfVersion.AutoCad2000) {
                MessageBox.Show("THE FILE {0} IS NOT A VALID DXF", file);
                return;
            }

             //drawObjectList.Clear();

            DxfDocument dxf = DxfDocument.Load(file);

            ////取出图中所有的弧
            ////草他妈，arc竟然按照度数来的！！！！
            ////并且AUTOCAD会更改弧的方向，也就是说，总是让start<end,只有跨4，1象限的时候，start才会大于end。
            foreach (Arc arc in dxf.Arcs) {
                CDrawingObjectArc arcObj = new CDrawingObjectArc(new PointF((float)arc.Center.X, (float)arc.Center.Y), (float)arc.Radius, (float)arc.StartAngle, (float)arc.EndAngle, this._model.DrawCanvas, true);
                drawObjectList.Add(arcObj.ToLineWithBulge());
                System.Diagnostics.Debug.WriteLine(arc.StartAngle.ToString() + "->" + arc.EndAngle.ToString());
            }
            //取出图中所有的圆

            foreach (Circle cirle in dxf.Circles) {
                PointF center = new PointF((float)cirle.Center.X, (float)cirle.Center.Y);
                CDrawingObjectCircleR circleR = new CDrawingObjectCircleR(center, (float)cirle.Radius, model.DrawCanvas);
                drawObjectList.Add(circleR);
            }
            ////取出图中所有的线
            foreach (netDxf.Entities.Line line in dxf.Lines) {
                drawObjectList.Add(new CDrawingObjectSingleLine(new PointF((float)line.StartPoint.X, (float)line.StartPoint.Y),
                    new PointF((float)line.EndPoint.X, (float)line.EndPoint.Y), 0, model.DrawCanvas));
            }
            CDrawingObjectLWPolyLine lwPolyLine = null;
            ////取出图中所有的LWPolyLine
            foreach (LwPolyline line in dxf.LwPolylines) {
                lwPolyLine = new CDrawingObjectLWPolyLine(model.DrawCanvas);

                for (int i = 0; i < line.Vertexes.Count - 1; ++i) {
                    int nexti = (i + 1);
                    PointF pStart = new PointF((float)line.Vertexes[i].Location.X, (float)line.Vertexes[i].Location.Y);
                    PointF pEnd = new PointF((float)line.Vertexes[nexti].Location.X, (float)line.Vertexes[nexti].Location.Y);
                    CDrawingObjectSingleLine l = new CDrawingObjectSingleLine(pStart, pEnd, line.Vertexes[i].Bulge, model.DrawCanvas);
                    lwPolyLine.AddLine(l);
                    //break;
                }
                if (line.Vertexes.Count > 2) {
                    int nexti = (0);
                    LwPolylineVertex lastVertex = line.Vertexes[line.Vertexes.Count - 1];
                    PointF pStart = new PointF((float)lastVertex.Location.X, (float)lastVertex.Location.Y);
                    PointF pEnd = new PointF((float)line.Vertexes[nexti].Location.X, (float)line.Vertexes[nexti].Location.Y);
                    CDrawingObjectSingleLine l = new CDrawingObjectSingleLine(pStart, pEnd, lastVertex.Bulge, model.DrawCanvas);
                    lwPolyLine.AddLine(l);
                }

                drawObjectList.Add(lwPolyLine);

            }
        }



    }
}
