using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CADEngine;
using System.Drawing;
using CADEngine.DrawingObject;

namespace UnitTestCotour
{
    [TestClass]
    public class TestObjArcLine
    {
        [TestMethod]
        public void TestLine2Arc()
        {
            PointF start = new PointF(10,0);
            PointF end = new PointF(-10,0);
            
            CDrawingObjectSingleLine line = new CDrawingObjectSingleLine(start, end,0, null);
   

        }
        [TestMethod]
        public void TestArc2Line()
        {

        }
    }
}
