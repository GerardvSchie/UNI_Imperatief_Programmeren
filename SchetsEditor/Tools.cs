using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SchetsEditor
{
	public interface ISchetsTool
	{
		void MuisVast(SchetsControl s, Point p);
		void MuisDrag(SchetsControl s, Point p);
		void MuisLos(SchetsControl s, Point p);
		void Letter(SchetsControl s, char c);

	}


	public abstract class StartpuntTool : ISchetsTool
	{
		public Point startpunt;
		public Brush kwast;
		public Font lettertype;
		public int lijndikte;
		public virtual void MuisVast(SchetsControl s, Point p)
		{	//lijndikte en lettertype worden ook meegenomen
			startpunt = p;
			lijndikte = s.LijnDikte;
			lettertype = s.Lettertype;
		}
		public virtual void MuisLos(SchetsControl s, Point p)
		{   kwast = new SolidBrush(s.PenKleur);
		}
		public abstract void MuisDrag(SchetsControl s, Point p);
		public abstract void Letter(SchetsControl s, char c);
	}
	// verplaatst vormen naar een andere plek
	public class DragTool : StartpuntTool
	{	// Voor dragtool wordt er nog een extra set punten aangemaakt voor tijdens het slepen visuele feedback
		Vorm v = new Vorm();
		Point begin = new Point();
		Point eind = new Point();
		public override string ToString() { return "drag"; }
		public override void MuisVast(SchetsControl s, Point p)
		{
			startpunt = p;
			v = s.MoveVorm(p);
			begin = v.Beginpunt;
			eind = v.Eindpunt;
		}
		public override void MuisLos(SchetsControl s, Point p) 
		{
			if (v != null)
			{
				v.Beginpunt = new Point(begin.X + p.X - startpunt.X, begin.Y + p.Y - startpunt.Y);
				v.Eindpunt = new Point(eind.X + p.X - startpunt.X, eind.Y +  p.Y - startpunt.Y);
				s.VoegToeAanLijst(v);
			}
			s.Invalidate();
		}
		public override void MuisDrag(SchetsControl s, Point p)
		{
			v.Beginpunt = new Point(begin.X + p.X - startpunt.X, begin.Y + p.Y - startpunt.Y);
			v.Eindpunt = new Point(eind.X + p.X - startpunt.X, eind.Y + p.Y - startpunt.Y);
			s.Invalidate();
		}
		public override void Letter(SchetsControl s, char c)
		{ }
	}

	public class TekstTool : StartpuntTool
	{
		public override string ToString() { return "tekst"; }

		public override void MuisDrag(SchetsControl s, Point p) { }

		public override void Letter(SchetsControl s, char c)
		{
			if (c >= 32)
			{	// Char wordt apart toegevoegd
				s.Removegeschiedenis();
				Graphics gr = s.MaakBitmapGraphics();
				string tekst = c.ToString();
				SizeF sz = gr.MeasureString(tekst, lettertype, this.startpunt, StringFormat.GenericTypographic);
				Vorm tijdelijk = new Vorm { Beginpunt = startpunt, Eindpunt = startpunt, Vormkleur = kwast, Tekst = c.ToString(), Lettertype = lettertype, Tool = "tekst", Lijndikte = lijndikte };
				s.VoegToeAanLijst(tijdelijk);
				startpunt.X += (int)sz.Width;
				s.Invalidate();
			}
		}
	}

	// Vorm wordt bovenin de lijst gelegd
	public class BovenopTool : StartpuntTool
	{
		public override string ToString() { return "boven"; }

		public override void MuisDrag(SchetsControl s, Point p) { }

		public override void MuisLos(SchetsControl s, Point p)
		{
			s.Removegeschiedenis();
			s.Bovenop(p);
		}
		public override void Letter(SchetsControl s, char c) { }
	}
	// Vorm wordt onderin de lijst
	public class OnderopTool : StartpuntTool
	{
		public override string ToString() { return "onder"; }

		public override void MuisDrag(SchetsControl s, Point p) { }

		public override void MuisLos(SchetsControl s, Point p)
		{
			s.Removegeschiedenis();
			s.Onderop(p);
		}
		public override void Letter(SchetsControl s, char c) { }
	}

	public abstract class TweepuntTool : StartpuntTool
	{
		public static Rectangle Punten2Rechthoek(Point p1, Point p2)
		{ return new Rectangle(new Point(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y))
							  , new Size(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y))
							  );
		}
		public static Pen MaakPen(Brush b, int dikte)
		{	Pen pen = new Pen(b, dikte);
			pen.StartCap = LineCap.Round;
			pen.EndCap = LineCap.Round;
			return pen;
		}
		public override void MuisVast(SchetsControl s, Point p)
		{	base.MuisVast(s, p);
			kwast = new SolidBrush(s.PenKleur);
		}
		public override void MuisDrag(SchetsControl s, Point p)
		{	s.Refresh();
			this.Bezig(s.CreateGraphics(), this.startpunt, p);
		}
		public override void MuisLos(SchetsControl s, Point p) //Hier wordt een element aan de lijst toegevoegd als de tool geen gum is, anders dan wordt de functie gum aangeroepen.
		{
			base.MuisLos(s, p);
			s.Removegeschiedenis();
			Vorm v = new Vorm { Beginpunt = startpunt, Eindpunt = p, Vormkleur = kwast, Tekst = "", Lettertype = lettertype, Tool = this.ToString(), Lijndikte = lijndikte };
			if (this.ToString() != "gum")
				s.VoegToeAanLijst(v);
			else
				s.gum(v.Beginpunt);
			this.Compleet(s.MaakBitmapGraphics(), this.startpunt, p);
			s.Invalidate();
		}
		public override void Letter(SchetsControl s, char c)
		{
		}
		public abstract void Bezig(Graphics g, Point p1, Point p2);

		public virtual void Compleet(Graphics g, Point p1, Point p2)
		{
		}
	}

	public class RechthoekTool : TweepuntTool
	{
		public override string ToString() { return "kader"; }

		public override void Bezig(Graphics g, Point p1, Point p2)
		{ g.DrawRectangle(MaakPen(kwast, lijndikte), TweepuntTool.Punten2Rechthoek(p1, p2));
		}
	}

	public class VolRechthoekTool : RechthoekTool
	{
		public override string ToString() { return "vlak"; }

		public override void Bezig(Graphics g, Point p1, Point p2)
		{
			g.FillRectangle(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
		}
	}

	public class CirkelTool : TweepuntTool //deze functie tekent een cirkel
	{
		public override string ToString() { return "rand"; }

		public override void Bezig(Graphics g, Point p1, Point p2)
		{ g.DrawEllipse(MaakPen(kwast, 5), TweepuntTool.Punten2Rechthoek(p1, p2));
		}
	}

	public class VolCirkelTool : CirkelTool //deze functie tekend een gevulde cirkel
	{
		public override string ToString() { return "ellipse"; }

		public override void Bezig(Graphics g, Point p1, Point p2)
		{
			g.FillEllipse(kwast, TweepuntTool.Punten2Rechthoek(p1, p2));
		}
	}

	public class LijnTool : TweepuntTool
	{
		public override string ToString() { return "lijn"; }

		public override void Bezig(Graphics g, Point p1, Point p2)
		{
			g.DrawLine(MaakPen(kwast, lijndikte), p1, p2);
		}
	}

	public class PenTool : LijnTool
	{
		public override string ToString() { return "pen"; }

		public override void MuisDrag(SchetsControl s, Point p)
		{ this.MuisLos(s, p);
			this.MuisVast(s, p);
		}
	}

	public class GumTool : PenTool //dit is een gumtool die met behulp van de functie gum in schetscontrol elementen verwijderd.
	{
		public override string ToString() { return "gum"; }

		public override void Bezig(Graphics g, Point p1, Point p2)
		{
		}
	}
}

