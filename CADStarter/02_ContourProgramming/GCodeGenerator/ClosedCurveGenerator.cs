using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CADEngine.DrawingObject;
using System.IO;
using System.Windows.Forms;
using System.Windows;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ContourProgramming {
    public class CloseCurveGenerator {
        StringBuilder strBuilder = new StringBuilder();

        public void GenerateGCodeFile(List<CDrawingObjectBase> objectList) {
            List<CuttingTreeNode> nodeList = new List<CuttingTreeNode>();
            int i = 0;
            foreach (CDrawingObjectBase obj in objectList) {
                i++;
                CuttingTreeNode node = new CuttingTreeNode();
                node.Data = obj;
                node.ID = i;
                nodeList.Add(node);

            }
            CCuttingTree testTree = new CCuttingTree();

            testTree.CreateCuttingTree(testTree._rootNode, nodeList);

            string strOuput = "访问顺序是(";

            testTree.TranversCuttingTree(testTree._rootNode, ref strOuput);

            strOuput += ")";

            MessageBox.Show(strOuput);




            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            //openFileDialog1.InitialDirectory = @"D:\18_CADEngine\CADDxfDrawing";
            //openFileDialog1.Filter = "DXF文件|*.dxf|所有文件|*.*";
            //openFileDialog1.RestoreDirectory = true;
            //openFileDialog1.FilterIndex = 2;

            //if (openFileDialog1.ShowDialog() != DialogResult.OK)
            //{
            //    return;
            //}

            //string path = openFileDialog1.FileName + @"\OutputGode.nc";

            // string path = @"D:\TestGCode.nc";
            // StringBuilder strBuilder = new StringBuilder();
            // //if (System.IO.File.Exists(path))
            // //{
            //     using (StreamWriter sw = new StreamWriter(path, false, Encoding.Default)) 
            //     {
            //         foreach (CDrawingObjectBase obj in objectList)
            //         {
            //             strBuilder.Append(obj.ToGcode());
            //         }
            //         sw.WriteLine(strBuilder);
            //         sw.Close();
            //     }
            //// }
            //     MessageBox.Show("G code文件已保存至:" + path);



        }

    }
}
