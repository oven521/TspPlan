using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MainHMI
{
	static class Program
	{
		public static int TracePaint = 1;
        public static string AppName = "LaserMaster CAD";
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new MainWin());
		}
	}
}