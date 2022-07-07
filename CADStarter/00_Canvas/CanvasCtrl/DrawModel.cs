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
using ZedGraph;
using CADEngine;

namespace MainHMI {
    public class DrawModel {
        List<CDrawingObjectBase> _DrawObjectList = new List<CDrawingObjectBase>();
        List<CDrawingObjectArc> _ArcObjectList = new List<CDrawingObjectArc>();
        List<CDrawingObjectSingleLine> _LineObjectList = new List<CDrawingObjectSingleLine>();
        CanvasCtrl _canvas;

        public DrawModel(CanvasCtrl canvas) {
            this._canvas = canvas;
            this._canvas.DrawModel = this;
        }

        public CanvasCtrl DrawCanvas {
            get { return _canvas; }
        }
        /// <summary>
        /// 保存了所有的绘图对象
        /// </summary>
        public List<CDrawingObjectBase> DrawObjectList {
            get { return _DrawObjectList; }
        }
        /// <summary>
        /// 保存了所有的圆弧对象
        /// </summary>
        public List<CDrawingObjectArc> ArcObjectList {
            get { return _ArcObjectList; }
        }
        /// <summary>
        /// 保存了所有的直线对象
        /// </summary>
        public List<CDrawingObjectSingleLine> LineObjectList {
            get { return _LineObjectList; }
        }
        public List<CDrawingObjectBase> DrawingObjectList {
            get { return this._DrawObjectList; }
        }
        public void AddDrawingObject(CDrawingObjectBase obj) {
            this.DrawObjectList.Add(obj);
        }
        public void MergeSelectedSandaloneLines() {
            List<CDrawingObjectSingleLine> alllines2Merge = new List<CDrawingObjectSingleLine>();
            List<CDrawingObjectBase> selectedObjects = new List<CDrawingObjectBase>();
            foreach (CDrawingObjectBase obj in this.DrawObjectList) {
                CDrawingObjectSingleLine line = obj as CDrawingObjectSingleLine;
                if (line != null && obj.Selected) {
                    alllines2Merge.Add(line);
                    //稍后从_Model.DrawObjectList删除selectedObjects里的对象
                    selectedObjects.Add(line);
                }
                //如果是没有闭合的polyline，先把polyline打散成Line，再组合
                CDrawingObjectLWPolyLine polyline = obj as CDrawingObjectLWPolyLine;
                if (polyline != null && !polyline.IsClosed() && obj.Selected) {
                    foreach (CDrawingObjectSingleLine l in polyline.GetAllLines()) {
                        //多线段炸断为单个线
                        l.IsInClosedPolyLine = false;
                        alllines2Merge.Add(l);
                    }
                    //稍后从_Model.DrawObjectList删除selectedObjects里的对象
                    selectedObjects.Add(polyline);
                }
            }

            List<CDrawingObjectLWPolyLine> lwPolyLines = this.MergeAllStandaloneLines(alllines2Merge);

            //从ObjectList里面删除lwPolyLines里的所有的线段

            foreach (CDrawingObjectLWPolyLine pwLine in lwPolyLines) {
                this.DrawObjectList.Add(pwLine);
            }

            foreach (CDrawingObjectBase obj in selectedObjects) {
                this.DrawObjectList.Remove(obj);
            }
            this.DrawCanvas.DrawAndBackupBufferGraphic();
            this.DrawCanvas.Refresh();
        }

        public void BoomPolyLines() {
            List<CDrawingObjectBase> objectToDelete = new List<CDrawingObjectBase>();
            List<CDrawingObjectBase> boomedLineObjects = new List<CDrawingObjectBase>();
            foreach (CDrawingObjectBase obj in this.DrawObjectList) {
                List<CDrawingObjectSingleLine> singleLines = obj.GetAllLines();
                if (obj.Selected && singleLines != null) {
                    foreach (CDrawingObjectSingleLine l in singleLines) {
                        //多线段炸断为单个线
                        l.IsInClosedPolyLine = false;
                        l.Selected = false;
                        boomedLineObjects.Add(l);
                    }
                    objectToDelete.Add(obj);
                }
            }

            //从ObjectList里面删除lwPolyLines里的所有的线段
            //把炸碎后的图形重新加入列表
            foreach (CDrawingObjectBase obj in boomedLineObjects) {
                this.DrawObjectList.Add(obj);
            }
            foreach (CDrawingObjectBase obj in objectToDelete) {
                this.DrawObjectList.Remove(obj);
            }

            this.DrawCanvas.DrawAndBackupBufferGraphic();
            this.DrawCanvas.Refresh();

        }
        public void ReverseSelectedObjects() {
            foreach (CDrawingObjectBase obj in this.DrawObjectList) {
                if (obj.Selected) {
                    obj.Reverse();
                }
            }
            this.DrawCanvas.DrawAndBackupBufferGraphic();
            this.DrawCanvas.Refresh();

        }
        public void RemoveAllDrawingObject() {
            while (this.DrawObjectList.Count >= 1) {
                this.DrawObjectList.RemoveAt(this.DrawObjectList.Count - 1);
            }
        }

        public List<CDrawingObjectLWPolyLine> MergeAllStandaloneLines(List<CDrawingObjectSingleLine> standaloneLines) {
            List<CDrawingObjectLWPolyLine> _mergedLWline = new List<CDrawingObjectLWPolyLine>();

            foreach (CDrawingObjectSingleLine lineObj in standaloneLines) {
                if (!lineObj.IsInClosedPolyLine) {
                    CDrawingObjectLWPolyLine polyLineObj = new CDrawingObjectLWPolyLine(this.DrawCanvas);
                    polyLineObj.AddLine(lineObj);
                    //递归添加剩下的line到当前的polyLine中
                    MergeLine2PWLine(standaloneLines, polyLineObj);
                    //如果添加的polyLine不闭合，那么再反向找找
                    if (!polyLineObj.IsClosed()) {
                        polyLineObj.Reverse();
                        MergeLine2PWLine(standaloneLines, polyLineObj);
                    }
                    _mergedLWline.Add(polyLineObj);
                }
            }
            return _mergedLWline;

        }

        private void MergeLine2PWLine(List<CDrawingObjectSingleLine> standAloneLines, CDrawingObjectLWPolyLine polyLine) {
            foreach (CDrawingObjectSingleLine line in standAloneLines) {
                if (line.IsInClosedPolyLine)
                    continue;
                if (CGeometry.dist(polyLine.EndPoint, line.StartPoint) <= 0.1) {
                    polyLine.AddLine(line);
                    MergeLine2PWLine(standAloneLines, polyLine);
                }
                else if (CGeometry.dist(polyLine.EndPoint, line.EndPoint) <= 0.1) {
                    line.Reverse();
                    polyLine.AddLine(line);
                    MergeLine2PWLine(standAloneLines, polyLine);
                }

            }
        }



    }
}
