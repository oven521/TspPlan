namespace MainHMI
{
	partial class CanvasCtrl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.SuspendLayout();
            // 
            // CanvasCtrl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Name = "CanvasCtrl";
            this.Size = new System.Drawing.Size(841, 498);
            this.SizeChanged += new System.EventHandler(this.CanvasCtrl_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CanvasCtrl_Paint);
            this.MouseEnter += new System.EventHandler(this.CanvasCtrl_MouseEnter);
            this.ResumeLayout(false);

		}

		#endregion


    }
}
