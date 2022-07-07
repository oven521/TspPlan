using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CADEngine;
using System.Drawing;

namespace UnitTestCotour
{
    [TestClass]
    public class TestGeometryLine
    {
        [TestMethod]
        public void TestMethodFootOnLine()
        {
            GLineSeg seg= new GLineSeg(new PointF(0, 0), new PointF(10, 10));
            PointF p = new PointF(10,0);
            PointF foot =new PointF();
            bool isFootOnSeg = false;

            double distance = CGeometryLine.ptolinesegdist(p,seg,ref foot,out isFootOnSeg);
            Assert.AreEqual(isFootOnSeg, true);
            Assert.AreEqual(Math.Abs(distance-7.07)<1E-2,true);

            p = new PointF(30,0);
            distance = CGeometryLine.ptolinesegdist(p, seg, ref foot, out isFootOnSeg);

            Assert.AreEqual(isFootOnSeg, false);
        }
    }
}
