using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CADEngine;
using CADEngine.DrawingObject;
using MainHMI;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Tsp;
namespace TXTMananger
{
    public class TxtFileMananger
    {
        string _strTXTFilePath;
        DrawModel _model;
        float Offset=1;//将所有坐标乘上一个系数，防止点过于密集，方便观察
        //存储点坐标
        public List<PointF> SpotList = new List<PointF>();

        public List<xPoint> SpotListXPoint = new List<xPoint>();
        public TxtFileMananger(string filePath, DrawModel model)
        {
            _strTXTFilePath = filePath;
            _model = model;
        }

        private void DeCodeTXTFile(string[] sourcetext)
        {
            //Console.WriteLine(sourcetext);
            //遍历sourcetext
            try
            {
                foreach (string Str in sourcetext)
                {
                    string[] Coordinate = Str.Split(' ');
                    PointF Point = new PointF(float.Parse(Coordinate[0])* Offset, float.Parse(Coordinate[1])* Offset);
                    SpotList.Add(Point);
                }
            }
            catch
            {
                MessageBox.Show("文件打开失败,请选择正确文件格式：X Y");
            }
        }

        public void ImportTXTFile(string filePath)
        {
            string[] sourcetext = File.ReadAllLines(filePath, Encoding.Default);
            //将读入的点存入SpotList当中
            SpotList.Clear();
            SpotListXPoint.Clear();
            DeCodeTXTFile(sourcetext);
            ChangeIntoXPoint();
        }
        //生成多线段列表用于输出到屏幕
        public void GeneratePolyLine(DrawModel model)
        {
            ChangeIntoPointF();
            model.RemoveAllDrawingObject();
            List<CDrawingObjectBase> drawObjectList = model.DrawingObjectList;
            CDrawingObjectLWPolyLine lwPolyLine = new CDrawingObjectLWPolyLine(model.DrawCanvas);
            for (int i = 0; i < SpotList.Count() - 1; i++)
            {
                CDrawingObjectSingleLine l = new CDrawingObjectSingleLine(SpotList[i], SpotList[i + 1], 0, model.DrawCanvas);
                lwPolyLine.AddLine(l);

            }
            drawObjectList.Add(lwPolyLine);
        }

        public void ChangeIntoXPoint()
        {
            for(int i=0;i< SpotList.Count;i++)
            {
                xPoint xPoint = new xPoint();
                xPoint.XPos = SpotList[i].X;
                xPoint.YPos = SpotList[i].Y;
                SpotListXPoint.Add(xPoint);
            }
        }

        public void ChangeIntoPointF()
        {
            SpotList.Clear();
            for (int i = 0; i < SpotListXPoint.Count; i++)
            {
                PointF Point = new PointF(SpotListXPoint[i].XPos, SpotListXPoint[i].YPos);
                SpotList.Add(Point);
            }
        }
        public void Output(List<xPoint> xPoints)
        {

            using (StreamWriter stream = new StreamWriter("结果.txt"))
            {
                for (int i = 0; i < xPoints.Count(); i++)
                {
                    stream.WriteLine(xPoints[i].XPos + " " + xPoints[i].YPos);
                }
            }
        }
    }
}
