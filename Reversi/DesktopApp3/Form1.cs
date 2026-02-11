using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace DesktopApp3 
{
	public partial class Form1 : Form
	{   // Met deze variabelen wordt de grootte van het speelbord veranderd en het aantal vakjes
		static int BoardSizeY = 6, BoardSizeX = 6; // Moet hoger zijn dan 2

		// Globale ints e.d. Bordarray voor de basale reversieberekeningen
		int maxpicturesize = 700, x, y, xField = 0, yField = 0, FieldSize, BlueStones = 0, RedStones = 0, Xoff, Yoff, AmountOfValidMoves, laag = 0, minFieldSize, locationt;
        public string[,] Board = new string[BoardSizeY, BoardSizeX]; public string[,] OldBoard = new string[BoardSizeY, BoardSizeX];
        double Paintedhokjes, MoveValue;
        Boolean AImove = false, DoeZet = false, Kangeenzetdoen = false, ShowValidMoves = false, StartBoard = true;
        string Player = "r", Opponent = "b", AIPlayer;
        Bitmap BitBoard;
		List<double> tijdelijk = new List<double>();
		
		// Managen van de AI waardes in vooral stacks voor efficiëntie 
        Stack<double> maxwaardesstack = new Stack<double>();
        Stack<List<double>> lijsten = new Stack<List<double>>(); 
        public Form1 ShallowCopy() { return (Form1)this.MemberwiseClone(); }
        Stack<string> probeer = new Stack<string>();
        Stack<string[,]> Bordstack = new Stack<string[,]>();
        List<List<int>> maxwaardes = new List<List<int>>();
        List<int> waardes = new List<int>(), u = new List<int>();
		List<string[,]> opslaanborden = new List<string[,]>();
        List<Stack<List<double>>> stackopslag = new List<Stack<List<double>>>();
        List<Stack<double>> maxwaardesstacklist = new List<Stack<double>>();

        // Bepaalt grootte van het GUI speelbord (Een bitmap wordt op de picturebox getekent)
		// Hij probeert hokjes van 60 pixels te tekenen maar als dat niet past worden ze kleiner
		void NewBoardSize()
        {
            if (BoardSizeY * 60 > maxpicturesize || BoardSizeX * 60 > maxpicturesize)
            {
                if (BoardSizeY < BoardSizeX)
                    FieldSize = maxpicturesize / BoardSizeX;
                else FieldSize = maxpicturesize / BoardSizeY;
            }
            else FieldSize = 60;
			minFieldSize = FieldSize;

			pictureBox1.Size = new Size(FieldSize * BoardSizeX + 1, FieldSize * BoardSizeY + 1);
			int ClientSizeX = pictureBox1.Width + 2 * pictureBox1.Location.X;
			int ClientSizeY = pictureBox1.Height + pictureBox1.Location.Y + pictureBox1.Location.X;
			this.ClientSize = new Size(ClientSizeX, ClientSizeY);

			// Zet knoppen op goede plek
			button2.Location = new Point(2 * pictureBox1.Location.X + pictureBox1.Width - button1.Width, button2.Location.Y);
			button3.Location = new Point(2 * pictureBox1.Location.X + pictureBox1.Width - button1.Width, button3.Location.Y);
		}

		// Tekent de lijnen van het speelbord en alleen bij de situatie van een nieuw bord tekenen doet hij de beginsituatie invullen
		void DrawBoard(object obj, PaintEventArgs pea)
		{
			if (StartBoard == true)
			{
				NewBoard();
			}
			BitBoard = new Bitmap(BoardSizeX * FieldSize + 1, BoardSizeY * FieldSize + 1);

			for (int x = 0; x < FieldSize * BoardSizeX + 1; x += FieldSize)
			{
				for (int y = 0; y < BoardSizeY * FieldSize; y++)
				{
					BitBoard.SetPixel(x, y, Color.Black);
				}
			}
			for (int y = 0; y < FieldSize * BoardSizeY + 1; y += FieldSize)
			{
				for (int x = 0; x < BoardSizeX * FieldSize; x++)
				{
					BitBoard.SetPixel(x, y, Color.Black);
				}
			}
			// Roept Drawstones aan om de stenen te tekenen in de grid en invalidate
			DrawStones();
			pictureBox1.Invalidate();
			this.Controls.Add(pictureBox1);
		}

		// Vult de array met het beginspeelbord en kiest een random speler die mag beginnen
		void NewBoard()
        {
			StartBoard = false;
            NewBoardSize();
			// For loop maakt elke waarde in de array een onbezet vakje
            for (x = 0; x < BoardSizeX; x++)
            {
                for (y = 0; y < BoardSizeY; y++)
                    Board[y, x] = "w";
            }
            Board[BoardSizeY / 2, BoardSizeX / 2] = "b";
            Board[BoardSizeY / 2 - 1, BoardSizeX / 2] = "r";
            Board[BoardSizeY / 2, BoardSizeX / 2 - 1] = "r";
            Board[BoardSizeY / 2 - 1, BoardSizeX / 2 - 1] = "b";
			
			// Kiest random speler en update label die aangeeft wie er aan de beurt is
			Random rnd = new Random();
			int beginspeler = rnd.Next(2);
			if (beginspeler == 0)
			{
				Player = "r";
				Opponent = "b";
				if (Player == "b")
					label1.Text = "Speler blauw is aan de beurt.";
				else label1.Text = "Speler rood is aan de beurt.";
			}
			else SwapPlayer();
		}

        // Tekent de stenen in de picturebox grid en update de tussenstand van stenen  
        private void DrawStones()
        {
			SearchBoard();
			// For loop gaat door complete bordarray en leest elke waarde uit
            for (xField = 0; xField < BoardSizeX; xField++)
            {
                for (yField = 0; yField < BoardSizeY; yField++)
                {
					Graphics Bit = Graphics.FromImage(BitBoard);
					// StringToColor() leest waardes van array uit en geeft de correcte brushkleur terug
					Bit.FillEllipse(StringToColor(), xField * FieldSize + 1, yField * FieldSize + 1, FieldSize-2, FieldSize-2);
				}
            }
            Tussenstand();
            pictureBox1.Image = BitBoard;
        }

		// De uitgelezen waardes van de array geven een brushkleur voor de stenen
		private Brush StringToColor()
		{
			if (Board[yField, xField] == "r")
				return Brushes.Red;
			else if (Board[yField, xField] == "b")
				return Brushes.Blue;
			else if (Board[yField, xField] == "y" && ShowValidMoves && Player == "r")
				return Brushes.LightPink;
			else if (Board[yField, xField] == "y" && ShowValidMoves && Player == "b")
				return Brushes.LightBlue;
			else return Brushes.White;
		}

		// Muisevent voor als je op het speelbord klikt die uitrekened op velk vakje dat gebeurd, Als het geklikte vakje een valid move is doet hij de zet
		// anders geeft hij aan dat de speler geen zet kan doen
		private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
		{
			// Zoekt uit wat alle valid moves zijn
			SearchBoard();

			xField = (e.X / FieldSize);
			yField = (e.Y / FieldSize);
			if (AmountOfValidMoves > 0)
			{
				if (Board[yField, xField] == "y")
				{
					Kangeenzetdoen = false;
					AmountOfValidMoves = 0;

					DoeZet = true;
					FieldOffSet();
					DoeZet = false;
					SwapPlayer();
					// Zet is gedaan, alle valid moves worden weggedaan en de stenen van de nieuwe bordsituatie
					RemoveValidFields();
					DrawStones();
					this.Invalidate();
				}
			}
			// Als er geen valid moves zijn dan swapt hij de speler, als beide spelers geen zet kunnen doen wordt de tussenstand uitgerekend
			else
			{
				if (Kangeenzetdoen == true)
				{
					if (BlueStones > RedStones)
						label1.Text = "Blauw wint";
					else if (RedStones > BlueStones)
						label1.Text = "Rood wint";
					else label1.Text = "Remise";
				}
				else
				{
                    label1.Text = "Je kunt geen zet doen";
                    Kangeenzetdoen = true;
                    SwapPlayer();
                    SearchBoard();
                    this.Update();
                }
			}
			if (AIPlayer == Player)
			{	//De AI button is een toggle gemaakt, dus als je op de knop indrukt dan blijft die speler dus ook AI
				pictureBox1.Update();
				button3_MouseClick(null, null);
			}
		}

		// Bij het klikken van nieuw spel wordt de beginsituatie weer getekend
		private void button1_MouseClick(object sender, MouseEventArgs e)
        {
			StartBoard = true;
			Paint += DrawBoard;
			this.Invalidate();
		}

		// Als de 'Help' knop ingedrukt wordt laat hij ook alle mogelijke zetten zien
		private void button2_MouseClick(object sender, MouseEventArgs e)
        {
            SearchBoard();
			// Zorgt voor togglefunctie en tekent daarna de valid moves op het bord
			if (ShowValidMoves == true)
				ShowValidMoves = false;
			else ShowValidMoves = true;

            DrawStones();
        }

		// Vervangt alle valid moves velden met lege vakjes
		private void RemoveValidFields()
        {
            for (x = 0; x < BoardSizeX; x++)
            {
                for (y = 0; y < BoardSizeY; y++)
                {
                    if (Board[y, x] == "y")
                        Board[y, x] = "w";
                }
            }
        }

		// Wisselt speler die aan zet is om en update de labels
		private void SwapPlayer()
        {
            string Temp = Opponent;
            Opponent = Player;
            Player = Temp;
            if (Player == "b")
                label1.Text = "Speler blauw is aan de beurt.";
            else label1.Text = "Speler rood is aan de beurt.";
        }
		
		// Deze methode doet een zet. Als er op een valid move hokje geklikt is dan kijkt hij om zich heen of er een enemy steen is
		// als dit het geval is dan neemt hij die offset en kijkt hij waar er enemy stenen worden ingesloten door 2 player stenen 
        private void Plooi(int Xoff, int Yoff)
        {
			if (Board[yField + Yoff, xField + Xoff] == Opponent)
			{
				int xVerschil = Xoff;
				int yVerschil = Yoff;

				int Xhokje = xVerschil + xField;
				int Yhokje = yVerschil + yField;
				if (Yhokje >= 0 && Xhokje >= 0 && Yhokje < BoardSizeY && Xhokje < BoardSizeX)
				{	// Als er een steen van de enemy is dan telt hij de offset er weer bij op voor het nieuwe vakje
					while (Board[Yhokje, Xhokje] == Opponent)
					{
						Xhokje += xVerschil;        
						Yhokje += yVerschil;
						// Controleert of nieuwe waardes binnen de arraybounds zijn (try catch ontwijken)
						if (Yhokje >= 0 && Xhokje >= 0 && Yhokje < BoardSizeY && Xhokje < BoardSizeX) ;
						else break;
					}
				}
				// Hier komt hij een player steen tegen en weet hij dus dat hier een lijn verbonden wordt, hij telt de offset er weer vanaf om terug te gaan naar
				// het valid move hokje en zet alles ertussen naar de spelersteen op de array
				if (Yhokje >= 0 && Xhokje >= 0 && Yhokje < BoardSizeY && Xhokje < BoardSizeX)
				{
					if (Board[Yhokje, Xhokje] == Player)
					{
						Xhokje -= xVerschil;
						Yhokje -= yVerschil;
						if (Yhokje >= 0 && Xhokje >= 0 && Yhokje < BoardSizeY && Xhokje < BoardSizeX)
							while (Board[Yhokje, Xhokje] == Opponent)
							{
								Board[Yhokje, Xhokje] = Player;
								Xhokje -= xVerschil;
								Yhokje -= yVerschil;
							}
					}
				}
			} 
        }

		// Gaat door het hele bordarray en rekent valid moves uit in FieldOffSet()
		private void SearchBoard()  
        {
			for (xField = 0; xField < BoardSizeX; xField++)
			{
				for (yField = 0; yField < BoardSizeY; yField++)
				{
					FieldOffSet();
				}
			}
            pictureBox1.Image = BitBoard;
        }

		// Kijkt naar de hokjes om het hokje heen en doet er iets mee als hij een valid move tegenkomt of een speler steen
		void FieldOffSet()
        {
            Paintedhokjes = 0;
            if (Board[yField, xField] == Player || Board[yField, xField] == "y") 
            {   
                // Als er daadwerkelijk een zet gedaan wordt dan maakt hij het geklikte vakje van de valid move de kleur van de speler en haalt hij alle valid moves verder weg
                if (DoeZet)
                {
                    Board[yField, xField] = Player;
                    RemoveValidFields();
                }
                // Hier kijkt hij naar de 3x3 om het hokje
                for (Xoff = -1; Xoff < 2; Xoff++)
                {
                    for (Yoff = 1; Yoff > -2; Yoff--)
                    {   // Vangnet voor moves die out of range zijn van het bord
                        if (yField + Yoff >= 0 && xField + Xoff >= 0 && yField + Yoff < BoardSizeY && xField + Xoff < BoardSizeX)
                        {
                            // Doet werkelijk een zet en verandert kleuren in methode Plooi
                            if (DoeZet)
                                Plooi(Xoff, Yoff);
                            // Als hij alleen de valid moves wil vinden dan rekent hij alleen vanuit de stenen van de speler
                            else if (Board[yField, xField] == Player)
                                ValidFinder(Xoff, Yoff);
                        }
                    }
                }
            }
        }

        void ValidFinder(int Xoff, int Yoff)
        {
            // vervolgens kijkt hij voor alle pixels daaromheen of er "enemy stenen zijn"
            if (Board[yField + Yoff, xField + Xoff] == Opponent)
            {
                int xVerschil = Xoff;
                int yVerschil = Yoff;

                int Xhokje = xVerschil + xField;
                int Yhokje = yVerschil + yField;
                if (Yhokje >= 0 && Xhokje >= 0 && Yhokje < BoardSizeY && Xhokje < BoardSizeX) //kijkt of hij binnen het bord valt
                {
                    while (Board[Yhokje, Xhokje] == Opponent) // als dat het geval is dan kijkt hij of daar een valid move is
                    {
                        Xhokje += xVerschil;
                        Yhokje += yVerschil;
                        if (Yhokje >= 0 && Xhokje >= 0 && Yhokje < BoardSizeY && Xhokje < BoardSizeX)
                        {
                            if (Board[Yhokje, Xhokje] == "w")
                            {
                                Board[Yhokje, Xhokje] = "y";
                                AmountOfValidMoves++;
                            }
                        }
                        else break;
                    }
                }
            }
        }

		// Rekent de tussenstand uit en update de labels 
		void Tussenstand()
        {
            BlueStones = 0;
            RedStones = 0;
            for (xField = 0; xField < BoardSizeX; xField++)
            {
                for (yField = 0; yField < BoardSizeY; yField++)
                {
                    if (Board[yField, xField] == "b")
                        BlueStones++;
                    else if (Board[yField, xField] == "r")
						RedStones++;
                }
            }
			label2.Text = $"Speler blauw heeft {BlueStones} stenen";
			label3.Text = $"Speler rood heeft {RedStones} stenen";
		}

		// Bij het indrukken van de knop wordt de CPU speler bepaald en deze doet daarna de zetten
        private void button3_MouseClick(object sender, MouseEventArgs e)
        {
            DrawStones();
            pictureBox1.Invalidate();

            AIPlayer = Player;
            SearchBoard();
            bool Kanzetdoen = false;
			// Als er in het bord een valid zet is geeft hij dit aan met een boolean
            for(int x = 0; x < BoardSizeX; x++)
                for(int y = 0; y < BoardSizeY; y++)
                    if (Board[y, x] == "y")
                        Kanzetdoen = true;

            if (Kanzetdoen == true) // Als er een valid zet is dan 
            {
                Kangeenzetdoen = false;
                for (int t = 0; t < Maxlaag; t++) // Hij vult de stack op, zodat er overal een lege lijst instaat waar hij waardes in kan doen
                {
                    List<double> opvullen = new List<double>();
                    lijsten.Push(opvullen);
                }
				
                double MoveValue = AIrecur1();
                xFieldyField(locationt); // Zoekt welke xfield en yfield er bij die zet horen

                DrawStones();
            }
            else
			{
                if (Kangeenzetdoen == true) // Kijkt of de AI een zet kan doen of niet
                {
                    if (BlueStones > RedStones)
                        label1.Text = "Blauw wint";
                    else if (RedStones > BlueStones)
                        label1.Text = "Rood wint";
                    else
                        label1.Text = "Remise";
                }
                else
                {
                    label1.Text = "Je kunt geen zet doen";
                    Kangeenzetdoen = true;
                    SwapPlayer();
                    SearchBoard();
                    this.Update();
                }
            }
        }
		// Deze functie die koppelt coördinaten aan de zet die de AI doet en doet hij de zet zelf
        void xFieldyField(int t)
		{
            int teller = 0;
            Boolean zekerheid = true;
            for (xField = 0; xField < BoardSizeX; xField++) // Hij gaat door het hele bord heen en kijkt hoeveel valid moves hij tegenkomt
            {
                for (yField = 0; yField < BoardSizeY; yField++)
                {
                    if (Board[yField, xField] == "y")
                    {
                        if (t == teller) // Als hij een valid move tegenkomt die overeenkomt met de locatie in de lijst dan doet hij een zet
						{
							DoeZet = true;
                            FieldOffSet();
							DoeZet = false;
                            SwapPlayer();
                            zekerheid = false;
                        }
                        teller++;
                    }
                }
            }
            if (zekerheid) // Als hij geen zet kan doen moet hij alsnog de speler swappen
            {
                SwapPlayer();
            }
        }


		private void PopArray() //Bovenste element in de stack wordt het speelbord
		{
			for (x = 0; x < BoardSizeX; x++)
			{
				for (y = 0; y < BoardSizeY; y++)
				{
					Board[y, x] = Bordstack.Peek()[y,x];
				}
			}
		}

		private void AddArray()
		{
            string[,] Copy = new string[BoardSizeY, BoardSizeX]; //Hier maakt hij een nieuwe array en pusht hem op de stack, 
                        //vanwege het feit dat de refference anders ook wordt gecopieerd moet hier een apparte metode voor worden aangemaakt
			for (x =0; x < BoardSizeX; x++)
			{
				for (y = 0; y < BoardSizeY; y++)
				{
					Copy[y, x] = Board[y,x] ;
				}
			}
			Bordstack.Push(Copy);
		}

		uint Maxlaag = 3;                   //Maxlaag niet aanpassen.

        private double AIrecur1() //dit is de AI, hij heet AIrecur omdat hij gebruik maakt van recursie. Dit is een minimax AI met hoeken als sterk veld
        {
            /*
            Bij de Minimax AI maken we gebruik van 3 verschillende stacks, een van deze stacks is voor het opslaan van borden, een van deze stacks is voor
            het opslaan van alle uitkomsten en een van deze stacks is voor het opslaan van de waarde die bij de zet zelf hoort. 
            De AI berekend voor ieder bord de waaarde van de zet en returned als de speler de opponent is het laagste en als de speler de speler is het
            hoogste getal.
            */
			double Hoogste;
            AddArray(); 
            tijdelijk = geelfinder(); //hier vraagt hij om een lijst van alle coördinaten van gele punten en daaraan koppelt hij een waarde

			int aantal = tijdelijk.Count / 3;

            if (AIPlayer == Player)
                Hoogste = Hoogstewaarde3(tijdelijk); //Voor de speler die de AI is returned hij de hoogste max waarde, anders de minimale waarde voor de tegenstander
            else Hoogste = laagstewaarde3(tijdelijk);
            if (laag < (Maxlaag))
            {	
				// For loop doet een zet met de x en y coordinaat van de valid moves lijst
                for (int t = 0; t < aantal; t++)
                {
                    tijdelijk = geelfinder();
                    yField = (int)tijdelijk[3 * t];
                    xField = (int)tijdelijk[3 * t + 1];
					
                    DoeZet = true;
                    FieldOffSet();
                    DoeZet = false;
					// Gaat laag verder en rekent bord uit voor nieuwe speler
                    laag++;
                    SwapPlayer();
                    SearchBoard();

                    maxwaardesstack.Push(tijdelijk[3 * t + 2]);
                    double stackwaarde = AIrecur1();  // Roept eigen functie opnieuw aan

                    MoveValue = maxwaardesstack.Peek();

                    List<double> nieuw = new List<double>();

                    if (lijsten.Count > laag)
                    {
                        nieuw = lijsten.Pop(); //als er net een lijst verwijderd is moet er een nieuwe lijst bijkomen, als de lijst er dan vanaf haalt gaat het dus mis
                    }

					//hier wordt de hoogste/laagste waarde aan de lijst met maxwaardes toegevoegd, afhankelijk van het feit of het voor de speler of zijn opponent is.
					nieuw.Add(stackwaarde);
                    lijsten.Push(nieuw); 
                    maxwaardesstack.Pop();
                    Bordstack.Pop();
                    PopArray(); //vervolgens gebeurd hier nog wat stack administratie

                    laag--; // Er wordt 1 laag omlaag gegaan in de boom van borden en speler geswapt 
                    SwapPlayer(); 
                }
                if (aantal == 0)
                {	// Als er geen valid move is swapt hij speler en geeft hij waarde 0 aan de zet en dan gaat hij verder met de recursie
                    laag++;
                    SwapPlayer();
                    SearchBoard();

                    maxwaardesstack.Push(0);

                    double stackwaarde = AIrecur1();

                    MoveValue = 0;

                    List<double> nieuw = new List<double>();

                    if (lijsten.Count > laag)
                    {
                        nieuw = lijsten.Pop();
                    }

                    nieuw.Add(stackwaarde);

                    lijsten.Push(nieuw);

                    stackopslag.Add(lijsten);

                    maxwaardesstack.Pop();

                    Bordstack.Pop();
                    PopArray();
                    laag--;
                    SwapPlayer();
                }
            }
			if (laag == Maxlaag) //op het moment dat je bij de maximum laag komt dan returned hij de hoogste of laagste waarde van alle zetten van de laag eronder.
			{
				if (AIPlayer != Player)
				{
					return (laagstewaarde3(tijdelijk));
				}
				return Hoogste;
			}

			else
			{
				List<double> nieuw = new List<double>();

				nieuw = lijsten.Pop();

				double veroogste = Hoogstewaarde(nieuw);
				double returnvalue = veroogste + MoveValue; 
				if (laag == 0)                 //hier wordt het zetnummer bepaald van de beste zet. 
				{
					locationvant(nieuw);
				}
				if (AIPlayer == Player)
					return veroogste + MoveValue; // hier returned hij de hoogste waarde als de speler aan zet is.

				else
				{
					double laagstwaarde = laagstewaarde(nieuw); // hier returned hij de laagste waarde als de opponent aan zet is.
					return laagstwaarde + MoveValue;
				}
		    }
		}
        double laagstewaarde(List<double> axwaardes) //dit is de functie om de laagste waarde uit een lijst te halen
        {
            int aantal = axwaardes.Count;
            if (aantal == 0)
                return 0;
            double Laagste = axwaardes[0];
            for (int t = 0; t < aantal; t++) //hier gaat hij door de hele lijst heen
            {
                if (axwaardes[t] < Laagste)
                {
                    Laagste = axwaardes[t];
                }
            }
            return Laagste;
        }

		double laagstewaarde3(List<double> axwaardes) //dit is de functie om de laagste waarde uit de lijst te halen waar de coördinaten ook in staan
		{
			int aantal = axwaardes.Count / 3;

			double Laagste = 0;

			for (int t = 0; t < aantal; t++) //hier gaat hij dus weer door de hele lijst heen
			{
				if (axwaardes[3 * t + 2] <= Laagste)
				{
					Laagste = axwaardes[3 * t + 2];
				}
			}
			return Laagste;
		}

		double Hoogstewaarde3(List<double> axwaardes) //dit is de functie om de hoogste waarde uit de lijst te halen waar de coördinaten ook in staan
		{
			int aantal = axwaardes.Count / 3;
            
            double Hoogste = 0;

			for (int t = 0; t < aantal; t++)
			{
				if (axwaardes[3 * t + 2] >= Hoogste)
				{
					Hoogste = axwaardes[3 * t + 2];
				}
			}
			return Hoogste;
            
		}
		double Hoogstewaarde(List<double> axwaardes)//dit is de functie waarbij de hoogste waarde  uit een lijst wordt gehaald
		{
			int aantal = axwaardes.Count;
            if (aantal == 0)
                return 0;
			double Hoogste = axwaardes[0];
			for (int t = 0; t < aantal; t++)
			{
				if (axwaardes[t] > Hoogste)
				{
					Hoogste = axwaardes[t];
				}
			}
			return Hoogste;
		}

		//hier wordt de plaats van de beste zet terugherleid
		void locationvant(List<double> maxgetallen)
		{ 
            int aantal = maxgetallen.Count;
            if (aantal == 0)
			{
			}
            else
            {
                double Hoogste = maxgetallen[0];
                for (int t = 1; t < aantal; t++)
                {
                    if (maxgetallen[t] > Hoogste)
                    {
                        Hoogste = maxgetallen[t];
                        locationt = t;
                    }
                }
                if (Hoogste == maxgetallen[0])
                {
                    locationt = 0;
                }
            }
        }

		List<double> geelfinder() //hier maakt hij een lijst van de xcoordinaat, ycoordinaat en de value van de zet
		{
			tijdelijk.Clear();
			int AmountOfValidMoves = 0;

			PopArray();

			// forloop die vanuit elke valid move berekend hoeveel punten hij krijgt
			for (xField = 0; xField < BoardSizeX; xField++)
			{
				for (yField = 0; yField < BoardSizeY; yField++)
				{
					if (Board[yField, xField] == "y")
					{
                        Paintedhokjes = 0;
						AImove = true;
						tekenyellow2(); //hier berekend hij het aantal hokjes dat hij slaat en slaat het op in paintedhokjes
						AImove = false;

						AmountOfValidMoves++;
						tijdelijk.Add(yField);
                        tijdelijk.Add(xField);
						if (AIPlayer != Player)
							Paintedhokjes = -Paintedhokjes;

						tijdelijk.Add(Paintedhokjes);
					}
				}
			}
			return tijdelijk;
		}

		// Deze methode berekend aantal puten dat hij krijgt voor het hokje waarop hij slaat en alles wat hij ertussen kleurt
        void tekenyellow2()
        {
            if (AImove == true) //hier geeft hij een waarde aan het hokje waar hij op slaat
            {
                if ((xField == 0 && yField == 0) || (xField == BoardSizeX - 1 && yField == BoardSizeY - 1) || (yField == 0 && xField == BoardSizeX - 1) || (yField == BoardSizeY - 1 && xField == 0))
                	Paintedhokjes += 4; //een hoek is 4x zoveel als een normaal vakje waard
                else Paintedhokjes++;
            }
            for (int x = -1; x < 2; x++)
            {
                for (int y = 1; y > -2; y--)
                {
                    if (AImove == true)
                    {
                        berekenmax(x, y); // Hier doet hij alle vakjes ertussen nog optellen in een andere methode
                    }
                }
            }
        }

		private void berekenmax(int Xoff, int Yoff) // Hier berekent hij hoeveel hokjes hij in totaal slaat bij het doen van een zet
		{
            if (yField + Yoff >= 0 && xField + Xoff >= 0 && yField + Yoff < BoardSizeY && xField + Xoff < BoardSizeX)
            {
                if (Board[yField + Yoff, xField + Xoff] == Opponent)
                {
                    int xVerschil = Xoff;
                    int yVerschil = Yoff;
                    int Xhokje = xVerschil + xField;
                    int Yhokje = yVerschil + yField;
                    if (Xoff != 0 && Yoff != 0)
                    {
                        while (Yhokje >= 0 && Xhokje >= 0 && Yhokje < BoardSizeY && Xhokje < BoardSizeX && Board[Yhokje, Xhokje] == Opponent) //dit mag alleen gebeuren als hij zowel binnen het bord is, als het hokje van de opponent is.
                        {
                            Xhokje += xVerschil;
                            Yhokje += yVerschil;
                        }
                        if (Yhokje >= 0 && Xhokje >= 0 && Yhokje < BoardSizeY && Xhokje < BoardSizeX) // Dit is voor het geval hij buiten het bord komt.
                        {
                            if (Board[Yhokje, Xhokje] == Player)
                            {
                                Xhokje -= xVerschil;
                                Yhokje -= yVerschil;
                                while (Board[Yhokje, Xhokje] == Opponent) // Als dat het geval is dan kijkt hij of daar een valid move is
                                {
                                    Xhokje -= xVerschil;
                                    Yhokje -= yVerschil;
                                    Paintedhokjes++;
                                }
                            }
                        }
                    }
                }
            }
		}

		// Event die alleen in een bepaalde ratio het bord kan vergroten aan de hand van het aantal vakjes in horizontale en verticale richting
		private void Form1_SizeChanged(object sender, EventArgs e)
		{ 
			int FieldleftY, FieldleftX;
			FieldleftY = (ClientSize.Height - (pictureBox1.Location.X + pictureBox1.Location.Y + pictureBox1.Height - (BoardSizeY * FieldSize)));
			FieldleftX = (ClientSize.Width - (2 * pictureBox1.Location.X - (BoardSizeX * FieldSize)));
			
			// Nadat er gekeken wordt welke as de limiting factor is 
			if (FieldleftY <= FieldleftX && FieldSize > minFieldSize)
				FieldSize = ((ClientSize.Height - (pictureBox1.Location.Y + pictureBox1.Location.X)) / BoardSizeY);
			else FieldSize = (ClientSize.Width - 2 * pictureBox1.Location.X) / BoardSizeX;

			// Geeft grootte van het speelbord en rekent de clientsize hiervoor uit
			pictureBox1.Size = new Size(FieldSize * BoardSizeX + 1, FieldSize * BoardSizeY + 1);
			int ClientSizeX = pictureBox1.Width + 2 * pictureBox1.Location.X;
			int ClientSizeY = pictureBox1.Height + pictureBox1.Location.Y + pictureBox1.Location.X;
			this.ClientSize = new Size(ClientSizeX, ClientSizeY);
			
			this.Invalidate();
		}
		
		// Tekent rode en blauwe cirkel
		private void pictureBox2_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.FillEllipse(Brushes.Blue, 5, 0, 20, 20);
			e.Graphics.FillEllipse(Brushes.Red, 5, 30, 20, 20);
		}

		public Form1()
		{
				this.Controls.Add(pictureBox1);
				InitializeComponent();
				Paint += DrawBoard;
		}
			
		}
	}
	
		



