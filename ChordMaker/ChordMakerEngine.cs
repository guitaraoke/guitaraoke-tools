using System.Diagnostics;
using FFMpegCore;
using FFMpegCore.Pipes;
using FFProbe = ChordMaker.ffmpeg.FFProbe;
using FFProbeStream = ChordMaker.ffmpeg.FFProbeStream;

namespace ChordMaker;

public class ChordMakerEngine {
	private const int FPS_MULTIPLIER = 2;

	private const float MIN_CHORD_WIDTH = 160f;
	private const int SPEED = 120;

	// Any chord narrower than this many pixels means we need to split chords onto two lines
	private const float LINE_SPLIT_THRESHOLD = MIN_CHORD_WIDTH / SPEED;

	public async Task MakeChords(string videoPath, float duration) {

		var stats = await FFProbe.GetVideoStats(videoPath);
		var fps = stats.FPS * FPS_MULTIPLIER;
		var job = new VideoJob(videoPath);

		ShowJobInfo(job, stats);

		var chords = await ReadChords(job);

		duration = Math.Min(duration, stats.Duration);

		var frameCount = (int) (duration * fps);
		var fm = new FrameMaker(stats.Width, stats.Height, fps);
		var frames = fm.CreateFrames(frameCount, chords, SPEED, job.OverlayColor);

		var videoFramesSource = new RawVideoPipeSource(frames) { FrameRate = fps };
		FFMpegArguments
			.FromPipeInput(videoFramesSource)
			.OutputToFile($"{job.OverlayFilePath}", overwrite: true, options => options.WithVideoCodec("libvpx-vp9"))
			.ProcessSynchronously();

		var startInfo = new ProcessStartInfo {
			CreateNoWindow = false,
			UseShellExecute = false,
			FileName = @"ffmpeg"
		};
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
		using (var ffmpeg = Process.Start(startInfo)) await ffmpeg.WaitForExitAsync();
		ShowDoneBanner(job);
	}

	private static void ShowJobInfo(VideoJob job, FFProbeStream stats) {
		Console.WriteLine($"Artist: {job.Artist}");
		Console.WriteLine($"Title: {job.Title}");
		Console.WriteLine($"Size: {stats.Width}x{stats.Height}");
		Console.WriteLine($"FPS: {stats.FPS}");
		Console.WriteLine($"r_frame_rate: {stats.RFrameRate}");
		Console.WriteLine(String.Empty.PadRight(40, '-'));
		Console.WriteLine($"Source:  {job.SourceFilePath}");
		Console.WriteLine($"Chords:  {job.ChordTimesFilePath}");
		Console.WriteLine($"Overlay: {job.OverlayFilePath}");
		Console.WriteLine($"Output:  {job.OutputFilePath}");
	}

	private void ShowDoneBanner(VideoJob job) {
		var color = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Magenta;
		Console.WriteLine(@"                                                                                                                                                                                   
#############               #########       ########        ########  ######################   ### 
#::::::::::::###          ##:::::::::##     #:::::::#       #::::::#  #::::::::::::::::::::#  ##:##
#:::::::::::::::##      ##:::::::::::::##   #::::::::#      #::::::#  #::::::::::::::::::::#  #:::#
###:::::#####:::::#    #:::::::###:::::::#  #:::::::::#     #::::::#  ##::::::#########::::#  #:::#
  #:::::#    #:::::#   #::::::#   #::::::#  #::::::::::#    #::::::#    #:::::#       ######  #:::#
  #:::::#     #:::::#  #:::::#     #:::::#  #:::::::::::#   #::::::#    #:::::#               #:::#
  #:::::#     #:::::#  #:::::#     #:::::#  #:::::::#::::#  #::::::#    #::::::##########     #:::#
  #:::::#     #:::::#  #:::::#     #:::::#  #::::::# #::::# #::::::#    #:::::::::::::::#     #:::#
  #:::::#     #:::::#  #:::::#     #:::::#  #::::::#  #::::#:::::::#    #:::::::::::::::#     #:::#
  #:::::#     #:::::#  #:::::#     #:::::#  #::::::#   #:::::::::::#    #::::::##########     #:::#
  #:::::#     #:::::#  #:::::#     #:::::#  #::::::#    #::::::::::#    #:::::#               ##:##
  #:::::#    #:::::#   #::::::#   #::::::#  #::::::#     #:::::::::#    #:::::#       ######   ### 
###:::::#####:::::#    #:::::::###:::::::#  #::::::#      #::::::::#  ##::::::########:::::#       
#:::::::::::::::##      ##:::::::::::::##   #::::::#       #:::::::#  #::::::::::::::::::::#   ### 
#::::::::::::###          ##:::::::::##     #::::::#        #::::::#  #::::::::::::::::::::#  ##:##
#############               #########       ########         #######  ######################   ###");
		Console.ForegroundColor = color;
		Console.WriteLine();
		Console.WriteLine();
		Console.WriteLine($"{job.OutputFilePath}");
	}

	private static async Task<List<Chord>> ReadChords(VideoJob job) {
		var chords = (await File.ReadAllLinesAsync(job.ChordTimesFilePath))
			.Select(line => new Chord(line))
			.Where(chord => chord.Time >= 0)
			.ToList();

		for (var i = 1; i < chords.Count; i++) {
			chords[i - 1].Duration = chords[i].Time - chords[i - 1].Time;
		}

		for (var i = 1; i < chords.Count; i++) {
			if (!(chords[i - 1].Duration < LINE_SPLIT_THRESHOLD)) continue;
			chords[i++].TextLine++;
		}

		return chords;
	}
}
