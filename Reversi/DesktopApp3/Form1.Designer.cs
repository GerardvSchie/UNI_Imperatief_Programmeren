namespace DesktopApp3
{
	partial class Form1
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(33, 148);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(481, 481);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(33, 12);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(117, 31);
			this.button1.TabIndex = 1;
			this.button1.Text = "Nieuw spel";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.button1_MouseClick);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(429, 12);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(85, 31);
			this.button2.TabIndex = 2;
			this.button2.Text = "Help";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.button2_MouseClick);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(429, 83);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(85, 45);
			this.button3.TabIndex = 3;
			this.button3.Text = "Speler wordt AI";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.MouseClick += new System.Windows.Forms.MouseEventHandler(this.button3_MouseClick);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(29, 110);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(254, 22);
			this.label1.TabIndex = 5;
			this.label1.Text = "Speler rood is aan de beurt";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(90, 57);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(46, 17);
			this.label2.TabIndex = 6;
			this.label2.Text = "label2";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(90, 83);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(46, 17);
			this.label3.TabIndex = 7;
			this.label3.Text = "label3";
			// 
			// pictureBox2
			// 
			this.pictureBox2.Location = new System.Drawing.Point(55, 54);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(30, 53);
			this.pictureBox2.TabIndex = 8;
			this.pictureBox2.TabStop = false;
			this.pictureBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox2_Paint);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(552, 652);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.pictureBox1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MinimumSize = new System.Drawing.Size(460, 300);
			this.Name = "Form1";
			this.Text = "Reversi";
			this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		public System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.PictureBox pictureBox2;
	}
	}
	


