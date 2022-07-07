using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using CADEngine.DrawingObject;
using CADEngine.DrawingState;
using CADEngine;
using ContourProgramming;
using ZedGraph;
using _03_DXFMananger;
using System.IO;
using _05_SplineFunction;

namespace MainHMI {
    public partial class MainWin : Form {
        // 绘图部分：
        Rectangle theRectangle = new Rectangle
            (new Point(0, 0), new Size(0, 0));
        Cursor m_CursorOld = Cursors.Arrow;
        IDrawingStateBase drawingState;
        CDrawingStateLine lineState;
        CDrawingStateCircleR circleRState;
        CDrawingStateIdle idleState;
        CDrawingStateRect rectState;
        CDrawingStateAngleArc angleArcState;
        CDrawingStateCircle3P circle3PState;
        CDrawingStateSelect selectState;
        CDrawingStatePolyLine polylineState;
        CDrawingStateText textState;

        List<CDrawingStateBase> stateList = new List<CDrawingStateBase>();

        DrawModel _drawModel;

        static MainWin _Instance = null;

        DxfFileMananger _dxfFileManager;
        public MainWin() {
            InitializeComponent();
            //Text = Program.AppName;
            string[] args = Environment.GetCommandLineArgs();

            splitContainerWhole.Dock = DockStyle.Fill;
            this.rtxtDrawCmd.SelectionAlignment = HorizontalAlignment.Left;


            canvasMain.MouseDown += new MouseEventHandler(canvasMain_MouseDown);
            canvasMain.MouseUp += new MouseEventHandler(canvasMain_MouseUp);
            canvasMain.MouseMove += new MouseEventHandler(canvasMain_MouseMove);
            canvasMain.Location = new Point(0, 0);
            canvasMain.Size = new Size(600, 1024);


            _drawModel = new DrawModel(this.canvasMain);
            _dxfFileManager = new DxfFileMananger("", _drawModel);

            //初始DrawingState
            lineState = new CDrawingStateLine(this.canvasMain);
            circleRState = new CDrawingStateCircleR(this.canvasMain);
            idleState = new CDrawingStateIdle(this.canvasMain);
            rectState = new CDrawingStateRect(this.canvasMain);
            angleArcState = new CDrawingStateAngleArc(this.canvasMain);
            circle3PState = new CDrawingStateCircle3P(this.canvasMain);
            selectState = new CDrawingStateSelect(this.canvasMain);
            polylineState = new CDrawingStatePolyLine(this.canvasMain);
            textState = new CDrawingStateText(this.canvasMain);

            //splitContainerUP.Panel2.Width = 600;
            stateList.Add(lineState);
            stateList.Add(circleRState);
            stateList.Add(idleState);

            stateList.Add(rectState);
            stateList.Add(angleArcState);
            stateList.Add(circle3PState);


            drawingState = idleState;
            this.Cursor = Cursors.Hand;



        }
        static MainWin() {
            _Instance = new MainWin();
        }
        public static MainWin MainWinIns {
            get { return _Instance; }
        }

        void canvasMain_MouseMove(object sender, MouseEventArgs e) {
            drawingState.MouseMove(sender, e);

            PointF mouseLocation = this.canvasMain.SPt2LPt(e.Location);
            tslLogicalPoint.Text = mouseLocation.ToString();
        }

        void canvasMain_MouseUp(object sender, MouseEventArgs e) {
            drawingState.MouseUp(sender, e);
        }

        void canvasMain_MouseDown(object sender, MouseEventArgs e) {
            drawingState.MouseDown(sender, e);
        }
        protected override bool ProcessDialogKey(Keys keyData) {
            drawingState.KeyDown(keyData);
            return base.ProcessDialogKey(keyData);

        }
        private void canvasMain_KeyDown(object sender, KeyEventArgs e) {

        }
        private void canvasMain_KeyPress(object sender, KeyPressEventArgs e) {
            //MessageBox.Show("KeyPress");
        }
        private void buttonMargins_Click(object sender, EventArgs e) {
            CloseCurveGenerator gcodeGenetor = new CloseCurveGenerator();
            gcodeGenetor.GenerateGCodeFile(this._drawModel.DrawingObjectList);
        }

        private void btnImportDxfFile_Click(object sender, EventArgs e) {
            openFileDialog1.InitialDirectory = @"..\CADDxfDrawing";
            openFileDialog1.Filter = "DXF文件|*.dxf|所有文件|*.*";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FilterIndex = 1;

            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                if (openFileDialog1.FileName != "") {
                    _dxfFileManager.ImportDxfFile(openFileDialog1.FileName, this._drawModel);

                }
            }
            this.canvasMain.DrawAndBackupBufferGraphic();
            //导入Dxf文件后，重绘界面。
            this.canvasMain.Invalidate();
        }

        private void btnImportNCFile_Click(object sender, EventArgs e) {
            openFileDialog1.InitialDirectory = @"..\CADDxfDrawing";
            openFileDialog1.Filter = "NC文件|*.nc;*.cnc|所有文件|*.*";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.FilterIndex = 1;

            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                if (openFileDialog1.FileName != "") {
                    DecodeNCCode2DrawingObj ncDoc = new DecodeNCCode2DrawingObj(this.canvasMain);
                    ncDoc.Load(openFileDialog1.FileName);
                }
            }
            this.canvasMain.Invalidate();


        }

        private void MainWin_Load(object sender, EventArgs e) {

        }
        public void AddDrawCmd(string strCmd) {
            this.rtxtDrawCmd.Text += strCmd + Environment.NewLine;

        }

        #region Draw State
        private void btnMove_Click(object sender, EventArgs e) {
            drawingState = this.idleState;
            StateSwith(sender);
        }
        private void btnSelect_Click(object sender, EventArgs e) {
            drawingState = selectState;
            StateSwith(sender);
        }
        private void btnLine_Click(object sender, EventArgs e) {
            drawingState = lineState;

            StateSwith(sender);
        }

        private void btnCircle_Click(object sender, EventArgs e) {
            drawingState = circleRState;
            StateSwith(sender);
        }

        private void btnRect_Click(object sender, EventArgs e) {
            drawingState = rectState;
            StateSwith(sender);
        }
        private void btnArc3p_Click(object sender, EventArgs e) {
            drawingState = angleArcState;
            StateSwith(sender);
        }
        private void btnPolyLine_Click(object sender, EventArgs e) {
            drawingState = polylineState;
            this.rtxtDrawCmd.Text += "指定下一点，【C】切换成圆弧，:" + Environment.NewLine;
            StateSwith(sender);
        }
        private void StateSwith(Object sender) {
            drawingState.Init();
        }
        private void btnFont_Click(object sender, EventArgs e) {
            drawingState = textState;
            StateSwith(sender);
        }

        #endregion

        private void btnScanArc_Click(object sender, EventArgs e) {

        }

        private void btnReverse_Click(object sender, EventArgs e) {
            this._drawModel.ReverseSelectedObjects();
        }

        private void btnMergeLineSegs_Click(object sender, EventArgs e) {
            this._drawModel.MergeSelectedSandaloneLines();
        }

        private void btnBoomLineSegs_Click(object sender, EventArgs e) {
            this._drawModel.BoomPolyLines();
        }

    }
}