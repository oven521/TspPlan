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

        private void DeCodeTXTFile(string sourcetext)
        {
            string tempstring = "";
            bool Xflag = true;
            float X = 0;
            float Y;
            //test
            //Console.WriteLine(sourcetext);

            //遍历sourcetext
            try
            {
                foreach (char Character in sourcetext)
                {
                    //判断读入数据为横坐标或纵坐标
                    if ((Character == '\r' || Character == '\n' || Character == ' ') && tempstring != "")
                    {
                        if (Xflag)
                        {
                            X = float.Parse(tempstring);
                        }
                        else
                        {
                            Y = float.Parse(tempstring);
                            PointF Point = new PointF(X, Y);
                            SpotList.Add(Point);
                        }
                        tempstring = "";
                    }
                    else if (Character == 'X')
                    {
                        Xflag = true;
                    }
                    else if (Character == 'Y')
                    {
                        Xflag = false;
                    }
                    else if(char.IsDigit(Character))
                    {
                        tempstring += Character;
                    }
                }
            }
            catch
            {
                MessageBox.Show("文件打开失败");
            }
        }

        public void ImportTXTFile(string filePath, DrawModel model)
        {
            
            string sourcetext;
            using (StreamReader stream = new StreamReader(filePath))
            {
                sourcetext = stream.ReadToEnd();
            }
            DeCodeTXTFile(sourcetext);
            //将读入的点存入singleline当中
            List<CDrawingObjectBase> drawObjectList = model.DrawingObjectList;
            CDrawingObjectLWPolyLine lwPolyLine = new CDrawingObjectLWPolyLine(model.DrawCanvas);
            for (int i=0;i<SpotList.Count()-1;i++)
            {

                CDrawingObjectSingleLine l = new CDrawingObjectSingleLine(SpotList[i], SpotList[i + 1], 0, model.DrawCanvas);
                lwPolyLine.AddLine(l);

            }
            drawObjectList.Add(lwPolyLine);
        }
    }
}
