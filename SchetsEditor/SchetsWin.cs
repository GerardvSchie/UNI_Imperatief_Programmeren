using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;

namespace SchetsEditor
{
    public class SchetsWin : Form
    {   
        MenuStrip menuStrip;
        SchetsControl schetscontrol;
        ISchetsTool huidigeTool;
        Panel paneel;
        bool vast;
		ResourceManager resourcemanager
            = new ResourceManager("SchetsEditor.Properties.Resources"
                                 , Assembly.GetExecutingAssembly()
                                 );

        private void veranderAfmeting(object o, EventArgs ea)
        { 
            schetscontrol.Size = new Size ( this.ClientSize.Width  - 70
                                          , this.ClientSize.Height - 50);
            paneel.Location = new Point(64, this.ClientSize.Height - 30);
        }

		private void klikToolMenu(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((ToolStripMenuItem)obj).Tag;
        }

        private void klikToolButton(object obj, EventArgs ea)
        {
            this.huidigeTool = (ISchetsTool)((RadioButton)obj).Tag;
        }

        private void afsluiten(object obj, EventArgs ea)
        {
            this.Close();
        }
		
		public SchetsWin()
		{	// extra tools, kleuren en lijndiktes toegevoegd
            ISchetsTool[] deTools = { new PenTool()
									, new LijnTool()
									, new RechthoekTool()
									, new VolRechthoekTool()
									, new CirkelTool()
									, new VolCirkelTool()
									, new TekstTool()
									, new GumTool()
									, new DragTool()
									, new BovenopTool()
									, new OnderopTool()
									};
			String[] deKleuren = { 
								"Black", "White", "Gray", "Red", "Orange", "LightGreen", "Green", "LightBlue", "Blue"      
                                 , "Yellow", "Pink", "Magenta", "Cyan", "Purple", "DarkGray", "Firebrick" , "Chocolate" 
								 , "Salmon" , "Beige"
                                 };
			int[] lijndikte = { 3, 4, 5, 6, 7, 8, 9, 10, 11 };

            this.ClientSize = new Size(800, 800);
            huidigeTool = deTools[0];

			schetscontrol = new SchetsControl();
            schetscontrol.Location = new Point(64, 10);
            schetscontrol.MouseDown += (object o, MouseEventArgs mea) =>
                                       {   vast=true;  
                                           huidigeTool.MuisVast(schetscontrol, mea.Location); 
                                       };
            schetscontrol.MouseMove += (object o, MouseEventArgs mea) =>
                                       {   if (vast)
                                           huidigeTool.MuisDrag(schetscontrol, mea.Location); 
                                       };
            schetscontrol.MouseUp   += (object o, MouseEventArgs mea) =>
                                       {   if (vast)
                                           huidigeTool.MuisLos (schetscontrol, mea.Location);
                                           vast = false; 
                                       };
            schetscontrol.KeyPress +=  (object o, KeyPressEventArgs kpea) => 
                                       {   huidigeTool.Letter  (schetscontrol, kpea.KeyChar); 
                                       };
            this.Controls.Add(schetscontrol);

            menuStrip = new MenuStrip();
            menuStrip.Visible = false;
            this.Controls.Add(menuStrip);
            this.maakFileMenu();
            this.maakToolMenu(deTools);
            this.maakAktieMenu(deKleuren, lijndikte);
            this.maakToolButtons(deTools);
            this.maakAktieButtons(deKleuren, lijndikte);
            this.Resize += this.veranderAfmeting;
            this.veranderAfmeting(null, null);
        }

		// opslaan en openen worden aan filemenu toegevoegd wanneer er een venster open is
        private void maakFileMenu()
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("File");
            menu.MergeAction = MergeAction.MatchOnly;
			menu.DropDownItems.Add("Opslaan", null, schetscontrol.Opslaan);
			menu.DropDownItems.Add("Openen", null, schetscontrol.Openen);
			menu.DropDownItems.Add("Sluiten", null, this.afsluiten);
			menuStrip.Items.Add(menu);
        }

        private void maakToolMenu(ICollection<ISchetsTool> tools)
        {   
            ToolStripMenuItem menu = new ToolStripMenuItem("Tool");
            foreach (ISchetsTool tool in tools)
            {   ToolStripItem item = new ToolStripMenuItem();
                item.Tag = tool;
                item.Text = tool.ToString();
                item.Image = (Image)resourcemanager.GetObject(tool.ToString());
                item.Click += this.klikToolMenu;
                menu.DropDownItems.Add(item);
            }
            menuStrip.Items.Add(menu);
        }

        private void maakAktieMenu(String[] kleuren, int [] lijndikte)
        {   // verschillende aktie methodes toegevoegd 
            ToolStripMenuItem menu = new ToolStripMenuItem("Aktie");
            menu.DropDownItems.Add("Clear", null, schetscontrol.Schoon );
            menu.DropDownItems.Add("Roteer", null, schetscontrol.Roteer );
			menu.DropDownItems.Add("Lettertype", null, schetscontrol.VeranderFont );
			ToolStripMenuItem submenu = new ToolStripMenuItem("Kies kleur");
            foreach (string k in kleuren)
                submenu.DropDownItems.Add(k, null, schetscontrol.VeranderKleurViaMenu);
			ToolStripMenuItem submenu2 = new ToolStripMenuItem("Kies lijndikte");
			foreach (int l in lijndikte)
				submenu2.DropDownItems.Add(l.ToString(), null, schetscontrol.VeranderLijndikteViaMenu);
            menu.DropDownItems.Add(submenu);
			menu.DropDownItems.Add(submenu2);
			menu.DropDownItems.Add("Kleurenspectrum", null, schetscontrol.Kleurenspectrum );
			menu.DropDownItems.Add("Undo", null, schetscontrol.Undo);
			menu.DropDownItems.Add("Redo", null, schetscontrol.Redo);
			menuStrip.Items.Add(menu);
        }


        private void maakToolButtons(ICollection<ISchetsTool> tools)
        {
            int t = 0;
            foreach (ISchetsTool tool in tools)
            {
                RadioButton b = new RadioButton();
                b.Appearance = Appearance.Button;
                b.Size = new Size(45, 62);
                b.Location = new Point(10, 10 + t * 62);
                b.Tag = tool;
                b.Text = tool.ToString();
                b.Image = (Image)resourcemanager.GetObject(tool.ToString());
                b.TextAlign = ContentAlignment.TopCenter;
                b.ImageAlign = ContentAlignment.BottomCenter;
                b.Click += this.klikToolButton;
                this.Controls.Add(b);
                if (t == 0) b.Select();
                t++;
            }
        }

        private void maakAktieButtons(String[] kleuren, int[] lijndikte)
        {   // een aantal extra dingen toegevoegd aan GUI zoals labels, undo, redo, kleurenspectrum en fontkeuze (ColorDialog en FontDialog)
            paneel = new Panel();
            paneel.Size = new Size(700, 24);
            this.Controls.Add(paneel);
            
            Button b; Label l; ComboBox cbb; NumericUpDown ld;
            b = new Button(); 
            b.Text = "Clear";  
            b.Location = new Point(  0, 0); 
            b.Click += schetscontrol.Schoon; 
            paneel.Controls.Add(b);

			Button e = new Button();
			e.Image = (Image)resourcemanager.GetObject("kleuren");
			e.Size = new Size(20, 20);
			e.Location = new Point(305, 0);
			e.Click += schetscontrol.Kleurenspectrum;
			paneel.Controls.Add(e);

			Button f = new Button();
			f.Text = "Font";
			f.Size = new Size(50, f.Height);
			f.Location = new Point(420, 0);
			f.Click += schetscontrol.VeranderFont;
			paneel.Controls.Add(f);

			Button g = new Button();
			g.Image = (Image)resourcemanager.GetObject("Undo");
			g.Size = new Size(22, 22);
			g.Location = new Point(475, 0);
			g.Click += schetscontrol.Undo;
			paneel.Controls.Add(g);

			Button h = new Button();
			h.Image = (Image)resourcemanager.GetObject("Redo");
			h.Size = new Size(22, 22);
			h.Location = new Point(500, 0);
			h.Click += schetscontrol.Redo;
			paneel.Controls.Add(h);

			b = new Button(); 
            b.Text = "Rotate"; 
            b.Location = new Point( 80, 0); 
            b.Click += schetscontrol.Roteer; 
            paneel.Controls.Add(b);
            
            l = new Label();  
            l.Text = "Dikte:"; 
            l.Location = new Point(330, 3); 
            l.AutoSize = true;               
            paneel.Controls.Add(l);

			l = new Label();
			l.Text = "Penkleur:";
			l.Location = new Point(165, 3);
			l.AutoSize = true;
			paneel.Controls.Add(l);

			// Numbericupdown voor de lijndikte met range 2-60
			ld = new NumericUpDown(); ld.Location = new Point(365, 0);
			ld.Value = 5; ld.Size = new Size(50, ld.Height);
			paneel.Controls.Add(ld);
			ld.ValueChanged += schetscontrol.VeranderLijndikte;
			ld.Minimum = 2; ld.Maximum = 60;

			cbb = new ComboBox(); cbb.Location = new Point(220, 0);
			cbb.Size = new Size(80, cbb.Height);
            cbb.DropDownStyle = ComboBoxStyle.DropDownList; 
            cbb.SelectedValueChanged += schetscontrol.VeranderKleur;
            foreach (string k in kleuren)
                cbb.Items.Add(k);
            cbb.SelectedIndex = 0;
            paneel.Controls.Add(cbb);	
        }
	}
}
