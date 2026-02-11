using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;

namespace SchetsEditor
{
	public class Vorm
	{
		public Point Beginpunt { get; set; } //Hier worden de elementen die in het vorm object zitten gedeclareerd
		public Point Eindpunt { get; set; }
		public Brush Vormkleur { get; set; }
		public string Tekst { get; set; }
		public Font Lettertype { get; set; }
		public string Tool { get; set; }
		public int Lijndikte { get; set; }
        public int Roteergetal { get; set; }

		int Breedte;
		int Hoogte;

        public Vorm()
		{
            Roteergetal = 0;
        }

		//Deze methode neemt een vorm en ziet of deze punt p bedekt
		public bool ClickedOnObject(Vorm V, Point p, int Lijndikte, SizeF sz) 
		{
            Lijndikte = Lijndikte / 2;
			// voorkomt dat lijndikte 2 lastig te klikken is 
			if (Lijndikte < 5)
                Lijndikte = 5;
			if (V.Tool == "rand" || V.Tool == "ellipse") 
			{
				bool hier = rand(V, p, Lijndikte);
				return hier;
			}
			else if (V.Tool == "tekst")
			{
				return text(V, p, sz);
			}
			else if (V.Tool == "vlak")
			{
				bool hier = Vlak(V, p, Lijndikte);
				return hier;
			}
			else if (V.Tool == "kader")
			{
				bool hier = Rechthoek(V, p, Lijndikte);
				return hier;
			}
			else if (V.Tool == "lijn" || V.Tool == "pen")
			{
				bool hier = Lijn(V, p, Lijndikte);
				return hier;
			}
			return false;
		}

		private bool text(Vorm V, Point p, SizeF sz) //Hier kijkt hij met behulp van een vlak of er op de char geklikt is
		{
            int x = (int)sz.Width;
            int y = (int)sz.Height;
            Point startpunt = new Point(V.Beginpunt.X, V.Beginpunt.Y + y);
            Point eindpunt = new Point(V.Beginpunt.X + x, V.Beginpunt.Y);
            Vorm test = new Vorm { Beginpunt = startpunt, Eindpunt = eindpunt, Vormkleur = V.Vormkleur, Tekst = "", Lettertype = V.Lettertype, Tool = V.Tool, Lijndikte = 0};
            return Vlak(test, p, 0);
		}

		private bool Rechthoek(Vorm V, Point p, int Lijndikte) //hier kijkt hij wat de afstand tot de lijnen is, een if statement voor iedere lijn van de kader
		{
			LijnTool.Punten2Rechthoek(V.Beginpunt, V.Eindpunt);
			int y1 = V.Beginpunt.Y;
			int x1 = V.Beginpunt.X;
			int y2 = V.Eindpunt.Y;
			int x2 = V.Eindpunt.X;

			if (Math.Abs(p.X - x1) < (Lijndikte + 2) && (p.Y > (y1 - (Lijndikte + 2))) && (p.Y < (y2 + (Lijndikte + 2))))
				return true;
			else if (Math.Abs(p.X - x2) < (Lijndikte + 2) && (p.Y > (y1 - (Lijndikte + 2))) && (p.Y < (y2 + (Lijndikte + 2))))
				return true;
			else if (Math.Abs(p.Y - y1) < (Lijndikte + 2) && (p.X > (x1 - (Lijndikte + 2))) && (p.X < (x2 + (Lijndikte + 2))))
				return true;
			else if (Math.Abs(p.Y - y2) < (Lijndikte + 2) && (p.X > (x1 - (Lijndikte + 2))) && (p.X < (x2 + (Lijndikte + 2))))
				return true;
			return false;
		}

		private bool Vlak(Vorm V, Point p, int Lijndikte) //hier bekijkt hij of je op een vlak klikt
		{
			int y1 = V.Beginpunt.Y, x1 = V.Beginpunt.X; 
			int y2 = V.Eindpunt.Y, x2 = V.Eindpunt.X;
			if (((Math.Min(x1, x2) - (Lijndikte)) < p.X && p.X < (Math.Max(x1, x2) + (Lijndikte))) && ((Math.Min(y1, y2) - (Lijndikte)) < p.Y && p.Y < (Math.Max(y1, y2) + (Lijndikte))))
				return true;
			return false;
		}
     
		private int AfstandTotPunt(Point p1, Point p2) //dit is een hulp functie voor de afstand tot een punt die twee punten vergelijkt
		{
			int dx = p1.X - p2.X;
			int dy = p1.Y - p2.Y;
			return (int)(Math.Sqrt(dx * dx + dy * dy));
		}
		private int Brandpunten(Point p1, Point p2) //dit is een hulpfunctie voor de elipsen en randen waarbij hij de brandpunten berekend.
		{
			Breedte = Math.Abs(p1.X - p2.X);
			Hoogte = Math.Abs(p1.Y - p2.Y);
			if (Breedte >= Hoogte)
			{
				return (int)Math.Sqrt(Breedte ^ 2 - Hoogte ^ 2);
			}
			return (int)Math.Sqrt(Hoogte ^ 2 - Breedte ^ 2);
			
		}
		private bool Lijn(Vorm V, Point p, int Lijndikte) //Hier berekend hij (met behulp van een functie van wikipedia) de afstand van een punt tot een lijn
		{
			Point p1 = V.Beginpunt, p2 = V.Eindpunt;		
			int afstand = (int)(Math.Abs((p2.Y - p1.Y) * p.X - (p2.X - p1.X) * p.Y + p2.X * p1.Y - p2.Y * p1.X) / (Math.Sqrt((p2.Y - p1.Y) * (p2.Y - p1.Y) + (p2.X - p1.X) * (p2.X - p1.X)))); 
			
            if (afstand < (Lijndikte + 2) && p.X > (Math.Min(p1.X, p2.X)-(Lijndikte + 2)) && p.X < (Math.Max(p1.X, p2.X) + (Lijndikte + 2)))
			{
				return true;
			}
			else return false;
		}
		private bool rand(Vorm V, Point p, int Lijndikte) //hier berekend hij de afstand tot de rand doormiddel van de afstand tot beide branpunten te berekenen.
		{
			int Brandpunt = Brandpunten(V.Beginpunt,V.Eindpunt);
			int xMid = (int)(V.Beginpunt.X + V.Eindpunt.X) / 2;
			int yMid = (int)(V.Beginpunt.Y + V.Eindpunt.Y) / 2;
            if (Breedte >= Hoogte)              //op het moment dat de breedte groter is dan de hoogte zitten de brandpunten op de x as en dan moeten er daarmee gerekent worden
            {
                Point p1 = new Point(xMid + Brandpunt, yMid);
                Point p2 = new Point(xMid - Brandpunt, yMid);
                int d1 = AfstandTotPunt(p, p1);
                int d2 = AfstandTotPunt(p, p2);
                if (V.Tool == "rand" && (d1 + d2) > (Breedte - (2 * Lijndikte + 2)) && (d1 + d2) < (Breedte + (2 * Lijndikte + 2)))
                {
                    return true;
                }
                else if (V.Tool == "ellipse" && (d1 + d2) < (Breedte + 2))
                {
                    return true;
                }
                return false;
            }
            else                               //Als hoogte groter is dan breedte vice versa (brandpunten liggen op y-as)
			{
				Point p1 = new Point(xMid, yMid - Brandpunt);
				Point p2 = new Point(xMid, yMid + Brandpunt);
				int d1 = AfstandTotPunt(p, p1);
				int d2 = AfstandTotPunt(p, p2);
				if (V.Tool == "rand" && (d1 + d2) > (Hoogte - (Lijndikte + 2)) && (d1 + d2) < (Hoogte + (Lijndikte + 2)))
				{
					return true;
				}
				else if (V.Tool == "ellipse" && ((d1 + d2) < (Hoogte + 2)))
				{
					return true;
				}
				return false;
			}
		}
	}
	public class SchetsControl : UserControl
	{
		public List<List<Vorm>> ObjectenGeschiedenis = new List<List<Vorm>>();
		public List<Vorm> Objecten = new List<Vorm>();
		int teller = 2;
		public void VoegToeAanLijst(Vorm V) //dit is een functie die een Vorm toevoegd aan de lijst en in de geschiedenis zet
		{
			if (ObjectenGeschiedenis.Count == 0)
				Objectengeschiedenis(Objecten); 
			Objecten.Add(V);
			Objectengeschiedenis(Objecten);
		}
		public void Objectengeschiedenis(List<Vorm> V) //hier slaat hij de geschiedenis van objecten op, dit doet hij op deze manier omdat lijsten by refference zijn en dus op deze manier gekopiëerd moeten worden
		{
			List<Vorm> tijdelijk = new List<Vorm>();

			foreach (Vorm nieuw in V)
			{
				if (nieuw != null)
					tijdelijk.Add(new Vorm { Beginpunt = nieuw.Beginpunt, Eindpunt = nieuw.Eindpunt, Vormkleur = nieuw.Vormkleur, Tekst = nieuw.Tekst, Lettertype = nieuw.Lettertype, Tool = nieuw.Tool, Lijndikte = nieuw.Lijndikte });
			}
			ObjectenGeschiedenis.Add(tijdelijk);
		}

		public List<Vorm> Parse(string parsestring) // Vanuit de textfile wordt de string uitgelezen en naar vormen gezeet die aan de objectenlijst worden toegevoegd
		{
			List<Vorm> vormenlijst = new List<Vorm>();
			string[] s = parsestring.Split(' ');
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
			for (int x = 0; x < (s.Length - 1); x += 10)
			{
				Vorm v = new Vorm();
				v.Beginpunt = new Point(int.Parse(s[x]), int.Parse(s[x + 1]));
				v.Eindpunt = new Point(int.Parse(s[x + 2]), int.Parse(s[x + 3]));
				v.Vormkleur = new SolidBrush(Color.FromArgb(int.Parse(s[x + 4]))); 
				v.Tekst = s[x + 5];
				s[x + 6] = s[x + 6].Replace('_', ' ');
				v.Lettertype = (Font)converter.ConvertFromString(s[x + 6]);
				v.Tool = s[x + 7];
				v.Lijndikte = int.Parse(s[x + 8]);
				v.Roteergetal = int.Parse(s[x + 9]);
				vormenlijst.Add(v);
			}
			return vormenlijst;
		}
		public void Undo(object obj, EventArgs e) // Dit is de undo functie, die maakt gebruik van de objectengeschiedenis lijst om de laatste handeling terug te halen
		{
			int lengte = Objecten.Count;
			int lengte2 = ObjectenGeschiedenis.Count;

			try
			{
				objecten();
				teller++;
			}
			catch { }
			Update();
			Refresh();
			Invalidate();
		}
		public void Removegeschiedenis() //hier verwijderd hij de Vorm objecten die niet meer nodig zijn op het moment dat je een nieuw item toevoegd aan de geschiedenis
		{
			if (teller != 2) {
				for (int t = ObjectenGeschiedenis.Count - teller + 2; t < (ObjectenGeschiedenis.Count); t++)
				{
					try
					{
						ObjectenGeschiedenis.RemoveAt(t);
					}
					catch { }
				}
				if (ObjectenGeschiedenis[0].Count != 0)
					ObjectenGeschiedenis.RemoveAt(0);
				teller = 2;
				for (int t = 1; t < ObjectenGeschiedenis.Count; t++)
				{

					if (ObjectenGeschiedenis[t].Count != ObjectenGeschiedenis[t - 1].Count + 1 && ObjectenGeschiedenis[t].Count != 0)
					{
						ObjectenGeschiedenis.RemoveAt(t);
					}
				}
			}
		}
		public void objecten() //hier maakt hij een deep copy van de Objectengeschiedenis en stelt deze gelijk aan de List<Vorm> Objecten zodat we de oude(re) situatie weer opnieuw tekenen
		{
			List<Vorm> tijdelijk = new List<Vorm>();
			foreach (Vorm nieuw in ObjectenGeschiedenis[ObjectenGeschiedenis.Count - teller])
			{
				tijdelijk.Add(new Vorm { Beginpunt = nieuw.Beginpunt, Eindpunt = nieuw.Eindpunt, Vormkleur = nieuw.Vormkleur, Tekst = nieuw.Tekst, Lettertype = nieuw.Lettertype, Tool = nieuw.Tool, Lijndikte = nieuw.Lijndikte });
			}
			Objecten = tijdelijk;
		}

		public void Redo(object obj, EventArgs e) //Deze functie maakt gebruik van de objectengeschiedenis om de reco ongedaan te maken.
		{
			int lengte = Objecten.Count;
			int lengte2 = ObjectenGeschiedenis.Count;
			if (teller > 1)
			{
				try
				{
					teller--;
					hardcopyobjecten();
				}
				catch { }
				Invalidate();
			}
		}
		public void hardcopyobjecten() //deze functie maakt een hard copy van de geschiedenis, zodat we geen dingen veranderen omdat lijsten by reference zijn.
		{
			List<Vorm> tijdelijk = new List<Vorm>();
			foreach (Vorm nieuw in ObjectenGeschiedenis[ObjectenGeschiedenis.Count - teller + 1])
			{
				tijdelijk.Add(new Vorm { Beginpunt = nieuw.Beginpunt, Eindpunt = nieuw.Eindpunt, Vormkleur = nieuw.Vormkleur, Tekst = nieuw.Tekst, Lettertype = nieuw.Lettertype, Tool = nieuw.Tool, Lijndikte = nieuw.Lijndikte });
			}
			Objecten = tijdelijk;
		}

		public virtual string ListToString(List<Vorm> Vormen) // de lijst van vormen worden naar 1 lange string gezet waarin de properties met een spatie ertussen later uit elkaar te halen zijn
		{
			string tekstregel = "";
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
			foreach (Vorm v in Vormen)
			{
				Color c = ((SolidBrush)v.Vormkleur).Color;
				//Font naar string geeft spaties die later bij de splitfunctie niet gezien moeten worden 
				string g = converter.ConvertToString(v.Lettertype);
				g = g.Replace(" ", "_");
				tekstregel += $"{v.Beginpunt.X} {v.Beginpunt.Y} {v.Eindpunt.X} {v.Eindpunt.Y} {c.ToArgb()} {v.Tekst} {g} {v.Tool} {v.Lijndikte} {v.Roteergetal} ";
			}
			return tekstregel;
		}

		public Vorm MoveVorm(Point p) // Er wordt berekend welke vorm er aangeklikt wordt en geeft deze terug aan de plek waar methode wordt aangeroepen
		{	
			for (int t = 0; t < Objecten.Count; t++)
			{
				if (Objecten[t].ClickedOnObject(Objecten[t], p, Objecten[t].Lijndikte, sz))
				{
					this.Update();
					return Objecten[t];
				}
			}
			// anders null omdat er geen object aangeklikt wordt
			return null;
		}

		public Point startpunt;
		public Point p;
		public SchetsControl s;
		private Font lettertype = new Font("Tahoma", 40);
		private Schets schets;
		private Color penkleur;
		private int lijndikte = 5;
		// lettertype en lijndikte wordt ook aangemaakt met een get property
		public Font Lettertype
		{
			get { return lettertype; }
		}
		public int LijnDikte
		{
			get { return lijndikte; }
		}
		public Color PenKleur
		{
			get { return penkleur; }
		}
		public Schets Schets
		{
			get { return schets; }
		}
		public SchetsControl()
		{
			this.BorderStyle = BorderStyle.Fixed3D;
			this.schets = new Schets();
			this.Paint += this.teken;
			this.Resize += this.veranderAfmeting;
			this.veranderAfmeting(null, null);
		}
		protected override void OnPaintBackground(PaintEventArgs e)
		{
		}

        private void teken(object o, PaintEventArgs pea) //hier tekent hij met behulp van een lijst de getekende dingen op de bitmap
        {

            schets.Teken(pea.Graphics);
            p = startpunt;
            int q;
            for (q = 0; q < Objecten.Count; q++) //de hele lijst objecten wordt doorgegaan
            {
                Vorm v = Objecten[q];
                if (v != null)
                {
                    if (v.Tool == "pen")
                        pea.Graphics.DrawLine(GumTool.MaakPen(v.Vormkleur, v.Lijndikte), v.Beginpunt.X, v.Beginpunt.Y, v.Eindpunt.X, v.Eindpunt.Y);
                    else if (v.Tool == "lijn")
                        pea.Graphics.DrawLine(TweepuntTool.MaakPen(v.Vormkleur, v.Lijndikte), v.Beginpunt.X, v.Beginpunt.Y, v.Eindpunt.X, v.Eindpunt.Y);
                    else if (v.Tool == "tekst")
                    {
                        if (v.Eindpunt != v.Beginpunt) 
                        {
                            pea.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit; //hier roteert hij de string
                            GraphicsState state = pea.Graphics.Save();
                            pea.Graphics.ResetTransform();
                            pea.Graphics.RotateTransform(v.Eindpunt.Y);      
                            pea.Graphics.DrawString(v.Tekst, v.Lettertype, v.Vormkleur, v.Beginpunt);
                            pea.Graphics.Restore(state);
                        }

                        else
                        {
                            pea.Graphics.DrawString(v.Tekst, v.Lettertype, v.Vormkleur, v.Beginpunt);
                        }

                    }
                    else if (v.Tool == "rand")
                        pea.Graphics.DrawEllipse(TweepuntTool.MaakPen(v.Vormkleur, v.Lijndikte), LijnTool.Punten2Rechthoek(v.Beginpunt, v.Eindpunt));
                    else if (v.Tool == "ellipse")
                        pea.Graphics.FillEllipse(v.Vormkleur, LijnTool.Punten2Rechthoek(v.Beginpunt, v.Eindpunt));
                    else if (v.Tool == "kader")
                        pea.Graphics.DrawRectangle(TweepuntTool.MaakPen(v.Vormkleur, v.Lijndikte), LijnTool.Punten2Rechthoek(v.Beginpunt, v.Eindpunt));
                    else if (v.Tool == "vlak")
                        pea.Graphics.FillRectangle(v.Vormkleur, LijnTool.Punten2Rechthoek(v.Beginpunt, v.Eindpunt));
                }
            }
        }
        public void Bovenop(Point p)
        {
            for (int t = 0; t < Objecten.Count; t++) //hier kijkt hij op welk object geklikt is, deze doet hij dan bovenop
            {
                if(Objecten[t].ClickedOnObject(Objecten[t], p, Objecten[t].Lijndikte, sz))
                {
                    Objecten.Insert(Objecten.Count, Objecten[t]);
                    Objecten.RemoveAt(t);
                    this.Update();
                    break;
                }
            }
            this.Invalidate();
        }
        public void Onderop(Point p) //hier kijkt hij welk object geklikt is, deze doet hj dan onderop
        {
            Graphics graphics = CreateGraphics();
            SizeF sz = graphics.MeasureString("", Lettertype);
            for (int t = 0; t < Objecten.Count; t++)
            {
                if (Objecten[t].ClickedOnObject(Objecten[t], p, Objecten[t].Lijndikte, sz))
                {
                    Objecten.Insert(0, Objecten[t]);
                    Objecten.RemoveAt(t+1);
                    this.Update();
                    break;
                }
            }
            this.Invalidate();
        }
        SizeF sz;
        public void gum(Point p)
		{	//dit is de gum functie, hier kijkt hij waar er op geklikt is en deze verwijderd hij dan van de lijst
            for(int t = Objecten.Count - 1; t>=0; t--){
                Vorm tijdelijk = new Vorm();
                tijdelijk = Objecten[t];
                if(tijdelijk.Tool == "tekst"){
                    Graphics G = CreateGraphics();
                    sz = G.MeasureString(tijdelijk.Tekst, tijdelijk.Lettertype);
                    
                }

                if (tijdelijk.ClickedOnObject(tijdelijk, p, tijdelijk.Lijndikte, sz))
                {
					Objecten.RemoveAt(t);
					this.Invalidate();
                    break;
                }
            }
        }
		private void veranderAfmeting(object o, EventArgs ea)
		{
			schets.VeranderAfmeting(this.ClientSize);
			this.Invalidate();
		}
		public Graphics MaakBitmapGraphics()
		{
			Graphics g = schets.BitmapGraphics;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			return g;
		}
		public void Schoon(object o, EventArgs ea) // schoon maakt nu de lijst objecten leeg
		{
			Objecten.Clear();
			this.Invalidate();
            VoegToeAanLijst(null);
		}
		public void Roteer(object o, EventArgs ea) //hier worden de tekenpunten van de vormen verandert en roteert hij de objecten rondom het middelpunt.
		{        
            int xmidden = this.ClientSize.Width / 2;
            int ymidden = this.ClientSize.Height / 2;
			for (int t = 0; t < Objecten.Count; t++)
			{
				if (Objecten[t] != null) 
				{
					Vorm nieuw = Objecten[t];
					Vorm v = Objecten[t];
					int Punt1y = -1 * (v.Beginpunt.X - xmidden);
					int Punt1x = (v.Beginpunt.Y - ymidden);
					int Punt2y = -1 * (v.Eindpunt.X - xmidden);
					int Punt2x = (v.Eindpunt.Y - ymidden);
					Point beginpunt = new Point(Punt1x + xmidden, Punt1y + ymidden);
					Point eindpunt = new Point(Punt2x + xmidden, Punt2y + ymidden);

					if (Objecten[t].Tool == "tekst") //bij tekst wordt er alleen maar beginpunt gebruikt
					{
						if (v.Eindpunt == v.Beginpunt)
						{
							Point p = new Point(0, 90);
						}
						else
						{
							Point p = new Point(0, v.Eindpunt.Y + 90);
						}
						Vorm Tijdelijkevorm = new Vorm { Beginpunt = beginpunt, Eindpunt = p, Vormkleur = nieuw.Vormkleur, Tekst = nieuw.Tekst, Lettertype = nieuw.Lettertype, Tool = nieuw.Tool, Lijndikte = nieuw.Lijndikte };
						Objecten[t] = Tijdelijkevorm;
					}
					else
					{
						Vorm Tijdelijkevorm = new Vorm { Beginpunt = beginpunt, Eindpunt = eindpunt, Vormkleur = nieuw.Vormkleur, Tekst = nieuw.Tekst, Lettertype = nieuw.Lettertype, Tool = nieuw.Tool, Lijndikte = nieuw.Lijndikte };
						Objecten[t] = Tijdelijkevorm;
					}
				}

			}
            this.Invalidate();
		}
		public void Opslaan(object o, EventArgs ea) // met de hier wordt de lange string als een file opgeslagen
		{
			SaveFileDialog savefiledialog1 = new SaveFileDialog(); 
			if (savefiledialog1.ShowDialog() == DialogResult.OK)
			{
				File.WriteAllText(savefiledialog1.FileName, this.ListToString(Objecten));
			}
		}
		public void Openen(object o, EventArgs ea) // met de parse methode wordt deze lange string uitgelezen convert naar een lijst met objecten
		{
			OpenFileDialog openfiledialog1 = new OpenFileDialog();
			if (openfiledialog1.ShowDialog() == DialogResult.OK)
			{
				Objecten = Parse(File.ReadAllText(openfiledialog1.FileName));
			}
			ObjectenGeschiedenis.Add(Objecten);
			this.Invalidate();
		}
		public void Kleurenspectrum(object o, EventArgs ea) // Met ColorDialog wordt een penkleur bepaald
		{
			ColorDialog colorDialog1 = new ColorDialog();
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				penkleur = colorDialog1.Color;
			}
		}
		public void VeranderKleur(object obj, EventArgs ea) // penkleur verander met combobox
		{
			string kleurNaam = ((ComboBox)obj).Text;
			penkleur = Color.FromName(kleurNaam);
		}
		public void VeranderKleurViaMenu(object obj, EventArgs ea) // penkleur via menu veranderd
		{
			string kleurNaam = ((ToolStripMenuItem)obj).Text;
			penkleur = Color.FromName(kleurNaam);
		}
		public void VeranderFont(object o, EventArgs ea) // font wordt verandert mbv een fontdialog (Daar kan ook penkleur worden aangepast)
		{
			FontDialog fontDialog1 = new FontDialog();
			fontDialog1.Font = Lettertype;
			fontDialog1.ShowColor = true;
			fontDialog1.FontMustExist = true;
			if (fontDialog1.ShowDialog() == DialogResult.OK)
			{
				penkleur = fontDialog1.Color;
				lettertype = fontDialog1.Font;
			}
		}
		public void VeranderLijndikte(object obj, EventArgs ea) // Lijndikte uit numericupdown
		{
			string LijndikteNaam = ((NumericUpDown)obj).Text;
			lijndikte = int.Parse(LijndikteNaam);
		}
		public void VeranderLijndikteViaMenu(object obj, EventArgs ea) // lijndikte via menu
		{
			string LijndikteNaam = ((ToolStripMenuItem) obj).Text;
			lijndikte = int.Parse(LijndikteNaam);
		}
	}

}

