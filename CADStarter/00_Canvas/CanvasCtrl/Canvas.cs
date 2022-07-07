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

    public partial class CanvasCtrl : UserControl {
        enum eCommandType {
            select,
            pan,
            move,
            draw,
            edit,
            editNode,
        }
        CursorCollection m_cursors = new CursorCollection();
        bool m_runningSnaps = true;
        Type[] m_runningSnapTypes = null;
        string m_drawObjectId = string.Empty;
        string m_editToolId = string.Empty;
        //������
        private float m_deviceOffsetX = 100;//��Ļ��ԭʼ���X��λ��
        private float m_deviceOffsetY = 600;////��Ļ��ԭʼ���Y��λ��
        private float _zoomScale = 1;
        private PointF m_mouseDownPos = new PointF(0, 0);
        private PointF m_mouseUpPos = new PointF(0, 0);
        public PointF[] m_Points;
        public PointF m_lastPoint;
        public PointF m_currentPoint;
        private Pen pen;
        PointF ptOnMouseZoomONScreen = new PointF(0, 0);

        public float ArrowLength = 15;
        private DrawModel _Model;

        Matrix _matrixLeftBottom = new Matrix(1, 0, 0, 1, 0, 0);
        Matrix _matrixCenter = new Matrix(1, 0, 0, 1, 0, 0);


        public BufferedGraphicsContext context;
        public BufferedGraphics bufferedGraphic;
        public BufferedGraphicsContext context2;
        public BufferedGraphics bufferedGraphicPrev;

        List<CDrawingObjectBase> _listRealTimeDrawing = new List<CDrawingObjectBase>();

        public DrawModel DrawModel {
            set { _Model = value; }
            get { return _Model; }
        }
        public Pen DrawPen {
            get { return pen; }
            set { pen = value; }
        }
        //�����϶������ǰѶ�Ӧ��Offset�������ƫ���ϣ�����
        public void AddDeviceOffsetXY(float offsetX, float offsetY) {
            this.m_deviceOffsetX += offsetX;
            this.m_deviceOffsetY += offsetY;

            this._matrixLeftBottom = GetTransformMatrtix();

            this.DrawAndBackupBufferGraphic();
            this.Invalidate();
        }
        public PointF ScreenCenter {
            get { return new PointF(m_deviceOffsetX, m_deviceOffsetY); }
        }
        public PointF CurrentPoint {
            get {
                return m_currentPoint;
            }
            set {
                m_currentPoint = value;
            }
        }
        public PointF LastPoint {
            get {
                return m_lastPoint;
            }
            set {
                m_lastPoint = value;
            }
        }

        public Type[] RunningSnaps {
            get { return m_runningSnapTypes; }
            set { m_runningSnapTypes = value; }
        }
        public bool RunningSnapsEnabled {
            get { return m_runningSnaps; }
            set { m_runningSnaps = value; }
        }

        System.Drawing.Drawing2D.SmoothingMode m_smoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        public System.Drawing.Drawing2D.SmoothingMode SmoothingMode {
            get { return m_smoothingMode; }
            set { m_smoothingMode = value; }
        }

        public CanvasCtrl() {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, false);//��֪��������ɶ��˼��
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            m_cursors.AddCursor(eCommandType.select, Cursors.Arrow);
            m_cursors.AddCursor(eCommandType.draw, Cursors.Cross);
            m_cursors.AddCursor(eCommandType.pan, "hmove.cur");
            m_cursors.AddCursor(eCommandType.move, Cursors.SizeAll);
            m_cursors.AddCursor(eCommandType.edit, Cursors.Cross);
            //
            m_Points = new PointF[5];

            RecreateBuffers();
        }
        private void CanvasCtrl_SizeChanged(object sender, EventArgs e) {
            this._matrixLeftBottom = GetTransformMatrtix();

            RecreateBuffers();

            DrawAndBackupBufferGraphic();
        }
        public void RecreateBuffers() {

            context = BufferedGraphicsManager.Current;
            context.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);

            if (bufferedGraphic != null)
                bufferedGraphic.Dispose();

            if (bufferedGraphicPrev != null)
                bufferedGraphicPrev.Dispose();

            bufferedGraphic = context.Allocate(this.CreateGraphics(),
                 new Rectangle(0, 0, this.Width, this.Height));
            bufferedGraphicPrev = context.Allocate(this.CreateGraphics(),
           new Rectangle(0, 0, this.Width, this.Height));
        }
        public void DrawAndBackupBufferGraphic() {

            if (_Model == null || bufferedGraphic == null)
                return;
            Graphics g = this.bufferedGraphic.Graphics;
            ResetTransform(g);
            g.FillRectangle(Brushes.DarkSlateGray, new Rectangle(0, 0, this.Width, this.Height));
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.Transform = _matrixLeftBottom;
            //��ͼ�����ķŴ����С
            //Transform2Center(g);
            foreach (CDrawingObjectBase obj in this._Model.DrawObjectList) {
                obj.Draw(g);
            }

            ResetTransform(g);
            DrawAxes(bufferedGraphic);
            g.Transform = _matrixLeftBottom;

            bufferedGraphic.Render(bufferedGraphicPrev.Graphics);
        }
        public void RestoreBufferGraphic() {
            bufferedGraphicPrev.Render(bufferedGraphic.Graphics);
        }
        public Graphics BufferGrahicHandle {
            get { return bufferedGraphic.Graphics; }
        }

        public void DrawAxes(BufferedGraphics bgfx) {
            DrawXAxisEX(bgfx.Graphics);
            DrawYAxisEX(bgfx.Graphics);

            bgfx.Graphics.DrawLine(GDIDrawMaterails.ZeroGridPen, m_deviceOffsetX, 0, m_deviceOffsetX, 2000);
            bgfx.Graphics.DrawLine(GDIDrawMaterails.ZeroGridPen, 0, m_deviceOffsetY, 2000, m_deviceOffsetY);
        }

        public double ZoomScale {
            get { return _zoomScale; }
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            //�������д���ǳ���Ҫ��һ��Ҳ���ܶ��������� Control.MousePosition
            //Control.MousePosition���ص��������Canvas���λ�ã��ǳ���Ҫ��һ�δ��롣
            PointF point = this.PointToClient(Control.MousePosition);
            ptOnMouseZoomONScreen = point;
            //PointF point = e.Location;
            float wheeldeltatick = 120;
            //���ÿ���ƶ������Ƕ�120tick
            float zoomdelta = (1.25f * (Math.Abs(e.Delta) / wheeldeltatick));

            float zoomScalePrev = _zoomScale;
            //��ƽ�Ƶ�(deviceOffsetX,deviceOffsetY)���ٽ�ͼ�Ŵ�
            //ƽ�Ƶ��ĵ�(deviceOffsetX,deviceOffsetY)����ͼ���߼�0��
            Matrix matrixScale = new Matrix(zoomScalePrev, 0.0f, 0.0f, -zoomScalePrev, m_deviceOffsetX, m_deviceOffsetY);
            matrixScale.Invert();
            PointF[] pts = new PointF[] { ptOnMouseZoomONScreen };
            matrixScale.TransformPoints(pts);
            PointF logicPointOfMouseUnderPreScale = pts[0];

            if (e.Delta < 0) {
                _zoomScale /= zoomdelta;
            }
            else {
                _zoomScale *= zoomdelta;
            }
            if (_zoomScale > 20) {
                _zoomScale = 20;
            }
            else if (_zoomScale < 0.05) {
                _zoomScale = 0.05F;
            }

            _zoomScale = (float)Math.Round(_zoomScale, 2);
            ChangePenWith();

            _matrixLeftBottom = Transform2LogicCorOnMouseWheel(zoomScalePrev, _zoomScale, logicPointOfMouseUnderPreScale);
            DrawAndBackupBufferGraphic();
            this.Invalidate();
        }
        public Matrix Transform2Center(Graphics g) {
            Matrix matrixScale = new Matrix(_zoomScale, 0.0f, 0.0f, -_zoomScale, m_deviceOffsetX, m_deviceOffsetY);
            Matrix matrixScaleClone = matrixScale.Clone();
            matrixScaleClone.Invert();
            //�������Ļ���������߼��ϵ�λ��,������Ļ����Ϊ��600��400��
            PointF[] pts = new PointF[] { new PointF(600, 400) };
            matrixScaleClone.TransformPoints(pts);
            PointF ptLogicalCenter = pts[0];
            matrixScale.Translate(ptLogicalCenter.X - 415, ptLogicalCenter.Y - 175);

            //�����µ�offset
            m_deviceOffsetX = matrixScale.OffsetX;
            m_deviceOffsetY = matrixScale.OffsetY;

            return matrixScale;
        }
        public Matrix GetTransformMatrtix() {
            Matrix matrixScale = new Matrix(_zoomScale, 0.0f, 0.0f, -_zoomScale, m_deviceOffsetX, m_deviceOffsetY);
            return matrixScale;
        }
        public Matrix Transform2LogicCorOnMouseWheel(float scalePrev, float scaleNew, PointF ptMousePositionPrev) {
            Matrix matrixScale = new Matrix(scalePrev, 0.0f, 0.0f, -scalePrev, m_deviceOffsetX, m_deviceOffsetY);
            matrixScale.Translate(ptMousePositionPrev.X, ptMousePositionPrev.Y);
            matrixScale.Scale(scaleNew / scalePrev, scaleNew / scalePrev);
            matrixScale.Translate(-ptMousePositionPrev.X, -ptMousePositionPrev.Y);
            //�����µ�offset
            m_deviceOffsetX = matrixScale.OffsetX;
            m_deviceOffsetY = matrixScale.OffsetY;

            return matrixScale;
        }
        public void ResetTransform(Graphics g) {
            g.ResetTransform();
        }
        private void CanvasCtrl_Paint(object sender, PaintEventArgs e) {
            if (_Model == null)
                return;

            Graphics g = e.Graphics;
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            //ֱ�Ӱ�buffer���ڴ渴�ƹ���
            bufferedGraphic.Render(g);
        }
        private void ChangePenWith() {
            GDIDrawMaterails.GreenPen2.Width = 1 / _zoomScale;
            GDIDrawMaterails.GreenDashPen.Width = GDIDrawMaterails.GreenPen2.Width;
            //��ͷ�ĳ��ȸ��ݷŴ����Զ�����
            ArrowLength = 15 / _zoomScale + 1;
        }
        /// <summary>
        /// ����Ļ�ϵ�ת�����ߵ��XPOS
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public float LPt2SptOnXPos(float logicalXPos) {
            return m_deviceOffsetX + logicalXPos * _zoomScale;
        }
        /// <summary>
        /// ����Ļ�ϵ��߼���Posת����Screen��XPOS
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public float SPt2LPtOnXPos(float screenXPos) {
            return (screenXPos - m_deviceOffsetX) / _zoomScale;
        }
        /// <summary>
        /// ����Ļ�ϵĵ�ת�����߼���
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public PointF SPt2LPt(PointF screenPoint) {
            PointF logicPoint = new PointF();
            logicPoint.X = (screenPoint.X - m_deviceOffsetX) / this._zoomScale;
            logicPoint.Y = -1 * (screenPoint.Y - m_deviceOffsetY) / _zoomScale;
            return logicPoint;
        }
        /// <summary>
        /// ���߼���������Ļ�ϵĵ�
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public PointF LPt2SPt(PointF logicPoint) {
            PointF screenPoint = new PointF();
            screenPoint.X = logicPoint.X * _zoomScale + m_deviceOffsetX;
            screenPoint.Y = m_deviceOffsetY - logicPoint.Y * _zoomScale;

            return screenPoint;
        }
        /// <summary>
        /// ����Ļ�ϵ�ת�����ߵ��YPOS,���������ϵ����ת������µģ�ע������������֣����������Ҫ��
        /// ��grid�õģ���������ϵ����ת��ע����X�����֣�X����û�з�ת������ġ�
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public float LPt2SptOnYPosNotInvert(float logicalYPos) {
            return m_deviceOffsetY - logicalYPos * _zoomScale;
        }
        private void CanvasCtrl_MouseEnter(object sender, EventArgs e) {
            this.Focus();
        }
        private int GetGridIntervalBaseOnZoomScale(float zs) {
            int gridLogicalInterval = 100;
            if (zs < 0.05) {
                gridLogicalInterval = 1000;
            }
            else if (zs >= 0.05 && zs < 0.1) {
                gridLogicalInterval = 200;
            }
            else if (zs >= 0.2 && zs < 0.5) {
                gridLogicalInterval = 50;
            }
            else if (zs >= 0.5 && zs <= 1) {
                gridLogicalInterval = 20;
            }
            else if (zs >= 1 && zs < 1.5) {
                gridLogicalInterval = 10;
            }
            else if (zs >= 1.5 && zs < 4) {
                gridLogicalInterval = 5;
            }
            else if (zs >= 4) {
                gridLogicalInterval = 2;
            }
            return gridLogicalInterval;
        }
        private void DrawXAxisEX(Graphics g) {
            int gridLogicalInterval = GetGridIntervalBaseOnZoomScale(this._zoomScale);
            PointF SPtScreenStart = new PointF(this.Left + 20, this.Top);
            PointF LPtLogicalStart = SPt2LPt(SPtScreenStart);
            //����ȡ���ҵ���һ��������
            LPtLogicalStart.X = (int)(LPtLogicalStart.X / gridLogicalInterval) * gridLogicalInterval;
            //�ҵ���һ�������ߵ���Ļ����
            PointF SPtFirstGrid = this.LPt2SPt(LPtLogicalStart);
            //�ҵ����һ�������ߵ���Ļ����
            float SPLastGridXPos = SPtFirstGrid.X + this.Width;
            //��ʼѭ������,���߼���㿪ʼ��ÿ�������Ļ��grid��λ��
            int LPtGridXPos = (int)LPtLogicalStart.X;
            float SPtGridXpos;
            do {
                SPtGridXpos = this.LPt2SptOnXPos(LPtGridXPos);
                int pitchLen = 5;

                if (LPtGridXPos % (5 * gridLogicalInterval) == 0) {
                    g.DrawString(LPtGridXPos.ToString("F0"), this.Font, Brushes.OrangeRed, (int)SPtGridXpos, pitchLen);
                    g.DrawLine(GDIDrawMaterails.GridPen, (int)SPtGridXpos, 0, (int)SPtGridXpos, 2000);
                    pitchLen = 15;
                }

                g.DrawLine(GDIDrawMaterails.AxisPen, (int)SPtGridXpos, 0, (int)SPtGridXpos, pitchLen);
                LPtGridXPos += (int)gridLogicalInterval;
            } while (SPtGridXpos <= SPLastGridXPos);

        }

        private void DrawYAxisEX(Graphics g) {
            int gridLogicalInterval = GetGridIntervalBaseOnZoomScale(this._zoomScale);
            PointF SPtScreenStart = new PointF(0, 10);
            PointF LPtLogicalStart = SPt2LPt(SPtScreenStart);
            //����ȡ���ҵ���һ��������
            LPtLogicalStart.Y = (int)(LPtLogicalStart.Y / gridLogicalInterval) * gridLogicalInterval;
            //�ҵ���һ�������ߵ���Ļ����
            PointF SPtFirstGrid = this.LPt2SPt(LPtLogicalStart);
            //�ҵ����һ�������ߵ���Ļ����
            float SPLastGridYPos = SPtFirstGrid.Y + this.Height;
            //��ʼѭ������,���߼���㿪ʼ��ÿ�������Ļ��grid��λ��
            int LPtGridYPos = (int)LPtLogicalStart.Y;
            float SPtGridYpos;
            do {
                SPtGridYpos = this.LPt2SptOnYPosNotInvert(LPtGridYPos);
                int pitchLen = 5;

                if (LPtGridYPos % (5 * gridLogicalInterval) == 0) {
                    g.DrawString(LPtGridYPos.ToString("F0"), this.Font, Brushes.OrangeRed, 10, (int)SPtGridYpos);
                    g.DrawLine(GDIDrawMaterails.GridPen, 0, (int)SPtGridYpos, 2000, (int)SPtGridYpos);
                    pitchLen = 15;
                }
                g.DrawLine(GDIDrawMaterails.AxisPen, 0, (int)SPtGridYpos, pitchLen, (int)SPtGridYpos);
                LPtGridYPos -= (int)gridLogicalInterval;
            } while (SPtGridYpos <= SPLastGridYPos);

        }
    }
}
