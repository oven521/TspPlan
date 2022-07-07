using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MainHMI;


namespace CADEngine.DrawingObject {
    public class CDrawingObjectFonts : CDrawingObjectBase {
        PointF _location;
        string _FontName;
        string _text2Draw = "文字";

        PointF[] _pathPoints;
        byte[] _pathTypes;
        public CDrawingObjectFonts(PointF location, string fontName, string text, CanvasCtrl canvas)
            : base(null, canvas) {
            _location = location;
            _FontName = fontName;
            _text2Draw = text;
            GetPointPath(_text2Draw);
        }
        public override void Draw(Graphics g) {
            for (int i = 0; i + 1 < _pathPoints.Length; ++i) {
                byte type = _pathTypes[i];
                //根据type类型来判断起刀和落刀点！
                //if (type <127)
                //{
                PointF pt = new PointF(_pathPoints[i].X + _location.X, _pathPoints[i].Y + _location.Y);
                PointF ptNext = new PointF(_pathPoints[i + 1].X + _location.X, _pathPoints[i + 1].Y + _location.Y);
                g.DrawLine(GDIDrawMaterails.GreenPen2, pt, ptNext);
                //}


            }
        }

        public void GetPointPath(string text) {
            //     Dim graphics_path1 As New Drawing2D.GraphicsPath  ' Create a GraphicsPath.
            //graphics_path1.AddString(TB_Input.Text, _
            //New FontFamily(FontName), FontStyle, FontSize, _
            //New Point(8, 2), StringFormat.GenericTypographic) ' Screen Start location
            //PlotFlag = False
            //'e.Graphics.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias 'This will smooth out the drawing
            //graphics_path1.CloseAllFigures()
            //'e.Graphics.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
            //e.Graphics.DrawPath(New Pen(Color.Black, 0), graphics_path1) ' Draw the path.
            //'e.Graphics..FillPath(Brushes.Black, graphics_path1)

            //GetPointData(e, graphics_path1.PathPoints, graphics_path1.PathTypes, 0) 'x offset <------- Build the G Code in RichTextBox1
            //RichTextBox1.AppendText("M5" & vbCrLf)
            //RichTextBox1.AppendText("M30" & vbCrLf)
            //If Org = 0 Then
            //    RichTextBox1.Text = Replace(RichTextBox1.Text, "(X/Y Origin:)", "(X/Y Origin: Top Left and " & TB_Xneg.Text & ", " & TB_Yneg.Text & ")") 'ROld, RNew)
            //End If
            //TB_Xsize.Text = Val(TB_Xpos.Text) - Val(TB_Xneg.Text) ' update stats
            //TB_Ysize.Text = Val(TB_Ypos.Text) - Val(TB_Yneg.Text)
            //graphics_path1.Dispose()
            System.Drawing.Drawing2D.GraphicsPath graphics_path1 = new System.Drawing.Drawing2D.GraphicsPath();
            graphics_path1.AddString(text, new FontFamily("宋体"), 0, 125, new Point(0, 0), StringFormat.GenericTypographic);
            graphics_path1.CloseAllFigures();
            _pathPoints = graphics_path1.PathPoints;
            //Bit 6: 1=Last line in chr. - Byte: 0=First line, 1=Stright line, 3=Curve 
            _pathTypes = graphics_path1.PathTypes;

            //下面的代码完整了演示了字的提笔下笔的功能！！！
            //Public Sub GetPointData(ByVal e As PaintEventArgs, ByVal PathPoints() As PointF, ByVal pathtypes() As Byte, ByVal xOfset As Integer)
            //    'Build the G Code in RichTextBox1
            //    GText = "G00"
            //    ProgressBar.Maximum = PathPoints.Length - 1
            //    For i = 0 To PathPoints.Length - 1
            //        If PathPoints(i).IsEmpty = False Then
            //            x = PathPoints(i).X : x = (x / 10) + XOffSet : x = x * Xscale : x = Math.Round(x, 4) 'calc x
            //            y = PathPoints(i).Y : y = (y / 10) + YOffSet : y = y * Yscale : y = Math.Round(y, 4) : y = 0 - y ' calc y
            //            If TB_Xneg.Text = "*" Then TB_Xneg.Text = x
            //            If TB_Xpos.Text = "*" Then TB_Xpos.Text = x
            //            If TB_Yneg.Text = "*" Then TB_Yneg.Text = y
            //            If TB_Ypos.Text = "*" Then TB_Ypos.Text = y ' calc min & max x & y
            //            If x < TB_Xneg.Text Then TB_Xneg.Text = x
            //            If x > TB_Xpos.Text Then TB_Xpos.Text = x
            //            If y < TB_Yneg.Text Then TB_Yneg.Text = y
            //            If y > TB_Ypos.Text Then TB_Ypos.Text = y

            //            ZType = pathtypes(i) 'Bit 6: 1=Last line in chr. - Byte: 0=First line, 1=Stright line, 3=Curve 
            //            If ZType = 0 Then XSave = x : YSave = y 'Type 0 is first line in chr., Save for closure
            //            If ZType < 128 Then z = ZDepth 'Bit 8 off is pen down
            //            If ZType > 127 Then z = ZSafe 'Bit 8 on is pen up


            //            If Feed = CFeed Then RichTextBox1.AppendText(GText & " X" & x & " Y" & y & vbCrLf) ' 1st G00 & All G01's Except last one in Chr.
            //            If Feed = DFeed Then Feed = CFeed : RichTextBox1.AppendText(GText & " X" & x & " Y" & y & " F" & CFeed & vbCrLf)
            //            GText = "G01" : If z > 0 Then GText = "G00" 'Z: <0 = G01, >0 = G00
            //            If ZType > 127 Then RichTextBox1.AppendText("G01" & " X" & XSave & " Y" & YSave & vbCrLf) 'Last G01 line, move to start.
            //            If ZSave <> z And z > 0 Then RichTextBox1.AppendText("G00" & " Z" & z & vbCrLf) 'Z Change, <0 = G01, >0 = G00
            //            If ZSave <> z And z < 0 Then Feed = DFeed : RichTextBox1.AppendText("G01" & " Z" & z & " F" & DFeed & vbCrLf) 'Z Change, <0 = G01, >0 = G00
            //            ZSave = z
            //        End If
            //        ProgressBar.Value = i
            //    Next i
            //    ProgressBar.Value = 0
            //End Sub

        }

    }
}
