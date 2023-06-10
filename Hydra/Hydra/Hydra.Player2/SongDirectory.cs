using NAudio.Wave;

namespace Hydra.Player2;

public class SongDirectory {
	public string Name { get; set; }
	public string Path { get; set; }
	public override string ToString() => Name;
}

public class FileTrackExtractor {
	public Dictionary<string, ISampleProvider> SampleProviders { get; } = new();

	private const float THRESHOLD = 0.01f;

	public FileTrackExtractor(string filePath) {
		using var formatReader = new AudioFileReader(filePath);
		var format = formatReader.WaveFormat;

		
		var bytes = File.ReadAllBytes(filePath);
		using var stream = new MemoryStream(bytes);
		var streamReader = new Mp3FileReader(stream);
		var buffer = new float[format.Channels * format.SampleRate];
		var bytesPerSample = format.Channels * 4;
		var channels = Enumerable.Range(0, format.Channels)
			.Select(_ => new float[(int) (streamReader.Length)])
			.ToList();

		long offset = 0;
		var c = 0;
		var channelCount = channels.Count;
		long samplesRead;
		var provider = streamReader.ToSampleProvider();
		while ((samplesRead = provider.Read(buffer, 0, buffer.Length)) > 0) {
			for(var i = 0; i < samplesRead; i++) {
				var sample = buffer[i];
				channels[c][offset] = sample;
				if (++c < channelCount) continue;
				offset++;
				c = 0;
			}
		}

		var nonZeros = new int[format.Channels];
		var differences = new int[format.Channels];

		for (c = 1; c < channels.Count; c++) {
			for (var i = 0; i < channels[c].Length; i++) {
				if (channels[0][i] < THRESHOLD && channels[c][i] < THRESHOLD) continue;
				nonZeros[c]++;
				if (Math.Abs(channels[0][i] - channels[c][i]) > THRESHOLD) differences[c]++;
			}
		}

		var cc = 1;
		while (cc < channels.Count) {
			var ratio = differences[cc] / (float) nonZeros[cc];
			if (ratio < 0.01f) {
				channels.RemoveAt(cc);
			} else {
				cc++;
			}
		}

		var monoFormat = WaveFormat.CreateIeeeFloatWaveFormat(format.SampleRate, 1);
		switch (channels.Count) {
			case 1:
				SampleProviders.Add("Mono", new CachedSoundSampleProvider(monoFormat, channels[0].ToArray()));
				return;
			case 2:
				SampleProviders.Add("Left", new CachedSoundSampleProvider(monoFormat, channels[0].ToArray()));
				SampleProviders.Add("Right", new CachedSoundSampleProvider(monoFormat, channels[1].ToArray()));
				return;
			default:
				for (var i = 0; i < channels.Count; i++) {
					SampleProviders.Add(i.ToString(), new CachedSoundSampleProvider(monoFormat, channels[0].ToArray()));
				}
				return;
		}
	}
}

class CachedSoundSampleProvider : ISampleProvider {
	private readonly float[] samples;
	private long position;

	public CachedSoundSampleProvider(WaveFormat waveFormat, float[] samples) {
		this.WaveFormat = waveFormat;
		this.samples = samples;
	}

	public int Read(float[] buffer, int offset, int count) {
		var availableSamples = samples.Length - position;
		var samplesToCopy = Math.Min(availableSamples, count);
		Array.Copy(samples, position, buffer, offset, samplesToCopy);
		position += samplesToCopy;
		return (int) samplesToCopy;
	}

	public WaveFormat WaveFormat { get; }
}
