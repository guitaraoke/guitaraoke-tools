using LibVLCSharp.Shared;
using NAudio.Gui;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Hydra.MediaPlayer {
	public partial class Form1 : Form {
		private const string PATH = @"D:\Dropbox\Creative\Guitaraoke\Tooling\TestSong";
		readonly List<Action> actions = new();
		private IWavePlayer wavePlayer;
		private List<VideoViewForm> VideoForms = new();
		public Form1() {
			InitializeComponent();
			var mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));
			foreach (var file in Directory.GetFiles(PATH, "*.mp3")) {
				var samples = new Mp3FileReader(file).ToSampleProvider();
				var label = new Label();
				label.Text = new FileInfo(file).Name + "(" + samples.WaveFormat.Channels + ")";
				var provider = new VolumeSampleProvider(samples);
				mixer.AddMixerInput(provider);
				flowLayoutPanel1.Controls.Add(label);
				var slider = new VolumeSlider();
				slider.Width = flowLayoutPanel1.Width - label.Width;
				slider.VolumeChanged += (sender, args) => provider.Volume = ((VolumeSlider) sender).Volume;
				flowLayoutPanel1.Controls.Add(slider);
			}

			

			var offset = new OffsetSampleProvider(mixer);
			offset.DelayBy = TimeSpan.FromMilliseconds(150);
			wavePlayer = new DirectSoundOut();
			wavePlayer.Init(offset);
			var screens = Screen.AllScreens;
			foreach (var file in Directory.GetFiles(PATH, "*.mp4")) {
				var videoViewForm = new VideoViewForm();
				videoViewForm.Show();
				videoViewForm.Width = 640;
				videoViewForm.Height = 360;
				videoViewForm.FormBorderStyle = FormBorderStyle.Sizable;
				//player.Location = screens[i++].Bounds.Location;
				//player.WindowState = FormWindowState.Maximized;
				videoViewForm.Init(file);
				VideoForms.Add(videoViewForm);
			}
		}

		private void button1_Click(object sender, EventArgs e) {
			var actions = VideoForms.Select<VideoViewForm, Action>(vv => () => vv.Play()).ToList();
			actions.Add(wavePlayer.Play);
			Parallel.Invoke(actions.ToArray());
		}

		private void button2_Click(object sender, EventArgs e) {
			var actions = VideoForms.Select<VideoViewForm, Action>(vv => () => vv.Pause()).ToList();
			actions.Add(wavePlayer.Pause);
			Parallel.Invoke(actions.ToArray());
		}
	}
}
