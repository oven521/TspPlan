using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using netDxf;
using netDxf.Header;
using netDxf.Objects;
using netDxf.Collections;
using netDxf.Entities;
using CADEngine.DrawingObject;
using System.Drawing.Drawing2D;

namespace MainHMI {
    public class GDIDrawMaterails {
        private static Pen _greenPen2;
        private static Pen _yellowPen;
        private static Pen _greenDashPen;
        private static Pen _yellowDashPen;
        private static Pen _gridPen;
        private static Pen _axisPen;
        private static Pen _0GridPen;
        private static Pen _unClosedCurvePen;

        static GDIDrawMaterails() {
            _greenPen2 = new Pen(Color.Green, 2);
            _yellowPen = new Pen(Color.Green, 2);
            _greenDashPen = new Pen(Color.Green, 2);
            _greenDashPen.DashStyle = DashStyle.Dash;

            _yellowDashPen = new Pen(Color.Yellow, 0.2F);
            _yellowDashPen.DashStyle = DashStyle.Dash;
            _gridPen = new Pen(Color.Black, 0.2F);
            _gridPen.DashStyle = DashStyle.Dash;
            _axisPen = new Pen(Color.Orange, 1.0F);
            _0GridPen = new Pen(Color.Orange, 1.0F);
            _unClosedCurvePen = new Pen(Color.Red, 0.2F);

        }
        public static Pen UnClosedCurvePen {
            get { return _unClosedCurvePen; }
        }
        public static Pen GreenPen2 {
            get { return _greenPen2; }
        }
        public static Pen YelloPen {
            get { return _yellowPen; }
        }
        public static Pen GreenDashPen {
            get { return _greenDashPen; }
        }
        public static Pen GridPen {
            get { return _gridPen; }
        }
        public static Pen YellowDashPen {
            get { return _yellowDashPen; }
        }
        public static Pen AxisPen {
            get { return _axisPen; }
        }
        public static Pen ZeroGridPen {
            get { return _0GridPen; }
        }

    }
}
