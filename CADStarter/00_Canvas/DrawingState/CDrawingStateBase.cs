using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using CADEngine.DrawingObject;
using System.Runtime.InteropServices;
using CADEngine;
using MainHMI;

namespace CADEngine.DrawingState
{
   public  interface IDrawingStateBase
    {
        void MouseDown(object sender, MouseEventArgs e);
        void MouseUp(object sender, MouseEventArgs e);
        void MouseMove(object sender, MouseEventArgs e);
        void KeyDown(Keys keyData);
        void Init();
    }

    public class CDrawingStateBase : CDrawingObjectBase
    {
        public CanvasCtrl m_objCanvas;
        protected int step;

        private DrawModel _Model;

        public DrawModel DrawModel
        {
            set { _Model = value; }
            get { return _Model; }
        }

        public CDrawingStateBase(CanvasCtrl canvas)
            : base(canvas)
        {
            this.DrawModel = canvas.DrawModel;
        }

        public virtual void DecodeCmd(string strCmd)
        {

        }
    }

   
}
