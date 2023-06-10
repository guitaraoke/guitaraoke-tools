namespace Hydra.MediaPlayer {
	partial class VideoViewForm {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			videoView1 = new LibVLCSharp.WinForms.VideoView();
			((System.ComponentModel.ISupportInitialize) videoView1).BeginInit();
			this.SuspendLayout();
			// 
			// videoView1
			// 
			videoView1.BackColor = Color.Black;
			videoView1.Location = new Point(0, 0);
			videoView1.MediaPlayer = null;
			videoView1.Name = "videoView1";
			videoView1.Size = new Size(800, 450);
			videoView1.TabIndex = 0;
			videoView1.Text = "videoView1";
			// 
			// VideoViewForm
			// 
			this.AutoScaleDimensions = new SizeF(7F, 15F);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(800, 450);
			this.ControlBox = false;
			this.Controls.Add(videoView1);
			this.FormBorderStyle = FormBorderStyle.None;
			this.Name = "VideoViewForm";
			this.Text = "VideoViewForm";
			((System.ComponentModel.ISupportInitialize) videoView1).EndInit();
			this.ResumeLayout(false);
		}

		#endregion

		private LibVLCSharp.WinForms.VideoView videoView1;
	}
}