using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _05_SplineFunction;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace UnitTestCotour
{
    [TestClass]
    public class TestSplin3
    {
        [TestMethod]
        public void TestSplinefunction()
        {
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();
            InitData(xList, yList);

            Assert.IsTrue(xList.Count == 21);
            Assert.IsTrue(yList.Count == 21);

            //第一个点和最后一点的一阶导数为0
            Spline3 splineCublic = new Spline3(xList.ToArray(), yList.ToArray(), 0, 0);
            splineCublic.CubicSplineCalc();

            Assert.IsTrue(splineCublic.Velocity.Length ==22 );
            Assert.IsTrue(splineCublic.Accelation.Length == 22);

            Assert.IsTrue(Math.Abs(splineCublic.Velocity[1])<10e-6);
            Assert.IsTrue(Math.Abs(splineCublic.Accelation[1] - 0.0192855472) < 10e-6);

            Assert.IsTrue(Math.Abs(splineCublic.Velocity[3] - 0.4557184444) < 10e-6);
            Assert.IsTrue(Math.Abs(splineCublic.Accelation[3] - (-0.0039757931)) < 10e-6);

            Assert.IsTrue(Math.Abs(splineCublic.Velocity[10] - 0.1088398337) < 10e-6);
            Assert.IsTrue(Math.Abs(splineCublic.Accelation[10] - 0.0005453690) < 10e-6);


            Assert.IsTrue(Math.Abs(splineCublic.Velocity[21]) < 10e-6);
            Assert.IsTrue(Math.Abs(splineCublic.Accelation[21] - 0.028459648595453) < 10e-6);
        }
        [TestMethod]
        public void TestSplinefunction2()
        {
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();
            InitData(xList, yList);

            Assert.IsTrue(xList.Count == 21);
            Assert.IsTrue(yList.Count == 21);

            //第一个点和最后一点的一阶导数为0
            Spline3 splineCublic = new Spline3(xList.ToArray(), yList.ToArray(), 0, 0);
            splineCublic.CubicSplineCalc();

            double value = splineCublic.GetYPos(12.4627542496);
            Assert.IsTrue(Math.Abs(2.0137946864 - value)<1e-6);

            value = splineCublic.GetYPos(337.4948730469);
            Assert.IsTrue(Math.Abs(8.4929748520 - value) < 1e-6);

        }

        private void InitData( List<double> xList,List<double> yList )
        {
            xList.Add(0);
            xList.Add(12.4627542496);
            xList.Add(17.6343784332);
            xList.Add(22.7136592865);
            xList.Add(34.5333175659);
            xList.Add(46.1919555664);
            xList.Add(55.4953575134);
            xList.Add(64.7636718750);
            xList.Add(104.8250885010);
            xList.Add(143.8855743408);
            xList.Add(176.2502746582);
            xList.Add(207.6701965332);
            xList.Add(225.8615570068);
            xList.Add(243.7451324463);
            xList.Add(274.8629760742);
            xList.Add(306.3013916016);
            xList.Add(315.5754089355);
            xList.Add(324.9129333496);
            xList.Add(335.3691101074);
            xList.Add(337.4948730469);
            xList.Add(360);

            yList.Add(0);
            yList.Add(2.0137946864);
            yList.Add(4.2312098449);
            yList.Add(6.4813861383);
            yList.Add(10.7290609118);
            yList.Add(12.6662918025);
            yList.Add(13.2360700717);
            yList.Add(13.7237701603);
            yList.Add(16.6444008554);
            yList.Add(20.4707565552);
            yList.Add(24.3752476286);
            yList.Add(28.9170786378);
            yList.Add(31.5861616691);
            yList.Add(34.1952363888);
            yList.Add(35.4830706202);
            yList.Add(30.5938071532);
            yList.Add(26.7214089800);
            yList.Add(19.6195891463);
            yList.Add(10.2935103470);
            yList.Add(8.4929748520);
            yList.Add(0);          
        }
    }
}
