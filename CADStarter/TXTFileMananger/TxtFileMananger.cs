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

namespace TXTMananger
{
    public class TxtFileMananger
    {
        string _strTXTFilePath;
        DrawModel _model;
        //存储点坐标
        private List<PointF> SpotList = new List<PointF>();

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
                    PointF Point = new PointF(float.Parse(Coordinate[0]), float.Parse(Coordinate[1]));
                    SpotList.Add(Point);
                }
            }
            catch
            {
                MessageBox.Show("文件打开失败");
            }
        }

        public void ImportTXTFile(string filePath, DrawModel model)
        {
            string[] sourcetext = File.ReadAllLines(filePath, Encoding.Default);
            //将读入的点存入SpotList当中
            SpotList.Clear();
            DeCodeTXTFile(sourcetext);

            List<CDrawingObjectBase> drawObjectList = model.DrawingObjectList;
            CDrawingObjectLWPolyLine lwPolyLine = new CDrawingObjectLWPolyLine(model.DrawCanvas);
            for (int i = 0; i < SpotList.Count() - 1; i++)
            {
                CDrawingObjectSingleLine l = new CDrawingObjectSingleLine(SpotList[i], SpotList[i + 1], 0, model.DrawCanvas);
                lwPolyLine.AddLine(l);

            }
            drawObjectList.Add(lwPolyLine);
        }
    }
}
