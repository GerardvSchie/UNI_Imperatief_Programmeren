using System;
using System.Collections.Generic;
using System.Drawing;

namespace SchetsEditor
{
    public class Schets
    {

		public Bitmap bitmap;
        
        public Schets()
        {
            bitmap = new Bitmap(1, 1);
        }
        public Graphics BitmapGraphics
        {
            get { return Graphics.FromImage(bitmap); }
        }
		public void VeranderAfmeting(Size sz)
		{	try
			{
				Bitmap nieuw = new Bitmap(Math.Max(sz.Width, bitmap.Size.Width)
										 , Math.Max(sz.Height, bitmap.Size.Height)
										 );
				Graphics gr = Graphics.FromImage(nieuw);
				gr.FillRectangle(Brushes.White, 0, 0, sz.Width, sz.Height);
				bitmap = nieuw;
			}
			catch { }
		}
        
        public void Teken(Graphics gr)
        {
            gr.DrawImage(bitmap, 0, 0);
        }
        public void Schoon()
        {	
        }
    }
}
