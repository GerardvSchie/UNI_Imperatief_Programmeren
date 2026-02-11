using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Diagnostics;

namespace DesktopApp2
{
	public class Scherm : Form
	{
		// Globale variabelen maken die door methodes moet kunnen worden aangeroepen
		uint iMax = 90;
		const int WindowXY = 600;
		double xMid = 0, yMid = 0, Zoom = 1, DeltaXY;
		string UndoX = "0", UndoY = "0", UndoZoom = "1", UndoiMax = "90";

		Label L_Error = new Label(), L_Timer = new Label();
		TextBox TB_xMid, TB_yMid, TB_Zoom, TB_iMax, TB_Save;
		Stopwatch Duration = new Stopwatch();
		ListBox LB_Saved_Files = new ListBox();
		PictureBox MandelPicture = new PictureBox
		{
			Location = new Point(30, 150),
			Size = new Size(WindowXY, WindowXY),
		};

		List<string> savefile = new List<string>();

		public Scherm()
		{
			// Formaat e.d. van de window instellen met alle events en knoppen etc.
			this.ClientSize = new Size(WindowXY + 60, WindowXY + 180);
			this.Text = "Mandelbrot";
			this.BackColor = Color.White;
			/**************************/
			Button Undo = Button_Undo();
			Button Ok = Button_Ok();
			Button Save = Button_Save();
			Button Load = Load_file();

			TB_xMid = Textbox_xMid();
			TB_yMid = Textbox_yMid();
			TB_Zoom = Textbox_Zoom();
			TB_iMax = Textbox_iMax();
			TB_Save = Saveinput();

			LB_Saved_Files = Listbox_SavedFile();

			Label_xMid();
			Label_yMid();
			Label_iMax();
			Label_Zoom();
			Label_Error();
			Label_Timer();

			Voorbeelden();
			/**************************/
			Undo.Click += this.Undo;
			Ok.Click += this.Ok;
			Save.Click += this.Save;
			Load.Click += this.load;

			this.Paint += GrayRectangle;
			this.Paint += Mandelfiguur;
			this.Paint += LoadCircle;

			MandelPicture.MouseClick += ClickZoom;
		}
		void Undo(object sender, EventArgs e)
		{
			// Zet de 'Undo-waardes' in de textboxen
			TB_xMid.Text = UndoX;
			TB_yMid.Text = UndoY;
			TB_Zoom.Text = UndoZoom;
			TB_iMax.Text = UndoiMax;

			// Rekent met de nieuwe waardes de window uit (Voorkomt een fout dat de gebruiker een punt gebruikt i.p.v. een komma)
			xMid = double.Parse(TB_xMid.Text.Replace(".", ","));
			yMid = double.Parse(TB_yMid.Text.Replace(".", ","));
			Zoom = double.Parse(TB_Zoom.Text.Replace(".", ","));
			iMax = uint.Parse(TB_iMax.Text);
			MandelPicture.Invalidate();
			this.Invalidate();
		}

		void Ok(object sender, EventArgs e)
		{   
			// Slaat de waardes op voor de 'Undo-Knop'
			UndoX = Convert.ToString(xMid);
			UndoY = Convert.ToString(yMid);
			UndoZoom = Convert.ToString(Zoom);
			UndoiMax = Convert.ToString(iMax);

			// Leest textvelden uit en maakt deze de nieuwe waardes voor de functie
			xMid = double.Parse(TB_xMid.Text.Replace(".", ","));
			yMid = double.Parse(TB_yMid.Text.Replace(".", ","));
			Zoom = double.Parse(TB_Zoom.Text.Replace(".", ","));
			iMax = uint.Parse(((TB_iMax.Text).Replace(".", "")).Replace(",", ""));
			TB_iMax.Text = ((TB_iMax.Text).Replace(".", "")).Replace(",", "");
			this.Invalidate();
			MandelPicture.Invalidate();
		}
		// Eventhandler voor het klikken op de 'Save-Knop'
		void Save(object sender, EventArgs e)                                                
		{	
			// Leest string in textbox voor het opslaan
			string Filename = TB_Save.Text;                                                     
			if (Filename != "")                                                           // Kijkt of je geen lege string invult, als dat het geval is ga je verder
			{
				int waar = FileLocation(Filename);                                               // Hij kijkt of er al een file is opgeslagen met dezelfde naam
				if (waar == -1)                                                                 // Op het moment dat er geen file is opgeslagen returned de functie filelocatie -1
				{
					savefile.Add(Filename);                                                     // Vervolgens voegt hij items toe aan de list
					savefile.Add(Convert.ToString(iMax));
					savefile.Add(Convert.ToString(xMid));
					savefile.Add(Convert.ToString(yMid));
					savefile.Add(Convert.ToString(Zoom));
					LB_Saved_Files.Items.Add(Filename);                                            // Daarna voegt hij het toe aan de listbox zodat je de save ook weer terug kunt vinden
				}
				else L_Error.Text = "Error: gebrek aan originaliteit";
			TB_Save.Text = String.Empty;                                                    // Maak textbox leeg                                     
			}
		}

		void load(object sender, EventArgs e)
		{
			string Filename = LB_Saved_Files.Text;                  // Hij kijkt welke file je hebt aangeklikt                                       
			int Location = FileLocation(Filename);               // Vervolgens kijkt hij wat de locatie in de lijst is (als hij er is)

			if (Location != -1)                                        // Als er een file bestaat in de lijst met die naam
			{   
				// Slaat de waardes op voor de 'Undo-Knop'
				UndoX = TB_xMid.Text;                                 
				UndoY = TB_yMid.Text;
				UndoZoom = TB_Zoom.Text;
				UndoiMax = TB_iMax.Text;

				TB_iMax.Text = savefile[Location + 1];                          // Vervolgens laad hij de locatie van xmid, ymid, iMax en zoom in de textboxen
				TB_xMid.Text = Convert.ToString(savefile[Location + 2]);
				TB_yMid.Text = Convert.ToString(savefile[Location + 3]);
				TB_Zoom.Text = Convert.ToString(savefile[Location + 4]);

				iMax = uint.Parse(savefile[Location + 1]);                         // Daarna laad hij de waardes en past hij deze aan mbv een globale variabele
				xMid = double.Parse(savefile[Location + 2]);
				yMid = double.Parse(savefile[Location + 3]);
				Zoom = double.Parse(savefile[Location + 4]);

				MandelPicture.Invalidate();                                         // Hij laat nu het window opnieuw tekenen m.b.v. nieuwe waardes
				this.Invalidate();
			}
		}

		// Licht-grijze achtergrond om de GUI duidelijk te scheiden van het Mandelfiguur
		private void GrayRectangle(object sender, PaintEventArgs pea)
		{
			pea.Graphics.FillRectangle(Brushes.LightGray, 0, 135, WindowXY + 60, WindowXY + 45);
		}

		// Methode die het Mandelfiguur tekent m.b.v. bitmap
		private void Mandelfiguur(object sender, EventArgs e)
		{
			Bitmap MandelBitmap = new Bitmap(WindowXY, WindowXY);
			if (iMax == 0)
				L_Error.Text = "Error: tekent niks bij 0 iterraties";
			else
			{
				int xPixel = 0;
				int yPixel = 0;
				DeltaXY = 8 / (Math.Pow(2, Zoom));

				// Reset stopwatch en start hem daarna
				Duration.Reset();
				Duration.Start();

				// Rekent Mandelbrotgetal voor elke X en Y waarde die bij de pixels horen
				for (xPixel = 0; xPixel < WindowXY; xPixel++)
				{
					for (yPixel = 0; yPixel < WindowXY; yPixel++)
					{   // Geeft voor de X en Y pixels de x-Coördinaat en y-Coördinaat en rekent het Mandelgetal uit
						double xCoordinaat = ((xMid - DeltaXY / 2) + (DeltaXY / WindowXY) * xPixel);
						double yCoordinaat = ((yMid + DeltaXY / 2) - (DeltaXY / WindowXY) * yPixel);
						int Number = MandelNumber(xCoordinaat, yCoordinaat);
						// Geeft elke pixel een Kleur naar de methode van Kleurmaker
						MandelBitmap.SetPixel(xPixel, yPixel, ColorTable(Number));
					}
				}
				// Stopt stopwatch aan het einde van de bitmap berekening
				Duration.Stop();
				L_Timer.Text = Convert.ToString(Duration.ElapsedMilliseconds) + " msec";

				// Invalidate bitmap
				MandelPicture.Image = MandelBitmap;
				this.Controls.Add(MandelPicture);
			}
		}

		// Groene laadcircle
		private void LoadCircle(object sender, PaintEventArgs pea)
		{
			pea.Graphics.FillEllipse(Brushes.LightGreen, 260, 37, 20, 20);
		}

		void ClickZoom(object sender, MouseEventArgs e)
		{   // Slaat de waardes op voor de 'Undo-Knop'
			UndoX = Convert.ToString(xMid);
			UndoY = Convert.ToString(yMid);
			UndoZoom = Convert.ToString(Zoom);
			UndoiMax = Convert.ToString(iMax);

			// Rekent de nieuwe xMid en yMid uit met de muispositie en zoomt in/out of neemt nieuwe midden-waarde
			int CursorX = e.X;
			int CursorY = e.Y;

			xMid = ((xMid - DeltaXY / 2) + (DeltaXY / WindowXY) * CursorX);
			yMid = ((yMid + DeltaXY / 2) - (DeltaXY / WindowXY) * CursorY);
			if (e.Button == MouseButtons.Right)
				Zoom--;
			else if (e.Button == MouseButtons.Left)
				Zoom++;
			else // Zet alleen nieuw middelpunt bij Mousebutton3 klik zonder zoom
			// Geeft feedback wat de waardes van de window zijn in de tekstvelden
			TB_xMid.Text = Convert.ToString(xMid);
			TB_yMid.Text = Convert.ToString(yMid);
			TB_Zoom.Text = Convert.ToString(Zoom);

			MandelPicture.Invalidate();
			this.Invalidate();
		}

		// Methode die het Mandelgetal uitrekent
		int MandelNumber(double a, double b)
		{
			double NewA, x = a, y = b;
			int i;
			// Gaat met waardes door de loop tot de maximum iteratie als er niet onderhand 2 (of hoger) uit komt ('> 4' omdat er geen wortel van genomen wordt)
			for (i = 1; !((a * a + b * b) > 4 || i == iMax); i++)
			{
				NewA = a * a - b * b + x;
				b = 2 * a * b + y;
				a = NewA;
			}
			return (i);
		}

		int FileLocation(string FileName)
		{
			int Length = savefile.Count / 5;                    // We cycelen er per 5 dingen doorheen, omdat er per 5 dingen een string is, die altijd in dezelfde volgorde staan
			for (int t = 0; Length > t; t++)
			{                            // Voor iedere 5 elementen in de lijst kijken we of de string er tussen zit
				if (FileName == savefile[t * 5])				// Als we de string gevonden hebben, dan geven we de locatie door
					return (t * 5);
			}
			return -1;                                          // Als hij er niet in staat, dan returnen we -1 zodat we weten dat er een error is
		}

		// Bij deze methode word een kleurenspectrum gebruikt van wel 90 verschillende kleuren. Vandaar mod 90
		private Color ColorTable(int Number)                                                    
		{
			double Scope = 15, color = (Number % Scope) * 17;                              // Hij maakt daarbij gebruik van 6 verschillende kleurenspectra, hier bepalen we het nummer van ieder kleurenspectra.
			const int Mod = 90;                                                               // 90 kleuren dus mod 90

			if (Number == iMax)                                                         // Het deel van het mandelfiguur is zwart
			{
				Color Costum = Color.Black;
				return Costum;
			}
			else if (Number % Mod < (Scope * 1) && Number % Mod >= 0)                        // Dit deel gaat van de kleur paars richting de kleur rood
			{
				Color Costum = Color.FromArgb(255, (int)color, 0);
				return Costum;
			}
			else if (Number % Mod >= (Scope * 1) && Number % Mod < (Scope * 2))         // Dit deel gaat van de kleur rood richting de kleur geel
			{
				Color Costum = Color.FromArgb(255 - (int)color, 255, 0);
				return Costum;
			}
			else if (Number % Mod >= (Scope * 2) && Number % Mod < (Scope * 3))         // Dit deel gaat van de kleur geel richting de kleur groen
			{
				Color Costum = Color.FromArgb(0, 255, (int)color);
				return Costum;
			}
			else if (Number % Mod >= (Scope * 3) && Number % Mod < (Scope * 4))         // Dit deel gaat van de kleur groen richting de kleur licht blauw
			{
				Color Costum = Color.FromArgb(0, 255 - (int)color, 255);
				return Costum;
			}
			else if (Number % Mod >= (Scope * 4) && Number % Mod < (Scope * 5))         // Dit deel gaat van de kleur licht blauw naar de kleur donker blauw
			{
				Color Costum = Color.FromArgb((int)color, 0, 255);
				return Costum;
			}
			else if (Number % Mod >= (Scope * 5) && Number % Mod < (Scope * 6))         // Dit deel gaat van de kleur donker blauw richting de kleur paars
			{
				Color Costum = Color.FromArgb(255, 0, 255 - (int)color);
				return Costum;
			}
			return Color.Black;                                                     // Voor het geval dat er een foute input is gegeven gebruikt hij de kleur black.
		}
		
		static void Main()
		{
			Application.Run(new Scherm());
		}
		

        void Voorbeelden()
        {   // Maakt een paar voorbeeldplaatjes aan
            savefile.Add("Mandelbrot (90)");
            savefile.Add("90");
            savefile.Add("0");
            savefile.Add("0");
            savefile.Add("1");

            savefile.Add("Spiral (900)");
            savefile.Add("900");
            savefile.Add("-0,103836466471354");
            savefile.Add("0,923883972167968");
            savefile.Add("19");

            savefile.Add("Klavers (5000)");
            savefile.Add("5000");
            savefile.Add("-0,03939224243164");
            savefile.Add("-0,78481038411458");
            savefile.Add("19");

            savefile.Add("Hypnoses (25000)");
            savefile.Add("25000");
            savefile.Add("-0,0393918228149408");
            savefile.Add("-0,784810784657793");
            savefile.Add("22");

            savefile.Add("Octopus (900)");
            savefile.Add("900");
            savefile.Add("-1,76961730957031");
            savefile.Add("0,00352289835611926");
            savefile.Add("18");
        }
        Button Button_Undo()
        {
            Button Undo = new Button();
            Undo.Location = new Point(150, 70);
            Undo.Size = new Size(50, 20);
            Undo.Text = "Undo";
            this.Controls.Add(Undo);
            return Undo;
        }

        Button Button_Ok()
        {
            Button Ok = new Button();
            Ok.Location = new Point(150, 100);
            Ok.Size = new Size(50, 20);
            Ok.Text = "Ok";
            this.Controls.Add(Ok);
            return Ok;
        }

        Button Button_Save()
        {
            Button Save = new Button();
            Save.Location = new Point(370, 70);
            Save.Size = new Size(60, 20);
            Save.Text = "Save";
            Controls.Add(Save);
            return Save;
        }

        Button Load_file()
        {
            Button Load = new Button();
            Load.Location = new Point(260, 100);
            Load.Size = new Size(170, 20);
            Load.Text = "Load";
            this.Controls.Add(Load);
            return Load;
        }

        TextBox Textbox_xMid()
        {
            TextBox TB_xMid = new TextBox();
            TB_xMid.Location = new Point(90, 10);
            TB_xMid.Size = new Size(110, 30);
            TB_xMid.Text = Convert.ToString(xMid);
            this.Controls.Add(TB_xMid);
            return TB_xMid;
        }

        TextBox Textbox_yMid()
        {
            TextBox TB_yMid = new TextBox();
            TB_yMid.Location = new Point(90, 40);
            TB_yMid.Size = new Size(110, 30);
            TB_yMid.Text = Convert.ToString(yMid);
            this.Controls.Add(TB_yMid);
            return TB_yMid;
        }

        TextBox Textbox_Zoom()
        {
            TextBox TB_Zoom = new TextBox();
            TB_Zoom.Location = new Point(90, 70);
            TB_Zoom.Size = new Size(50, 30);
            TB_Zoom.Text = Convert.ToString(Zoom);
            this.Controls.Add(TB_Zoom);
            return TB_Zoom;
        }

        TextBox Textbox_iMax()
        {
            TextBox TB_iMax = new TextBox();
            TB_iMax.Location = new Point(90, 100);
            TB_iMax.Size = new Size(50, 20);
            TB_iMax.Text = Convert.ToString(iMax);
            this.Controls.Add(TB_iMax);
            return TB_iMax;
        }

        TextBox Saveinput()
        {
            TextBox TB_Save = new TextBox();
            TB_Save.Location = new Point(260, 70);
            TB_Save.Size = new Size(100, 20);
            this.Controls.Add(TB_Save);
            return TB_Save;
        }

        ListBox Listbox_SavedFile()
        {
            ListBox LB_Savedfile = new ListBox();
            LB_Savedfile.Location = new Point(510, 10);
            LB_Savedfile.Size = new Size(120, 120);

            LB_Savedfile.Items.Add("Mandelbrot (90)");
            LB_Savedfile.Items.Add("Spiral (900)");
            LB_Savedfile.Items.Add("Klavers (5000)");
            LB_Savedfile.Items.Add("Hypnoses (25000)");
            LB_Savedfile.Items.Add("Octopus (900)");

            this.Controls.Add(LB_Savedfile);
            return LB_Savedfile;
        }

        void Label_xMid()
        {
            Label L_xMid = new Label();
            L_xMid.Location = new Point(30, 10);
            L_xMid.Size = new Size(60, 30);
            L_xMid.Text = "X Midden:";
            this.Controls.Add(L_xMid);
        }

        void Label_yMid()
        {
            Label L_yMid = new Label();
            L_yMid.Location = new Point(30, 40);
            L_yMid.Size = new Size(60, 20);
            L_yMid.Text = "Y Midden:";
            this.Controls.Add(L_yMid);
        }

        void Label_Zoom()
        {
            Label L_Zoom = new Label();
            L_Zoom.Location = new Point(30, 70);
            L_Zoom.Size = new Size(40, 30);
            L_Zoom.Text = "Zoom:";
            this.Controls.Add(L_Zoom);
        }

        void Label_iMax()
        {
            Label L_iMax = new Label();
            L_iMax.Location = new Point(30, 100);
            L_iMax.Size = new Size(60, 20);
            L_iMax.Text = "Iteraties:";
            this.Controls.Add(L_iMax);
        }

        void Label_Error()
        {
            L_Error.Location = new Point(260, 10);
            L_Error.Size = new Size(170, 20);
            L_Error.Text = "Error: Geen";
            this.Controls.Add(L_Error);
        }

        void Label_Timer()
        {
            L_Timer.Location = new Point(290, 40);
            L_Timer.Size = new Size(70, 20);
            this.Controls.Add(L_Timer);
        }
    }
}




