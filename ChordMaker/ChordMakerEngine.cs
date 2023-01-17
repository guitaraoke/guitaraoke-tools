using System.Diagnostics;
using System.Text;
using ChordMaker;
using FFMpegCore;
using FFMpegCore.Pipes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SixLabors.ImageSharp;

public class ChordMakerEngine {
	const int FPS_MULTIPLIER = 2;
	
	public void MakeChords(string videoPath, bool draft, float duration) {

		var job = new VideoJob(videoPath, draft);

		const int MAX_CHORD_SPEED = 140; // Maximum chord speed in pixels per second
		const int MIN_CHORD_SPEED = 100; //

		var stats = GetVideoStats(videoPath);
		var fps = (float) Math.Round(stats.FPS, draft ? 1 : FPS_MULTIPLIER);
		var width = draft ? 640 : stats.Width;
		var height = draft ? 360 : stats.Height;
		
		Console.WriteLine($"Artist: {job.Artist}");
		Console.WriteLine($"Title: {job.Title}");
		Console.WriteLine($"Size: {width}x{height}");
		Console.WriteLine($"FPS: {fps}");
		Console.WriteLine($"r_frame_rate: {stats.RFrameRate}");
		Console.WriteLine(String.Empty.PadRight(40, '-'));
		Console.WriteLine($"Source:  {videoPath}");
		Console.WriteLine($"Chords:  {job.ChordTimesFilePath}");
		Console.WriteLine($"Overlay: {job.OverlayFilePath}");
		Console.WriteLine($"Output:  {job.OutputFilePath}");

		var chords = File.ReadAllLines(job.ChordTimesFilePath).Select(line => new Chord(line))
			.Where(chord => chord.Time >= 0)
			.ToList();

		for (var i = 1; i < chords.Count; i++) {
			chords[i - 1].Duration = chords[i].Time - chords[i - 1].Time;
		}

		var shortestChord = chords.Where(chord => chord.Duration > 0).OrderBy(chord => chord.Duration).First();
		var minChordWidth = 160f;
		var speed = minChordWidth / shortestChord.Duration;
		if (speed > MAX_CHORD_SPEED) speed = MAX_CHORD_SPEED;
		if (speed < MIN_CHORD_SPEED) speed = MIN_CHORD_SPEED;

		var lineSplitThreshold = minChordWidth / speed;

		for (var i = 1; i < chords.Count; i++) {
			if (chords[i - 1].Duration < lineSplitThreshold) {
				chords[i].TextLine++;
				i++;
			}
		}

		Console.WriteLine($"Shortest chord: {shortestChord.Time} {shortestChord.Name} {shortestChord.Duration}");
		Console.WriteLine($"Speed: {speed}");
		Console.WriteLine($"Line Split Threshold: {lineSplitThreshold}");

		duration = Math.Min(duration, stats.Duration);

		Console.WriteLine($"Duration: {duration}");

		var frameCount = (int) (duration * fps * FPS_MULTIPLIER); // stats.Frames * 2; //  (int) ((chords.Max(pair => pair.Time) + 5) * FPS);

		var fm = new FrameMaker(width, height, fps * FPS_MULTIPLIER);

		var frames = fm.CreateFrames(frameCount, chords, speed, job.OverlayColor);
		var videoFramesSource = new RawVideoPipeSource(frames) { FrameRate = fps * FPS_MULTIPLIER };
		var sw = new Stopwatch();
		sw.Start();
		FFMpegArguments
			.FromPipeInput(videoFramesSource)
			.OutputToFile($"{job.OverlayFilePath}", overwrite: true, options => options.WithVideoCodec("libvpx-vp9"))
			.ProcessSynchronously();
		sw.Stop();
		Console.WriteLine($"Generated {frameCount} frames in {sw.ElapsedMilliseconds} ms");

		
		var startInfo = new ProcessStartInfo();
		startInfo.CreateNoWindow = false;
		startInfo.UseShellExecute = false;
		startInfo.FileName = @"ffmpeg";
		var ffmpegArguments = $"-i \"{job.SourceFilePath}\" -c:v libvpx-vp9"
		                      + $" -r {fps * FPS_MULTIPLIER}"
		                      + $" -i \"{job.OverlayFilePath}\""
		                      + $" -metadata artist=\"{job.Artist}\""
		                      + $" -metadata title=\"{job.Title} (Guitaraoke Backing)\""
		                      + $" -metadata album=\"Guitaraoke\""
		                      + $" -filter_complex \"[0:0][1:0]overlay\""
		                      + $" -c:v libx264"
		                      + $" -b:v 3200k "
		                      + $" -y ";
		if (duration < 60) ffmpegArguments += $" -ss 00:00:00 -t 00:00:{duration}";
		ffmpegArguments += $" \"{job.OutputFilePath}\"";

		startInfo.Arguments = ffmpegArguments;
		Console.WriteLine(ffmpegArguments);
		using (var ffmpeg = Process.Start(startInfo)) ffmpeg.WaitForExit();

		Console.WriteLine("Done");
		Console.WriteLine($"{job.OutputFilePath}");
	}


	static ChordMaker.ffmpeg.FFProbeStream GetVideoStats(string videoFilePath) {
		JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
			ContractResolver = new DefaultContractResolver {
				NamingStrategy = new SnakeCaseNamingStrategy()
			}
		};
		var startInfo = new ProcessStartInfo();
		startInfo.CreateNoWindow = false;
		startInfo.UseShellExecute = false;
		startInfo.FileName = @"ffprobe";
		startInfo.RedirectStandardOutput = true;
		var ffprobeArguments = $"-v quiet -print_format json -show_format -show_streams \"{videoFilePath}\"";

		startInfo.Arguments = ffprobeArguments;
		Console.WriteLine($"ffprobe {ffprobeArguments}");
		using (var ffprobe = Process.Start(startInfo)) {
			if (ffprobe != null) {
				var stdout = new StringBuilder();
				ConsumeReader(ffprobe.StandardOutput, stdout);
				ffprobe.WaitForExit();
				var json = stdout.ToString();
				var result = JsonConvert.DeserializeObject<FFProbeResult>(json);
				if (result != null) {
					return result.Streams.FirstOrDefault(s => s.CodecType == "video");
				}
			}
		}
		throw new Exception($"Couldn't read FPS from {videoFilePath}");
	}

	static async Task ConsumeReader(TextReader reader, StringBuilder sb) {
		string text;
		while ((text = await reader.ReadLineAsync()) != null) sb.AppendLine(text);
	}
}
