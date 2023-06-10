using LibVLCSharp.Shared;

namespace Hydra.MediaPlayer {
	public partial class VideoViewForm : Form {
		public VideoViewForm() {
			InitializeComponent();
			Core.Initialize();
		}

		public void Init(string mediaFilePath) {
			var libVLC = new LibVLC();
			videoView1.MediaPlayer = new(libVLC) {
				Media = new(libVLC, mediaFilePath)
			};
			videoView1.MediaPlayer.EnableMouseInput = false;
			videoView1.MediaPlayer.EnableKeyInput = false;
			videoView1.MediaPlayer.Volume = 0;
			videoView1.DoubleClick += ToggleFullscreen;
			this.Click += this.ToggleFullscreen;
			this.KeyPress += this.ToggleFullscreen;
		}
		

		private bool fullscreen = false;
		private Point location;
		private Size size;
		private void ToggleFullscreen(object? sender, EventArgs e) {
			if (!fullscreen) {
				var screen = Screen.FromControl(this);
				this.FormBorderStyle = FormBorderStyle.None;
				location = this.Location;
				size = this.Size;
				this.Location = screen.Bounds.Location;
				this.Size = screen.Bounds.Size;
				this.fullscreen = true;
			} else {
				this.FormBorderStyle = FormBorderStyle.Sizable;
				this.Location = location;
				this.Size = size;
				this.fullscreen = false;
			}
		}

		public bool Play() => videoView1.MediaPlayer!.Play();
		public void Pause() => videoView1.MediaPlayer!.Pause();
	}
}
