using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ContourProgramming;
using System.Collections.Generic;
using CADEngine;
using CADEngine.DrawingObject;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace UnitTestCotour
{
    [TestClass]
    public class TestCuttingCurve
    {
        [TestMethod]
        public void TestMethodCurveContains()
        {
            List<CuttingTreeNode> nodeList = new List<CuttingTreeNode>();

            CDrawingObjectCircleR circle1= new CDrawingObjectCircleR(new PointF(100f, 100f), 10f,null);
            CDrawingObjectCircleR circle2 = new CDrawingObjectCircleR(new PointF(150f, 100f), 100f, null);
            CDrawingObjectCircleR circle3 = new CDrawingObjectCircleR(new PointF(200f, 100f), 10f, null);
            CDrawingObjectCircleR circle4 = new CDrawingObjectCircleR(new PointF(400f, 100f), 10f, null);
            CDrawingObjectCircleR circle5 = new CDrawingObjectCircleR(new PointF(400f, 100f), 50f, null);
            CDrawingObjectCircleR circle6 = new CDrawingObjectCircleR(new PointF(500f, 100f), 10f, null);

            for (int i=0;i<6;++i)
            {
                CuttingTreeNode node = new CuttingTreeNode();
                node.ID = i + 1;
                nodeList.Add(node);
            }


            nodeList[0].Data = circle1;
            nodeList[1].Data = circle2;
            nodeList[2].Data = circle3;
            nodeList[3].Data = circle4;
            nodeList[4].Data = circle5;
            nodeList[5].Data = circle6;
   

            CCuttingTree treeWith7Node = new CCuttingTree();

            Assert.IsTrue(nodeList[1].Contains(nodeList[0]));
            Assert.IsTrue(nodeList[1].Contains(nodeList[2]));
            Assert.IsTrue(nodeList[4].Contains(nodeList[3]));
           
       
             treeWith7Node.CreateCuttingTree(treeWith7Node._rootNode, nodeList);
        

            Assert.AreEqual(treeWith7Node._rootNode.ChildrenList.Count, 3);
            Assert.AreEqual(treeWith7Node._rootNode.ChildrenList[0].ChildrenList.Count, 2);
            Assert.AreEqual(treeWith7Node._rootNode.ChildrenList[1].ChildrenList.Count, 1);
            Assert.AreEqual(treeWith7Node._rootNode.ChildrenList[2].ChildrenList.Count, 0);

            Assert.AreEqual(treeWith7Node._rootNode.ChildrenList[0], nodeList[1]);

            string strOuput="访问顺序是(";
            treeWith7Node.TranversCuttingTree(treeWith7Node._rootNode, ref strOuput);

            strOuput += ")";


        }
         [TestMethod]
        public void TestMethodCurveContains2()
        {
            List<CuttingTreeNode> nodeList = new List<CuttingTreeNode>();

            for (int i = 0; i < 100; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    CDrawingObjectCircleR circle1 = new CDrawingObjectCircleR(new PointF(i * 100f, i * 100f), (float)(0.01 * j + 1),null);
                    CuttingTreeNode node = new CuttingTreeNode();
                    node.Data = circle1;
                    node.ID = i*10 + j;
                    nodeList.Add(node);
                }
            }

            CCuttingTree testTree = new CCuttingTree();

             testTree.CreateCuttingTree(testTree._rootNode, nodeList);

            string strOuput = "访问顺序是(";
            testTree.TranversCuttingTree(testTree._rootNode, ref strOuput);

            strOuput += ")";


        }
                             
   
    }
}
