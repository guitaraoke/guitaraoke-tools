namespace Hydra.Player2 {
	partial class TrackStrip {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			pot1 = new NAudio.Gui.Pot();
			pot2 = new NAudio.Gui.Pot();
			waveViewer1 = new NAudio.Gui.WaveViewer();
			label1 = new Label();
			this.SuspendLayout();
			// 
			// pot1
			// 
			pot1.Location = new Point(270, 3);
			pot1.Margin = new Padding(4, 3, 4, 3);
			pot1.Maximum = 1D;
			pot1.Minimum = 0D;
			pot1.Name = "pot1";
			pot1.Size = new Size(32, 28);
			pot1.TabIndex = 0;
			pot1.Value = 0.5D;
			pot1.ValueChanged += this.pot1_ValueChanged;
			pot1.DoubleClick += this.pot1_DoubleClick;
			// 
			// pot2
			// 
			pot2.Location = new Point(310, 3);
			pot2.Margin = new Padding(4, 3, 4, 3);
			pot2.Maximum = 1D;
			pot2.Minimum = 0D;
			pot2.Name = "pot2";
			pot2.Size = new Size(30, 28);
			pot2.TabIndex = 1;
			pot2.Value = 0.5D;
			pot2.DoubleClick += this.pot2_DoubleClick;
			// 
			// waveViewer1
			// 
			waveViewer1.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			waveViewer1.BackColor = Color.FromArgb(  255,   128,   0);
			waveViewer1.ForeColor = Color.FromArgb(  0,   192,   0);
			waveViewer1.Location = new Point(347, 3);
			waveViewer1.Name = "waveViewer1";
			waveViewer1.SamplesPerPixel = 128;
			waveViewer1.Size = new Size(269, 28);
			waveViewer1.StartPosition =  0L;
			waveViewer1.TabIndex = 2;
			waveViewer1.WaveStream = null;
			// 
			// label1
			// 
			label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
			label1.Location = new Point(3, 3);
			label1.Name = "label1";
			label1.Size = new Size(260, 23);
			label1.TabIndex = 3;
			label1.Text = "label1";
			// 
			// TrackStrip
			// 
			this.AutoScaleDimensions = new SizeF(7F, 15F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.Controls.Add(label1);
			this.Controls.Add(waveViewer1);
			this.Controls.Add(pot2);
			this.Controls.Add(pot1);
			this.Name = "TrackStrip";
			this.Size = new Size(619, 34);
			this.ResumeLayout(false);
		}

		#endregion

		private NAudio.Gui.Pot pot1;
		private NAudio.Gui.Pot pot2;
		private NAudio.Gui.WaveViewer waveViewer1;
		private Label label1;
	}
}
