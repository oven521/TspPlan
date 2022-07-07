using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CADEngine;
using System.Drawing;

namespace UnitTestCotour
{
    /// <summary>
    /// UnitTest3 的摘要说明
    /// </summary>
    [TestClass]
    public class TestGeometry
    {
        public TestGeometry()
        {
            //
            //TODO:  在此处添加构造函数逻辑
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，该上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        //
        // 编写测试时，可以使用以下附加特性: 
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前，使用 TestInitialize 来运行代码
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在每个测试运行完之后，使用 TestCleanup 来运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethodMakeline()
        {
            PointF startPoint = new PointF(0, 0);
            PointF endPoint = new PointF(10,10);
            
            //计算出曲线的解析方程
            GLine lineObj = CGeometry.MakeALine(startPoint, endPoint);
            GLineSeg lineSeg = new GLineSeg(startPoint, endPoint);
            Assert.AreEqual(lineObj.a, 10);
            Assert.AreEqual(lineObj.b, -10);
            Assert.AreEqual(lineObj.c,0);

            ////计算斜率
            //double slopeObj = CGeometry.CalcSlope(lineObj);
            //Assert.AreEqual(slopeObj,1);

            //注意算出的角度为弧度
            double angle = CGeometry.SlopeAngleHudu(lineSeg);
            double PI_4 = CGeometry.PI / 4;
            double diff = angle - PI_4;
            if (Math.Abs(diff) < 1e-6)
                diff = 0.0;
            Assert.AreEqual(diff,0);
            


        }
        [TestMethod]
        public void TestMethodMakeArrowLine()
        {
            PointF startPoint = new PointF(0, 0);
            PointF endPoint = new PointF(0, 10);

            double cosValue = Math.Cos(CGeometry.PI / 2);
            double sinValue = Math.Sin(CGeometry.PI / 2);

            //计算出曲线的解析方程
            GLine lineObj = CGeometry.MakeALine(startPoint, endPoint);

            GLineSeg segOriginal = new GLineSeg(startPoint,endPoint);
            //以endPoint为圆心，旋转startPoint，逆时针旋转的角度为PI/2,旋转半径为20
            GLineSeg arrowSeg1 = CGeometry.CalcArrowLine(segOriginal, 20, CGeometry.PI / 2);
            //以endPoint为圆心，旋转startPoint，逆时针旋转的角度为-PI/2,旋转半径为20
            GLineSeg arrowSeg2 = CGeometry.CalcArrowLine(segOriginal, 20, -CGeometry.PI / 2);

            //逆时针旋转后， arrowSeg1为 (10,10) ->>> (0,10)
            Assert.AreEqual(arrowSeg1.s.X, 20);
            Assert.AreEqual(arrowSeg1.s.Y, 10);
            Assert.AreEqual(arrowSeg1.e.X, 0);
            Assert.AreEqual(arrowSeg1.e.Y, 10);

            //顺时针旋转后， arrowSeg2为 (-10,10) ->>> (0,10)
            Assert.AreEqual(arrowSeg2.s.X, -20);
            Assert.AreEqual(arrowSeg2.s.Y, 10);
            Assert.AreEqual(arrowSeg2.e.X, 0);
            Assert.AreEqual(arrowSeg2.e.Y, 10);

        }
        [TestMethod]
        public void TestCircleCircleIntersect()
        {
            PointF center1 = new PointF(100, 0);
            PointF center2 = new PointF(0, 100);
            double r = 100;
              
            //计算出曲线的解析方程
            double x1 = 99.99;
            double y1 = 99.99;
            double x2 = 99.99;
            double y2 = 99.99;
            int result = CGeometryCircle.circle_circle_intersection(center1, r, center2, r,
                ref x1, ref y1, ref x2, ref y2);

            Assert.AreEqual(result, 1);
            Assert.AreEqual(x1, 0);
            Assert.AreEqual(y1, 0);
            Assert.AreEqual(x2, 100);
            Assert.AreEqual(y2,100);
            
        }
        [TestMethod]
        public void TestGetCenterofBulge()
        {
            PointF center1 = new PointF(100, 0);
            PointF center2 = new PointF(0, 100);

            double bulge = Math.Tan(Math.PI / 8);
            PointF center = CGeometryCircle.GetCenterofBulge(center1,center2,bulge);

            Assert.IsTrue(Math.Abs(center.X-0)<1E-10);
            Assert.IsTrue(Math.Abs(center.Y-0)<1E-10);


        }
    }
}
