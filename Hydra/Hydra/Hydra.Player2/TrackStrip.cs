using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace Hydra.Player2 {
	public partial class TrackStrip : UserControl {
		public TrackStrip(string name) {
			InitializeComponent();
			pot1.Maximum = 100;
			pot1.Minimum = 0;
			pot1.Value = 80;
			pot2.Maximum = 100;
			pot2.Minimum = -100;
			pot2.Value = 0;
			pot2.ValueChanged += pot1_ValueChanged;
			label1.Text = name;

			// waveViewer1.WaveStream = wave;
			//waveViewer1.SamplesPerPixel = 100000;
		}

		public event TrackEventHandler? PanChanged;

		private void pot1_ValueChanged(object sender, EventArgs e) {
			var volume = (float) pot1.Value / (float) pot1.Maximum;
			var pan = (float) pot2.Value / (float) pot2.Maximum;
			PanChanged?.Invoke(this, new(volume, pan));
		}

		private void pot1_DoubleClick(object sender, EventArgs e) {
			pot1.Value = 80;
		}

		private void pot2_DoubleClick(object sender, EventArgs e) {
			pot2.Value = 0;
		}
	}

	public delegate void TrackEventHandler(object sender, TrackEventArgs e);
}
