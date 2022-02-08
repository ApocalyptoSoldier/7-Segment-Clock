using System;
using System.Drawing;
using System.Windows.Forms;

namespace Clock
{
	public class MainForm : Form
	{	
		private bool IsResizing = false;
		
		private int pW = 10;
		private int pH = 10;

		private const int Interval = 100; // 100 milliseconds shouldn't cause too many missed ticks

		// a is top
		// b is top right
		// c bottom right
		// d is bottom
		// e is bottom left 
		// f is top left
		// g is middle
		// h is top center vertical (not in a typical 7 segment display, just for the semicolon)
		// i is bottom center vertical (ditto)
		private char[][] Bars = new char[][]
		{
			new char[] { 'a', 'b', 'c', 'd', 'e', 'f' }, 	  	// 0
			new char[] { 'b', 'c' },						  	// 1
			new char[] { 'a', 'b', 'd', 'e', 'g' },          	// 2
			new char[] { 'a', 'b', 'c', 'd', 'g' },          	// 3
			new char[] { 'b', 'c', 'f', 'g' },               	// 4
			new char[] { 'a', 'c', 'd', 'f', 'g' },          	// 5
			new char[] { 'a', 'c', 'd', 'e', 'f', 'g' },     	// 6
			new char[] { 'a', 'b', 'c' },                    	// 7
			new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g' },	// 8
			new char[] { 'a', 'b', 'c', 'd', 'f', 'g'  },    	// 9
			new char[] { 'h', 'i' }								// :
		};
		
		const int semicolon = 10;

		private Color BGColor = Color.Black;
		private Color FGColor = Color.Green;

		public MainForm()
		{
			this.Icon = new Icon(typeof(MainForm).Assembly.GetManifestResourceStream("icon.ico"));
			
			this.FormBorderStyle = FormBorderStyle.None;
			
			this.BackColor = BGColor;
			this.ForeColor = FGColor;

			//this.AutoSize = true;
			
			this.Width 		 = 550;
			this.Height 	 = 120;
			this.MinimumSize = new Size(220, 60);

			Timer t 	= new Timer();
			t.Interval 	= Interval;
			t.Tick		+= new EventHandler(On_Tick);
			t.Start();
		}
		
         [STAThread]
         static void Main()
         {
             Application.EnableVisualStyles();
			 Application.SetCompatibleTextRenderingDefault(false);
             Application.Run(new MainForm());
         }

		private void On_Tick(object sender, EventArgs e)
		{
			this.Text = DateTime.Now.ToString("hh:mm:ss");
			// Try not to mess with the size if the user is still busy resizing
			if (IsResizing)
				return;
			
			pW = (int)(Math.Floor((double)(this.Width / 55)));
			pH = (int)(Math.Floor((double)(this.Height / 12)));

			// Resize to maintain aspect ratio
			this.Width 	= pW * 55;
			this.Height = pH * 12;

			Graphics formGraphics = this.CreateGraphics();

			// First clear the entire screen
			SolidBrush cls = new SolidBrush(BGColor);
			formGraphics.FillRectangle(cls, new Rectangle(0, 0, this.Width, this.Height));
			cls.Dispose();

			// Break the current time into individual digits because one display can only display one digit
			int[] displayableTime = GetDisplayableTime();

			int h1 = displayableTime[0];
			int h2 = displayableTime[1];
			int m1 = displayableTime[2];
			int m2 = displayableTime[3];
			int s1 = displayableTime[4];
			int s2 = displayableTime[5];
			
			// Then draw the new digits
			// Offet is pixel width * 7 (6 for the digit and 1 for space)
			DrawShape(formGraphics, Bars[h1],			0);
			DrawShape(formGraphics, Bars[h2], 			pW * 7 * 1);
			DrawShape(formGraphics, Bars[semicolon],	pW * 7 * 2);
			DrawShape(formGraphics, Bars[m1], 			pW * 7 * 3);
			DrawShape(formGraphics, Bars[m2], 			pW * 7 * 4);
			DrawShape(formGraphics, Bars[semicolon], 	pW * 7 * 5);
			DrawShape(formGraphics, Bars[s1], 			pW * 7 * 6);
			DrawShape(formGraphics, Bars[s2], 			pW * 7 * 7);

			formGraphics.Dispose();
		}

		private int[] GetDisplayableTime()
		{
			DateTime now = DateTime.Now;

			int h1 = (int)Math.Floor((double)(now.Hour / 10));
			int h2 = now.Hour % 10;

			int m1 = (int)Math.Floor((double)(now.Minute / 10));
			int m2 = now.Minute % 10;

			int s1 = (int)Math.Floor((double)(now.Second / 10));
			int s2 = now.Second % 10;

			return new int[] {
				h1,
				h2,
				m1,
				m2,
				s1,
				s2
			};
		}
		
		private void DrawShape(Graphics graphics, char[] bars, int xOffset = 0)
		{				
			SolidBrush brush = new SolidBrush(FGColor);
		
			Rectangle a = new Rectangle(0  		 + xOffset, 0, 			pW * 6, pH * 2);
			Rectangle b = new Rectangle((pW * 4) + xOffset, 0, 			pW * 2, pH * 6);
			Rectangle c = new Rectangle((pW * 4) + xOffset, pH * 6,		pW * 2, pH * 6);
			Rectangle d = new Rectangle(0  		 + xOffset, pH * 10, 	pW * 6, pH * 2);
			Rectangle e = new Rectangle(0  		 + xOffset, pH * 6, 	pW * 2, pH * 6);
			Rectangle f = new Rectangle(0  		 + xOffset, 0,			pW * 2, pH * 6);
			Rectangle g = new Rectangle(0  		 + xOffset, pH * 5,  	pW * 6, pH * 2);
			Rectangle h = new Rectangle((pW * 2) + xOffset, 0, 			pW * 2, pH * 5);
			Rectangle i = new Rectangle((pW * 2) + xOffset, pH * 7,  	pW * 2, pH * 5);
			
			if (bars.HasBar('a'))
				graphics.FillRectangle(brush, a);
			if (bars.HasBar('b'))
				graphics.FillRectangle(brush, b);
			if (bars.HasBar('c'))
				graphics.FillRectangle(brush, c);
			if (bars.HasBar('d'))
				graphics.FillRectangle(brush, d);
			if (bars.HasBar('e'))
				graphics.FillRectangle(brush, e);
			if (bars.HasBar('f'))
				graphics.FillRectangle(brush, f);
			if (bars.HasBar('g'))
				graphics.FillRectangle(brush, g);
			if (bars.HasBar('h'))
				graphics.FillRectangle(brush, h);
			if (bars.HasBar('i'))
				graphics.FillRectangle(brush, i);
			
			brush.Dispose();
		}

		private int NextSecond()
		{
			DateTime now = DateTime.Now;
			DateTime then = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second + 1);

			return (int)((then - now).TotalMilliseconds);
		}

		// Black magic fuckery courtesy of https://stackoverflow.com/questions/31199437/borderless-and-resizable-form-c
		private const int WM_NCHITTEST 		 = 0x84;
		private const int HT_CLIENT 		 = 0x1;
		private const int HT_CAPTION 		 = 0x2;
		private const int HT_LEFT			 = 0xa;
		private const int HT_RIGHT			 = 0xb;
		private const int HT_TOP			 = 0xc;
		private const int HT_TOPLEFT		 = 0xd;
		private const int HT_TOPRIGHT		 = 0xe;
		private const int HT_BOTTOMLEFT      = 0x10;
		private const int HT_BOTTOM      	 = 0xf;
		private const int HT_BOTTOMRIGHT     = 0x11;
		private const int RESIZE_HANDLE_SIZE = 10;
		
		
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case WM_NCHITTEST:
					base.WndProc(ref m);

					if ((int)m.Result == HT_CLIENT)
					{
						IsResizing = true;
						
						Point screenPoint = new Point(m.LParam.ToInt32());
						Point clientPoint = this.PointToClient(screenPoint);                        
						if (clientPoint.Y <= RESIZE_HANDLE_SIZE)
						{
							if (clientPoint.X <= RESIZE_HANDLE_SIZE)
								m.Result = (IntPtr)HT_TOPLEFT;
							else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
								m.Result = (IntPtr)HT_TOP;
							else
								m.Result = (IntPtr)HT_TOPRIGHT ;
						}
						else if (clientPoint.Y <= (Size.Height - RESIZE_HANDLE_SIZE))
						{
							if (clientPoint.X <= RESIZE_HANDLE_SIZE)
								m.Result = (IntPtr)HT_LEFT;
							else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
								m.Result = (IntPtr)HT_CAPTION;
							else
								m.Result = (IntPtr)HT_RIGHT;
						}
						else
						{
							if (clientPoint.X <= RESIZE_HANDLE_SIZE)
								m.Result = (IntPtr)HT_BOTTOMLEFT;
							else if (clientPoint.X < (Size.Width - RESIZE_HANDLE_SIZE))
								m.Result = (IntPtr)HT_BOTTOM;
							else
								m.Result = (IntPtr)HT_BOTTOMRIGHT;
						}
						
						IsResizing = false;
					}
					return;
			}
			base.WndProc(ref m);
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.Style |= 0x20000; // <--- use 0x20000
				return cp;
			}
		}
		// End black magic fuckery
	}

	public static class Extensions
	{
		public static bool HasBar(this char[] array, char c)
		{
			return Array.Exists(array, _ => _ == c);
		}
	}
}

/*
0 is a b c d e f
1 is b c
2 is a b d e g
3 is a b c d g
4 is b c f g
5 is a c d f g
6 is a c d e f g
7 is a b c
8 is a b c d e f g
9 is a b c d f g
*/
