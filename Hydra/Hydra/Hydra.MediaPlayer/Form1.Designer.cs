﻿namespace Hydra.MediaPlayer {
	partial class Form1 {
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			flowLayoutPanel1 = new FlowLayoutPanel();
			button1 = new Button();
			button2 = new Button();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			flowLayoutPanel1.Location = new Point(-1, 54);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new Size(801, 396);
			flowLayoutPanel1.TabIndex = 0;
			// 
			// button1
			// 
			button1.Image = (Image) resources.GetObject("button1.Image");
			button1.Location = new Point(93, 12);
			button1.Name = "button1";
			button1.Size = new Size(75, 23);
			button1.TabIndex = 1;
			button1.Text = "Play";
			button1.TextImageRelation = TextImageRelation.ImageBeforeText;
			button1.UseVisualStyleBackColor = true;
			button1.Click += this.button1_Click;
			// 
			// button2
			// 
			button2.Image = (Image) resources.GetObject("button2.Image");
			button2.Location = new Point(12, 12);
			button2.Name = "button2";
			button2.Size = new Size(75, 23);
			button2.TabIndex = 2;
			button2.Text = "Pause";
			button2.TextImageRelation = TextImageRelation.ImageBeforeText;
			button2.UseVisualStyleBackColor = true;
			button2.Click += this.button2_Click;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new SizeF(7F, 15F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(800, 450);
			this.Controls.Add(button2);
			this.Controls.Add(button1);
			this.Controls.Add(flowLayoutPanel1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
		}

		#endregion

		private FlowLayoutPanel flowLayoutPanel1;
		private Button button1;
		private Button button2;
	}
}