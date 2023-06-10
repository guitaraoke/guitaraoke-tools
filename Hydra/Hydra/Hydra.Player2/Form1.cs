using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Hydra.Player2;

public partial class Form1 : Form {
	private IWavePlayer player;
	public Form1() {
		InitializeComponent();
		OpenPath(@"D:\Dropbox\Creative\Guitaraoke\6 Multitracks\");

	}

	private void toolStripButton1_Click(object sender, EventArgs e) {
		var dialog = new FolderBrowserDialog();
		if (dialog.ShowDialog() == DialogResult.OK) {
			OpenPath(dialog.SelectedPath);
		}
	}

	private void OpenPath(string path) {
		listBox1.Items.Clear();
		foreach (var songDirectory in Directory.GetDirectories(path)) {
			var name = new DirectoryInfo(songDirectory).Name;
			var item = new SongDirectory() { Name = name, Path = songDirectory };
			listBox1.Items.Add(item);
		}
	}

	private void listBox1_SelectedIndexChanged(object sender, EventArgs e) {
		var song = listBox1.SelectedItem as SongDirectory;
		LoadSong(song);
	}

	private void LoadSong(SongDirectory song) {
		if (player != null) {
			player.Stop();
			player.Dispose();
		}

		var mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));
		var info = new DirectoryInfo(song.Path);
		flowLayoutPanel1.Controls.Clear();
		foreach (var track in info.GetFiles("*.mp3")) {
			var extractor = new FileTrackExtractor(track.FullName);
			foreach (var (name, provider) in extractor.SampleProviders) {
				var volume = new VolumeSampleProvider(provider);
				var panner = new PanningSampleProvider(volume);
				mixer.AddMixerInput(panner);
				var strip = new TrackStrip($"{track.Name} + {name}");
				strip.PanChanged += (_, args) => {
					volume.Volume = args.Volume;
					panner.Pan = args.Pan;
				};
				strip.Width = flowLayoutPanel1.Width;
				strip.Anchor = AnchorStyles.Left | AnchorStyles.Right;
				flowLayoutPanel1.Controls.Add(strip);
			}
		}
		var offset = new OffsetSampleProvider(mixer) {
			DelayBy = TimeSpan.FromMilliseconds(150)
		};
		player = new DirectSoundOut();
		player.Init(offset);
	}

	private void toolStripButton2_Click(object sender, EventArgs e) {
		player.Play();
	}
}
