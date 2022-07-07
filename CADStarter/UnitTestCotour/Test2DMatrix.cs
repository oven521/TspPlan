using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace UnitTestCotour
{
    [TestClass]
    public class Test2DMatrix
    {

        [TestMethod]
        public void TestMatrixHome2Center()
        {
            float scalePrev = 1.0f;
            Matrix mtx = new Matrix(1, 0, 0, 1, 0, 0);
            //600.0是绘图上的逻辑位置，在1：1的时候，逻辑位置长度和屏幕上的一致。
            mtx.Translate(0, 600.0f);
            mtx.Scale(scalePrev, -scalePrev);

            //先算出(500，500）对应的逻辑点值（500，100）

            //把(50,50)移动到（500，500）
            mtx.Translate(450, 50);

            PointF ptScreen = new PointF(500,500);
            PointF[] logicalpts = new PointF[] { new PointF(50, 50) };

            mtx.TransformPoints(logicalpts);
            Assert.AreEqual<PointF>(ptScreen, logicalpts[0]);

        }

        [TestMethod]
        public void TestMatrixTranslateScaleExt()
        {
            float scalePrev = 2.0f;
            Matrix mtx = new Matrix(1, 0, 0, 1, 0, 0);
            //600.0是绘图上的逻辑位置，在1：1的时候，逻辑位置长度和屏幕上的一致。
            mtx.Translate(0, 500.0f);
            mtx.Scale(scalePrev, -scalePrev);
            mtx.Scale(2, 2);
            mtx.Translate(0, 100 / -(scalePrev*2));

            PointF[] logicalpts= new PointF[]{new PointF(50,50)};
            PointF ptScreen = new PointF(200,400);

            mtx.TransformPoints(logicalpts);
            Assert.AreEqual<PointF>(ptScreen, logicalpts[0]);

            //放大一倍
            //检查逻辑点（50，50），50，50应该是鼠标反算回来的。
            mtx.Translate(50, 50);
            //再放大多少倍，1表示再原来的基础上再加上
            mtx.Scale(2.0f, 2.0f);
            mtx.Translate(-50, -50);

            logicalpts = new PointF[] { new PointF(50, 50) };
            mtx.TransformPoints(logicalpts);
            Assert.AreEqual<PointF>(ptScreen, logicalpts[0]);

            //再放大1.2倍
            PointF[] screenpts = new PointF[] { new PointF(100, 100) };
            //求出逻辑点100，100对应的屏幕点
            mtx.TransformPoints(screenpts);

            mtx.Translate(100, 100);
            //再放大多少倍，1.2表示再原来的基础上再加上
            mtx.Scale(1.5f,1.5f);
            mtx.Translate(-100, -100);

            PointF[] screenpts2 = new PointF[] { new PointF(100, 100) };
            mtx.TransformPoints(screenpts2);
            Assert.AreEqual<PointF>(screenpts[0], screenpts2[0]);



        }
        [TestMethod]
        public void TestMatrixTranslate()
        {
            Matrix mtx = new Matrix(1, 0, 0, -1, 100, 100);
            Matrix invMtx = mtx.Clone();
            invMtx.Invert();

            PointF[] pts= new PointF[]{new PointF(0,0)};
            PointF pt = new PointF(100,100);
            //屏幕上的（0，0）===逻辑上（100，100）
            mtx.TransformPoints(pts);
            Assert.AreEqual<PointF>(pt, pts[0]);

            //屏幕上的（100，100）===逻辑上（0，0）
            pts= new PointF[]{new PointF(100,100)};
            pt = new PointF(0,0);
            invMtx.TransformPoints(pts);
            Assert.AreEqual<PointF>(pt, pts[0]);

        }
          [TestMethod]
        public void TestMatrixScaleTranslate()
        {
            Matrix mtx = new Matrix(0.5f, 0, 0, -0.5f, 100, 100);
           // mtx.Scale(0.5f, 0.5f, MatrixOrder.Append);

            Matrix mtxScaleTranslate = new Matrix(1, 0, 0, -1, 0, 0);
            mtxScaleTranslate.Scale(0.5f, 0.5f, MatrixOrder.Append);
            mtxScaleTranslate.Translate(100, 100, MatrixOrder.Append);
            Assert.AreEqual<Matrix>(mtx, mtxScaleTranslate);
            
            //逻辑上（10，10）==> 屏幕上（105，95）
            PointF[] pts= new PointF[]{new PointF(10,10)};
            PointF pt = new PointF(105,95);
            mtxScaleTranslate.TransformPoints(pts);
            Assert.AreEqual<PointF>(pt, pts[0]);

            
              
            //Matrix mtxTranslateScale= new Matrix(1, 0, 0, -1, 0, 0);
            //mtxTranslateScale.Translate(100, 100, MatrixOrder.Append);
            //mtxTranslateScale.Scale(0.5f, 0.5f, MatrixOrder.Append);
            //Assert.AreNotEqual<Matrix>(mtx, mtxTranslateScale);

        }

        [TestMethod]
        public void TestMatrixPointTranslate()
        {

            Matrix mtx = new Matrix(1,0,0,-1,0,600);

            //逻辑(0,0)--->屏幕(0,600)
            PointF[] pts = new PointF[] { new PointF(0, 0) };
            mtx.TransformPoints(pts);
            PointF pt = new PointF(0,600);
            Assert.AreEqual<PointF>(pt, pts[0]);
            //逻辑(10，10)-->屏幕(10,590)
            pts = new PointF[] { new PointF(10, 10) };
            mtx.TransformPoints(pts);
            pt = new PointF(10, 590);
            Assert.AreEqual<PointF>(pt, pts[0]);

            //x,y方向再平移50，50,用Append方式
            mtx.Translate(50, 50,MatrixOrder.Append);
            pts = new PointF[] { new PointF(10, 10) };
            mtx.TransformPoints(pts);
            //屏幕(10,10)--->(60,640) 因为y轴为负，
            pt = new PointF(60, 640);
            Assert.AreEqual<PointF>(pt, pts[0]);

            Assert.AreEqual<float>(50, mtx.OffsetX);
            Assert.AreEqual<float>(650, mtx.OffsetY);

            Matrix invertMtx = mtx.Clone();
            invertMtx.Invert();
            //整个坐标系相当与平移了X方向为50，Y方向600+50，
            //所以屏幕上的（50，650）-->逻辑的（0，0）
            pts = new PointF[] { new PointF(50, 650) };
            invertMtx.TransformPoints(pts);
            pt = new PointF(0,0);
            Assert.AreEqual<PointF>(pt, pts[0]);

        }
    }
}
